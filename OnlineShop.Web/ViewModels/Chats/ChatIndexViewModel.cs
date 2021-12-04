using System.Collections.Generic;
using OnlineShop.Data.Models;

namespace OnlineShop.Web.ViewModels.Chats;

public class ChatIndexViewModel
{
    public ICollection<Chat> Chats { get; set; }
    public Chat CurrentChat { get; set; }
}