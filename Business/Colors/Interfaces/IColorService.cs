using Business.Colors.Dtos;
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
        Task<GridOperationResult> GetColorGrid(ColorPagination colormodel);
        Task<OperationResult> AddColor(ColorDto color);
        Task<OperationResult> GetColorById(int id);
        Task<OperationResult> UpdateColor(ColorDto color);
        Task<OperationResult> DeleteColor(int id);
    }
}
