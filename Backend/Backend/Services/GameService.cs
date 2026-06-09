using Backend.Models;
using Backend.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Backend.Services;

public class GameService : IGameService
{
    private readonly AppDbContext _context; 
    private readonly IGameSessionRepository _sessionRepo;
    private readonly IGameConfigRepository _configRepo;
    private readonly IConfiguration _config;

    public GameService(AppDbContext context, IGameSessionRepository sessionRepo, IGameConfigRepository configRepo, IConfiguration config)
    {
        _context = context; _sessionRepo = sessionRepo; _configRepo = configRepo; _config = config;
    }

    public string? Authenticate(string username)
    {
        var player = _context.Players.FirstOrDefault(p => p.Username == username);
        if (player == null) return null; 

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"] ?? "KeySuperSecret1234567890ForExams!");
        var tokenDescriptor = new SecurityTokenDescriptor {
            Subject = new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, player.Id.ToString())]),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
    }

    public GameSession StartGame(int playerId)
    {
        var conf = _configRepo.GetAll().First();
        return _sessionRepo.Add(new GameSession { PlayerId = playerId, GameConfigId = conf.Id, StartTime = DateTime.Now, CurrentColumn = 1 });
    }

    public GameEndDTO ProcessMove(int sessionId, int row, int playerId)
    {
        var session = _sessionRepo.GetById(sessionId);
        if (session == null || session.PlayerId != playerId || session.Status != "Active") throw new Exception("Invalid Session");

        var conf = _configRepo.GetById(session.GameConfigId)!;
        var safePath = conf.SafePath.Split(',').Select(int.Parse).ToArray();
        var obstacles = conf.Obstacles.Split(',');

        _context.Guesses.Add(new Guess { GameSessionId = session.Id, Column = session.CurrentColumn, Row = row });
        
        string posStr = $"{row}-{session.CurrentColumn}";
        bool isSafe = safePath[session.CurrentColumn - 1] == row;
        bool isObstacle = obstacles.Contains(posStr);

        if (isSafe && !isObstacle) 
        {
            session.Score += 5 * row; 
            session.CurrentColumn++;
            if (session.CurrentColumn > 4) { session.Status = "Won"; session.DurationInSeconds = (int)(DateTime.Now - session.StartTime).TotalSeconds; }
        }
        else { session.Status = "Lost"; session.DurationInSeconds = (int)(DateTime.Now - session.StartTime).TotalSeconds; }
        
        _sessionRepo.Update(session); 

        if (session.Status != "Active")
        {
            int rank = session.Status == "Won" ? GetLeaderboard().FindIndex(l => l.Score == session.Score) + 1 : -1;
            return new GameEndDTO { Status = session.Status, Score = session.Score, Rank = rank, SafePath = conf.SafePath, Obstacles = conf.Obstacles };
        }
        return new GameEndDTO { Status = "Active", Score = session.Score, Rank = -1, SafePath = "", Obstacles = "" };
    }

    public List<LeaderboardDTO> GetLeaderboard() => _sessionRepo.GetLeaderboard();
    public IEnumerable<FailedGameDTO> GetUnfinishedGames(int playerId) => _sessionRepo.GetFailedGamesByPlayer(playerId, _context);
    public void UpdateConfig(int configId, ConfigUpdateReq req) => _configRepo.UpdateObstacle(configId, req.Row, req.Column, req.Type);
}
