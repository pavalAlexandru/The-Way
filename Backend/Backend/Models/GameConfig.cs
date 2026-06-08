namespace Backend.Models;

public class GameConfig : Entity<int>
{
   public string SafePath { get; set; } = string.Empty;
   public string Obstacles { get; set; } = string.Empty;
}