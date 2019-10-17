using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using HexagonalTest.PlayerAPI;
using HexagonalTest.Players;

namespace Hexagonal
{
    /// <summary>
    /// Represents a 2D hexagon board
    /// </summary>
    public class Board : IBoardState
    {
        private static readonly int MAX_DICE = 8;
        private static readonly Random RANDOM = new Random();
        private static readonly RelativeDirection[] _allDirections = new[] { RelativeDirection.N, RelativeDirection.NE, RelativeDirection.SE, RelativeDirection.S, RelativeDirection.SW, RelativeDirection.NW };
        private Hexagonal.Hex[,] hexes;
        private int width;
        private int height;
        private int xOffset;
        private int yOffset;
        private int side;
        private HexagonalTest.DTOClass transferObject;
        private HexagonalTest.Fight fightForm;
        private System.Drawing.Color backgroundColor;
        private List<Player> players = new List<Player>();
        private int[,] textPosX;
        private Hexagonal.BoardState boardState;

        // MaHa
        private int[,] textPosY;

        private float pixelWidth;

        // MaHa
        private float pixelHeight;

        private List<int> fieldHelper;
        private Dictionary<Color, Player> _playersByColor;
        private int nonWaterFields;

        /// <param name="width">Board width</param>
        /// <param name="height">Board height</param>
        /// <param name="side">Hexagon side length</param>
        /// <param name="xOffset">X coordinate offset</param>
        /// <param name="yOffset">Y coordinate offset</param>
        public Board(int width, int height, int side, int xOffset, int yOffset, BoardState boardState, List<Player> players, HexagonalTest.DTOClass dataTransfer)
        {
            this.width = width;
            this.height = height;
            this.nonWaterFields = width * height;
            this.side = side;
            this.transferObject = dataTransfer;

            this.xOffset = xOffset;
            this.yOffset = yOffset;
            this.boardState = boardState;
            this.players = players;
            hexes = new Hex[height, width]; //opposite of what we'd expect

            textPosX = new int[height, width]; // MaHa
            textPosY = new int[height, width]; // MaHa

            float h = Hexagonal.Math.CalculateH(side); // short side
            float r = Hexagonal.Math.CalculateR(side); // long side

            //Initalize, fill and shuffle field helper to give each player the same amount of fields on the start
            this.fieldHelper = new List<int>(width * height);
            int f = 0;
            while (f < fieldHelper.Capacity)
            {
                for (int p = players.Count - 1; p >= 0 && f < fieldHelper.Capacity; p--)
                {
                    fieldHelper.Add(p);
                    f++;
                }
            }
            Math.Shuffle<int>(fieldHelper);

            //Preload some dice results
            RandomGenerator.getInstance().initialize();

            //
            // Calculate pixel info..remove?
            // because of staggering, need to add an extra r/h
            float hexWidth = 0;
            float hexHeight = 0;
            hexWidth = side + h;
            hexHeight = r + r;
            this.pixelWidth = (width * hexWidth) + h;
            this.pixelHeight = (height * hexHeight) + r;

            bool inTopRow = false;
            bool inBottomRow = false;
            bool inLeftColumn = false;
            bool inRightColumn = false;
            bool isTopLeft = false;
            bool isTopRight = false;
            bool isBotomLeft = false;
            bool isBottomRight = false;

            // f = field number in 2D plane
            f = 0;
            // i = y coordinate (rows), j = x coordinate (columns) of the hex tiles 2D plane
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Player player = getPlayerByID(fieldHelper[f]);

                    bool isBorderWater = i == 0 || j == 0 || i == height - 1 || j == width - 1;
                    isBorderWater &= RandomGenerator.getInstance().rollTheDice(1) < 3;

                    bool isInnerWater = i > 1 && j > 1 && i < height - 2 && j < width - 2;
                    isInnerWater &= RandomGenerator.getInstance().rollTheDice(1) == 1;

                    bool isWater = isBorderWater || isInnerWater;
                    Hex current = new Hex(side, isWater ? Hex.WaterColor : player.Color, j, i, isWater ? 0 : 1, isWater);

                    if (isWater)
                    {
                        nonWaterFields--;
                    }
                    else
                    {
                        player.addField();
                    }

                    f++;
                    // Set position booleans

                    #region Position Booleans

                    if (i == 0)
                    {
                        inTopRow = true;
                    }
                    else
                    {
                        inTopRow = false;
                    }

                    if (i == height - 1)
                    {
                        inBottomRow = true;
                    }
                    else
                    {
                        inBottomRow = false;
                    }

                    if (j == 0)
                    {
                        inLeftColumn = true;
                    }
                    else
                    {
                        inLeftColumn = false;
                    }

                    if (j == width - 1)
                    {
                        inRightColumn = true;
                    }
                    else
                    {
                        inRightColumn = false;
                    }

                    if (inTopRow && inLeftColumn)
                    {
                        isTopLeft = true;
                    }
                    else
                    {
                        isTopLeft = false;
                    }

                    if (inTopRow && inRightColumn)
                    {
                        isTopRight = true;
                    }
                    else
                    {
                        isTopRight = false;
                    }

                    if (inBottomRow && inLeftColumn)
                    {
                        isBotomLeft = true;
                    }
                    else
                    {
                        isBotomLeft = false;
                    }

                    if (inBottomRow && inRightColumn)
                    {
                        isBottomRight = true;
                    }
                    else
                    {
                        isBottomRight = false;
                    }

                    #endregion

                    //
                    // Calculate Hex positions
                    //
                    if (isTopLeft)
                    {
                        //First hex
                        current.Initialize(0 + h + xOffset, 0 + yOffset);
                        hexes[0, 0] = current;
                    }
                    else
                    {
                        if (inLeftColumn)
                        {
                            // Calculate from hex above
                            current.Initialize(hexes[i - 1, j].Points[(int)Hexagonal.FlatVertice.BottomLeft]);
                            hexes[i, j] = current;
                        }
                        else
                        {
                            // Calculate from Hex to the left and need to stagger the columns
                            if (j % 2 == 0)
                            {
                                // Calculate from Hex to left's Upper Right Vertice plus h and R offset
                                float x = hexes[i, j - 1].Points[(int)Hexagonal.FlatVertice.UpperRight].X;
                                float y = hexes[i, j - 1].Points[(int)Hexagonal.FlatVertice.UpperRight].Y;
                                x += h;
                                y -= r;
                                current.Initialize(x, y);
                                hexes[i, j] = current;
                            }
                            else
                            {
                                // Calculate from Hex to left's Middle Right Vertice
                                current.Initialize(hexes[i, j - 1].Points[(int)Hexagonal.FlatVertice.MiddleRight]);
                                hexes[i, j] = current;
                            }
                        }
                    }
                }
            }
            //Give players extra dices at the start.
            //Each player gets total number of fields minus his largest patch plus MAX_DICE
            foreach (Player player in players)
            {
                int largestPatch = FindLargesPatchForPlayer(player);
                this.distributeDices(player, (int)((nonWaterFields) * 0.2 - largestPatch + MAX_DICE));
                player.Reward = largestPatch;
            }

