using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Sandbox.Services;

namespace Sandbox.Controllers
{
    public class ImageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult UploadImage(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                // Resize the image
                var resizedFilePath = Path.Combine(uploadsFolder, "resized_" + uniqueFileName);
                long originalSize, resizedSize;

                using (var stream = file.OpenReadStream())
                {
                    (originalSize, resizedSize) = ImageHelper.ResizeImage(stream, resizedFilePath, 800, 600); // Adjust dimensions as needed
                }

                // Pass file sizes to the view
                ViewBag.OriginalSize = originalSize;
                ViewBag.ResizedSize = resizedSize;
                ViewBag.OriginalFileName = file.FileName;
                ViewBag.ResizedFileName = Path.GetFileName(resizedFilePath);

                return View("UploadResult");
            }

            return View("Error");
        }
    }
}
