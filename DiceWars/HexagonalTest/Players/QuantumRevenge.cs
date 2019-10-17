using System.Drawing;
using System.Linq;
using Hexagonal;
using HexagonalTest.PlayerAPI;
using static Hexagonal.Board;

namespace HexagonalTest.Players
{
    public class QuantumRevenge : IPlayerLogic
    {
        private Player _player;

        public void Initialize(Player player, IBoardState initialBoardState)
        {
            this._player = player;
        }

        public void PlayTurn(IBoardState boardState)
        {
            Color lastAttackedBy = boardState.AttackHistory.LastOrDefault(x => x.defender.playerColor == this._player.Color).attacker.playerColor;

            LOGIC_START:
            foreach (Hex own in boardState.GetFieldsForPlayer(this._player))
            {
                if (own.Dices == 1)
                {
                    continue;
                }

                foreach ((Hex other, RelativeDirection direction) in boardState.GetNeighborsOfColor(lastAttackedBy, own))
                {
                    if (own.Dices >= other.Dices && boardState.CanAttack(own, other, out _))
                    {
                        bool won = boardState.PerformAttack(own, other);
                        if (won)
                        {
                            // Restart the logic to also process the freshly conquered fields
                            goto LOGIC_START;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }
    }
}