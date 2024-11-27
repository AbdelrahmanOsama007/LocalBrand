using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{

    public class FileCloudService: IFileCloudService
    {

        private readonly Cloudinary _cloudinary;
        public FileCloudService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }
        public string GetImageUrl(string? publicId)
        {
            if (publicId is null) return null;
            var getParams = new GetResourceParams(publicId);

            var result = _cloudinary.GetResource(getParams);
            // Return the URL of the retrieved image
            return result.SecureUrl;
        }

        public string getPublicId(string url)
        {
            var uri = new Uri(url);
            var publicId = uri.Segments[^1].Split('.')[0];
            return publicId;
        }

        public async Task<bool> DeleteImageAsync(string publicId)
        {
            var deletionParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deletionParams);
            return result.Result == "ok";
        }

        public async Task<string> UploadImagesAsync(IFormFile picture)
        {
            if (picture.Length <= 0) return null;

            await using var stream = picture.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(picture.FileName, stream)
            };
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult.PublicId;
        }

        public async Task<string> UpdateImageAsync(IFormFile picture, string publicId)
        {
            var deletionResult = await DeleteImageAsync(publicId);
            if (!deletionResult) return null;

            var newPublicId = await UploadImagesAsync(picture);
            return newPublicId;
        }
        public  IFormFile ConvertFilePathToIFormFile(string filePath)
        {
            // Ensure the file exists
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The file was not found.", filePath);
            }

            // Open the file as a stream
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            // Create a FormFile from the stream
            IFormFile formFile = new FormFile(fileStream, 0, fileStream.Length, "file", Path.GetFileName(filePath));

            return formFile;
        }

    }
}
