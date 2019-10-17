using Hexagonal;
using HexagonalTest.PlayerAPI;
using static Hexagonal.Board;

namespace HexagonalTest.Players
{
    public class BlockchainPrepper : IPlayerLogic
    {
        private Player _player;

        public void Initialize(Player player, IBoardState initialBoardState)
        {
            this._player = player;
        }

        public void PlayTurn(IBoardState boardState)
        {
            LOGIC_START:
            foreach (Hex own in boardState.GetFieldsForPlayer(this._player))
            {
                if (own.Dices < 3)
                {
                    continue;
                }

                foreach ((Hex other, RelativeDirection direction) in boardState.GetNeighborsOfDifferentColor(this._player.Color, own))
                {
                    if (own.Dices - other.Dices > 2 && boardState.CanAttack(own, other, out _))
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