using Application.Common.Interfaces.GenericRepository;
using Infrastructure.Persistencia.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistencia.Repositorios;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly AppDbContext _dbContext;
    private readonly DbSet<T> _entidade;

    protected GenericRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
        _entidade = dbContext.Set<T>();
    }

    public void Add(T entity)
    {
        _entidade.Add(entity);
    }

    public void Update(T entity)
    {
        _entidade.Update(entity);
    }

    public void Delete(T entity)
    {
        _entidade.Remove(entity);
    }

    public async Task CommitAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}