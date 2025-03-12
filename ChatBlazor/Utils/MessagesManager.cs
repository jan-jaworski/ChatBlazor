using ChatBlazor.Models;
using ChatBlazor.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using ChatBlazor.Hubs;
using Microsoft.AspNetCore.SignalR.Client;
using System.Globalization;


namespace ChatBlazor.Utils;


/// <summary>
/// Manages chat messages and user interactions in the ChatBlazor application.
/// </summary>
/// <remarks>
/// This class provides functionality for retrieving users, managing chats,
/// sending messages, and handling real-time communication using SignalR.
/// </remarks>
public class MessagesManager
{



    private readonly AppDbContext _context;
    private readonly IdentityUser sender;
    private readonly IdentityUser? receiver;
    private readonly IHubContext<ChatHub> _hubContext;


    /// <summary>
    /// Initializes a new instance of the <see cref="MessagesManager"/> class with a sender but without a specific receiver.
    /// </summary>
    /// <param name="context">The database context for the application, providing access to the underlying data store.</param>
    /// <param name="hubContext">The SignalR hub context for real-time communication, enabling broadcasting messages to connected clients.</param>
    /// <param name="sender">The identity of the user sending messages, representing the current user of the messaging system.</param>
    /// <remarks>
    /// This constructor is typically used when initializing the MessagesManager for general operations
    /// that don't require a specific receiver, such as retrieving all users or managing global chat functionalities.
    /// </remarks>
    public MessagesManager(AppDbContext context, IHubContext<ChatHub> hubContext, IdentityUser sender)
    {
        _context = context;
        _hubContext = hubContext;
        this.sender = sender;
    }








    /// <summary>
    /// Initializes a new instance of the <see cref="MessagesManager"/> class.
    /// </summary>
    /// <param name="context">The database context for the application.</param>
    /// <param name="hubContext">The SignalR hub context for real-time communication.</param>
    /// <param name="sender">The user sending the message.</param>
    /// <param name="receiver">The user receiving the message.</param>
    public MessagesManager(AppDbContext context, IHubContext<ChatHub> hubContext, IdentityUser sender, IdentityUser receiver)
    {
        _context = context;
        _hubContext = hubContext;
        this.sender = sender;
        this.receiver = receiver;
    }

    /// <summary>
    /// Retrieves a list of all registered users from the application's database.
    /// </summary>
    /// <returns>
    /// A list of <see cref="IdentityUser"/> objects representing all registered users.
    /// </returns>
    public async Task<List<IdentityUser>> GetUsers()
    {
        return await _context.Users.ToListAsync();
    }


    public async Task<List<UserCard>> GetUserCards()
    {
        var users = await GetUsers();
        var userCards = new List<UserCard>();
        foreach (var user in users)
        {
            string username = user.UserName;
            var lastMessage = await GetLastMessageForChat(user);
            string lastMessageText = lastMessage is not null ? lastMessage.Text : "";
            CultureInfo provider = new CultureInfo("pl-PL");
            string lastMessageTime;

            if (lastMessage?.Timestamp.Date == DateTime.Today)
            {
                lastMessageTime = lastMessage.Timestamp.ToString("t", provider);
            }
            else if (lastMessage is not null)
            {
                lastMessageTime = lastMessage.Timestamp.ToString("f");
            }
            else
            {
                lastMessageTime = "";
            }


            UserCard userCard = new UserCard(username, CheckIfUserOnline(user), lastMessageTime, lastMessageText);
            userCards.Add(userCard);
        }
        return userCards;
    }





    /// <summary>
    /// Retrieves all messages for a chat between the sender and receiver.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains:
    /// <list type="bullet">
    /// <item>
    /// <description>A list of <see cref="Message"/> objects if the chat exists.</description>
    /// </item>
    /// <item>
    /// <description><c>null</c> if the chat does not exist.</description>
    /// </item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// This method checks if a chat exists between the sender and receiver using the <see cref="CheckIfChatExists"/> method.
    /// If the chat exists, it returns all messages from the database context.
    /// </remarks>
    public async Task<List<Message>?> GetMessagesForChat()
    {
        if (CheckIfChatExists(sender, receiver))
        {
            var chatId = FindChat(sender, receiver)!.ChatId;
            return await _context.Messages.Where(m => m.ChatId == chatId).OrderBy(m => m.Timestamp).ToListAsync(); //from the oldest to newest if u want reverse it use OrderByDescending() instead OrderBy 
        }
        else return null;
    }


