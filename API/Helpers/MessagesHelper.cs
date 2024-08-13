using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;

namespace API.Helpers
{
    public static class MessagesHelper
    {
        public static bool AreMessagesPropsNull(AppUser? recipient,
        AppUser? sender, string senderUsername,
         string recipientusername)
        {
            return recipient == null || sender == null || senderUsername == null || recipientusername == null;
        }
    }
}