using Course_projectWPF.Network;
using EngineLibrary;
using GameLibrary.Bonuses;
using GameLibrary.Enemies;
using GameLibrary.Interfaces;
using GameLibrary.Level;
using GameLibrary.Network;
using GameLibrary.Players;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary.InvasionGame
{
    /// <summary>
    /// Класс описывающий основную игровую сцену
    /// </summary>
    public class LevelScene : Scene
    {
        /// <summary>
        /// Фабрика игрока
        /// </summary>
        public PlayerFactory PlayerFactory { get; private set; }

        private string firstWeapon;
        private string secondWeapon;

        private INetworkHandler networkHandler;

        private string hostPlayer = string.Empty;

        private NetworkManagerPlayer networkManagerPlayer;
        private NetworkManagerBullet networkManagerBullet;
        private NetworkManagerBonus networkManagerBonus;
        private NetworkManagerEnemy networkManagerEnemy;

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="firstWeapon">Первый вид оружия</param>
        /// <param name="secondWeapon">Второй вид оружия</param>
        public LevelScene(string firstWeapon, string secondWeapon)
        {
            this.firstWeapon = firstWeapon;
            this.secondWeapon = secondWeapon;
        }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="firstWeapon">Первый вид оружия</param>
        /// <param name="secondWeapon">Второй вид оружия</param>
        public LevelScene(string firstWeapon, string secondWeapon, INetworkHandler handler, string hostPlayer)
        {
            this.firstWeapon = firstWeapon;
            this.secondWeapon = secondWeapon;

            networkHandler = handler;
            this.hostPlayer = hostPlayer;

            networkManagerPlayer = new NetworkManagerPlayer(networkHandler);
            networkManagerBullet = new NetworkManagerBullet(networkHandler);
            networkManagerBonus = new NetworkManagerBonus(networkHandler);
            networkManagerEnemy = new NetworkManagerEnemy(networkHandler);
        }

        public override void Init()
        {
            PlayerFactory = new PlayerFactory();

            LevelField levelField = new LevelField();
            levelField.PlayerFactory = PlayerFactory;
            levelField.CreateLevel();

            GameObject firstPlayer;
            GameObject weapon1;
            GameObject secondPlayer;
            GameObject weapon2;

            BonusLauncher bonusLauncher = new BonusLauncher();
            EnemyWaveSystem enemyWaveSystem = new EnemyWaveSystem();

            if (hostPlayer == "FirstPlayer")
            {
                firstPlayer = PlayerFactory.CreatePlayer("FirstPlayer", networkManagerPlayer);
                weapon1 = PlayerFactory.CreateWeapon(firstWeapon, networkManagerBullet);
                firstPlayer.SetChildGameObject(weapon1);

                secondPlayer = PlayerFactory.CreateNetworkPlayer("SecondPlayer", networkManagerPlayer);
                weapon2 = PlayerFactory.CreateNetworkWeapon(secondWeapon, networkManagerBullet);
                secondPlayer.SetChildGameObject(weapon2);
                networkManagerBonus.OnWriteData += bonusLauncher.WriteNetworkData;
                networkManagerEnemy.OnWriteData += enemyWaveSystem.WriteNetworkData;
            }
            else if (hostPlayer == "SecondPlayer")
            {
                firstPlayer = PlayerFactory.CreateNetworkPlayer("FirstPlayer", networkManagerPlayer);
                weapon1 = PlayerFactory.CreateNetworkWeapon(firstWeapon, networkManagerBullet);
                firstPlayer.SetChildGameObject(weapon1);

                secondPlayer = PlayerFactory.CreatePlayer("SecondPlayer", networkManagerPlayer);
                weapon2 = PlayerFactory.CreateWeapon(secondWeapon, networkManagerBullet);
                secondPlayer.SetChildGameObject(weapon2);
                bonusLauncher.IsNetwork = true;
                enemyWaveSystem.IsNetwork = true;
                networkManagerBonus.OnUpdateData += bonusLauncher.UpdateNetworkData;
                networkManagerEnemy.OnUpdateData += enemyWaveSystem.UpdateNetworkData;
            }
            else
            {
                firstPlayer = PlayerFactory.CreatePlayer("FirstPlayer");
                weapon1 = PlayerFactory.CreateWeapon(firstWeapon);
                firstPlayer.SetChildGameObject(weapon1);

                secondPlayer = PlayerFactory.CreatePlayer("SecondPlayer");
                weapon2 = PlayerFactory.CreateWeapon(secondWeapon);
                secondPlayer.SetChildGameObject(weapon2);
            }

            GameObject waveSystem = new GameObject();
            enemyWaveSystem.Inputs.Add(new Vector2(20, 350));
            enemyWaveSystem.Inputs.Add(new Vector2(1200, 350));
            enemyWaveSystem.TagWeapons.Add("EnemyAutoWeapon");
            enemyWaveSystem.TagWeapons.Add("BFG");
            enemyWaveSystem.TagWeapons.Add("EnemyMiniWeapon");
            enemyWaveSystem.FollowGameObjects.Add(firstPlayer);
            enemyWaveSystem.FollowGameObjects.Add(secondPlayer);
            enemyWaveSystem.Start();
            waveSystem.SetComponent(enemyWaveSystem);

            GameObject bonusSystem = new GameObject();
            bonusLauncher.SetSpawnPoint(new[] { new Vector2(50, 350), new Vector2(1150, 350), new Vector2(600, 50), new Vector2(600, 600) });
            bonusLauncher.Start();
            bonusSystem.SetComponent(bonusLauncher);

            GameObject endGame = new GameObject();
            EndGame endGame1 = new EndGame();
            endGame1.OnAction += delegate (string end) { EndScene(end); };
            endGame1.EnemyWaveSystem = enemyWaveSystem;
            endGame1.Healths = new[] { firstPlayer.Script as IHealth, secondPlayer.Script as IHealth };
            endGame.SetComponent(endGame1);

            game.AddObjectOnScene(firstPlayer);
            game.AddObjectOnScene(secondPlayer);
            game.AddObjectOnScene(weapon1);
            game.AddObjectOnScene(weapon2);
            game.AddObjectOnScene(waveSystem);
            game.AddObjectOnScene(bonusSystem);
            game.AddObjectOnScene(endGame);

            var networkManagerPlayerObject = new GameObject();
            networkManagerPlayerObject.GameObjectTag = "NetworkManagerPlayer";
            networkManagerPlayerObject.SetComponent(new TransformComponent(new Vector2(0, 0), new Vector2(1f, 1f), new Vector2(0, 0), 0));
            networkManagerPlayerObject.SetComponent(networkManagerPlayer);

            game.AddObjectOnScene(networkManagerPlayerObject);

            var networkManagerBulletObject = new GameObject();
            networkManagerBulletObject.GameObjectTag = "NetworkManagerBullet";
            networkManagerBulletObject.SetComponent(new TransformComponent(new Vector2(0, 0), new Vector2(1f, 1f), new Vector2(0, 0), 0));
            networkManagerBulletObject.SetComponent(networkManagerBullet);

            game.AddObjectOnScene(networkManagerBulletObject);

            var networkManagerBonusObject = new GameObject();
            networkManagerBonusObject.GameObjectTag = "NetworkManagerBonus";
            networkManagerBonusObject.SetComponent(new TransformComponent(new Vector2(0, 0), new Vector2(1f, 1f), new Vector2(0, 0), 0));
            networkManagerBonusObject.SetComponent(networkManagerBonus);

            game.AddObjectOnScene(networkManagerBonusObject);

            var networkManagerEnemyObject = new GameObject();
            networkManagerEnemyObject.GameObjectTag = "NetworkManagerEnemy";
            networkManagerEnemyObject.SetComponent(new TransformComponent(new Vector2(0, 0), new Vector2(1f, 1f), new Vector2(0, 0), 0));
            networkManagerEnemyObject.SetComponent(networkManagerEnemy);

            game.AddObjectOnScene(networkManagerEnemyObject);
        }

        /// <summary>
        /// Поведение при завершении сцены
        /// </summary>
        protected override void EndScene(string end)
        {
            GameEvents.EndGame(end);
        }
    }
}
