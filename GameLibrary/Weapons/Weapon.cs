using EngineLibrary;
using GameLibrary.Bullets;
using GameLibrary.Network;
using GameLibrary.Players;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary.Weapons
{
    /// <summary>
    /// Класс скрипт описывающий оружие в игре
    /// </summary>
    public class Weapon : ObjectScript
    {
        /// <summary>
        /// Экземпляр сцены игры
        /// </summary>
        protected Game game;

        private float currentReloadTime = 0;

        private Player player;

        private bool isDown = false;
        private bool isShoot = false;

        private Vector2 position;
        private Vector2 direction;
        private float power;
        private float speed;

        public bool IsNetwork { get; set; } = false;

        public override void Start()
        {
            game = Game.instance;
        }

        public override void Update()
        {
            player = GameObject.ParentGameObject.Script as Player;

            if (player == null || IsNetwork) return;

            currentReloadTime += Time.DeltaTime;

            if (Input.GetButtonDawn(player.Control.ShootKey) &&
                currentReloadTime > player.PlayerProperities.ReloadTime && !isDown)
            {
                currentReloadTime = 0;

                isDown = true;

                position = GameObject.Transform.Position;
                direction = player.Direction;
                power = player.PlayerProperities.Power;
                speed = player.PlayerProperities.SpeedBullet;

                isShoot = true;
                SpawnBullet(position, direction, speed, power);
            }
            else if (!Input.GetButtonDawn(player.Control.ShootKey))
                isDown = false;
        }

        /// <summary>
        /// Создание пули из фабрики
        /// </summary>
        /// <param name="position">Позиция создания</param>
        /// <param name="direction">Направление пули</param>
        protected void SpawnBullet(Vector2 position, Vector2 direction, float speed, float power = 1)
        {
            BulletFactory factory = new BulletFactory();
            game.AddObjectOnScene(factory.CreateBullet(position, direction, speed, power, "Enemy", "Wall"));
        }

        /// <summary>
        /// Запись сетевых данных об оружии
        /// </summary>
        /// <param name="manager">Менеджер сетевого взаимодействия</param>
        public void WriteNetworkData(BulletNetworkData networkData)
        {
            networkData.IsShoot = isShoot;
            networkData.SpawnPosition[0] = position.X;
            networkData.SpawnPosition[1] = position.Y;
            networkData.Direction[0] = direction.X;
            networkData.Direction[1] = direction.Y;
            networkData.Power = power;
            networkData.Speed = speed;
            networkData.Tag = "Enemy";

            isShoot = false;
        }

        /// <summary>
        /// Обновление данные сетевого оружия
        /// </summary>
        /// <param name="manager">Менеджер сетевого взаимодействия</param>
        public void UpdateNetworkData(BulletNetworkData networkData)
        {
            if (networkData.IsShoot)
            {
                var position = new Vector2(networkData.SpawnPosition[0], networkData.SpawnPosition[1]);
                var direction = new Vector2(networkData.Direction[0], networkData.Direction[1]);
                var speed = networkData.Speed;
                var power = networkData.Power;

                currentReloadTime = 0;

                SpawnBullet(position, direction, speed, power);
            }
        }
    }
}
