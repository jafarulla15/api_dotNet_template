using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Transporter.Repository
{
    public class UnitOfWork<TContext> : IUnitOfWork<TContext> where TContext : DbContext, new()
    {
        public readonly TContext _dbContext;
        private Dictionary<Type, object> _repositories;

        public UnitOfWork(TContext context)
        {
            _dbContext = context ??
                throw new ArgumentNullException(nameof(context));
        }

        public TContext DbContext => _dbContext;

        public virtual IGenericRepository<T> Repository<T>() where T : class, new()
        {
            if (_repositories == null)
                _repositories = new Dictionary<Type, object>();

            var type = typeof(T);

            if (!_repositories.ContainsKey(type))
            {
                _repositories[type] = new GenericRepository<T>(_dbContext);
            }

            return (IGenericRepository<T>)_repositories[type];
        }

        public virtual int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            return _dbContext.SaveChanges(acceptAllChangesOnSuccess);
        }

        public virtual async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess)
        {
            return await _dbContext.SaveChangesAsync(acceptAllChangesOnSuccess);
        }

        public virtual int SaveChanges()
        {
            int value = _dbContext.SaveChanges();
            foreach (var entry in _dbContext.ChangeTracker.Entries().ToArray())
            {
                entry.State = EntityState.Detached;
            }
            return value;
        }

        public virtual async Task<int> SaveChangesAsync()
        {
            int value = await _dbContext.SaveChangesAsync();
            foreach (var entry in _dbContext.ChangeTracker.Entries().ToArray())
            {
                entry.State = EntityState.Detached;
            }
            return value;
        }

        public List<T> RawSqlQuery<T>(string query) where T : class, new()
        {
            //return null;

            using (var command = _dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;
                command.CommandType = CommandType.Text;

                _dbContext.Database.OpenConnection();

                using (var reader = command.ExecuteReader())
                {
                    var lst = new List<T>();
                    var lstColumns = new T().GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).ToList();
                    while (reader.Read())
                    {
                        var newObject = new T();
                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            var name = reader.GetName(i);
                            PropertyInfo prop = lstColumns.FirstOrDefault(a => a.Name.ToLower().Equals(name.ToLower()));
                            if (prop == null)
                            {
                                continue;
                            }
                            var val = reader.IsDBNull(i) ? null : reader[i];
                            prop.SetValue(newObject, val, null);
                        }
                        lst.Add(newObject);
                    }
                    return lst;
                }

            }
        }

        public async Task<List<T>> RawSqlQueryAsync<T>(string query) where T : class, new()
        {
            using (var command = _dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;
                command.CommandType = CommandType.Text;

                await _dbContext.Database.OpenConnectionAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    var lst = new List<T>();
                    var lstColumns = new T().GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).ToList();
                    while (await reader.ReadAsync())
                    {
                        var newObject = new T();
                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            var name = reader.GetName(i);
                            PropertyInfo prop = lstColumns.FirstOrDefault(a => a.Name.ToLower().Equals(name.ToLower()));
                            if (prop == null)
                            {
                                continue;
                            }
                            var val = reader.IsDBNull(i) ? null : reader[i];
                            prop.SetValue(newObject, val, null);
                        }
                        lst.Add(newObject);
                    }
                    return lst;
                }

            }

            return null;

        }

        public async Task<int> GetCountFromRawSqlAsync(string query)
        {
            //var query = "SELECT COUNT(*) FROM YourTableName";
            var count = await _dbContext.Database.ExecuteSqlRawAsync(query);
            return count;
        }

        //public int ExecuteScalarSql(string sql)
        //{
        //    int count = _dbContext.SomeScalarQueryResults.FromSqlRaw(sql).AsEnumerable().FirstOrDefault();
        //    return count?.Result ?? 0;
        //}

        //public async Task<int> ExecuteScalarSqlAsync(string sql)
        //{
        //    var kkk = _sbDbContext.SomeScalarQueryResults

        //    var count = await _dbContext.SomeScalarQueryResults.FromSqlRaw(sql).AsAsyncEnumerable().FirstOrDefaultAsync();
        //    return count?.Result ?? 0;
        //}

        private bool _disposed = false;
        /// <summary>
        /// Dispose the dbContext after finish task
        /// </summary>
        /// <param name="disposing">Flag for indicating desposing or not</param>
        public virtual void Dispose(bool disposing)
        {
            if (!_disposed)
                if (disposing)
                {
                    _dbContext.Dispose();
                    if (_repositories != null)
                    {
                        _repositories.Clear();
                    }
                }

            _disposed = true;
        }

        /// <summary>
        /// Dispose the dbContext after finish task
        /// </summary>
        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }


}
