﻿using FUC.Data.Abstractions;
using FUC.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FUC.Data;

public interface IUnitOfWork<out TContext> where TContext : DbContext, IDisposable
{
    IRepository<TEntity> GetRepository<TEntity>() where TEntity : Entity;

    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    Task CommitAsync(CancellationToken cancellationToken = default);

    Task RollbackAsync(CancellationToken cancellationToken = default);
    Task<int> ExecuteSqlCommandAsync(string sql, params object[] parameters);
    IQueryable<T> ExecuteSqlQueryAsync<T>(string sql, params object[] parameters) where T : class;
}
