namespace SpaceShooter
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using SpaceShooter.SpaceObjects;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private Ship _playerShip;
        private Texture2D _laserTexture, _smallStarTexture, _mediumStarTexture, _bigStarTexture;
        private List<Laser> _playerSipLasers, _ennemyLasers;
        private Rectangle _smallStar1, _smallStar2, _mediumStar1, _mediumStar2, _bigStar1, _bigStar2;
        private int _smallStarSpeed, _mediumStarSpeed, _bigStarSpeed;
        private List<Ship> _ennemyShips;
        private int _maxEnnemyCount;
        private Texture2D _ennemyShipTexture, _ennemyLaserTexture;
        private SpriteFont _gameFont;
        private int _score;
        private bool _isGameOver;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

       
        protected override void Initialize()
        {
            _playerSipLasers = new List<Laser>();
            _ennemyLasers = new List<Laser>();
            _ennemyShips = new List<Ship>();
            _smallStarSpeed = 2;
            _mediumStarSpeed = 4;
            _bigStarSpeed = 6;
            _maxEnnemyCount = 10;
            _score = 0;
            _isGameOver = false;

            InitWindow();
            InitStars();
            InitPlayerShip();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            _playerShip.Texture = Content.Load<Texture2D>("Images/Ship");
            _laserTexture = Content.Load<Texture2D>("Images/Laser");
            _smallStarTexture = Content.Load<Texture2D>("Images/Stars_Small");
            _mediumStarTexture= Content.Load<Texture2D>("Images/Stars_Medium");
            _bigStarTexture = Content.Load<Texture2D>("Images/Stars_Big");
            _ennemyShipTexture = Content.Load<Texture2D>("Images/EnnemyShip");
            _ennemyLaserTexture = Content.Load<Texture2D>("Images/Bullet");
            _gameFont = Content.Load<SpriteFont>("Fonts/GameFont");
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (_playerShip.Life <= 0)
                _isGameOver = true;

            if(!_isGameOver)
            {
                AddEnnemy();
                MoveStars();
                MovePlayerLasers();
                MoveEnnemies();
                ShotEnnemyLaser(gameTime);
                MoveEnemyLasers();
                CheckAndRemove();
                CheckAndRemoveEnnemy();
                CheckCollisions();
                HandleInput(gameTime);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(2, 3, 19));
            spriteBatch.Begin();
            spriteBatch.Draw(_smallStarTexture, _smallStar1, Color.White);
            spriteBatch.Draw(_smallStarTexture, _smallStar2, Color.White);
            spriteBatch.Draw(_mediumStarTexture, _mediumStar1, Color.White);
            spriteBatch.Draw(_mediumStarTexture, _mediumStar2, Color.White);
            spriteBatch.Draw(_bigStarTexture, _bigStar1, Color.White);
            spriteBatch.Draw(_bigStarTexture, _bigStar2, Color.White);

            foreach (var laser in _playerSipLasers)
            {
                spriteBatch.Draw(laser.Texture, laser.Rectangle, Color.White);
            }
            foreach (var laser in _ennemyLasers)
            {
                spriteBatch.Draw(laser.Texture, laser.Rectangle, Color.White);
            }

            foreach (var ennemy in _ennemyShips)
            {
                spriteBatch.Draw(_ennemyShipTexture, ennemy.Rectangle, Color.White);
            }
            spriteBatch.Draw(_playerShip.Texture, _playerShip.Rectangle, Color.White);

            if(_isGameOver)
            {
                spriteBatch.DrawString(_gameFont, $"GAME OVER.", new Vector2(550, 300), Color.White);
                spriteBatch.DrawString(_gameFont, $"Score : {_score} pts.", new Vector2(550, 330), Color.White);
            }
            else
            {
                spriteBatch.DrawString(_gameFont, $"Score : {_score} pts.", new Vector2(20, 20), Color.White);
                spriteBatch.DrawString(_gameFont, $"Life : {_playerShip.Life}", new Vector2(1180, 20), Color.White);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void InitWindow()
        {
            graphics.PreferredBackBufferWidth = 1280;  // set this value to the desired width of your window
            graphics.PreferredBackBufferHeight = 720;   // set this value to the desired height of your window
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
        }

        private void InitPlayerShip()
        {
            int width = 50;
            int height = 100;
            var rectangle = new Rectangle(graphics.PreferredBackBufferWidth / 2 - width / 2, graphics.PreferredBackBufferHeight / 2 - height / 2, width, height);
            _playerShip = new Ship(rectangle, 5, 8, 0.15);
        }

        private void InitStars()
        {
            _smallStar1 = new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            _smallStar2 = new Rectangle(0, -graphics.PreferredBackBufferHeight, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            _mediumStar1 = new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            _mediumStar2 = new Rectangle(0, -graphics.PreferredBackBufferHeight, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            _bigStar1 = new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            _bigStar2 = new Rectangle(0, -graphics.PreferredBackBufferHeight, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
        }

        private void HandleInput(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                _playerShip.MoveRight();

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                _playerShip.MoveLeft();

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                _playerShip.MoveUp();

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                _playerShip.MoveDown();

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
                ShootLaser(gameTime);

            CheckPosition();
        }

        private void CheckPosition()
        {
            if (_playerShip.Rectangle.X < 0)
                _playerShip.Rectangle = new Rectangle(0, _playerShip.Rectangle.Y, _playerShip.Rectangle.Width, _playerShip.Rectangle.Height);

            if (_playerShip.Rectangle.X + _playerShip.Rectangle.Width > graphics.PreferredBackBufferWidth)
                _playerShip.Rectangle = new Rectangle(graphics.PreferredBackBufferWidth - _playerShip.Rectangle.Width, _playerShip.Rectangle.Y, _playerShip.Rectangle.Width, _playerShip.Rectangle.Height);

            if (_playerShip.Rectangle.Y < 0)
                _playerShip.Rectangle = new Rectangle(_playerShip.Rectangle.X, 0, _playerShip.Rectangle.Width, _playerShip.Rectangle.Height);

            if (_playerShip.Rectangle.Y + _playerShip.Rectangle.Height> graphics.PreferredBackBufferHeight)
                _playerShip.Rectangle = new Rectangle(_playerShip.Rectangle.X, graphics.PreferredBackBufferHeight - _playerShip.Rectangle.Height, _playerShip.Rectangle.Width, _playerShip.Rectangle.Height);
        }

        private void ShootLaser(GameTime gameTime)
        {
            _playerShip.CurrentShotInterval -= gameTime.ElapsedGameTime.TotalSeconds;
            if (_playerShip.CurrentShotInterval <= 0)
            {
                _playerShip.CurrentShotInterval = _playerShip.ShotInterval;

                int width = 20;
                int height = 50;
                Rectangle rect = new Rectangle(_playerShip.Rectangle.X + _playerShip.Rectangle.Width / 2 - width / 2,
                                               _playerShip.Rectangle.Y, width, height);

                var laser = new Laser(rect, _laserTexture);
                _playerSipLasers.Add(laser);
            }
        }

        private void MovePlayerLasers()
        {
            foreach (var laser in _playerSipLasers)
                laser.MoveUp();
        }

        private void MoveStars()
        {
            _smallStar1.Y += _smallStarSpeed;
            _smallStar2.Y += _smallStarSpeed;
            _mediumStar1.Y += _mediumStarSpeed;
            _mediumStar2.Y += _mediumStarSpeed;
            _bigStar1.Y += _bigStarSpeed;
            _bigStar2.Y += _bigStarSpeed;

            if (_smallStar1.Y >= graphics.PreferredBackBufferHeight)
                _smallStar1.Y = -graphics.PreferredBackBufferHeight;
            if (_smallStar2.Y >= graphics.PreferredBackBufferHeight)
                _smallStar2.Y = -graphics.PreferredBackBufferHeight;

            if (_mediumStar1.Y >= graphics.PreferredBackBufferHeight)
                _mediumStar1.Y = -graphics.PreferredBackBufferHeight;
            if (_mediumStar2.Y >= graphics.PreferredBackBufferHeight)
                _mediumStar2.Y = -graphics.PreferredBackBufferHeight;

            if (_bigStar1.Y >= graphics.PreferredBackBufferHeight)
                _bigStar1.Y = -graphics.PreferredBackBufferHeight;
            if (_bigStar2.Y >= graphics.PreferredBackBufferHeight)
                _bigStar2.Y = -graphics.PreferredBackBufferHeight;
        }

        private void MoveEnnemies()
        {
            foreach(var ennemy in _ennemyShips)
            {
                ennemy.MoveDown();
            }
        }

        private void CheckAndRemove()
        {
            for (int i = 0; i < _playerSipLasers.Count(); i++)
            {
                if (_playerSipLasers.ElementAt(i).Rectangle.Y < -_playerSipLasers.ElementAt(i).Rectangle.Height || _playerSipLasers.ElementAt(i).CollisionEntered)
                {
                    _playerSipLasers.Remove(_playerSipLasers.ElementAt(i));
                    continue;
                }
            }

            for (int i = 0; i < _ennemyLasers.Count(); i++)
            {
                if (_ennemyLasers.ElementAt(i).Rectangle.Y > graphics.PreferredBackBufferHeight + _ennemyLasers.ElementAt(i).Rectangle.Height || _ennemyLasers.ElementAt(i).CollisionEntered)
                {
                    _ennemyLasers.Remove(_ennemyLasers.ElementAt(i));
                    continue;
                }
            }
        }

        private void AddEnnemy()
        {
            if (_ennemyShips.Count() < _maxEnnemyCount)
            {
                var ennemy = new Ship(GetShipRandomRectangle(), 1, 2, 3);
                ennemy.Texture = _ennemyShipTexture;
                _ennemyShips.Add(ennemy);

            }
        }

        private void CheckAndRemoveEnnemy()
        {
            for (int i = 0; i < _ennemyShips.Count(); i++)
            {
                if (_ennemyShips.ElementAt(i).Rectangle.Y > graphics.PreferredBackBufferHeight || _ennemyShips.ElementAt(i).Life <= 0)
                {
                    if (_ennemyShips.ElementAt(i).Life <= 0)
                        _score++;

                    _ennemyShips.Remove(_ennemyShips.ElementAt(i));
                    continue;
                }
            }
        }

        private Rectangle GetShipRandomRectangle()
        {
            var random = new Random(Guid.NewGuid().GetHashCode());
            int width = 50;
            int height = 100;
            int yPosition = yPosition = random.Next(- graphics.PreferredBackBufferHeight * 3, -height * 3);
            int xPoisition = random.Next(0, graphics.PreferredBackBufferWidth - width);

            var rectangle = new Rectangle(xPoisition, yPosition, width, height);

            foreach (var ennemy in _ennemyShips)
            {
                if (ennemy.Rectangle.Intersects(rectangle))
                    return GetShipRandomRectangle();
            }

            return rectangle;
        }

        private void ShotEnnemyLaser(GameTime gameTime)
        {
            foreach (var ennemy in _ennemyShips)
            {
                if (ennemy.Rectangle.Y > -ennemy.Rectangle.Height)
                {
                    ennemy.CurrentShotInterval -= gameTime.ElapsedGameTime.TotalSeconds;
                    if (ennemy.CurrentShotInterval <= 0)
                    {
                        ennemy.CurrentShotInterval = ennemy.ShotInterval;

                        int width = 20;
                        int height = 20;
                        Rectangle rect = new Rectangle(ennemy.Rectangle.X + ennemy.Rectangle.Width / 2 - width / 2,
                                                       ennemy.Rectangle.Y + ennemy.Rectangle.Height / 2, width, height);

                        var laser = new Laser(rect, _ennemyLaserTexture);
                        _ennemyLasers.Add(laser);
                    }
                }
            }
        }

        private void MoveEnemyLasers()
        {
            foreach (var laser in _ennemyLasers)
                laser.MoveDown();
        }

        private void CheckCollisions()
        {
            foreach(var laser in _playerSipLasers)
            {
                foreach(var ennemy in _ennemyShips)
                {
                    if(laser.Rectangle.Intersects(ennemy.Rectangle))
                    {
                        ennemy.Life--;
                        laser.CollisionEntered = true;
                    }
                }

                foreach(var ennemyLaser in _ennemyLasers)
                {
                    if(laser.Rectangle.Intersects(ennemyLaser.Rectangle))
                    {
                        laser.CollisionEntered = true;
                        ennemyLaser.CollisionEntered = true;
                    }
                }
            }

            foreach(var laser in _ennemyLasers)
            {
                if(laser.Rectangle.Intersects(_playerShip.Rectangle))
                {
                    _playerShip.Life--;
                    laser.CollisionEntered = true;
                }
            }

            foreach(var ennemy in _ennemyShips)
            {
                if (ennemy.Rectangle.Intersects(_playerShip.Rectangle))
                {
                    _isGameOver = true;
                }
            }
        }
    }
}
