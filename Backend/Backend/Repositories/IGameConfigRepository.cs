using Backend.Models;

namespace Backend.Repositories;

public interface IGameConfigRepository : ICrudRepository<GameConfig, int>
{
    void UpdateObstacle(int id, int row, int column, string type);
}
