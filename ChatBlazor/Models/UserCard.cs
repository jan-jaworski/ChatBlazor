namespace ChatBlazor.Models
{
    public class UserCard
    {
        public string UserName;
        public bool IsOnline;
        public string LastMessageTime;
        public string LastMessageText;
        public DateTime LastMessageDateTime;

        public UserCard(string UserName, bool IsOnline, string LastMessageTime, string LastMessage, DateTime LastMessageDateTime)
        {
            this.UserName = UserName;
            this.IsOnline = IsOnline;
            this.LastMessageTime = LastMessageTime;
            this.LastMessageText = LastMessage;
            this.LastMessageDateTime = LastMessageDateTime;
            // TODO: Add more properties if needed
        }
    }
}
