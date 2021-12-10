using OnlineShop.Services.Parser;

namespace OnlineShop.Services
{
    public class Config
    {
        public static string ConnectionString { get; set; }

        public static string CompanyName { get; set; }
        public static string PathToPhoto { get; set; }
        public static ConfigProductParser ConfigProductParser { get; set; }
        public static string WebRootPath { get; set; }
    }
}