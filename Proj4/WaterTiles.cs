//---------------------------------------------------------------------------------
// BackgroundTiles
// Author: Aidan Harries
// Date: 10/27/23
// Description: This class manages the background tile animation for the Puzzle
//              Pond game.
//---------------------------------------------------------------------------------

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Proj4
{
    public class BackgroundTiles
    {
        private Texture2D _texture;
        private int _rows, _columns;
        private int _tileWidth, _tileHeight;
        private int _currentFrame;
        private float _animationTimer;
        private const float _animationSpeed = 0.3f; // Adjust as needed
        private const int _screenRows = 9;
        private const int _screenColumns = 16;

        /// <summary>
        /// Initializes a new instance of the BackgroundTiles class with the specified texture, number of rows, and columns.
        /// </summary>
        /// <param name="texture">The texture representing the background tiles.</param>
        /// <param name="rows">The number of rows in the tileset.</param>
        /// <param name="columns">The number of columns in the tileset.</param>
        public BackgroundTiles(Texture2D texture, int rows, int columns)
        {
            _texture = texture;
            _rows = rows;
            _columns = columns;
            _tileWidth = texture.Width / columns;
            _tileHeight = texture.Height / rows;
        }

        /// <summary>
        /// Updates the animation of the background tiles based on the elapsed game time.
        /// </summary>
        /// <param name="gameTime">Time passed since the last frame.</param>
        public void Update(GameTime gameTime)
        {
            _animationTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_animationTimer > _animationSpeed)
            {
                _currentFrame++;
                _currentFrame %= _rows * _columns;
                _animationTimer -= _animationSpeed;
            }
        }

        /// <summary>
        /// Draws the animated background tiles on the game screen.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch used for rendering.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            var sourceRectangle = new Rectangle(_currentFrame * _tileWidth, 0, _tileWidth, _tileHeight);

            for (int screenRow = 0; screenRow < _screenRows; screenRow++)
            {
                for (int screenCol = 0; screenCol < _screenColumns; screenCol++)
                {
                    var position = new Vector2(screenCol * _tileWidth, screenRow * _tileHeight);
                    spriteBatch.Draw(_texture, position, sourceRectangle, Color.White);
                }
            }
        }
    }
}
