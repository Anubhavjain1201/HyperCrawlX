using Microsoft.AspNetCore.Mvc;

namespace HyperCrawlX.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CrawlController : ControllerBase
    {
        private readonly ILogger<CrawlController> _logger;

        public CrawlController(ILogger<CrawlController> logger)
        {
            _logger = logger;
        }


        [HttpGet("")]
        public async Task<IActionResult> GetCrawlStatus()
        {
            throw new NotImplementedException();
        }

        [HttpPost("")]
        public async Task<IActionResult> FetchProductLinks()
        {
            throw new NotImplementedException();
        }
    }
}
