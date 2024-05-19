using Course_projectWPF.Network;
using GameLibrary.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary.Network
{
    public class NetworkManagerChoiceWeapon : NetworkManager<NetworkDataWeapon>
    {
        public NetworkManagerChoiceWeapon(INetworkHandler networkHandler) : base(networkHandler)
        {
            
        }

        protected override void ClearData()
        {
            NetworkNetworkData.ResetToDefaut();
        }

        public override void Start()
        {
            
        }

        public override void Update()
        {
            UpdateData();
        }
    }
}
