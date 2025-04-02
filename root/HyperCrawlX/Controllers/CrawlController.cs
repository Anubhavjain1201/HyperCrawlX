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


        [HttpPost("getRequestStatus")]
        public async Task<IActionResult> GetCrawlStatus([FromBody] long? requestId)
        {
            var result = await _crawlRequestService.GetCrawlRequestStatus(requestId);
            return Ok(result);
        }

        [HttpPost("submitCrawlRequest")]
        public async Task<IActionResult> SubmitCrawlRequest([FromBody] string? url)
        {
            var result = await _crawlRequestService.SubmitCrawlRequest(url);
            return Ok(result);
        }
    }
}
