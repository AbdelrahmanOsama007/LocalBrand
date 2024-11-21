using Infrastructure.Context;
using Infrastructure.IGenericRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Model.Interfaces;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.GenericRepository
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {

        private readonly MyAppContext _context;
        private readonly DbSet<TEntity> _dbSet;
        public GenericRepository(MyAppContext context) {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        public async Task<OperationResult> AddAsync(TEntity entity)
        { 
            try
            {
                var entityType = typeof(TEntity).Name;
                _context.Add(entity);
                await _context.SaveChangesAsync();
                return new OperationResult() { Success = true, Message = $"{entityType} Added Successfully" };
            }
            catch (Exception ex)
            {
                return new OperationResult() { Success = false, Message = "Something Went Wrong. Please Try Again Later" , DevelopMessage = ex.Message};
            }
        }

        public async Task<OperationResult> DeleteAsync(int id)
        {
            try
            {
                var result = await _dbSet.FindAsync(id);
                var entityType = typeof(TEntity).Name;
                if (result != null)
                {
                    if(result is ISoftDelete softDeletableEntity)
                    {
                        softDeletableEntity.IsDeleted = true;
                        _dbSet.Update(result);
                        await _context.SaveChangesAsync();
                        return new OperationResult() { Success = true, Message = $"{entityType} Deleted Successfully" };
                    }
                    else
                    {
                        _dbSet.Remove(result);
                        await _context.SaveChangesAsync();
                        return new OperationResult() { Success = true, Message = $"{entityType} Deleted Successfully" };
                    }
                }
                return new OperationResult() { Success = false, Message = $"{entityType} Not Found" };
            }
            catch (Exception ex) {
                return new OperationResult() { Success = false, Message = "Something Went Wrong. Please Try Again Later" , DevelopMessage = ex.Message};
            }

        }

        public async Task<OperationResult> GetAllAsync(string? searchQuery = null, string? searchProperty = "Name")
        {
            try
            {
                IQueryable<TEntity> query = _dbSet;
                if (!string.IsNullOrWhiteSpace(searchQuery))
                {
                    query = query.Where(e => EF.Functions.Like(
                        EF.Property<string>(e, searchProperty), $"%{searchQuery}%"));
                }
                var result = await query.ToListAsync();
                if(result.Count > 0)
                {
                    return new OperationResult { Success = true, Message = "Data retrieved successfully", Data = result };
                }
                else
                {
                    return new OperationResult { Success = true, Message = "NO Data Found!!", Data = result };
                }
            }
            catch (Exception ex)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = "Something Went Wrong. Please Try Again Later",
                    DevelopMessage = ex.Message
                };
            }
        }
        public async Task<OperationResult> GetByIdAsync(int id)
        {
            try
            {
                var entityType = typeof(TEntity).Name;
                var result = await _dbSet.FindAsync(id);
                if (result != null)
                {
                    return new OperationResult { Success = true, Message = "Data retrieved successfully", Data = result };
                }
                return new OperationResult { Success = false, Message = $"{entityType} Not Found" };
            }
            catch (Exception ex)
            {
                return new OperationResult() { Success = false, Message = "Something Went Wrong. Please Try Again Later", DevelopMessage = ex.Message };
            }
        }

        public async Task<OperationResult> UpdateAsync(TEntity updatedEntity)
        {
            try
            {
                var entityType = typeof(TEntity).Name;
                _dbSet.Update(updatedEntity);
                await _context.SaveChangesAsync();
                return new OperationResult() { Success = true, Message = $"{entityType} Updated Successfully" };
            }
            catch (Exception ex)
            {
                return new OperationResult() { Success = false, Message = "Something Went Wrong. Please Try Again Later", DevelopMessage = ex.Message };
            }
        }
        public async Task DeleteRangeAsync(IEnumerable<TEntity> entities)
        {
            _dbSet.RemoveRange(entities);
            await _context.SaveChangesAsync();
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}