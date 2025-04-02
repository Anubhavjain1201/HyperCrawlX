using HyperCrawlX.Models;
using HyperCrawlX.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HyperCrawlX.Controllers
{
    [ApiController]
    [Route("hyperCrawlX")]
    public class CrawlController : ControllerBase
    {
        private readonly ILogger<CrawlController> _logger;
        private readonly ICrawlRequestService _crawlRequestService;

        public CrawlController(
            ILogger<CrawlController> logger,
            ICrawlRequestService crawlRequestService)
        {
            _logger = logger;
            _crawlRequestService = crawlRequestService;
        }


        [HttpGet("getRequestStatus/{requestId}")]
        public async Task<IActionResult> GetCrawlStatus(long requestId)
        {
            var result = await _crawlRequestService.GetCrawlRequestStatus(requestId);
            return Ok(result);
        }

        [HttpPost("submitCrawlRequest")]
        public async Task<IActionResult> SubmitCrawlRequest([FromBody] CrawlRequest crawlRequest)
        {
            var result = await _crawlRequestService.SubmitCrawlRequest(crawlRequest.Url);
            return Ok(result);
        }
    }
}
