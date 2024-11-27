
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public interface IFileCloudService
    {
        string GetImageUrl(string publicId);
        string getPublicId(string url);
        Task<bool> DeleteImageAsync(string publicId);
        Task<string> UploadImagesAsync(IFormFile picture);
        Task<string> UpdateImageAsync(IFormFile picture, string publicId);
        public IFormFile ConvertFilePathToIFormFile(string filePath);
 
    }
}
