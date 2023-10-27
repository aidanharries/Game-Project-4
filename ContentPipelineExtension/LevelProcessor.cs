//---------------------------------------------------------------------------------
// LevelProcessor
// Author: Aidan Harries
// Date: 10/27/23
// Description: This class defines a content processor for handling level data 
//              for the game, including scaling the level and processing tileset
//              textures.
//---------------------------------------------------------------------------------

using ContentPipelineExtension;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace ContentPipelineExtension
{
    [ContentProcessor(DisplayName = "LevelProcessor")]
    public class LevelProcessor : ContentProcessor<LevelDataContent, LevelDataContent>
    {
        /// <summary>
        /// Gets or sets the scaling factor for the level data. Default is 1.0f.
        /// </summary>
        public float Scale { get; set; } = 1.0f;

        /// <summary>
        /// Processes level data, including loading tileset textures and scaling for use in the game.
        /// </summary>
        /// <param name="map">The input level data to process.</param>
        /// <param name="context">The content processor context.</param>
        /// <returns>The processed level data.</returns>
        public override LevelDataContent Process(LevelDataContent map, ContentProcessorContext context)
        {
            map.TilesetTexture = context.BuildAndLoadAsset<TextureContent, Texture2DContent>(new ExternalReference<TextureContent>(map.TilesetImageFilename), "TextureProcessor");

            int tilesetColumns = map.TilesetTexture.Mipmaps[0].Width / map.TileWidth;
            int tilesetRows = map.TilesetTexture.Mipmaps[0].Height / map.TileHeight;

            map.Tiles = new Rectangle[tilesetColumns * tilesetRows];
            context.Logger.LogMessage($"{map.Tiles.Length} Total tiles");
            for (int y = 0; y < tilesetRows; y++)
            {
                for (int x = 0; x < tilesetColumns; x++)
                {
                    map.Tiles[y * tilesetColumns + x] = new Rectangle(
                        x * map.TileWidth,
                        y * map.TileHeight,
                        map.TileWidth,
                        map.TileHeight
                    );
                }

            }

            map.TileWidth = (int)(map.TileWidth * Scale);
            map.TileHeight = (int)(map.TileHeight * Scale);

            return map;

        }
    }
}
