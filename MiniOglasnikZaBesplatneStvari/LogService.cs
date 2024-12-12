using MiniOglasnikZaBesplatneStvari.Models;

namespace MiniOglasnikZaBesplatneStvari
{
    public class LogService : ILogService
    {
        private readonly AdvertisementRwaContext _context;

        public LogService(AdvertisementRwaContext context)
        {
            _context = context;
        }

        public void Log(DateTime timestamp, string level, string message)
        {
            var log = new Log
            {
                Level = level,
                Message = message,
                Timestamp = timestamp
            };
            _context.Logs.Add(log);
            _context.SaveChanges();
        }
    }
}
