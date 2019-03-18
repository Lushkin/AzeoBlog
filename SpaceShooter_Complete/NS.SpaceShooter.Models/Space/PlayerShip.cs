namespace NS.SpaceShooter.Models.Space
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using NS.SpaceShooter.Models.GamePlay;
    using NS.SpaceShooter.Models.Helpers;
    using System.Collections.Generic;

    public class PlayerShip : Ship
    {
        public int Boost { get; set; }
        public List<(int index, Bonus bonus)> Bonuses { get; set; }
        public int MaxBonusCapacity { get; private set; }
        public Rectangle Shield { get; set; }

        private int _maxBoost;
        private bool _isBoostEnabled;
        private double _boostInterval, _currentBoostInterval;
        private int _speed;
        private int _maxHealt;

        public PlayerShip(Texture2D texture, int width, int height, int x, int y, Texture2D whitePixel = null):base(texture, width, height, x, y, 5, whitePixel)
        {
            Initialize();
        }

        public PlayerShip(Texture2D texture, Rectangle rectangle, Texture2D whitePixel = null) : base(texture, rectangle, 5, whitePixel)
        {
            Initialize();
        }

        private void Initialize()
        {
            Bonuses = new List<(int index, Bonus bonus)>();
            Speed = _speed = 6;
            Boost = _maxBoost = 100;
            _boostInterval = 0.05;
            _isBoostEnabled = false;
            _maxHealt = 5;
            MaxBonusCapacity = 5;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_isBoostEnabled)
            {
                _currentBoostInterval -= gameTime.ElapsedGameTime.TotalSeconds;
                if (_currentBoostInterval <= 0)
                {
                    _currentBoostInterval = _boostInterval;
                    Speed = 12;
                    Boost--;
                }
            }

            UseActiveBonus(gameTime);
            UpdateActiveBonus();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
            DrawActiveBonus(gameTime, spriteBatch);
        }

        public void UseBoost()
        {
            if (Boost > 0)
            {
                _isBoostEnabled = true;
            }
            else
            {
                Speed = _speed;
                _isBoostEnabled = false;
            }
        }

        public void StopBoost()
        {
            Speed = _speed;
            _isBoostEnabled = false;
        }

        public void AddBoost(int value)
        {
            if (Boost + value > _maxBoost)
                Boost = _maxBoost;
            else
                Boost += value;
        }

        public void RestoreHealth()
        {
            if (Life < _maxHealt)
                Life = _maxHealt;
        }

        public void UseActiveBonus(GameTime gameTime)
        {
            foreach(var bonus in Bonuses)
            {
                if(bonus.bonus.IsActive && bonus.bonus.HasDuration && bonus.bonus.Duration > 0)
                {
                    var elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
                    bonus.bonus.Duration -= elapsedTime;
                }
            }
        }

        public void UpdateActiveBonus()
        {
            foreach (var bonus in Bonuses)
            {
                if (bonus.bonus.IsActive && bonus.bonus.HasDuration && bonus.bonus.Duration > 0 && bonus.bonus.FollowShip)
                {
                    bonus.bonus.Rectangle = bonus.bonus.Rectangle.AlignCenter(Rectangle);
                }
            }
        }

        public void DrawActiveBonus(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var bonus in Bonuses)
            {
                if (bonus.bonus.IsActive && bonus.bonus.HasDuration && bonus.bonus.Duration > 0)
                {
                    bonus.bonus.Draw(gameTime, spriteBatch);
                }
            }
        }
    }
}
