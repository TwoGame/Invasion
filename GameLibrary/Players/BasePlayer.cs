using EngineLibrary;
using OpenTK.Input;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EngineLibrary.Input;
using GameLibrary.Interfaces;
using GameLibrary.Network;

namespace GameLibrary.Players
{
    public class BasePlayer : ObjectScript, IHealth
    {
        /// <summary>
        /// Количество жизней
        /// </summary>
        public float Health { get; private set; } = 10;
        /// <summary>
        /// Направление движения
        /// </summary>
        public Vector2 Direction = Vector2.Zero;
        /// <summary>
        /// Начальная позиция
        /// </summary>
        private Vector2 startPosition;
        /// <summary>
        /// Возможность передвижения
        /// </summary>
        public bool IsCanMove { get; set; } = true;

        public override void Start()
        {
            
        }

        public override void Update()
        {
            if (!GameObject.IsActive && Health > 0)
                Revival();

           Move();
        }
        /// <summary>
        /// Метод, реализующий передвиение игровых объектов
        /// </summary>
        protected virtual void Move()
        {
            if (Direction.X > 0)
            {
                GameObject.Texture.SetAnimation("Right");
                GameObject.ChildGameObject.Texture.SetAnimation("Right");
            }
            else if (Direction.X < 0)
            {
                GameObject.Texture.SetAnimation("Left");
                GameObject.ChildGameObject.Texture.SetAnimation("Left");
            }
        }

       
        /// <summary>
        /// Уменьшение количества жизней
        /// </summary>
        /// <param name="value">Количество жизней</param>
        public void Remove(float value)
        {
            if (value > 0)
                Health -= value;

            Death();
            GameEvents.PlayerState.Invoke(GameObject.GameObjectTag, Health.ToString());
        }
        /// <summary>
        /// Добавление количества жизней
        /// </summary>
        /// <param name="value">Количество жизней</param>
        public void Add(float value)
        {
            if (value > 0)
                Health += value;

            GameEvents.PlayerState.Invoke(GameObject.GameObjectTag, Health.ToString());
        }
        /// <summary>
        /// Метод, реализующий смерть
        /// </summary>
        private void Death()
        {
            if (Health > 0)
            {
                GameObject.ChildGameObject.IsActive = false;
                GameObject.IsActive = false;
                IsCanMove = false;
            }
            else
            {
                GameObject.ChildGameObject.Colliders.DelGameObject(GameObject.ChildGameObject);
                GameObject.Colliders.DelGameObject(GameObject);
                GameObject.IsActive = false;
                Game.instance.AddObjectsToRemove(GameObject.ChildGameObject);
                Game.instance.AddObjectsToRemove(GameObject);
            }
        }
        /// <summary>
        /// Метод, реализующий возрождение игрока
        /// </summary>
        protected void Revival()
        {
            GameObject.ChildGameObject.IsActive = true;
            GameObject.IsActive = true;
            IsCanMove = true;
            GameObject.Transform.Position = startPosition;
            var position = startPosition;
            position.Y += 25;
            GameObject.ChildGameObject.Transform.Position = position;
        }
        /// <summary>
        /// Установка стартовой позиции
        /// </summary>
        /// <param name="startPosition">Стартовая позиция</param>
        public void SetStartPosition(Vector2 startPosition)
        {
            this.startPosition = startPosition;
        }

        /// <summary>
        /// Запись сетевых данных об игроке
        /// </summary>
        /// <param name="manager">Менеджер сетевого взаимодействия</param>
        public void WriteNetworkData(NetworkDataPlayer networkDataPlayer)
        {
            networkDataPlayer.PlayerPosition[0] = GameObject.Transform.Position.X;
            networkDataPlayer.PlayerPosition[1] = GameObject.Transform.Position.Y;
            networkDataPlayer.ChildPosition[0] = GameObject.ChildGameObject.Transform.Position.X;
            networkDataPlayer.ChildPosition[1] = GameObject.ChildGameObject.Transform.Position.Y;
            networkDataPlayer.PlayerSpriteFlip[0] = Direction.X;
            networkDataPlayer.PlayerSpriteFlip[1] = Direction.Y;
            networkDataPlayer.Health = Health;

            //Console.WriteLine("Отправил игрок: " + gameObject.GameObjectTag + " Data: ");
        }

        /// <summary>
        /// Обновление данные сетевого игрока
        /// </summary>
        /// <param name="manager">Менеджер сетевого взаимодействия</param>
        public void UpdateNetworkData(NetworkDataPlayer networkDataPlayer)
        {
            Vector2 pos = new Vector2(networkDataPlayer.PlayerPosition[0], networkDataPlayer.PlayerPosition[1]);
            Vector2 childPos = new Vector2(networkDataPlayer.ChildPosition[0], networkDataPlayer.ChildPosition[1]);
            Direction = new Vector2(networkDataPlayer.PlayerSpriteFlip[0], networkDataPlayer.PlayerSpriteFlip[1]);

            GameObject.Transform.Position = pos;
            GameObject.ChildGameObject.Transform.Position = childPos;

            float damage = Health - networkDataPlayer.Health;
            Health = networkDataPlayer.Health;

            if(damage > 0)
            {
                Death();
                GameEvents.PlayerState.Invoke(GameObject.GameObjectTag, Health.ToString());
            }
        }
    }
}
