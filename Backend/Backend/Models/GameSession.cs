namespace Backend.Models;

public class GameSession : Entity<int>
{
   public int PlayerId { get; set; }
   public int GameConfigId { get; set; }
   public DateTime StartTime { get; set; }
   public int DurationInSeconds { get; set; }
   public int Score { get; set; }
   public int CurrentColumn { get; set; }
   public string Status { get; set; } = "Active";
}