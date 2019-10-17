using System;
using Hexagonal;
using HexagonalTest.PlayerAPI;
using static Hexagonal.Board;

namespace HexagonalTest.Players
{
    public class AlphaRandom : IPlayerLogic
    {
        private Player _player;
        private Random _random;

        public void Initialize(Player player, IBoardState initialBoardState)
        {
            this._player = player;
            this._random = new Random(DateTime.UtcNow.Millisecond);
        }

        public void PlayTurn(IBoardState boardState)
        {
            LOGIC_START:
            foreach (Hex own in boardState.GetFieldsForPlayer(this._player))
            {
                if (own.Dices == 1)
                {
                    continue;
                }

                foreach ((Hex other, RelativeDirection direction) in boardState.GetNeighborsOfDifferentColor(this._player.Color, own))
                {
                    if (this._random.NextDouble() > 0.5 && boardState.CanAttack(own, other, out _))
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