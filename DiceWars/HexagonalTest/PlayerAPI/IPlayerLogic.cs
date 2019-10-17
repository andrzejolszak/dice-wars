using Hexagonal;

namespace HexagonalTest.PlayerAPI
{
    public interface IPlayerLogic
    {
        void Initialize(Player player, IBoardState initialBoardState);

        void PlayTurn(IBoardState boardState);
    }
}