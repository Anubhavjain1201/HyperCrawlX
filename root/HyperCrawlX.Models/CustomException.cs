namespace HyperCrawlX.Models
{
    public class CustomException : Exception
    {
        public int StatusCode { get; set; }

        public string ErrorMessage { get; set; }

        public CustomException(int statusCode, string errorMessage)
        {
            StatusCode = statusCode;
            ErrorMessage = errorMessage;
        }
    }
}
