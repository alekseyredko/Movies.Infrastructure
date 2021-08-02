using AutoMapper;
using Movies.Infrastructure.MappingProfiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Infrastructure.Models.Messages
{
    public class ChatMessageService
    {
        private int insertMessageId = 1;
        public void AddMessage(List<ChatMessageResponse> messages, ChatMessageRequest chatMessageRequest)
        {
            if (!chatMessageRequest.ParentMessageId.HasValue)
            {
                ChatMessageResponse mapped = CreateMessage(chatMessageRequest);
                messages.Add(mapped);
                return;
            }
            else
            {
                var found = FindMessage(messages, chatMessageRequest.ParentMessageId.Value);
                if (found == null)
                {
                    ChatMessageResponse mapped = CreateMessage(chatMessageRequest);
                    messages.Add(mapped);
                    return;
                }
                else
                {
                    ChatMessageResponse mapped = CreateMessage(chatMessageRequest);
                    mapped.ParentMessage = found;
                    found.Replies.Add(mapped);
                    return;
                }
            }
        }

        private ChatMessageResponse CreateMessage(ChatMessageRequest chatMessageRequest)
        {
            var config = new MapperConfiguration(config => config.AddProfile<ChatMessageMappingProfile>());
            var mapper = new Mapper(config);
            var mapped = mapper.Map<ChatMessageResponse>(chatMessageRequest);
            mapped.DateTime = DateTime.Now;
            mapped.ChatMessageId = insertMessageId++;
            return mapped;
        }

        public ChatMessageResponse FindMessage(ICollection<ChatMessageResponse> messages, int id)
        {
            foreach (var message in messages)
            {
                if (message.ChatMessageId != id)
                {
                    var result = FindMessage(message.Replies, id);
                    if (result != null) return result;
                }

                else return message;
            }
            return null;
        }
    }
}
