using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class ImageService
    {
        private readonly Cloudinary _cloudinary;

        public ImageService()
        {
            const string cloudName = "dmwxxtnfy";
            const string apiKey = "875994724468753";
            const string apiSecret = "BLQEajzAb_VugC6PYQJFwjlnOrY";
            var account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account);
        }
        public List<string> UploadBase64Images(List<string> base64Images)
        {
            // Store uploaded URLs
            var uploadedImageUrls = new List<string>();

            foreach (var base64Image in base64Images)
            {
                // Ensure the base64 string starts with the appropriate prefix
                string formattedBase64Image = base64Image.StartsWith("data:image")
                    ? base64Image
                    : "data:image/png;base64," + base64Image;

                // Set up the upload parameters
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(formattedBase64Image),
                    Folder = "your_folder_name" // Optional: specify a folder
                };

                // Perform the upload
                var uploadResult = _cloudinary.Upload(uploadParams);

                if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    uploadedImageUrls.Add(uploadResult.Url.ToString()); // Collect the URL
                }
                else
                {
                    throw new Exception($"Image upload failed: {uploadResult.Error?.Message}");
                }
            }

            return uploadedImageUrls; // Return the list of URLs
        }

    }
    }
