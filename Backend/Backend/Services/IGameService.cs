using Backend.Models;

namespace Backend.Services;

public interface IGameService
{
    string? Authenticate(string username);
    GameSession StartGame(int playerId);
    GameEndDTO ProcessMove(int sessionId, int row, int playerId);
    List<LeaderboardDTO> GetLeaderboard();
    IEnumerable<FailedGameDTO> GetUnfinishedGames(int playerId);
    void UpdateConfig(int configId, ConfigUpdateReq req);
}
