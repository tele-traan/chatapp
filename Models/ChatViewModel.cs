﻿using System.Collections.Generic;
namespace ChatApp.Models
{
    public class ChatViewModel : BaseViewModel
    {
        public List<GlobalChatUser> Users { get; set; }
    }
}
