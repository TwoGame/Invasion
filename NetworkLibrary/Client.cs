using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace Course_projectWPF.Network
{
    public class Client : INetworkHandler, IDisposable
    {
        private string ip;
        private const int port = 11000;

        public event Action OnStartGame;

        private TcpClient tcpClient;
        private IPEndPoint _endPoint;

        private NetworkStream nwStream;

        public event Action<object> OnDataGot;

        public Client(string ip)
        {
            this.ip = ip;
            _endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            tcpClient = new TcpClient();

            tcpClient.Connect(_endPoint);

            nwStream = tcpClient.GetStream();
        }

        public void ClearListeners()
        {
            OnDataGot = null;
        }

        public void Dispose()
        {
            tcpClient.Close();
            tcpClient.Dispose();
            nwStream.Close();
            nwStream?.Dispose();
        }

        public void UpdateData<T>(T obj)
        {
            try
            {
                string message = JsonConvert.SerializeObject(obj);
                byte[] bytes = Encoding.UTF8.GetBytes(message);

                nwStream.Write(bytes, 0, bytes.Length);

                GetData<T>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
            }
        }

        /// <summary>
        /// Получение данных
        /// </summary>
        /// <typeparam name="T">Тип получаемых данных</typeparam>
        public void GetData<T>()
        {
            try
            {
                byte[] bytes = new byte[tcpClient.ReceiveBufferSize];

                int bytesRead = nwStream.Read(bytes, 0, tcpClient.ReceiveBufferSize);

                string message = Encoding.UTF8.GetString(bytes, 0, bytesRead);

                T serverData = JsonConvert.DeserializeObject<T>(message);

                OnDataGot?.Invoke(serverData);
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
