﻿using ActionStreetMap.Core.Elevation;
using ActionStreetMap.Core.Unity;
using ActionStreetMap.Core.Utilities;

namespace ActionStreetMap.Core.Tiling.Models
{
    /// <summary> Represents map tile. </summary>
    public class Tile : Model
    {
        /// <summary> Stores map center coordinate in lat/lon. </summary>
        public GeoCoordinate RelativeNullPoint { get; private set; }

        /// <summary> Stores tile center coordinate in Unity metrics. </summary>
        public MapPoint MapCenter { get; private set; }

        /// <summary> Gets width in meters. </summary>
        public float Width { get; private set; }

        /// <summary> Gets height in meters. </summary>
        public float Height { get; private set; }

        /// <summary> Gets or sets tile canvas. </summary>
        public Canvas Canvas { get; private set; }

        /// <summary> Gets bounding box for current tile. </summary>
        public BoundingBox BoundingBox { get; private set; }

        /// <summary> Gets or sets game object which is used to represent this tile. </summary>
        public IGameObject GameObject { get; set; }

        /// <summary> Gets or sets heightmap of given tile. </summary>
        public HeightMap HeightMap { get; set; }

        /// <summary> Gets ModelRegistry of given tile. </summary>
        public TileRegistry Registry { get; private set; }

        /// <summary> Gets top left point on map. </summary>
        public MapPoint TopLeft { get; private set; }

        /// <summary> Gets top right point on map. </summary>
        public MapPoint TopRight { get; private set; }

        /// <summary> Gets bottom left point on map. </summary>
        public MapPoint BottomLeft { get; private set; }

        /// <summary> Gets bottom right point on map. </summary>
        public MapPoint BottomRight { get; private set; }

        /// <inheritdoc />
        public override bool IsClosed { get { return false; } }

        /// <summary> Creates tile. </summary>
        /// <param name="relativeNullPoint">Relative null point.</param>
        /// <param name="mapCenter">Center of map.</param>
        /// <param name="canvas">Map canvas.</param>
        /// <param name="width">Tile width in meters.</param>
        /// <param name="height">Tile height in meters.</param>
        public Tile(GeoCoordinate relativeNullPoint, MapPoint mapCenter, Canvas canvas, 
            float width, float height)
        {
            RelativeNullPoint = relativeNullPoint;
            MapCenter = mapCenter;
            Canvas = canvas;

            Width = width;
            Height = height;

            var geoCenter = GeoProjection.ToGeoCoordinate(relativeNullPoint, mapCenter);
            BoundingBox = BoundingBox.CreateBoundingBox(geoCenter, width, height);

            TopLeft = new MapPoint(MapCenter.X - width / 2, MapCenter.Y + height / 2);
            BottomRight = new MapPoint(MapCenter.X + width / 2, MapCenter.Y - height / 2);

            TopRight = new MapPoint(MapCenter.X + width / 2, MapCenter.Y + height / 2);
            BottomLeft = new MapPoint(MapCenter.X - width / 2, MapCenter.Y - height / 2);

            Registry = new TileRegistry();
        }

        /// <summary> Checks whether absolute position locates in tile with bound offset. </summary>
        /// <param name="position">Absolute position in game.</param>
        /// <param name="offset">offset from bounds.</param>
        /// <returns>Tres if position in tile</returns>
        public bool Contains(MapPoint position, float offset)
        {
            return (position.X > TopLeft.X + offset) && (position.Y < TopLeft.Y - offset) &&
                         (position.X < BottomRight.X - offset) && (position.Y > BottomRight.Y + offset);
        }

        /// <inheritdoc />
        public override void Accept(Tile tile, IModelLoader loader)
        {
            System.Diagnostics.Debug.Assert(tile == this);
            loader.PrepareTile(this);
        }
    }
}