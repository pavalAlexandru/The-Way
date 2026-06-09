using Backend.Models;

namespace Backend.Repositories;

public interface IGameSessionRepository : ICrudRepository<GameSession, int>
{
    List<LeaderboardDTO> GetLeaderboard();
    IEnumerable<FailedGameDTO> GetFailedGamesByPlayer(int playerId, AppDbContext ctx);
}
