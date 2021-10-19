namespace OnlineShop.Web.ViewModels
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public string ErrorStatusCode { get; set; }

        public string OriginalUrl { get; set; }
        public bool ShowOriginalUrl => !string.IsNullOrEmpty(OriginalUrl);
    }
}