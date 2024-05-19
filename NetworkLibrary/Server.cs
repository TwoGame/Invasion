using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Course_projectWPF.Network
{
    public class Server : INetworkHandler, IDisposable
    {
        private IPAddress _ip = IPAddress.Any;
        private const int port = 11000;

        TcpListener tcpListener;

        private IPEndPoint _endPoint;

        public event Action<object> OnDataGot;

        NetworkStream nwStream;
        TcpClient tcpClient;

        public Server()
        {
            tcpListener = new TcpListener(IPAddress.Any, port);
            tcpListener.Start();


            //_endPoint = new IPEndPoint(_ip, port);
            //_udpServer = new UdpClient(port);
            //_udpServer.Client.Bind(_endPoint);
        }

        public void ClearListeners()
        {
            OnDataGot = null;
        }

        public void Dispose()
        {
            nwStream.Close();
            nwStream.Dispose();
            tcpListener.Stop();
            tcpClient.Close();
            tcpClient.Dispose();
        }

        public void Connection()
        {
            tcpClient = tcpListener.AcceptTcpClient();

            nwStream = tcpClient.GetStream();
        }

        public void UpdateData<T>(T obj)
        {
            try
            {
                byte[] buffer = new byte[tcpClient.ReceiveBufferSize];

                int bytesRead = nwStream.Read(buffer, 0, tcpClient.ReceiveBufferSize);
                    
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                T clientData = JsonConvert.DeserializeObject<T>(message);

                SendResponse<T>(obj, tcpClient);

                //Console.WriteLine("Получено1-" + clientData.ToString());

                OnDataGot?.Invoke(clientData);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
            }
        }

        private void SendResponse<T>(T obj, TcpClient tcpClient)
        {
            try
            {
                string message = JsonConvert.SerializeObject(obj);
                byte[] buffer = Encoding.UTF8.GetBytes(message);

                nwStream.Write(buffer, 0, buffer.Length);

                //Console.WriteLine("Отправлено1-" + obj.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task UpdateDataAsync<T>(T obj)
        {
            await Task.Run(() => UpdateData<T>(obj));
        }
    }
}
