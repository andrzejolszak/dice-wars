using System;
using Hexagonal;
using HexagonalTest.PlayerAPI;

namespace HexagonalTest.Players
{
    public class UserPlayer : IPlayerLogic
    {
        public void Initialize(Player player, IBoardState initialBoardState)
        {
        }

        public void PlayTurn(IBoardState boardState)
        {
            throw new InvalidOperationException("This is a special-case player who is controlled by the user GUI input!");
        }
    }
}