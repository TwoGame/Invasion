using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary.Network
{
    [Serializable]
    public class NetworkDataWeapon
    {
        /// <summary>
        /// Индекс текущего выбора оружия
        /// </summary>
        public int WeaponChoice { get; set; }

        /// <summary>
        /// Обозначает выбрано ли оружее
        /// </summary>
        public bool IsSelected { get; set; }

        public NetworkDataWeapon()
        {
            IsSelected = false;
            ResetToDefaut();
        }

        /// <summary>
        /// Сброс до значений
        /// </summary>
        public void ResetToDefaut()
        {
            WeaponChoice = (int) WeaponDataCode.Default;
        }
    }
}
