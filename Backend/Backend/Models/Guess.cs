namespace Backend.Models;

public class Guess : Entity<int>
{
   public int GameSessionId { get; set; }
   public int Column { get; set; }
   public int Row { get; set; }   
}