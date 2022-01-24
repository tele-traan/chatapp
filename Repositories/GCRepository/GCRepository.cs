using System.Linq;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;

using ChatApp.DB;
using ChatApp.Models;

namespace ChatApp.Repositories
{
    public class GCRepository : IGCRepository
    {
        private readonly ChatDbContext _context;
        public GCRepository(ChatDbContext context)
        {
            _context = context;
        }
        public void AddMessage(GCMessage message)
        {
            if (_context.GCMessages.Count() > 14) _context.GCMessages.Remove(_context.GCMessages.First());

            _context.GCMessages.Add(message);
            _context.SaveChanges();
        }
        public IEnumerable<GCMessage> GetLastMessages() 
            => _context.GCMessages
            .AsNoTracking()
            .OrderByDescending(gcm => gcm.DateTime);
    }
}