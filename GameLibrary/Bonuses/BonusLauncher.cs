using EngineLibrary;
using GameLibrary.Network;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary.Bonuses
{
    /// <summary>
    /// Класс скрипт реализующий спавн бонусов и ловушек на игровой сцене
    /// </summary>
    public class BonusLauncher : ObjectScript
    {
        private float reloadTimeSpawn;
        private float currRealoadTimeSpawn;
        private Random random;
        private Game game;
        private BonusFactory bonusFactory;
        private Vector2[] spawnPoint;

        private Vector2 currSpawnPosition;
        private int currIdBonus;

        public bool IsNetwork { get; set; } = false;
        private bool isSpawn = false;

        public override void Start()
        {
            reloadTimeSpawn = 10;
            currRealoadTimeSpawn = reloadTimeSpawn;
            random = new Random();
            game = Game.instance;
            bonusFactory = new BonusFactory();
        }
        /// <summary>
        /// Установка точек появления бонусов и ловушек
        /// </summary>
        /// <param name="spawnPoint">Точка появления</param>
        public void SetSpawnPoint(Vector2[] spawnPoint)
        {
            this.spawnPoint = spawnPoint;
        }

        public override void Update()
        {
            if (IsNetwork) return;

            if (Time.CurrentTime >= currRealoadTimeSpawn)
            {
                currRealoadTimeSpawn = reloadTimeSpawn + Time.CurrentTime;

                currSpawnPosition = spawnPoint[random.Next(0, spawnPoint.Length)];
                currIdBonus = random.Next(0, 5);

                isSpawn = true;

                CreateBonus(currIdBonus, currSpawnPosition);
            }
        }

        private void CreateBonus(int id, Vector2 position)
        {
            GameObject bonus = null;

            switch (id)
            {
                case 0:
                    bonus = bonusFactory.CreateFreezeBonus(position);
                    break;
                case 1:
                    bonus = bonusFactory.CreatePowerBonus(position);
                    break;
                case 2:
                    bonus = bonusFactory.CreateReloadTimeBonus(position);
                    break;
                case 3:
                    bonus = bonusFactory.CreateSlowdownBonus(position);
                    break;
                case 4:
                    bonus = bonusFactory.CreateSpeedBulletBonus(position);
                    break;
                default: break;
            }

            game.AddObjectOnScene(bonus);
        }

        /// <summary>
        /// Запись сетевых данных
        /// </summary>
        /// <param name="manager">Менеджер сетевого взаимодействия</param>
        public void WriteNetworkData(BonusNetworkData bonusNetworkData)
        {
            bonusNetworkData.IsSpawn = isSpawn;
            bonusNetworkData.SpawnPosition[0] = currSpawnPosition.X;
            bonusNetworkData.SpawnPosition[1] = currSpawnPosition.Y;
            bonusNetworkData.Id = currIdBonus;

            isSpawn = false;
        }

        /// <summary>
        /// Обновление сетевых данных
        /// </summary>
        /// <param name="manager">Менеджер сетевого взаимодействия</param>
        public void UpdateNetworkData(BonusNetworkData bonusNetworkData)
        {
            Vector2 position = new Vector2(bonusNetworkData.SpawnPosition[0], bonusNetworkData.SpawnPosition[1]);
            int id = bonusNetworkData.Id;

            if (bonusNetworkData.IsSpawn)
            {
                bonusNetworkData.IsSpawn = isSpawn = false;

                CreateBonus(id, position);
            }
        }
    }
}
