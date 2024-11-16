using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Colors.Interfaces
{
    public interface IColorService
    {
        Task<OperationResult> GetAllColors();
    }
}
