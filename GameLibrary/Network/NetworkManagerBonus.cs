using Course_projectWPF.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary.Network
{
    public class NetworkManagerBonus : NetworkManager<BonusNetworkData>
    {
        public NetworkManagerBonus(INetworkHandler networkHandler) : base(networkHandler)
        {
        }

        public override void Start()
        {

        }

        public override void Update()
        {
            UpdateData();
        }

        protected override void ClearData()
        {
            CurrentNetworkData.IsSpawn = false;
        }
    }
}
