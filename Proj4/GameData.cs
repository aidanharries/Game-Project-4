//---------------------------------------------------------------------------------
// GameData
// Author: Aidan Harries
// Date: 10/27/23
// Description: This class represents game data - the player's best time,
//              and is used for serialization in the game.
//---------------------------------------------------------------------------------

using System;

namespace Proj4
{
    [Serializable]
    public class GameData
    {
        /// <summary>
        /// Gets or sets the best player time recorded in the game.
        /// </summary>
        public TimeSpan BestTime { get; set; }
    }
}
