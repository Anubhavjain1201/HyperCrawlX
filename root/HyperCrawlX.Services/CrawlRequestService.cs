using HyperCrawlX.DAL.Interfaces;
using HyperCrawlX.Models;
using HyperCrawlX.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net;

namespace HyperCrawlX.Services
{
    public class CrawlRequestService : ICrawlRequestService
    {
        private readonly ILogger<CrawlRequestService> _logger;
        private readonly ICrawlRequestRepository _crawlRequestRepository;

        public CrawlRequestService(
            ILogger<CrawlRequestService> logger,
            ICrawlRequestRepository crawlRequestRepository)
        {
            _logger = logger;
            _crawlRequestRepository = crawlRequestRepository;
        }

        /// <inheritdoc/>
        public async Task<CrawlRequestStatus> GetCrawlRequestStatus(long? requestId)
        {
            await ValidateRequestId(requestId);
            return await _crawlRequestRepository.GetCrawlRequestStatus(requestId);
        }

        /// <inheritdoc/>
        public async Task<CrawlRequest> SubmitCrawlRequest(string? url)
        {
            ValidateUrl(url);
            int requestId = await _crawlRequestRepository.SubmitCrawlRequest(url);
            return new CrawlRequest{ RequestId = requestId, Url = url };
        }

        /// <summary>
        /// Validates the requestId
        /// </summary>
        private async Task ValidateRequestId(long? requestId)
        {
            _logger.LogInformation("CrawlRequestService - Validating requestId");

            if (requestId == null || requestId == 0)
            {
                _logger.LogError("CrawlRequestService - requestId is a required field");
                throw new CustomException((int)HttpStatusCode.BadRequest, "requestId is a required field");
            }

            bool isValidRequestId = await _crawlRequestRepository.IsValidRequestId((long)requestId);
            if (!isValidRequestId)
            {
                _logger.LogError("CrawlRequestService - requestId is invalid");
                throw new CustomException((int)HttpStatusCode.BadRequest, "requestId is invalid");
            }

            _logger.LogInformation("CrawlRequestService - Finished validating requestId");
        }

        /// <summary>
        /// Validates the url
        /// </summary>
        private void ValidateUrl(string? url)
        {
            _logger.LogInformation("CrawlRequestService - Validating url");

            if (string.IsNullOrWhiteSpace(url))
            {
                _logger.LogError("CrawlRequestService - url is a required field");
                throw new CustomException((int)HttpStatusCode.BadRequest, "url is a required field");
            }

            bool isValidUrl = Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            if (!isValidUrl)
            {
                _logger.LogError("CrawlRequestService - url is invalid");
                throw new CustomException((int)HttpStatusCode.BadRequest, "url is invalid");
            }

            _logger.LogInformation("CrawlRequestService - Finished validating url");
        }
    }
}