            foreach (Player player in players)
            {
                player.PlayerLogic.Initialize(player, this);
            }

            this._playersByColor = players.ToDictionary(x => x.Color);
        }

        ///      N
        ///     ----
        /// NW /    \ NE
        ///   /      \
        ///   \      /
        /// SW \____/ SE
        ///      S
        public enum RelativeDirection
        {
            NE,
            SE,
            NW,
            SW,
            N,
            S
        }

        public List<((Color playerColor, int hexX, int hexY, int diceCount) attacker, (Color playerColor, int hexX, int hexY, int diceCount) defender, bool attackerWon)> AttackHistory { get; } = new List<((Color playerColor, int hexX, int hexY, int diceCount) attacker, (Color playerColor, int hexX, int hexY, int diceCount) defender, bool attackerWon)>();

        #region Properties

        public Hexagonal.Hex[,] Hexes
        {
            get
            {
                return hexes;
            }
            internal set
            {
            }
        }

        public float PixelWidth
        {
            get
            {
                return pixelWidth;
            }
            set
            {
            }
        }

        public float PixelHeight
        {
            get
            {
                return pixelHeight;
            }
            set
            {
            }
        }

        public int XOffset
        {
            get
            {
                return xOffset;
            }
            set
            {
            }
        }

        public int YOffset
        {
            get
            {
                return xOffset;
            }
            set
            {
            }
        }

        public int Width
        {
            get
            {
                return width;
            }
            set
            {
            }
        }

        public int Height
        {
            get
            {
                return height;
            }
            set
            {
            }
        }

        public System.Drawing.Color BackgroundColor
        {
            get
            {
                return backgroundColor;
            }
            set
            {
                backgroundColor = value;
            }
        }

        public Hexagonal.BoardState BoardState
        {
            get
            {
                return this.boardState;
            }
            internal set
            {
                throw new System.NotImplementedException();
            }
        }

        #endregion
        public List<Player> Players => this.players;

        internal int[,] TextPosX
        {
            get
            {
                return this.textPosX;
            }
        }

        internal int[,] TextPosY
        {
            get
            {
                return this.textPosY;
            }
        }

        public bool HasWon(Player player) => player.Fields == nonWaterFields;

        /// <summary>
        /// This function is triggered, when an player attacks another player
        /// </summary>
        /// <param name="attacker">the hex field where the attack has started</param>
        /// <param name="defender">the desination where the attack leads to</param>
        public (bool, int, int) PerformAttack(Hex attacker, Hex defender)
        {
            if (!this.CanAttack(attacker, defender, out string reason))
            {
                throw new InvalidOperationException("Wrong attack move: " + reason);
            }

            Player attackerP = findPlayerByColor(attacker.HexState.BackgroundColor);
            Player defenderP = findPlayerByColor(defender.HexState.BackgroundColor);
            int attackerEyes = RandomGenerator.getInstance().rollTheDice(attacker.Dices);
            int defenderEyes = RandomGenerator.getInstance().rollTheDice(defender.Dices);

            if (attackerEyes > defenderEyes)
            {
                // Attacked wins
                defender.HexState.BackgroundColor = attacker.HexState.BackgroundColor;
                defender.Dices = attacker.Dices - 1;
                attacker.Dices = 1;
                attackerP.Fields += 1;
                defenderP.Fields -= 1;
                this.BoardState.ActiveHex = defender;

                attackerP.Reward = FindLargesPatchForPlayer(attackerP);
                defenderP.Reward = FindLargesPatchForPlayer(defenderP);
                return (true, attackerEyes, defenderEyes);
            }
            else
            {
                // Defender wins
                attacker.Dices = 1;

                attackerP.Reward = FindLargesPatchForPlayer(attackerP);
                defenderP.Reward = FindLargesPatchForPlayer(defenderP);
                return (false, attackerEyes, defenderEyes);
            }
        }

        /// <summary>
        /// Finds the largest patch for the given player color
        /// </summary>
        /// <param name="player">The players</param>
        /// <returns>the size of the lagest patch</returns>
        public int FindLargesPatchForPlayer(Player player)
        {
            int largestField = 0;
            List<Hex> fields = GetFieldsForPlayer(player);
            while (fields.Count > 0)
            {
                HashSet<Hex> patch = getPatch((Hex)fields[0], new HashSet<Hex>());
                
                //test if larger
                if (largestField < patch.Count)
                {
                    largestField = patch.Count;
                }

                //strike out visited
                foreach (Hex patchHex in patch)
                {
                    foreach (Hex hex in fields)
                    {
                        if (hex == patchHex)
                        {
                            //already visited
                            fields.Remove(patchHex);
                            break;
                        }
                    }
                }
            }

            return largestField;
        }

        /// <summary>
        /// Function to get all connected hex fields
        /// </summary>
        /// <param name="hex">the starting hex</param>
        /// <param name="visited">for recursion, first call must be an empty arraylist</param>
        /// <returns>List of hexes that are connected containing the start hex</returns>
        public HashSet<Hex> GetPatch(Hex hex)
        {
            return this.getPatch(hex, new HashSet<Hex>());
        }

        /// <summary>
        /// Get all fields as list for the player
        /// </summary>
        /// <param name="playerColor">The players color</param>
        /// <returns>The fields</returns>
        public List<Hex> GetFieldsForPlayer(Player player)
        {
            List<Hex> fields = new List<Hex>();
            for (int x = 0; x < this.height; x++)
            {
                for (int y = 0; y < this.width; y++)
                {
                    Hex field = this.Hexes[x, y];
                    if (field.HexState.BackgroundColor == player.Color)
                    {
                        field.Exhausted = false; //reset exhausted state
                        fields.Add(field);
                    }
                }
            }
            return fields.OrderByDescending(x => x.Dices).ToList();
        }

        /// <summary>
        /// Function to find a player by its color
        /// </summary>
        /// <param name="color">The color of the player</param>
        /// <returns>The player object</returns>
        public Player findPlayerByColor(Color color)
        {
            foreach (Player player in players)
            {
                if (player.Color == color)
                {
                    return player;
                }
            }
            throw new ArgumentException("This should never have happend and I'm really sorry");
        }

        public Player FindOwnerPlayer(Hex hex)
        {
            return findPlayerByColor(hex.HexState.BackgroundColor);
        }

        public Player FindPlayer(Color color)
        {
            return this._playersByColor[color];
        }

        public bool CanAttack(Hex attacker, Hex defender, out string reason)
        {
            reason = null;

            if (attacker == null || defender == null)
            {
                reason = "Nulls not allowed";
            }
            else if (attacker.HexState.BackgroundColor != this.getCurrentPlayerColor())
            {
                reason = "The attacker hex does not belong to the current player!";
            }
            else if (attacker.Dices <= 1 || attacker.Exhausted)
            {
                reason = "Attack with less than 2 dice not possible!";
            }
            else if (attacker.HexState.BackgroundColor == defender.HexState.BackgroundColor)
            {
                reason = "Attack to the same color not possible!";
            }
            else if (!attacker.IsNeighbor(defender))
            {
                reason = "Attack to a non-neighbor not possible!";
            }
            else if (defender.IsWater)
            {
                reason = "Attack to a water hex not possible!";
            }

            return reason == null;
        }

        public Hex GetNeighborOrNull(Hex hex, RelativeDirection neighborDirection)
        {
            int x = hex.GridPositionX, y = hex.GridPositionY;

            if (hex.GridPositionY % 2 == 0)
            {
                switch (neighborDirection)
                {
                    case RelativeDirection.N:
                        x -= 1;
                        break;

                    case RelativeDirection.S:
                        x += 1;
                        break;

                    case RelativeDirection.NE:
                        x -= 1;
                        y += 1;
                        break;

                    case RelativeDirection.NW:
                        x -= 1;
                        y -= 1;
                        break;

                    case RelativeDirection.SE:
                        y += 1;
                        break;

                    case RelativeDirection.SW:
                        y -= 1;
                        break;

                    default:
                        throw new InvalidOperationException("Unknown direction: " + neighborDirection);
                }
            }
            else
            {
                switch (neighborDirection)
                {
                    case RelativeDirection.N:
                        x -= 1;
                        break;

                    case RelativeDirection.S:
                        x += 1;
                        break;

                    case RelativeDirection.NE:
                        y += 1;
                        break;

                    case RelativeDirection.NW:
                        y -= 1;
                        break;

                    case RelativeDirection.SE:
                        x += 1;
                        y += 1;
                        break;

                    case RelativeDirection.SW:
                        x += 1;
                        y -= 1;
                        break;

                    default:
                        throw new InvalidOperationException("Unknown direction: " + neighborDirection);
                }
            }

            if (x < 0 || y < 0 || x >= height || y >= width)
            {
                return null;
            }

            return Hexes[x, y];
        }

        public List<(Hex, RelativeDirection)> GetNeighborsOfColor(Color color, Hex hex)
        {
            List<(Hex, RelativeDirection)> results = new List<(Hex, RelativeDirection)>();
            foreach (RelativeDirection direction in _allDirections)
            {
                Hex neighbor = this.GetNeighborOrNull(hex, direction);
                if (neighbor != null && neighbor.HexState.BackgroundColor == color)
                {
                    results.Add((neighbor, direction));
                }
            }

            return results.OrderByDescending(x => x.Item1.Dices).ToList();
        }

        public List<(Hex, RelativeDirection)> GetNeighborsOfDifferentColor(Color color, Hex hex)
        {
            List<(Hex, RelativeDirection)> results = new List<(Hex, RelativeDirection)>();
            foreach (RelativeDirection direction in _allDirections)
            {
                Hex neighbor = this.GetNeighborOrNull(hex, direction);
                if (neighbor != null && neighbor.HexState.BackgroundColor != color)
                {
                    results.Add((neighbor, direction));
                }
            }

            return results.OrderByDescending(x => x.Item1.Dices).ToList();
        }

        /// <summary>
        /// Function to get all connected hex fields
        /// </summary>
        /// <param name="hex">the starting hex</param>
        /// <param name="visited">for recursion, first call must be an empty arraylist</param>
        /// <returns>List of hexes that are connected containing the start hex</returns>
        internal HashSet<Hex> getPatch(Hex hex, HashSet<Hex> visited)
        {
            visited.Add(hex);
            List<Hex> neighbors = GetNeighborsOfColor(hex.HexState.BackgroundColor, hex).Select(x => x.Item1).ToList();
            foreach(Hex n in neighbors)
            {
                if (visited.Add(n))
                {
                    getPatch(n, visited);
                }
            }
            
            return visited;
        }

        /// <summary>
        /// Function to get the color of the ActivePlayer
        /// </summary>
        /// <returns>A Color</returns>
        internal Color getCurrentPlayerColor()
        {
            return ((Player)this.players[this.BoardState.ActivePlayer]).Color;
        }

        /// <summary>
        /// Get the field distribution for every player as string
        /// </summary>
        /// <returns>A String</returns>
        internal String getStatus()
        {
            string status = "";
            foreach (Player player in players)
            {
                if (player.Fields > 0)
                {
                    status += $"{player.PlayerLogic.GetType().Name}({player.Color.Name}): {player.Fields}({player.Reward})\r\n";
                }
            }
            return status;
        }

        internal bool PointInBoardRectangle(System.Drawing.Point point)
        {
            return PointInBoardRectangle(point.X, point.Y);
        }

        internal bool PointInBoardRectangle(int x, int y)
        {
            //
            // Quick check to see if X,Y coordinate is even in the bounding rectangle of the board.
            // Can produce a false positive because of the staggerring effect of hexes around the edge
            // of the board, but can be used to rule out an x,y point.
            //
            int topLeftX = 0 + XOffset;
            int topLeftY = 0 + yOffset;
            float bottomRightX = topLeftX + pixelWidth;
            float bottomRightY = topLeftY + PixelHeight;

            if (x > topLeftX && x < bottomRightX && y > topLeftY && y < bottomRightY)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        internal Hex FindHexMouseClick(System.Drawing.Point point)
        {
            return FindHexMouseClick(point.X, point.Y);
        }

        internal Hex FindHexMouseClick(int x, int y)
        {
            Hex target = null;

            if (PointInBoardRectangle(x, y))
            {
                for (int i = 0; i < hexes.GetLength(0); i++)
                {
                    for (int j = 0; j < hexes.GetLength(1); j++)
                    {
                        if (Math.InsidePolygon(hexes[i, j].Points, 6, new System.Drawing.PointF(x, y)))
                        {
                            target = hexes[i, j];
                            break;
                        }
                    }

                    if (target != null)
                    {
                        break;
                    }
                }
            }

            return target;
        }

        /// <summary>
        /// Function to set the Active player to be the next player
        /// </summary>
        internal void nextPlayer()
        {
            Player currentPlayer = this.getPlayerByID(this.BoardState.ActivePlayer);

            int largestPatch = FindLargesPatchForPlayer(currentPlayer);
            this.distributeDices(currentPlayer, largestPatch);
            // Console.WriteLine("Dice to distribute: " + largestPatch);
            // Console.WriteLine("Current Bank " + currentPlayer.Bank);

            BoardState.ActivePlayer = nextActivePlayer(currentPlayer.ID);
            this.BoardState.ActiveHex = null;
            currentPlayer = this.getPlayerByID(this.BoardState.ActivePlayer);

            if (currentPlayer.PlayerLogic is UserPlayer)
            {
                return;
            }

            currentPlayer.PlayerLogic.PlayTurn(this);
            bool hasWon = this.HasWon(currentPlayer);
            if (hasWon)
            {
                return;
            }

            this.nextPlayer();
        }

        /// <summary>
        /// Function to move 2/3 of the units from an hex to anoter
        /// From hex must be not null, not exhausted and neighbor of to
        /// </summary>
        /// <param name="from">Units to move from</param>
        /// <param name="to">Units to move to</param>
        internal bool moveDices(Hex from, Hex to)
        {
            throw new InvalidOperationException("Not supported in this version!");

            if (from == null || !from.IsNeighbor(to) || from.Exhausted)
            {
                Console.WriteLine("Move of units not possible");
                return false;
            }
            if (from.Dices > 2)
            {
                int modulo = from.Dices % 3;
                int transfer = (int)((from.Dices - modulo) * (2.0 / 3.0));
                if (to.Dices + transfer > MAX_DICE)
                {
                    transfer = MAX_DICE - to.Dices;
                }
                from.Dices -= transfer;
                to.Dices += transfer;
                to.Exhausted = true;

                return false;
            }
            return true;
        }

        /// <summary>
        /// Select next player and test if he still has fields
        /// Either return this player or select next one via recursion
        /// </summary>
        /// <param name="currentPlayerId">The current players id uses for recursion</param>
        /// <returns>the next player who still has fields</returns>
        private int nextActivePlayer(int currentPlayerId)
        {
            //next Player:
            if (currentPlayerId + 1 >= players.Count)
            {
                currentPlayerId = 0;
            }
            else
            {
                currentPlayerId = currentPlayerId + 1;
            }
            //termination condition for recursion, prevents stack overflow.
            if (currentPlayerId == BoardState.ActivePlayer)
            {
                return BoardState.ActivePlayer;
            }
            //test if player has fields or find next player recursive
            if (getPlayerByID(currentPlayerId).Fields > 0)
            {
                return currentPlayerId;
            }
            else
            {
                return nextActivePlayer(currentPlayerId);
            }
        }

        /// <summary>
        /// Function to give the given player new dices
        /// </summary>
        /// <param name="player">The player</param>
        /// <param name="dice">number of dices</param>
        private void distributeDices(Player player, int dice)
        {
            List<Hex> fields = GetFieldsForPlayer(player);

            while (fields.Count > 0 && dice > 0)
            {
                Hex randomHex = (Hex)fields[RANDOM.Next(fields.Count)];
                if (randomHex.Dices < MAX_DICE)
                {
                    randomHex.Dices += 1;
                    dice -= 1;
                }
                else
                {
                    fields.Remove(randomHex);
                }
            }
        }

        /// <summary>
        /// Function to find a player by its id
        /// </summary>
        /// <param name="id">the players id</param>
        /// <returns>The Player</returns>
        private Player getPlayerByID(int id)
        {
            return (Player)this.players[id];
        }
    }
}