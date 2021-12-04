using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data.Models;
using OnlineShop.Services.Chats;
using OnlineShop.Web.Hubs;
using OnlineShop.Web.ViewModels.Chats;

namespace OnlineShop.Web.Controllers;
public class ChatController : Controller
{
    private readonly IHubContext<ChatHub> _chatHub;
    private readonly UserManager<User> _userManager;
    private readonly ChatService _chatService;
    private readonly MessageService _messageService;

    public ChatController(IHubContext<ChatHub> chatHub, ChatService chatService, MessageService messageService,
        UserManager<User> userManager)
    {
        _chatHub = chatHub;
        _chatService = chatService;
        _messageService = messageService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var chats = await _chatService.GetAllChats().ToListAsync();
        var model = new ChatIndexViewModel()
        {
            Chats = chats,
            CurrentChat = chats.LastOrDefault()
        };
        return View(model);
    }
    
    public async Task<IActionResult> Current(string chatId)
    {
        var chats = _chatService.GetAllChats();
        var model = new ChatIndexViewModel
        {
            Chats = await chats.Include(x=> x.Messages).ToListAsync(),
            CurrentChat = chats.Include(x=>x.Messages).FirstOrDefault()
        };
        return View("Index",model);
    }
    
    public async Task<IActionResult> CreateRoom(string name)
    {
        var chat = new Chat{
            Name = name,
        };

        var user = await _userManager.GetUserAsync(User);
        
        chat.Users.Add(new ChatUser
        {
            UserId = user.Id,
        });
        await _chatService.CreateChat(chat);
        return Ok();
    }

    
    [HttpPost]
    public async Task<IActionResult> SendMessage(int roomId, string message)
    {
        var user = await _userManager.GetUserAsync(User);
        var chat = await _chatService.GetAllChats().FirstOrDefaultAsync();
        var messages = new Message
        {
            ChatId = chat.Id,
            Text = message,
            UserName = user.Name,
            Timestamp = DateTime.Now
        };
        
        await _messageService.CreateMessage(messages);
        await _chatHub.Clients.All
            .SendAsync("ReceiveMessage", new{
                Text = messages.Text,
                Name = messages.UserName,
                Timestamp = messages.Timestamp.ToString("dd/MM/yyyy hh:mm:ss") 
            });
        return Ok();
    }
    
}