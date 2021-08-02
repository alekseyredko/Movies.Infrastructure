using AutoMapper;
using Movies.Infrastructure.Models.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Infrastructure.MappingProfiles
{
    public class ChatMessageMappingProfile: Profile
    {
        public ChatMessageMappingProfile()
        {
            CreateMap<ChatMessageRequest, ChatMessageResponse>();
        }
    }
}
