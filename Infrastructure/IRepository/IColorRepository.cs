using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.IRepository
{
    public interface IColorRepository
    {
        Task<GridOperationResult> GetColorGrid(ColorPagination colormodel);
    }
}
