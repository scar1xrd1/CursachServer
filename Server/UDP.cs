using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    public class UDP
    {
        public string Ip { get; set; }
        public string LocalPort { get; set; }
        public string RemotePort { get; set; }
        private UdpClient udp;

        public UDP()
        {
            Ip = IPAddress.Any.ToString();
            LocalPort = "55960";
            RemotePort = "55961";

            udp = new UdpClient(new IPEndPoint(IPAddress.Any, 55961));
        }

        public UDP(string ip, string localPort, string remotePort)
        {
            Ip = ip;
            LocalPort = localPort;
            RemotePort = remotePort;

            udp = new UdpClient(new IPEndPoint(IPAddress.Parse(Ip), int.Parse(RemotePort)));
        }

        public void SetPoint(string ip, string localPort, string remotePort)
        {
            Ip = ip;
            LocalPort = localPort;
            RemotePort = remotePort;
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
                await udp.SendAsync(Encoding.UTF8.GetBytes(data));
            }
            catch (Exception e) { feedback = $"Ошибка отправки данных: {e.Message}"; }
            return feedback;
        }
    }
}
