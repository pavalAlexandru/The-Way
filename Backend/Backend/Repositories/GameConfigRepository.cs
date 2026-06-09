using Backend.Models;

namespace Backend.Repositories;

public class GameConfigRepository : BaseRepository<GameConfig, int>, IGameConfigRepository
{
    public GameConfigRepository(AppDbContext context) : base(context) { }

    public void UpdateObstacle(int id, int row, int column, string type)
    {
        var conf = GetById(id);
        if (conf == null) return;

        var obsList = conf.Obstacles.Split(',').ToList();
        var pos = $"{row}-{column}";
        
        if (type == "clear") obsList.Remove(pos);
        else if (type == "obstacle" && !obsList.Contains(pos)) obsList.Add(pos);
        
        conf.Obstacles = string.Join(",", obsList.Where(o => !string.IsNullOrEmpty(o)));
        Update(conf);
    }
}
