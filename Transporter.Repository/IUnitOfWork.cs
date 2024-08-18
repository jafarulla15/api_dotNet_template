using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transporter.Repository
{
    public interface IUnitOfWork<TContext> : IDisposable where TContext : DbContext, new()
    {
        TContext DbContext { get; }
        int SaveChanges(bool acceptAllChangesOnSuccess);
        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess);
        int SaveChanges();
        Task<int> SaveChangesAsync();
        IGenericRepository<T> Repository<T>() where T : class, new();
        List<T> RawSqlQuery<T>(string query) where T : class, new();
        Task<List<T>> RawSqlQueryAsync<T>(string query) where T : class, new();

        Task<int> GetCountFromRawSqlAsync(string query);

        //int ExecuteScalarSql(string sql);
        //Task<int> ExecuteScalarSqlAsync(string sql);
    }
}
