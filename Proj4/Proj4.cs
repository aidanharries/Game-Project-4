//---------------------------------------------------------------------------------
// Proj4
// Author: Aidan Harries
// Date: 10/27/23
// Description: This class manages the game loop, states, and main logic for 
//              the Puzzle Pond game.
//---------------------------------------------------------------------------------

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace Proj4
{
    /// <summary>
    /// Represents the different states the game can be in.
    /// </summary>
    public enum GameState
    {
        MainMenu,
        HowToPlay,
        Playing,
        Win
    }

    /// <summary>
    /// Represents the main game class for the game.
    /// </summary>
    public class Proj4 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Tilemap _tilemap;
        private BackgroundTiles _backgroundTiles;
        private Lilypad _lilypad;
        private Frog _frog;

        private GameState _currentState = GameState.Playing;

        private SpriteFont _winningFont;
        private SpriteFont _titleFont;

        private KeyboardState _previousKeyboardState;

        private Stopwatch _gameStopwatch = new Stopwatch();
        private SpriteFont _timeFont;

        private TimeSpan _bestTime = TimeSpan.MaxValue;

        /// <summary>
        /// Initializes a new instance of the game class
        /// </summary>
        public Proj4()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _currentState = GameState.MainMenu;

            // Set screen width and height
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.ApplyChanges();
        }

        /// <summary>
        /// Initializes the game by loading saved game data and setting the best time.
        /// </summary>
        protected override void Initialize()
        {
            GameData savedData = LoadGameData();
            _bestTime = savedData.BestTime;
            base.Initialize();
        }

        /// <summary>
        /// Loads game content, initializing textures, fonts, and game objects.
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load font
            _winningFont = Content.Load<SpriteFont>("winning");
            _titleFont = Content.Load<SpriteFont>("title");
            _timeFont = Content.Load<SpriteFont>("time");

            // Load the background texture
            var backgroundTexture = Content.Load<Texture2D>("water_tiles");
            _backgroundTiles = new BackgroundTiles(backgroundTexture, 1, 18);

            // Loading the tilemap
            _tilemap = Content.Load<Tilemap>("tilemap");

            // Find the portal position
            Point portalPosition = FindPortalPosition(_tilemap);

            // Load the lilypad texture
            var lilypadTexture = Content.Load<Texture2D>("lilypad");
            List<Point> unstableTiles = FindUnstableTiles(_tilemap);
            _lilypad = new Lilypad(lilypadTexture, 120, 120, 9, 16, portalPosition, unstableTiles);

            // Load the player's texture
            var playerTexture = Content.Load<Texture2D>("frog");

            // Initialize the player
            _frog = new Frog(new Point(0, 8), playerTexture, GraphicsDevice.Viewport);
        }

        /// <summary>
        /// Searches the tilemap for a portal tile (index 3) and returns its position as a Point.
        /// </summary>
        /// <param name="tilemap">The tilemap to search within.</param>
        /// <returns>The position of the portal tile, or Point.Zero if not found.</returns>
        private Point FindPortalPosition(Tilemap tilemap)
        {
            for (int y = 0; y < tilemap.MapHeight; y++)
            {
                for (int x = 0; x < tilemap.MapWidth; x++)
                {
                    int index = tilemap.TileIndices[y * tilemap.MapWidth + x];
                    if (index == 3)
                    {
                        return new Point(x, y);
                    }
                }
            }
            return Point.Zero;  // Return a default value if portal is not found
        }

        /// <summary>
        /// Finds and returns a list of points representing the positions of all unstable tiles in a given tilemap.
        /// </summary>
        /// <param name="tilemap">The tilemap to search through.</param>
        /// <returns>A list of points where each point represents the position of an unstable tile in the tilemap.</returns>
        public List<Point> FindUnstableTiles(Tilemap tilemap)
        {
            List<Point> unstableTiles = new List<Point>();
            for (int y = 0; y < tilemap.MapHeight; y++)
            {
                for (int x = 0; x < tilemap.MapWidth; x++)
                {
                    int index = tilemap.TileIndices[y * tilemap.MapWidth + x];
                    if (index == 2)
                    {
                        unstableTiles.Add(new Point(x, y));
                    }
                }
            }
            return unstableTiles;
        }

        /// <summary>
        /// Saves the current game data, including the best time, to a local file.
        /// </summary>
        private void SaveGameData()
        {
            GameData data = new GameData
            {
                BestTime = _gameStopwatch.Elapsed
            };

            string jsonString = JsonSerializer.Serialize(data);
            File.WriteAllText("save.dat", jsonString);
        }

        /// <summary>
        /// Loads game data from a saved file, or returns default values if the file does not exist.
        /// </summary>
        /// <returns>An instance of GameData containing the loaded or default game data.</returns>
        private GameData LoadGameData()
        {
            if (File.Exists("save.dat"))
            {
                string jsonString = File.ReadAllText("save.dat");
                return JsonSerializer.Deserialize<GameData>(jsonString);
            }
            return new GameData { BestTime = TimeSpan.MaxValue };
        }

        /// <summary>
        /// Updates the game logic and handles user input in the game loop.
        /// </summary>
        /// <param name="gameTime">Time passed since the last frame.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState currentKeyboardState = Keyboard.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (_currentState == GameState.MainMenu)
            {
                if (_previousKeyboardState.IsKeyUp(Keys.Enter) && currentKeyboardState.IsKeyDown(Keys.Enter))
                {
                    _currentState = GameState.HowToPlay;
                }
            }
            else if (_currentState == GameState.HowToPlay)
            {
                if (_previousKeyboardState.IsKeyUp(Keys.Enter) && currentKeyboardState.IsKeyDown(Keys.Enter))
                {
                    _currentState = GameState.Playing;
                    _gameStopwatch.Start();
                }
            }
            else
            {
                // Update the player
                _frog.Update(gameTime);

                // Update the lilypad visibility based on the player's position
                if (_lilypad.UpdateVisibility(_frog.Position))
                {
                    // Reset player to starting position
                    _frog.ResetToStartingPosition();
                }

                // Update the animated background tiles
                _backgroundTiles.Update(gameTime);

                // Check if the player has reached the portal
                if (_lilypad.HasWon)
                {
                    _currentState = GameState.Win;
                    _gameStopwatch.Stop();

                    // Check if current time is better than the best time and save if it is
                    GameData currentData = LoadGameData();
                    if (_gameStopwatch.Elapsed < currentData.BestTime)
                    {
                        SaveGameData();
                    }
                }
            }

            _previousKeyboardState = currentKeyboardState;  // Store the current state for the next frame

            base.Update(gameTime);
        }

        /// <summary>
        /// Draws game content based on the current game state.
        /// </summary>
        /// <param name="gameTime">Time passed since the last frame.</param>
        protected override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            switch (_currentState)
            {
                case GameState.MainMenu:
                    GraphicsDevice.Clear(Color.Black);

                    _spriteBatch.DrawString(_titleFont, "Puzzle Pond", new Vector2(GraphicsDevice.Viewport.Width / 2 - _titleFont.MeasureString("Puzzle Pond").X / 2, GraphicsDevice.Viewport.Height / 4 - _titleFont.MeasureString("Puzzle Pond").Y / 2), Color.White);
                    _spriteBatch.DrawString(_timeFont, "Press ENTER to start", new Vector2(GraphicsDevice.Viewport.Width / 2 - _timeFont.MeasureString("Press ENTER to start").X / 2, GraphicsDevice.Viewport.Height / 2 - _timeFont.MeasureString("Press ENTER to start").Y / 2), Color.White);
                    break;

                case GameState.HowToPlay:
                    GraphicsDevice.Clear(Color.Black);

                    _spriteBatch.DrawString(_titleFont, "How to Play", new Vector2(GraphicsDevice.Viewport.Width / 2 - _titleFont.MeasureString("How to Play").X / 2, GraphicsDevice.Viewport.Height / 10), Color.White);
                    _spriteBatch.DrawString(_timeFont, "Use arrow keys to move the frog.", new Vector2(GraphicsDevice.Viewport.Width / 2 - _timeFont.MeasureString("Use arrow keys to move the frog.").X / 2, GraphicsDevice.Viewport.Height * 3 / 7), Color.White);
                    _spriteBatch.DrawString(_timeFont, "Goal: Get through the maze to the purple lily pad.", new Vector2(GraphicsDevice.Viewport.Width / 2 - _timeFont.MeasureString("Goal: Get through the maze to the purple lily pad.").X / 2, GraphicsDevice.Viewport.Height * 4 / 7), Color.White);
                    _spriteBatch.DrawString(_timeFont, "Your time starts when you hit ENTER. Good luck!", new Vector2(GraphicsDevice.Viewport.Width / 2 - _timeFont.MeasureString("Your time starts when you hit ENTER. Good luck!").X / 2, GraphicsDevice.Viewport.Height * 5 / 7), Color.White);
                    _spriteBatch.DrawString(_timeFont, "Press ENTER to continue", new Vector2(GraphicsDevice.Viewport.Width / 2 - _timeFont.MeasureString("Press ENTER to continue").X / 2, GraphicsDevice.Viewport.Height * 6 / 7), Color.White);
                    break;

                case GameState.Win:
                    GraphicsDevice.Clear(Color.Black);

                    _spriteBatch.DrawString(_winningFont, "YOU WON!", new Vector2(GraphicsDevice.Viewport.Width / 2 - _winningFont.MeasureString("YOU WON!").X / 2, GraphicsDevice.Viewport.Height / 4 - _winningFont.MeasureString("YOU WON!").Y / 2), Color.White);

                    // Calculate the position for the elapsed time
                    Vector2 timeSize = _timeFont.MeasureString($"Time: {_gameStopwatch.Elapsed.ToString("mm\\:ss")}");

                    // Calculate the position for the best time
                    Vector2 bestTimeSize = _timeFont.MeasureString($"Best Time: {_bestTime.ToString("mm\\:ss")}");

                    // Calculate the total height occupied by both strings and the gap in between
                    float totalHeight = timeSize.Y + bestTimeSize.Y + 10;

                    // Calculate the starting Y-coordinate to ensure that both strings are vertically centered
                    float startingY = (GraphicsDevice.Viewport.Height - totalHeight) / 2;

                    Vector2 timePosition = new Vector2((GraphicsDevice.Viewport.Width - timeSize.X) / 2, startingY);
                    Vector2 bestTimePosition = new Vector2((GraphicsDevice.Viewport.Width - bestTimeSize.X) / 2, startingY + timeSize.Y + 10);  // +10 provides a small gap between the two

                    // Displaying the elapsed time on win screen
                    _spriteBatch.DrawString(_timeFont, $"Time: {_gameStopwatch.Elapsed.ToString("mm\\:ss")}", timePosition, Color.White);
                    _spriteBatch.DrawString(_timeFont, $"Best Time: {_bestTime.ToString("mm\\:ss")}", bestTimePosition, Color.White);
                    break;

                default: // For the Playing state and potentially other states
                    GraphicsDevice.Clear(Color.CornflowerBlue);

                    // Draw the animated background tiles
                    _backgroundTiles.Draw(_spriteBatch);

                    // Drawing the tilemap
                    _tilemap.Draw(gameTime, _spriteBatch);

                    // Draw the lilypad sprites
                    _lilypad.Draw(_spriteBatch);

                    // Draw the player
                    _frog.Draw(_spriteBatch);

                    // Displaying the stopwatch time during gameplay
                    _spriteBatch.DrawString(_timeFont, _gameStopwatch.Elapsed.ToString("mm\\:ss"), new Vector2(10, 10), Color.White);
                    break;
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

    }
}