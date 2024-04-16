using System.IO;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AzureStorageLibrary;
using Microsoft.AspNetCore.Mvc;
using MVCWebApp.Models;

namespace MVCWebApp.Controllers
{
    public class BlobsController : Controller
    {
        private readonly IBlobStorage _blobStorage;

        public BlobsController(IBlobStorage blobStorage)
        {
            _blobStorage = blobStorage;
        }

        public IActionResult Index()
        {
            var names = _blobStorage.GetNames(EContainerName.pictures);
            var pdfs = _blobStorage.GetNames(EContainerName.pdf);
            string blobUrl = $"{_blobStorage.BlobUrl}/{EContainerName.pictures.ToString()}";
            string blobUrlPdf = $"{_blobStorage.BlobUrl}/{EContainerName.pdf.ToString()}";

            ViewBag.blobs = names.Select(x => new FileBlob { Name = x, Url = $"{blobUrl}/{x}" }).ToList();
            ViewBag.blobsPdf = pdfs.Select(x => new FileBlob { Name = x, Url = $"{blobUrlPdf}/{x}" }).ToList();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile picture)
        {
            var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(picture.FileName);
            await _blobStorage.UploadAsync(picture.OpenReadStream(), newFileName, EContainerName.pictures);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UploadPdf(IFormFile pdf)
        {
            string connectionString = ConnectionStrings.AzureStorageConnectionString; // Gerçek bağlantı dizesi ile değiştirin
            string containerName = EContainerName.pdf.ToString(); // Gerçek konteyner adı ile değiştirin

            string fileName = Path.GetFileName(pdf.FileName);

            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            // Dosyayı Azure Blob Storage'e yükle
            using (Stream stream = pdf.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, true);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Download(string filename)
        {
            var stream = await _blobStorage.DownloadAsync(filename, EContainerName.pictures);
            return File(stream, "application/octet-stream", filename);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string fileName)
        {
            await _blobStorage.DeleteAsync(fileName, EContainerName.pictures);
            return RedirectToAction("Index");
        }


    }
}
