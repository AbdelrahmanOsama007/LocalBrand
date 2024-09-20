using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.IRepository
{
    public interface IProductRepository
    {
        Task<OperationResult> GetProductsBySubCaregory(int id);
        Task<OperationResult> GetBestSellers();
    }
}
