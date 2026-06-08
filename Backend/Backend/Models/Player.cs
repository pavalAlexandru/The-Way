namespace Backend.Models;

public class Player : Entity<int>
{
   public string Username { get; set; } = string.Empty;
}