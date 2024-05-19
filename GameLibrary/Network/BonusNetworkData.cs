using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary.Network
{
    public class BonusNetworkData
    {
        public bool IsSpawn { get; set; } = false;

        /// <summary>
        /// Тэг цели
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Позиция появления
        /// </summary>
        public float[] SpawnPosition { get; set; }

        public BonusNetworkData()
        {
            SpawnPosition = new float[2];
        }

        public override string ToString()
        {
            return "{" + $"\"SpawnPosition\":[{SpawnPosition[0]},{SpawnPosition[1]}],\"Id\":{Id}" + "}";
        }
    }
}
