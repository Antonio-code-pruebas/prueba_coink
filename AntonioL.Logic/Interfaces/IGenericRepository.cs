using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AntonioL.Logic.Interfaces
{
    public interface IGenericRepository<T>
    {
        
        Task<T> GetByIdAsync(int id);
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>> filter = null,
            Expression<Func<T, string>> orderBy = null);
        Task<(int count, IReadOnlyList<T> data)> GetAndCountAsync(Expression<Func<T, bool>> filter = null,
            Expression<Func<T, string>> orderBy = null);
        
        Task<int> Add(T entity);
        Task<int> Update(T entity);
        Task<List<T>> ExecuteStoredProcedureMAsync<T>(string storedProcedureName, params NpgsqlParameter[] parameters);
        Task<string> ExecuteStoredProcedureAsync(string procedureName, params NpgsqlParameter[] parameters);
        Task<List<T>> ExecuteFunctionAsync<T>(string storedProcedureName, params NpgsqlParameter[] parameters);
        void AddEntity(T Entity);
        void UpdateEntity(T Entity);
        void DeleteEntity(T Entity);
        void RemoveRange(IEnumerable<T> entities);
        //Task<int> RemoveRange(IEnumerable<T> entities)
    }
}
