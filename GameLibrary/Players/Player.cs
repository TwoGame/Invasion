using EngineLibrary;
using GameLibrary.Interfaces;
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
    /// Класс описывающий игрока
    /// </summary>
    public class Player : BasePlayer
    {
        /// <summary>
        /// Управление игрока
        /// </summary>
        public virtual PlayerControl Control { get; protected set; }
        /// <summary>
        /// Свойства игрока
        /// </summary>
        public PlayerProperties PlayerProperities { get; protected set; }
        /// <summary>
        /// Время действия эффектов
        /// </summary>
        public float ResetEffectTime { get; private set; } = 10;
        /// <summary>
        /// Текущее время сброса эффекта
        /// </summary>
        private float currResetEffectTime = 0;

        public override void Start()
        {
            if (GameObject.GameObjectTag == "FirstPlayer")
            {
                Control = new PlayerControl(AxisOfInput.Horizontal, AxisOfInput.Vertical, Key.F);
            }
            else
            {
                Control = new PlayerControl(AxisOfInput.AlternativeHorizontal, AxisOfInput.AlternativeVertical, Key.ControlRight);
            }

            PlayerProperities = new PlayerPropertiesStandart();
            GameEvents.PlayerState.Invoke(GameObject.GameObjectTag, Health.ToString());
        }

        public override void Update()
        {
            if (Time.CurrentTime >= currResetEffectTime)
                ResetEffect();

            if (IsCanMove)
                Move();

            if (!GameObject.IsActive && Health > 0)
                Revival();
        }

        /// <summary>
        /// Метод, реализующий передвиение игровых объектов
        /// </summary>
        protected override void Move()
        {
            var directionX = Input.GetAxis(Control.HorizontalAxis);
            var directionY = Input.GetAxis(Control.VerticalAxis);

            if (directionX > 0)
            {
                GameObject.Texture.SetAnimation("Right");
                GameObject.ChildGameObject.Texture.SetAnimation("Right");
            }
            else if (directionX < 0)
            {
                GameObject.Texture.SetAnimation("Left");
                GameObject.ChildGameObject.Texture.SetAnimation("Left");
            }

            if (directionX != 0 || directionY != 0)
            {
                Direction = new Vector2(directionX, directionY);
            }

            GameObject.Transform.SetMovement(new Vector2(directionX, directionY) * PlayerProperities.Speed * Time.DeltaTime);
            GameObject.ChildGameObject.Transform.SetMovement(new Vector2(directionX, directionY) * PlayerProperities.Speed * Time.DeltaTime);

            DetectCollision();
        }

        /// <summary>
        /// Распознавание столкновений и реакция на них
        /// </summary>
        private void DetectCollision()
        {
            if (GameObject.Colliders.CheckGameObjectIntersection(out GameObject otherGameObject, tagNames: new[] { "FirstPlayer", "SecondPlayer", "Wall" }))
            {
                GameObject.Transform.ResetMovement();
                GameObject.ChildGameObject.Transform.ResetMovement();
            }
        }

        /// <summary>
        /// Сброс эффекта
        /// </summary>
        public void ResetEffect()
        {
            PlayerProperities = new PlayerPropertiesStandart();
            currResetEffectTime = 0;
        }
        /// <summary>
        /// Наложение эффекта
        /// </summary>
        /// <param name="playerProperties"></param>
        public void SetEffect(PlayerProperties playerProperties)
        {
            PlayerProperities = playerProperties;
            currResetEffectTime = ResetEffectTime + Time.CurrentTime;
        }
    }

    /// <summary>
    /// Структура игрового управления персонажа
    /// </summary>
    public struct PlayerControl
    {
        /// <summary>
        /// Горизонтальная ось передвижения
        /// </summary>
        public AxisOfInput HorizontalAxis { get; private set; }
        /// <summary>
        /// Вертикальная ось передвижения
        /// </summary>
        public AxisOfInput VerticalAxis { get; private set; }
        /// <summary>
        /// Кнопка стрельбы
        /// </summary>
        public Key ShootKey { get; private set; }

        /// <summary>
        /// Конструктор структуры
        /// </summary>
        /// <param name="horizontalAxis">Горизонтальная ось передвижения</param>
        /// <param name="verticalAxis"> Вертикальная ось передвижения</param>
        /// <param name="shootKey">Кнопка стрельбы</param>
        public PlayerControl(AxisOfInput horizontalAxis, AxisOfInput verticalAxis, Key shootKey)
        {
            HorizontalAxis = horizontalAxis;
            VerticalAxis = verticalAxis;
            ShootKey = shootKey;
        }
    }
}
