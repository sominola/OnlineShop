using System;
using System.Threading.Tasks;
using OnlineShop.Data;
using OnlineShop.Data.Models;

namespace OnlineShop.Services.Chats
{
    public class MessageService
    {
        private readonly AppDbContext _db;

        public MessageService(AppDbContext db)
        {
            _db = db;
        }

        public async Task CreateMessage(Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            
            await _db.Messages.AddAsync(message);
            await _db.SaveChangesAsync();
        }
    }
}