using Microsoft.AspNetCore.SignalR;
using Movies.Infrastructure.Models.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Infrastructure.Hubs
{
    public class MovieChatHub: Hub
    {
        public async Task SendMessageAsync(ChatMessageRequest request)
        {
            await Clients.All.SendAsync("ReceiveMessageAsync", request);
        }
    }
}
