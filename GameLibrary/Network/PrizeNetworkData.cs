using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary.Network
{
    public class PrizeNetworkData
    {
        public int Id { get; set; }

        public float[] Position { get; set; }

        public PrizeNetworkData()
        {
            Position = new float[0];
        }

        public override string ToString()
        {
            return "{" + $"\"Id\":{Id},\"Position\":[{Position[0]},{Position[1]}]" + "}";
        }
    }
}
