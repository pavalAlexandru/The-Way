namespace Backend.Models;

public class AuthReq { public string Username { get; set; } = string.Empty; }
public class MoveReq { public int SessionId { get; set; } public int Row { get; set; } }
public class ConfigUpdateReq { public int Row { get; set; } public int Column { get; set; } }

public class LeaderboardDTO
{
    public string PlayerName { get; set; } = string.Empty;
    public int Score { get; set; }
    public int DurationInSeconds { get; set; }
}

public class GameEndDTO
{
    public string Status { get; set; } = string.Empty;
    public int Score { get; set; }
    public int Rank { get; set; }
    public string SafePath { get; set; } = string.Empty;
    public string Obstacles { get; set; } = string.Empty;
}

public class FailedGameDTO
{
    public int SessionId { get; set; }
    public int Score { get; set; }
    public string SafePath { get; set; } = string.Empty;
    public List<Guess> ProposedPositions { get; set; } = new List<Guess>();
}