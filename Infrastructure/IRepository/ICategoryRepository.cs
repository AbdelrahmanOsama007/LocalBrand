using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.IRepository
{
    public interface ICategoryRepository
    {
        Task<OperationResult> GetSubCatByCatId(int catId);
        Task<OperationResult> GetCategoryName(int catId);
    }
}
