using Microsoft.EntityFrameworkCore.Storage;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.IGenericRepository
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<OperationResult> GetAllAsync();
        Task<OperationResult> GetByIdAsync(int id);
        Task<OperationResult> AddAsync(TEntity entity);
        Task<OperationResult> UpdateAsync(TEntity updatedEntity);
        Task<OperationResult> DeleteAsync(int id);
        Task DeleteRangeAsync(IEnumerable<TEntity> entities);
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}