using Microsoft.AspNetCore.Identity;

namespace ChatBlazor.Models
{
    public class Message
    {
        public int MessageId { get; set; }
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }



        //relations
        public int ChatId { get; set; }
        public Chat Chat { get; set; }

        public string SenderId { get; set; }
        public IdentityUser Sender { get; set; }

        public string ReceiverId { get; set; }
        public IdentityUser Receiver { get; set; }

    }
}
