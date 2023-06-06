using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dtos
{
    public class SendLogNotificationApiDto
    {
        public CrawlerLogDto Log { get; set; }
        public CrawlerLogDto OrderLog { get; set; }
        public CrawlerLogDto ProductLog { get; set; }
        public string ConnectionId { get; set; }

        public SendLogNotificationApiDto(CrawlerLogDto log, CrawlerLogDto orderLog, CrawlerLogDto productLog, string connectionId)
        {
            Log = log;

            OrderLog = orderLog;

            ProductLog = productLog;

            ConnectionId = connectionId;
        }
    }
}
