using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary.Network
{
    public class BulletNetworkData
    {
        public bool IsShoot = false;

        /// <summary>
        /// Тэг цели
        /// </summary>
        public string Tag;

        /// <summary>
        /// Позиция появления
        /// </summary>
        public float[] SpawnPosition { get; set; }
        /// <summary>
        /// Направление пули
        /// </summary>
        public float[] Direction { get; set; }
        /// <summary>
        /// Сила пули
        /// </summary>
        public float Power { get; set; }
        /// <summary>
        /// Скорость пули
        /// </summary>
        public float Speed { get; set; }

        public BulletNetworkData()
        {
            SpawnPosition = new float[2];
            Direction = new float[2];
        }

        public override string ToString()
        {
            return "{" + $"\"SpawnPosition\":[{SpawnPosition[0]},{SpawnPosition[1]}],\"Direction\":[{Direction[0]},{Direction[1]}],\"Tag\":{Tag},\"Power\":{Power}" + "}";
        }
    }
}
