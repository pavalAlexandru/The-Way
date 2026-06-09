using Backend.Models;

namespace Backend.Repositories;

public interface ICrudRepository<T, TId> where T : Entity<TId>
{
    T Add(T entity);
    T? GetById(TId id);
    IEnumerable<T> GetAll();
    void Update(T entity);
}

public abstract class BaseRepository<T, TId> : ICrudRepository<T, TId> where T : Entity<TId>
{
    protected readonly AppDbContext _context;
    
    protected BaseRepository(AppDbContext context) { _context = context; }

    public virtual T Add(T entity) { _context.Set<T>().Add(entity); _context.SaveChanges(); return entity; }
    public virtual T? GetById(TId id) => _context.Set<T>().Find(id);
    public virtual IEnumerable<T> GetAll() => _context.Set<T>().ToList();
    public virtual void Update(T entity) { _context.Set<T>().Update(entity); _context.SaveChanges(); }
}
