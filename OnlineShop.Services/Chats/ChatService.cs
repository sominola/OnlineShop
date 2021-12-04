using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Data.Models;

namespace OnlineShop.Services.Chats
{
    public class ChatService
    {
        private readonly AppDbContext _db;

        public ChatService(AppDbContext db)
        {
            _db = db;
        }
        
        public async Task CreateChat(Chat chat)
        {
            if (chat == null)
            {
                throw new ArgumentNullException(nameof(chat));
            }
            await _db.Chats.AddAsync(chat);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteChat(int chatId)
        {
            var chat = await _db.Chats.Include(x=>x.Users).FirstOrDefaultAsync(x => x.Id == chatId);
            if (chat != null) _db.Chats.Remove(chat);
            await _db.SaveChangesAsync();
        }

        public IQueryable<Chat> GetAllChats()
        {
            return _db.Chats.AsNoTracking();
        }

        public async Task<Chat> GetChatById(int id)
        {
            return await _db.Chats.Include(x=>x.Messages).Include(x=> x.Users).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddUserToChat(int chatId, User user)
        {
            var chat = await _db.Chats.Include(x=>x.Users).FirstOrDefaultAsync(x => x.Id == chatId);
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var chatUser = new ChatUser()
            {
                UserId = user.Id
            };
            chat?.Users.Add(new ChatUser {UserId = user.Id});
            await _db.SaveChangesAsync();
        }
        
        public async Task RemoveUserFromChat(int chatId, User user)
        {
            var chat = await _db.Chats.Include(x=>x.Users).FirstOrDefaultAsync(x => x.Id == chatId);
            var dbUser = await _db.Users.FirstOrDefaultAsync(x => x.Id == user.Id);
            if (dbUser == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            
            chat?.Users.Remove(new ChatUser(){UserId = user.Id});
            await _db.SaveChangesAsync();
        }
        
    }
}