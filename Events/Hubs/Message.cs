using System;

namespace Events.Hubs
{
    public class Message
    {
        public string Event { get; set; }
        public string Application { get; set; }
        public string Area { get; set; }
        public DateTime TimeStamp { get; set; }
    }

    public class Event
    {
        public int Index { get; set; }
        public string Turn { get; set; }
        public bool Winner { get; set; }
    }
}