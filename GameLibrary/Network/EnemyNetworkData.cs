using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary.Network
{
    public class EnemyNetworkData
    {
        public bool IsSpawn { get; set; } = false;

        /// <summary>
        /// Тэг цели
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Позиция появления
        /// </summary>
        public float[] SpawnPosition { get; set; }

        public bool EndWave { get; set; } = false;

        public string TextWave;

        public EnemyNetworkData()
        {
            SpawnPosition = new float[2];
        }

        public override string ToString()
        {
            return "{" + $"\"SpawnPosition\":[{SpawnPosition[0]},{SpawnPosition[1]}],\"Tag\":{Tag}" + "}";
        }
    }
}
