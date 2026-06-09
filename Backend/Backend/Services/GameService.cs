using Backend.Models;
using Backend.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Backend.Services;

public class GameService : IGameService
{
    private readonly AppDbContext _context; 
    private readonly IGameSessionRepository _sessionRepo;
    private readonly IGameConfigRepository _configRepo;
    private readonly IConfiguration _config;
    private readonly ILogger<GameService> _logger;

    public GameService(AppDbContext context, IGameSessionRepository sessionRepo, IGameConfigRepository configRepo, IConfiguration config, ILogger<GameService> logger)
    {
        _context = context; _sessionRepo = sessionRepo; _configRepo = configRepo; _config = config;
        _logger = logger;
    }

    public string? Authenticate(string username)
    {
        _logger.LogInformation("Authentication attempt for user: {Username}", username);
        
        var player = _context.Players.FirstOrDefault(p => p.Username == username);
        if (player == null)
        {
            _logger.LogWarning("Authentication failed. Player {Username} not found.", username);
            return null;
        } 
        
        _logger.LogInformation("Player {Username} successfully authenticated.", username);

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"] ?? "KeySuperSecret1234567890ForExams!");
        var tokenDescriptor = new SecurityTokenDescriptor {
            Subject = new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, player.Id.ToString())]),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
    }

    // public GameSession StartGame(int playerId)
    // {
    //     _logger.LogInformation("Player ID {PlayerId} started a new game.", playerId);
    //     
    //     var conf = _configRepo.GetAll().First();
    //     return _sessionRepo.Add(new GameSession { PlayerId = playerId, GameConfigId = conf.Id, StartTime = DateTime.Now, CurrentColumn = 1 });
    // }
    
    public GameSession StartGame(int playerId)
    {
        var rnd = new Random();
        var safePath = new int[4];
        for (int i = 0; i < 4; i++) safePath[i] = rnd.Next(1, 5);
        
        var obstacles = new List<string>();
        while (obstacles.Count < 5)
        {
            int r = rnd.Next(1, 5);
            int c = rnd.Next(1, 5);
            
            if (safePath[c - 1] == r) continue; 
            
            string obs = $"{r}-{c}";
            if (!obstacles.Contains(obs)) obstacles.Add(obs);
        }

        var conf = new GameConfig 
        { 
            SafePath = string.Join(",", safePath), 
            Obstacles = string.Join(",", obstacles) 
        };
        _configRepo.Add(conf); // Add to DB

        // 4. Create the session linked to this unique configuration
        return _sessionRepo.Add(new GameSession 
        { 
            PlayerId = playerId, 
            GameConfigId = conf.Id, 
            StartTime = DateTime.Now, 
            CurrentColumn = 1,
            Status = "Active"
        });
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
