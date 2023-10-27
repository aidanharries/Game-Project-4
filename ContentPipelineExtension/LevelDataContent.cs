//---------------------------------------------------------------------------------
// LevelDataContent
// Author: Aidan Harries
// Date: 10/27/23
// Description: This class represents the content structure for LevelData used in
//              the game, including map dimensions, tile dimensions, tileset
//              texture, tileset data, and map data.
//---------------------------------------------------------------------------------

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using System;

namespace ContentPipelineExtension
{
    [ContentSerializerRuntimeType("Proj4.Tilemap, Proj4")]
    public class LevelDataContent
    {
        /// <summary>Map dimensions</summary>
        public int MapWidth, MapHeight;

        /// <summary>Tile dimensions</summary>
        public int TileWidth, TileHeight;

        /// <summary>The tileset texture</summary>
        public Texture2DContent TilesetTexture;

        /// <summary>The tileset data</summary>
        public Rectangle[] Tiles;

        /// <summary>The map data</summary>
        public int[] TileIndices;

        /// <summary> The tileset image filename </summary>
        [ContentSerializerIgnore]
        public String TilesetImageFilename;
    }
}
