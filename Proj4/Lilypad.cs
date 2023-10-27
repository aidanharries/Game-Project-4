//---------------------------------------------------------------------------------
// Lilypad
// Author: Aidan Harries
// Date: 10/27/23
// Description: This class represents the lilypads. It manages its visibility,
//              interactions with the player, and drawing on the screen.
//---------------------------------------------------------------------------------

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;

namespace Proj4
{
    public class Lilypad
    {
        /// <summary>
        /// Gets a value indicating whether the player has won the game by reaching the lilypad.
        /// </summary>
        public bool HasWon { get; private set; } = false;

        private Texture2D _texture;

        private int _tileWidth, _tileHeight;
        private int _rows, _columns;
        private bool[,] _isVisible;
        private int _portalX, _portalY;

        private List<Point> _unstableTiles;
        private List<Point> _steppedUnstableTiles = new List<Point>();
        private Point? _previousPlayerPosition = null;

        /// <summary>
        /// Initializes a new instance of the Lilypad class.
        /// </summary>
        /// <param name="texture">The texture representing the lilypad.</param>
        /// <param name="tileWidth">The width of a single tile on the lilypad.</param>
        /// <param name="tileHeight">The height of a single tile on the lilypad.</param>
        /// <param name="rows">The number of rows on the lilypad.</param>
        /// <param name="columns">The number of columns on the lilypad.</param>
        /// <param name="portalPosition">The position of the portal on the lilypad.</param>
        /// <param name="unstableTiles">A list of unstable tiles on the lilypad.</param>
        public Lilypad(Texture2D texture, int tileWidth, int tileHeight, int rows, int columns, Point portalPosition, List<Point> unstableTiles)
        {
            _texture = texture;
            _tileWidth = tileWidth;
            _tileHeight = tileHeight;
            _rows = rows;
            _columns = columns;
            _isVisible = new bool[columns, rows];
            _portalX = portalPosition.X;
            _portalY = portalPosition.Y;
            _unstableTiles = unstableTiles;

            for (int row = 0; row < _rows; row++)
            {
                for (int col = 0; col < _columns; col++)
                {
                    _isVisible[col, row] = true;
                }
            }

            // Portal Position
            _isVisible[_portalX, _portalY] = false;
        }

        /// <summary>
        /// Updates the visibility of the lilypad and checks for interactions with the player's position.
        /// </summary>
        /// <param name="playerPosition">The current position of the player on the game grid.</param>
        /// <returns>True if the player touched an unstable tile, otherwise false.</returns>
        public bool UpdateVisibility(Point playerPosition)
        {
            bool touchedUnstableTile = false;

            if (playerPosition.X >= 0 && playerPosition.X < _columns &&
                playerPosition.Y >= 0 && playerPosition.Y < _rows)
            {
                // Check if player stepped off the previous unstable tile
                if (_previousPlayerPosition.HasValue && _unstableTiles.Contains(_previousPlayerPosition.Value) && _previousPlayerPosition.Value != playerPosition)
                {
                    _steppedUnstableTiles.Remove(_previousPlayerPosition.Value);
                }

                if (playerPosition.X == _portalX && playerPosition.Y == _portalY)
                {
                    // Set the game as won when the player is on the portal
                    HasWon = true;
                }
                else if (_unstableTiles.Contains(playerPosition) && !_steppedUnstableTiles.Contains(playerPosition))
                {
                    Debug.WriteLine("Player is on an unstable tile!");
                    _isVisible[playerPosition.X, playerPosition.Y] = false;
                    _steppedUnstableTiles.Add(playerPosition);
                    touchedUnstableTile = true;
                }
                else
                {
                    _isVisible[playerPosition.X, playerPosition.Y] = false;
                }
            }

            _previousPlayerPosition = playerPosition;

            return touchedUnstableTile;
        }

        /// <summary>
        /// Draws the lilypad on the game screen, considering its visibility and position.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch used for rendering.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            for (int row = 0; row < _rows; row++)
            {
                for (int col = 0; col < _columns; col++)
                {
                    if (_isVisible[col, row])
                    {
                        var position = new Vector2(col * _tileWidth, row * _tileHeight);
                        spriteBatch.Draw(_texture, position, Color.White);
                    }
                }
            }
        }
    }
}

