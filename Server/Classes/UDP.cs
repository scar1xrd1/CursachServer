using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server.Classes
{
    public class UDP
    {
        public string Ip { get; set; }
        public string LocalPort { get; set; }
        public string RemotePort { get; set; }
        private UdpClient udp;
        private IPEndPoint? remoteEP;

        public UDP()
        {
            Ip = "127.0.0.1";
            LocalPort = "55960";
            RemotePort = "55961";

            udp = new UdpClient(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 55960));
        }

        public UDP(string ip, string localPort, string remotePort)
        {
            Ip = ip;
            LocalPort = localPort;
            RemotePort = remotePort;
            udp = new UdpClient(new IPEndPoint(IPAddress.Parse(Ip), int.Parse(LocalPort)));
        }

        public void SetPoint(string ip, string localPort, string remotePort)
        {
            Ip = ip;
            LocalPort = localPort;
            RemotePort = remotePort;
            udp = new UdpClient(new IPEndPoint(IPAddress.Parse(Ip), int.Parse(LocalPort)));
        }

        public async Task<string> ReceiveAsync()
        {
            string feedback = "";
            try
            {
                var result = await udp.ReceiveAsync();
                feedback = Encoding.UTF8.GetString(result.Buffer);
            }
            catch (Exception e) { feedback = $"Ошибка получения данных: {e.Message}"; }
            return feedback;
        }

        public async Task<string> SendAsync(string data)
        {
            string feedback = "Данные успешно отправлены.";
            try
            {
                remoteEP = new IPEndPoint(IPAddress.Parse(Ip), int.Parse(RemotePort));
                await udp.SendAsync(Encoding.UTF8.GetBytes(data), remoteEP);
            }
            catch (Exception e) { feedback = $"Ошибка отправки данных: {e.Message}"; }
            return feedback;
        }
    }
}