    /// <summary>
    /// Retrieves the most recent message from the chat between the sender and receiver.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains:
    /// <list type="bullet">
    ///   <item>
    ///     <description>The most recent <see cref="Message"/> object if the chat exists and contains messages.</description>
    ///   </item>
    ///   <item>
    ///     <description><c>null</c> if the chat does not exist or contains no messages.</description>
    ///   </item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// This method first checks if a chat exists between the sender and receiver using the <see cref="CheckIfChatExists"/> method.
    /// If the chat exists, it queries the database for the most recent message in that chat, ordered by timestamp.
    /// </remarks>
    public async Task<Message?> GetLastMessageForChat(IdentityUser receiverUser)
    {
        if (CheckIfChatExists(sender, receiverUser))
        {
            var chatId = FindChat(sender, receiverUser)!.ChatId;
            return await _context.Messages
                .Where(m => m.ChatId == chatId)
                .OrderByDescending(m => m.Timestamp)
                .FirstOrDefaultAsync();
        }
        else return null;
    }








    /// <summary>
    /// Sends a message from one user to another, creating a new chat if one doesn't exist.
    /// </summary>
    /// <param name="content">The string content of the message to be sent.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation of sending the message.</returns>
    /// <remarks>
    /// This method checks if a chat exists between the sender and receiver. If not, it creates a new chat.
    /// It then creates a new <see cref="Message"/> object and adds it to the database context.
    /// Note: SignalR hub implementation for real-time messaging is not included in this method.
    /// </remarks>
    public async Task SendMessage(string content)
    {


        if (!CheckIfChatExists(sender, receiver))
        {
            await _context.Chats.AddAsync(new Chat { User1Id = sender.Id, User2Id = receiver.Id });
            await _context.SaveChangesAsync();
        }
        Message message = new Message
        {
            Sender = sender,
            Receiver = receiver,
            Text = content,
            Chat = FindChat(sender, receiver)!
        };

        //add to context
        await _context.Messages.AddAsync(message);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception dbEx)
        {
            Console.WriteLine($"Database error: {dbEx.Message}");
            throw new Exception("Failed to save message to database", dbEx);
        }
        /////////////////


        var messageDTO = new MessageDto
        {
            SenderId = message.SenderId,
            SenderName = message.Sender.UserName!,
            ReceiverId = message.ReceiverId,
            Text = message.Text
        };

        // Access the static property
        var connectedUsers = ChatHub.ConnectedUsers;

        // Check if the users are connected
        if (connectedUsers.TryGetValue(message.SenderId, out var senderConnectionId) &&
            connectedUsers.TryGetValue(message.ReceiverId, out var receiverConnectionId))
        {
            // Send to specific connections
            await _hubContext.Clients.Clients(senderConnectionId, receiverConnectionId)
                .SendAsync("ReceiveMessage", messageDTO);
        }






    }


    /// <summary>
    /// Checks if a chat exists between the specified sender and receiver.
    /// </summary>
    /// <param name="sender">The <see cref="IdentityUser"/> representing the sender of the message.</param>
    /// <param name="receiver">The <see cref="IdentityUser"/> representing the receiver of the message.</param>
    /// <returns>
    /// <c>true</c> if a chat exists between the sender and receiver; otherwise, <c>false</c>.
    /// </returns>
    private Boolean CheckIfChatExists(IdentityUser sender, IdentityUser receiver)
    {
        return _context.Chats.Any(c => (c.User1Id == sender.Id && c.User2Id == receiver.Id) || (c.User1Id == receiver.Id && c.User2Id == sender.Id));
    }



    /// <summary>
    /// Finds a chat between two specified users.
    /// </summary>
    /// <param name="User1">The first <see cref="IdentityUser"/> involved in the chat.</param>
    /// <param name="User2">The second <see cref="IdentityUser"/> involved in the chat.</param>
    /// <returns>
    /// A <see cref="Chat"/> object representing the chat between the two users if found; otherwise, <c>null</c>.
    /// </returns>
    /// <remarks>
    /// This method searches for a chat where the two specified users are participants, regardless of their order (User1 or User2).
    /// </remarks>
    private Chat? FindChat(IdentityUser User1, IdentityUser User2) => _context.Chats.FirstOrDefault(c => (c.User1Id == User1.Id && c.User2Id == User2.Id) || (c.User1Id == User2.Id && c.User2Id == User1.Id));

    private static bool CheckIfUserOnline(IdentityUser user)
    {
        if (ChatHub.ConnectedUsers.ContainsKey(user.Id)) return true;
        else return false;
    }
    


}
