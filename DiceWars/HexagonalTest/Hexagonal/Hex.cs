using System.Drawing;

namespace Hexagonal
{
    public class Hex : Subject
    {
        public static readonly Color WaterColor = Color.White;
        private System.Drawing.PointF[] points;
        private float side;
        private float h;
        private float r;

        // MaHa
        private int tmpX;

        private int tmpY;

        //
        private float x;

        private float y;
        private int gridPosX;
        private int gridPosY;
        private HexState hexState;
        private int dices;

        private bool exhausted;
        private DiceLabels diceLabels;

        /// <summary>
        /// Constructor to initalize the Hexagon with the fixed values
        /// </summary>
        /// <param name="side">Hexagon side length</param>
        /// <param name="playerColor">The players color</param>
        /// <param name="posX">The X position in the grid</param>
        /// <param name="posY">The Y position in the grid</param>
        /// <param name="dices">Number of dices on this Hex</param>
        public Hex(float side, Color playerColor, int posX, int posY, int dices, bool isWater)
        {
            this.side = side;
            this.hexState = new HexState(isWater ? WaterColor : playerColor);
            this.dices = dices;
            this.IsWater = isWater;

            this.gridPosX = posY;
            this.gridPosY = posX;
        }

        public bool IsWater { get; }

        public int Dices
        {
            get
            {
                return dices;
            }
            internal set
            {
                dices = value;
                this.Notify();
            }
        }

        public float R
        {
            get
            {
                return r;
            }
            internal set
            {
            }
        }

        public bool Exhausted
        {
            get
            {
                return exhausted;
            }
            internal set
            {
                exhausted = value;
            }
        }

        public int GridPositionX
        {
            get
            {
                return gridPosX;
            }
            internal set
            {
                gridPosX = value;
            }
        }

        public int GridPositionY
        {
            get
            {
                return gridPosY;
            }
            internal set
            {
                gridPosY = value;
            }
        }

        public Hexagonal.HexState HexState
        {
            get
            {
                return hexState;
            }
            internal set
            {
                throw new System.NotImplementedException();
            }
        }

        internal System.Drawing.PointF[] Points
        {
            get
            {
                return points;
            }
            set
            {
            }
        }

        internal float Side
        {
            get
            {
                return side;
            }
            set
            {
            }
        }

        internal float H
        {
            get
            {
                return h;
            }
            set
            {
            }
        }

        // MaHa
        internal int TmpX
        {
            get
            {
                return tmpX;
            }
        }

        // MaHa
        internal int TmpY
        {
            get
            {
                return tmpY;
            }
        }

        /// <summary>
        /// Determines if that Hex is the neighbor of this Hex
        /// </summary>
        public bool IsNeighbor(Hex that)
        {
            if (that == null || this.Equals(that))
            {
                return false;
            }
            //Left or right of target
            if (this.GridPositionY == that.GridPositionY && (this.GridPositionX == that.GridPositionX + 1 || this.GridPositionX == that.GridPositionX - 1))
            {
                return true;
            }
            //Diagonal
            if (this.GridPositionX == that.GridPositionX && (this.GridPositionY == that.GridPositionY + 1 || this.GridPositionY == that.GridPositionY - 1))
            {
                return true;
            }
            //Diagonal the other way, depends on the GridPositionY
            if (this.GridPositionY % 2 == 0)
            {
                if (this.GridPositionX == that.GridPositionX + 1 && (this.GridPositionY == that.GridPositionY + 1 || this.GridPositionY == that.GridPositionY - 1))
                {
                    return true;
                }
            }
            else
            {
                if (this.GridPositionX == that.GridPositionX - 1 && (this.GridPositionY == that.GridPositionY + 1 || this.GridPositionY == that.GridPositionY - 1))
                {
                    return true;
                }
            }
            return false;
        }

        public override string ToString()
        {
            return "Hexagon[ x=" + gridPosX + ", y=" + gridPosY + "]";
        }

        public override int GetHashCode()
        {
            return gridPosX * 13371 + gridPosY;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (this.GetType() != obj.GetType())
            {
                return false;
            }

            Hex that = (Hex)obj;

            if (this.gridPosX == that.gridPosX &&
                this.gridPosY == that.gridPosY &&
                this.side == that.side &&
                this.h == that.h &&
                this.r == that.r &&
                this.x == that.x &&
                this.y == that.y)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        internal void Initialize(PointF point)
        {
            Initialize(point.X, point.Y);
        }

        /// <summary>
        /// Sets internal fields and calls CalculateVertices()
        /// </summary>
        internal void Initialize(float x, float y)
        {
            this.x = x;
            this.y = y;
            this.tmpX = (int)x;
            this.tmpY = (int)y;

            CalculateVertices();
        }

        /// <summary>
        /// Calculates the vertices of the hex based on orientation. Assumes that points[0] contains a value.
        /// </summary>
        private void CalculateVertices()
        {
            //
            //  h = short length (outside)
            //  r = long length (outside)
            //  side = length of a side of the hexagon, all 6 are equal length
            //
            //  h = sin (30 degrees) x side
            //  r = cos (30 degrees) x side
            //
            //		 h
            //	     ---
            //   ----     |r
            //  /    \    |
            // /      \   |
            // \      /
            //  \____/
            //
            // Flat orientation (scale is off)
            //
            //     /\
            //    /  \
            //   /    \
            //   |    |
            //   |    |
            //   |    |
            //   \    /
            //    \  /
            //     \/
            // Pointy orientation (scale is off)

            h = Hexagonal.Math.CalculateH(side);
            r = Hexagonal.Math.CalculateR(side);

            // x,y coordinates are top left point
            points = new System.Drawing.PointF[6];
            points[0] = new PointF(x, y);
            points[1] = new PointF(x + side, y);
            points[2] = new PointF(x + side + h, y + r);
            points[3] = new PointF(x + side, y + r + r);
            points[4] = new PointF(x, y + r + r);
            points[5] = new PointF(x - h, y + r);
        }
    }
}