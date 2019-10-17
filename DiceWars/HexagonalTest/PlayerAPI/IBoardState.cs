using System.Collections.Generic;
using System.Drawing;
using Hexagonal;
using static Hexagonal.Board;

namespace HexagonalTest.PlayerAPI
{
    public interface IBoardState
    {
        int Width { get; }

        int Height { get; }

        List<Player> Players { get; }

        Hex[,] Hexes { get; }

        List<((Color playerColor, int hexX, int hexY, int diceCount) attacker, (Color playerColor, int hexX, int hexY, int diceCount) defender, bool attackerWon)> AttackHistory { get; }

        HashSet<Hex> GetPatch(Hex hex);

        int FindLargesPatchForPlayer(Player player);

        List<Hex> GetFieldsForPlayer(Player player);

        Player FindOwnerPlayer(Hex hex);

        Player FindPlayer(Color color);

        bool HasWon(Player player);

        (bool victory, int attackerEyes, int defenderEyes) PerformAttack(Hex attacker, Hex defender);

        bool CanAttack(Hex attacker, Hex defender, out string reason);

        Hex GetNeighborOrNull(Hex hex, RelativeDirection neighborDirection);

        List<(Hex, RelativeDirection)> GetNeighborsOfColor(Color color, Hex hex);

        List<(Hex, RelativeDirection)> GetNeighborsOfDifferentColor(Color color, Hex hex);
    }
}