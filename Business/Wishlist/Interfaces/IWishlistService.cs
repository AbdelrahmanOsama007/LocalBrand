using Business.Wishlist.Dtos;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Wishlist.Interfaces
{
    public interface IWishlistService
    {
        Task<OperationResult> GetWishlistProducts(int[] Ids);
    }
}