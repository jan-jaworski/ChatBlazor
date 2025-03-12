namespace ChatBlazor.Models
{
    public class UserCard
    {
        public string UserName;
        public bool IsOnline;
        public string LastMessageTime;
        public string LastMessageText;

        public UserCard(string UserName, bool IsOnline, string LastMessageTime, string LastMessage)
        {
            this.UserName = UserName;
            this.IsOnline = IsOnline;
            this.LastMessageTime = LastMessageTime;
            this.LastMessageText = LastMessage;
            // TODO: Add more properties if needed
        }
    }
}
