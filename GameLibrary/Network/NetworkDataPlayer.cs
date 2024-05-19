using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary.Network
{
    public class NetworkDataPlayer
    {
        /// <summary>
        /// Позиция игрока
        /// </summary>
        public float[] PlayerPosition { get; set; }
        /// <summary>
        /// Позиция дочернего объекта
        /// </summary>
        public float[] ChildPosition { get; set; }
        /// <summary>
        /// Повернут ли спрайт игрока
        /// </summary>
        public float[] PlayerSpriteFlip { get; set; }

        public float Health { get; set; }

        public NetworkDataPlayer() 
        {
            PlayerPosition = new float[2];
            ChildPosition = new float[2];
            PlayerSpriteFlip = new float[2];
        }

        public override string ToString()
        {
            return "x: " + PlayerPosition[0] + " y: " + PlayerPosition[1];
        }
    }
}
