using Microsoft.Xaml.Behaviors.Media;

namespace Server.Classes
{
    public class SERVER
    {
        public UDP udp = new UDP();
        public string CurrentLogin { get; set; }
        //public List<User> Users = new List<User>();
        public async Task<string> SendClient(string message) => await udp.SendAsync(message);
    }
}
