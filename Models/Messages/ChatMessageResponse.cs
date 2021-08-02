using Movies.Infrastructure.Models.Reviewer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Infrastructure.Models.Messages
{
    public class ChatMessageResponse
    {
        public int ChatMessageId { get; set; }
        public string Message { get; set; }
        public DateTime DateTime{ get; set; }
        public int ReviewerId { get; set; }
        public ReviewerResponse Reviewer { get; set; }
        public int ParentMessageId { get; set; }
        public ChatMessageResponse ParentMessage { get; set; }
        public ICollection<ChatMessageResponse> Replies { get; set; }

        public ChatMessageResponse()
        {
            Replies = new List<ChatMessageResponse>();
        }
    }
}
