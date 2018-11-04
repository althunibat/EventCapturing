using System;

namespace Events.Hubs
{
    public class Message
    {
        public string Event { get; set; }
        public string Application { get; set; }
        public string Area { get; set; }
        public DateTime TimeStamp { get; set; }
        public override string ToString()
        {
            return $"{TimeStamp}:{Application}.{Area}-{Event}";
        }
    }
}