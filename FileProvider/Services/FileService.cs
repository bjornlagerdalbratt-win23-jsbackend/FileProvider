using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Data.Contexts;
using Data.Entities;
using FileProvider.Functions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.Extensions.Logging;
using System.Security.Policy;

namespace FileProvider.Services;

public class FileService(DataContext context, ILogger<FileService> logger, BlobServiceClient client)
{
    private readonly DataContext _context = context;
    private readonly ILogger<FileService> _logger = logger;
    private readonly BlobServiceClient _client = client;
    private BlobContainerClient _container;

    public async Task SetBlobContainerAsync(string containerName)
    {
        _container = _client.GetBlobContainerClient(containerName);
        await _container.CreateIfNotExistsAsync(PublicAccessType.BlobContainer);
    }

    public string SetFileName(IFormFile file)
    {
        var fileName = $"{Guid.NewGuid()}_{file.FileName}";
        return fileName;
    }

    public async Task<string> UploadFileAsync(IFormFile file, FileEntity fileEntity)
    {


        BlobHttpHeaders headers = new()
        {
            ContentType = file.ContentType, 
        };

        var blobClient = _container.GetBlobClient(fileEntity.FileName);

        using var stream = file.OpenReadStream();
        await blobClient.UploadAsync(stream, headers);

        return blobClient.Uri.ToString();

        
    }

    public async Task SaveToDatabaseAsync(FileEntity fileEntity)
    {
        _context.Files.Add(fileEntity);
        await _context.SaveChangesAsync();
    }

    //list all images
    public async Task<IEnumerable<FileEntity>> GetAllAsync()
    {
        return await _context.Files.ToListAsync();

    }

    //get one image

    public async Task<FileEntity> GetFileEntityAsync(string filePath)
    {
        return await _context.Files.FirstOrDefaultAsync(x => x.FilePath == filePath);
    }

    //get information from the file, such as filepath 
    public async Task<byte[]?> GetFileAsync(string filePath)
    {
        var fileEntity = await GetFileEntityAsync(filePath);
        if (fileEntity != null)
        {
            var blobClient = new BlobClient(new Uri(fileEntity.FilePath));
            if (await blobClient.ExistsAsync())
            {
                var downloadInfo = await blobClient.DownloadAsync();
                using var ms = new MemoryStream();
                await downloadInfo.Value.Content.CopyToAsync(ms);
                return ms.ToArray();
            }
        }
        return null;
    }
}
