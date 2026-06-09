using Backend.Models;

namespace Backend.Repositories;

public class GameSessionRepository : BaseRepository<GameSession, int>, IGameSessionRepository
{
    public GameSessionRepository(AppDbContext context) : base(context) { }

    public List<LeaderboardDTO> GetLeaderboard()
    {
        return (from s in _context.GameSessions
                join p in _context.Players on s.PlayerId equals p.Id
                where s.Status == "Won"
                orderby s.DurationInSeconds descending 
                select new LeaderboardDTO
                {
                    PlayerName = p.Username,
                    Score = s.Score,
                    DurationInSeconds = s.DurationInSeconds
                }).ToList();
    }

    public IEnumerable<FailedGameDTO> GetFailedGamesByPlayer(int playerId, AppDbContext ctx)
    {
        return (from session in _context.GameSessions
                join config in _context.GameConfigs on session.GameConfigId equals config.Id
                where session.PlayerId == playerId && session.Status == "Lost"
                select new FailedGameDTO
                {
                    SessionId = session.Id,
                    Score = session.Score,
                    SafePath = config.SafePath,
                    ProposedPositions = ctx.Guesses.Where(g => g.GameSessionId == session.Id).ToList()
                }).ToList();
    }
    
    public IEnumerable<PersonalHistoryDTO> GetPersonalHistory(int playerId)
    {
        return _context.GameSessions
            .Where(s => s.PlayerId == playerId && s.Status != "Active")
            .OrderByDescending(s => s.StartTime)
            .Select(s => new PersonalHistoryDTO
            {
                Status = s.Status,
                Score = s.Score,
                DurationInSeconds = s.DurationInSeconds,
                StartTime = s.StartTime
            }).ToList();
    }
}
