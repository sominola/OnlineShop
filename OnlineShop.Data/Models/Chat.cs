using System.Collections.Generic;

namespace OnlineShop.Data.Models
{
    public class Chat
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ChatUser> Users { get; set; } = new();
        public List<Message> Messages { get; set; } = new();
    }
}