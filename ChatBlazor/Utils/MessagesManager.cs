using ChatBlazor.Models;
using ChatBlazor.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChatBlazor.Utils;

public class MessagesManager
{
    private readonly AppDbContext _context;

    public MessagesManager(AppDbContext context)
    {
        _context = context;
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


    /// <summary>
    /// Sends a message from one user to another, creating a new chat if one doesn't exist.
    /// </summary>
    /// <param name="sender">The <see cref="IdentityUser"/> object representing the user sending the message.</param>
    /// <param name="receiver">The <see cref="IdentityUser"/> object representing the user receiving the message.</param>
    /// <param name="content">The string content of the message to be sent.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation of sending the message.</returns>
    /// <remarks>
    /// This method checks if a chat exists between the sender and receiver. If not, it creates a new chat.
    /// It then creates a new <see cref="Message"/> object and adds it to the database context.
    /// Note: SignalR hub implementation for real-time messaging is not included in this method.
    /// </remarks>
    public async Task SendMessage(IdentityUser sender, IdentityUser receiver, string content)
    {
        //here do the signalr hub thing 

        // Example:
        // await _hubContext.Clients.User(receiver.Id).SendAsync("ReceiveMessage", sender.UserName, message);}
        if (!CheckIfChatExists(sender, receiver))
        {
            await _context.Chats.AddAsync(new Chat { User1Id = sender.Id, User2Id = receiver.Id });
        }
        Message message = new Message
        {
            Sender = sender,
            Text = content,
            Chat = FindChat(sender, receiver)!
        };

        await _context.Messages.AddAsync(message);


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

}
