using Course_projectWPF.Network;
using EngineLibrary;
using GameLibrary.Bullets;
using GameLibrary.Decorators;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary.Network
{
    /// <summary>
    /// Менеджер сетевого взаимодействия
    /// </summary>
    public abstract class NetworkManager<T> : ObjectScript where T : class, new() 
    {
        /// <summary>
        /// Событие записи данных
        /// </summary>
        public event Action<T> OnWriteData;
        /// <summary>
        /// Событие обновления данных
        /// </summary>
        public event Action<T> OnUpdateData;

        private INetworkHandler networkHandler;
        /// <summary>
        /// Данные текущего игрока
        /// </summary>
        public T CurrentNetworkData { get; protected set; }
        /// <summary>
        /// Данные игрока по сети
        /// </summary>
        public T NetworkNetworkData { get; protected set; }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="networkHandler">Обработчик сетевого взаимодействия</param>
        public NetworkManager(INetworkHandler networkHandler)
        {
            this.networkHandler = networkHandler;
            CurrentNetworkData = new T();
            NetworkNetworkData = new T();
            networkHandler.OnDataGot += OnDataGot;
        }

        /// <summary>
        /// Обновление данных
        /// </summary>
        public void UpdateData()
        {
            OnWriteData?.Invoke(CurrentNetworkData);
            networkHandler.UpdateData(CurrentNetworkData);

            ClearData();
        }

        protected abstract void ClearData();

        /// <summary>
        /// Событие, когда данные по сети получены
        /// </summary>
        /// <param name="data">Данные</param>
        protected virtual void OnDataGot(object data)
        {
            if (!(data is T))
                return;

            NetworkNetworkData = data as T;

            OnUpdateData?.Invoke(NetworkNetworkData);

            ClearData();
        }
    }
}
