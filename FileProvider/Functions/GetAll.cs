using FileProvider.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FileProvider.Functions
{
    public class GetAll(ILogger<GetAll> logger, FileService fileService)
    {
        private readonly ILogger<GetAll> _logger = logger;
        private readonly FileService _fileService = fileService;

        [Function("Fetch")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {
            try
            {
                if (req.Form.Files["file"] == null)
                {
                    
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return new BadRequestResult();
        }
    }
}
