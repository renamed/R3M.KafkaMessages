using System;

namespace WritRead.Model
{
    public class MessageModel
    {
        public string Text { get; set; }
        public string ID { get; set; }
        public DateTime Timestamp { get; set; }
        public string ReqID { get; set; }
    }
}