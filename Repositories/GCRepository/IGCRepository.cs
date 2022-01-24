using System.Collections.Generic;

using ChatApp.Models;

namespace ChatApp.Repositories
{
    public interface IGCRepository
    {
        void AddMessage(GCMessage message);
        IEnumerable<GCMessage> GetLastMessages(); 

    }
}
