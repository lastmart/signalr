using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace BadNews.Hubs
{
    public class CommentsHub: Hub
    {
        public async Task SendComment(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveComment", user, message).ConfigureAwait(false);
        }
    }
}