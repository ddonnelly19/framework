﻿using Mercraft.Core;
using Mercraft.Core.Tiles;
using UnityEngine;

namespace Mercraft.Maps.UnitTests.Zones.Stubs
{
    public class TestTileListener : ITileListener
    {
        public void OnTileLoadStarted(MapPoint center, GeoCoordinate relativeNullPoint)
        {
            
        }

        public void OnTileLoadFinished(Tile tile)
        {
            
        }

        public void OnTileFound(Tile tile, MapPoint position)
        {

        }
    }
}