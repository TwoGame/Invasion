using EngineLibrary;
using GameLibrary.Network;
using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EngineLibrary.Input;

namespace GameLibrary.Players
{
    /// <summary>
    /// Класс, описывающий момент выбора оружия игрока
    /// </summary>
    public class PlayerChoice : ObjectScript
    {
        private GameObject[] weapons;

        private int currWeapons = 0;

        private bool isDown = false;

        public bool IsNetwork { get; set; }

        public bool IsSelected = false;

        private AxisOfInput axis;
        /// <summary>
        /// Действие при выборе оружия
        /// </summary>
        public Action OnAction;
        /// <summary>
        /// Установка оружия
        /// </summary>
        /// <param name="weapons">Объекты оружия</param>
        public void SetWeapons(GameObject[] weapons)
        {
            this.weapons = weapons;
        }

        public override void Start()
        {
            weapons[currWeapons].Transform.Position = new Vector2(0, 180);
            weapons[currWeapons].Transform.Scale = new Vector2(6, 6);
            GameObject.SetChildGameObject(weapons[currWeapons]);

            if (!IsNetwork)
            {
                if (GameObject.GameObjectTag == "FirstPlayer")
                    axis = AxisOfInput.Horizontal;
                else
                    axis = AxisOfInput.AlternativeHorizontal;
            }
        }

        public void SetWeapon(int weapon)
        {
            Game.instance.AddObjectsToRemove(GameObject.ChildGameObject);

            weapons[weapon].Transform.Position = new Vector2(0, 180);
            weapons[weapon].Transform.Scale = new Vector2(6, 6);

            GameObject.SetChildGameObject(weapons[weapon]);
            Game.instance.AddObjectOnScene(weapons[weapon]);
        }

        public override void Update()
        {
            if (IsNetwork) return;

            var directionX = Input.GetAxis(axis);

            if ((directionX > 0 || directionX < 0) && !isDown)
            {
                isDown = true;

                currWeapons += directionX;

                if (currWeapons < 0)
                    currWeapons = weapons.Length - 1;
                else if (currWeapons >= weapons.Length)
                    currWeapons = 0;

                SetWeapon(currWeapons);
            }
            else if (directionX == 0)
                isDown = false;

            if (Input.GetButtonDawn(Key.T) && OnAction != null && !IsSelected)
            {
                OnAction?.Invoke();
                IsSelected = true;
            }
        }

        /// <summary>
        /// Запись сетевых данных об игроке
        /// </summary>
        /// <param name="manager">Менеджер сетевого взаимодействия</param>
        public void WriteNetworkData(NetworkDataWeapon networkDataWeapon)
        {
            networkDataWeapon.WeaponChoice = currWeapons;
            networkDataWeapon.IsSelected = IsSelected;

            //Console.WriteLine("Отправил игрок: " + gameObject.GameObjectTag + " Data: ");
        }

        /// <summary>
        /// Обновление данные сетевого игрока
        /// </summary>
        /// <param name="manager">Менеджер сетевого взаимодействия</param>
        public void UpdateNetworkData(NetworkDataWeapon networkDataWeapon)
        {
            var newWeapons = networkDataWeapon.WeaponChoice;
            IsSelected = networkDataWeapon.IsSelected;

            if(newWeapons != currWeapons)
            {
                currWeapons = newWeapons;
                SetWeapon(currWeapons);
            }
            
            if(IsSelected)
                OnAction?.Invoke();
        }
    }
}
