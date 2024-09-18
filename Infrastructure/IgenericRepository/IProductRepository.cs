using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.IgenericRepository
{
    public interface IProductRepository
    {
        Task<OperationResult> GetProductsBySubCaregory(int id);
    }
}
