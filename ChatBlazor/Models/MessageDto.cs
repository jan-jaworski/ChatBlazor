namespace ChatBlazor.Models
{
    public class MessageDto
    {
        public int MessageId { get; set; }
        public string SenderId { get; set; }
        public string SenderName { get; set; }

        public string ReceiverId { get; set; }
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }
        public int ChatId { get; set; }
    }
}
