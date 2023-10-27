//---------------------------------------------------------------------------------
// Frog
// Author: Aidan Harries
// Date: 10/27/23
// Description: This class represents the player-controlled frog character in the
//              game. It handles the frog's movement, animation, and rendering.
//---------------------------------------------------------------------------------

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Proj4
{
    public class Frog
    {
        /// <summary>
        /// Gets the current position of the frog character on the game grid as a 2D point.
        /// </summary>
        public Point Position { get; private set; }

        private Texture2D _texture;
        private KeyboardState _previousKeyboardState;

        private Vector2 _currentPosition;
        private Vector2 _targetPosition;

        private float _animationTime;
        private const float _totalAnimationTime = 0.1f;

        private Viewport _viewport;
        private Point _startingPosition;

        private float _rotationAngle = 0f;

        /// <summary>
        /// Initializes a new instance of the Frog class with the specified starting position,
        /// texture, and viewport.
        /// </summary>
        /// <param name="position">The initial position of the frog.</param>
        /// <param name="texture">The texture representing the frog's appearance.</param>
        /// <param name="viewport">The viewport used for boundary checking.</param>
        public Frog(Point position, Texture2D texture, Viewport viewport)
        {
            Position = position;
            _texture = texture;
            _currentPosition = new Vector2(position.X, position.Y);
            _targetPosition = _currentPosition;
            _viewport = viewport;
            _startingPosition = position;
            Position = position;
        }

        /// <summary>
        /// Updates the frog character's position, animation, and input handling based on the current game time.
        /// </summary>
        /// <param name="gameTime">A snapshot of timing values, including elapsed time since the last update.</param>
        public void Update(GameTime gameTime)
        {
            var currentKeyboardState = Keyboard.GetState();

            if (_animationTime >= _totalAnimationTime)
            {
                if (currentKeyboardState.IsKeyDown(Keys.Up) && _previousKeyboardState.IsKeyUp(Keys.Up))
                {
                    _targetPosition = new Vector2(Position.X, Position.Y - 1);
                    _rotationAngle = 0; // Up
                }

                if (currentKeyboardState.IsKeyDown(Keys.Down) && _previousKeyboardState.IsKeyUp(Keys.Down))
                {
                    _targetPosition = new Vector2(Position.X, Position.Y + 1);
                    _rotationAngle = MathHelper.Pi; // Down
                }

                if (currentKeyboardState.IsKeyDown(Keys.Left) && _previousKeyboardState.IsKeyUp(Keys.Left))
                {
                    _targetPosition = new Vector2(Position.X - 1, Position.Y);
                    _rotationAngle = -MathHelper.PiOver2; // Left
                }

                if (currentKeyboardState.IsKeyDown(Keys.Right) && _previousKeyboardState.IsKeyUp(Keys.Right))
                {
                    _targetPosition = new Vector2(Position.X + 1, Position.Y);
                    _rotationAngle = MathHelper.PiOver2; // Right
                }

                // Check viewport boundaries
                _targetPosition = Vector2.Clamp(_targetPosition, Vector2.Zero, new Vector2((_viewport.Width / _texture.Width) - 1, (_viewport.Height / _texture.Height) - 1));

                if (_targetPosition != _currentPosition)
                {
                    Position = new Point((int)_targetPosition.X, (int)_targetPosition.Y);
                    _animationTime = 0;
                }
            }
            else
            {
                _animationTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                _currentPosition = Vector2.Lerp(_currentPosition, _targetPosition, _animationTime / _totalAnimationTime);
                if (_animationTime >= _totalAnimationTime)
                {
                    _currentPosition = _targetPosition;
                }
            }

            _previousKeyboardState = currentKeyboardState;
        }

        /// <summary>
        /// Draws the frog character on the screen using the provided SpriteBatch.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch used for rendering.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 origin = new Vector2(_texture.Width / 2f, _texture.Height / 2f); // Center of the sprite
            spriteBatch.Draw(
                _texture,
                new Rectangle(
                    (int)(_currentPosition.X * _texture.Width + _texture.Width / 2f),
                    (int)(_currentPosition.Y * _texture.Height + _texture.Height / 2f),
                    _texture.Width,
                    _texture.Height
                ),
                null,
                Color.White,
                _rotationAngle,
                origin,
                SpriteEffects.None,
                0
            );
        }

        /// <summary>
        /// Resets the frog's position and orientation to its starting point, facing upward.
        /// </summary>
        public void ResetToStartingPosition()
        {
            Position = _startingPosition;
            _currentPosition = new Vector2(_startingPosition.X, _startingPosition.Y);
            _targetPosition = _currentPosition;
            _rotationAngle = 0; // Set rotation angle to face upwards
        }
    }
}
