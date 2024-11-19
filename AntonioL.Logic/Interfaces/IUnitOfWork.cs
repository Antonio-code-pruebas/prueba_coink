using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntonioL.Logic.Interfaces
{
    public interface IUnitOfWork<TContext> : IDisposable where TContext : DbContext
    {
        Task<IDbContextTransaction> BeginTransactionAsync();
        void ResetContextState();
        IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class;

        //devuelve la cantidad de records actualizados
        Task<int> Complete();
    }
}
