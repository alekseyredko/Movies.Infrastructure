using Movies.Infrastructure.Models.Reviewer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Infrastructure.Models.Messages
{
    public class ChatMessageRequest
    {
        public ReviewerResponse Reviewer { get; set; }
        public string Message { get; set; }
        public int? ParentMessageId { get; set;  }
    }
}
