using Business.Colors.Dtos;
using Business.Sizes.Dtos;
using Business.Sizes.Interfaces;
using Infrastructure.IGenericRepository;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Sizes.Validator
{
    public class SizeService : ISizeService
    {
        private readonly IGenericRepository<Size> _sizerepository;
        public SizeService(IGenericRepository<Size> sizerepository)
        {
            _sizerepository = sizerepository;
        }
        public async Task<OperationResult> GetAllSizes()
        {
            try
            {
                var result = await _sizerepository.GetAllAsync();
                if (!result.Success)
                {
                    return result;
                }

                var sizes = (List<Size>)result.Data;
                var sizesdtos = new List<SizeDto>();

                foreach (var size in sizes)
                {
                    var sizedto = new SizeDto()
                    {
                        Id = size.Id,
                        SizeKey = size.SizeKey,
                        SizeName = size.SizeName,
                    };
                    sizesdtos.Add(sizedto);
                }
                return new OperationResult() { Success = true, Data = sizesdtos, Message = result.Message };
            }
            catch (Exception ex)
            {
                return new OperationResult() { Success = false, Message = "Something Went Wrong. Please Try Again Later", DevelopMessage = ex.Message };
            }
        }
    }
}
