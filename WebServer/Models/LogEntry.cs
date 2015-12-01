using System;
using NLog;

namespace WebServer.Models
{
    public class LogEntry : Entity
    {
        public virtual DateTime TimeStamp { get; set; }
        public virtual string Message { get; set; }
    }
}