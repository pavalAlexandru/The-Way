namespace Backend.Models;

public abstract class Entity<ID>
{
    public ID Id { get; set; }
}