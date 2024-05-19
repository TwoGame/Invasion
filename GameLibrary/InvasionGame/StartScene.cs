using Course_projectWPF.Network;
using EngineLibrary;
using EngineLibrary.ObjectComponents;
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
    /// Класс описывающий начальную сцену
    /// </summary>
    public class StartScene : Scene
    {
        private PlayerChoice firstWeapon;
        private PlayerChoice secondWeapon;

        private INetworkHandler networkHandler;

        private string hostPlayer = string.Empty;

        private NetworkManagerChoiceWeapon networkManager;

        private bool isSelectedFirst = false;
        private bool isSelectedSecond = false;

        public StartScene()
        {

        }

        public StartScene(INetworkHandler handler, string hostPlayer)
        {
            networkHandler = handler;
            this.hostPlayer = hostPlayer;

            networkManager = new NetworkManagerChoiceWeapon(networkHandler);
        }

        protected override void EndScene(string end)
        {
            var tagFirst = firstWeapon.GameObject.ChildGameObject.GameObjectTag;
            var tagSecond = secondWeapon.GameObject.ChildGameObject.GameObjectTag;

            if(hostPlayer != string.Empty)
            {
                networkHandler.ClearListeners();
                game.ChengeScene(new LevelScene(tagFirst, tagSecond, networkHandler, hostPlayer));
                return;
            }

            game.ChengeScene(new LevelScene(tagFirst, tagSecond));
        }

        public override void Init()
        {
            var factory = new PlayerFactory();

            var weapons = new[] { factory.CreateWeapon("SniperWeapon"), factory.CreateWeapon("AutoWeapon") };
            var weapons1 = new[] { factory.CreateWeapon("SniperWeapon"), factory.CreateWeapon("AutoWeapon") };

            GameObject gameObject = new GameObject();
            gameObject.GameObjectTag = "FirstPlayer";
            gameObject.SetComponent(new TransformComponent(new Vector2(400, 300), new Vector2(1.5f, 1.5f), new Vector2(0, 0), 0));
            gameObject.SetComponent(new SpriteComponent(ContentPipe.LoadTexture("FirstPlayerRight.png")));

            PlayerChoice playerChoice = new PlayerChoice();
            playerChoice.Initialize(gameObject);
            playerChoice.SetWeapons(weapons);
            playerChoice.Start();
            playerChoice.OnAction += delegate ()
            {
                isSelectedFirst = true;

                if (isSelectedFirst && isSelectedSecond)
                    EndScene("");
            };
            gameObject.SetComponent(playerChoice);

            GameObject gameObject1 = new GameObject();
            gameObject1.GameObjectTag = "SecondPlayer";
            gameObject1.SetComponent(new TransformComponent(new Vector2(800, 300), new Vector2(1.5f, 1.5f), new Vector2(0, 0), 0));
            gameObject1.SetComponent(new SpriteComponent(ContentPipe.LoadTexture("SecondPlayerRight.png")));

            PlayerChoice playerChoice1 = new PlayerChoice();
            playerChoice1.Initialize(gameObject1);
            playerChoice1.SetWeapons(weapons1);
            playerChoice1.Start();
            playerChoice1.OnAction += delegate ()
            {
                isSelectedSecond = true;

                if (isSelectedFirst && isSelectedSecond)
                    EndScene("");
            };
            gameObject1.SetComponent(playerChoice1);

            firstWeapon = playerChoice;
            secondWeapon = playerChoice1;

            game.AddObjectOnScene(gameObject);
            game.AddObjectOnScene(gameObject.ChildGameObject);
            game.AddObjectOnScene(gameObject1);
            game.AddObjectOnScene(gameObject1.ChildGameObject);

            if (hostPlayer == string.Empty)
                return;

            if (hostPlayer == "FirstPlayer")
            {
                networkManager.OnUpdateData += playerChoice1.UpdateNetworkData;
                networkManager.OnWriteData += playerChoice.WriteNetworkData;
                playerChoice1.IsNetwork = true;
            }
            else if(hostPlayer == "SecondPlayer")
            {
                networkManager.OnUpdateData += playerChoice.UpdateNetworkData;
                networkManager.OnWriteData += playerChoice1.WriteNetworkData;
                playerChoice.IsNetwork = true;
            }

            var networkManagerObject = new GameObject();
            networkManagerObject.GameObjectTag = "NetworkManager";
            networkManagerObject.SetComponent(new TransformComponent(new Vector2(0, 0), new Vector2(1f, 1f), new Vector2(0, 0), 0));
            networkManagerObject.SetComponent(networkManager);

            game.AddObjectOnScene(networkManagerObject);
        }
    }
}
