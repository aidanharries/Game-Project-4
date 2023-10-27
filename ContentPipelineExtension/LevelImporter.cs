//---------------------------------------------------------------------------------
// LevelImporter
// Author: Aidan Harries
// Date: 10/27/23
// Description: This class represents a custom content importer for level data
//              used in the game. It reads and processes level data from a .tmap
//              file to create LevelDataContent objects.
//---------------------------------------------------------------------------------

using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace ContentPipelineExtension
{
    [ContentImporter(".tmap", DisplayName = "LevelImporter", DefaultProcessor = "LevelProcessor")]
    public class LevelImporter : ContentImporter<LevelDataContent>
    {
        /// <summary>
        /// Imports level data from a .tmap file and creates a LevelDataContent object.
        /// </summary>
        /// <param name="filename">The path to the .tmap file containing level data.</param>
        /// <param name="context">The context for the content import operation.</param>
        /// <returns>A LevelDataContent object representing the imported level data.</returns>
        public override LevelDataContent Import(string filename, ContentImporterContext context)
        {
            // Create a new LevelDataContent
            LevelDataContent map = new();

            // Read in the map file and split along newlines
            string data = File.ReadAllText(filename);
            var lines = data.Split('\n');

            // First line in the map file is the image file name,
            // we store it so it can be loaded in the processor
            map.TilesetImageFilename = lines[0].Trim();

            // Second line is the tileset image size
            var secondLine = lines[1].Split(',');
            map.TileWidth = int.Parse(secondLine[0]);
            map.TileHeight = int.Parse(secondLine[1]);

            // Third line is the map size (in tiles)
            var thirdLine = lines[2].Split(',');
            map.MapWidth = int.Parse(thirdLine[0]);
            map.MapHeight = int.Parse(thirdLine[1]);

            // Fourth line is the map data (the indices of tiles in the map)
            // We can use the Linq Select() method to convert the array of strings
            // into an array of ints
            map.TileIndices = lines[3].Split(',').Select(index => int.Parse(index)).ToArray();

            // At this point, we've copied all of the file data into our
            // BasicTilemapContent object, so we pass it onto the processor
            return map;
        }
    }
}
