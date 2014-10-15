﻿using System;
using System.Collections.Generic;
using Mercraft.Core.Elevation;

namespace Mercraft.Core.Algorithms
{
    /// <summary>
    /// TODO rename class
    /// </summary>
    public static class PolygonHelper
    {
        /// <summary>
        /// Converts geo coordinates to map coordinates
        /// </summary>
        /// <param name="center">Map center</param>
        /// <param name="geoCoordinates">Geo coordinates</param>
        /// <param name="verticies">output points</param>
        public static void GetVerticies2D(GeoCoordinate center, List<GeoCoordinate> geoCoordinates, 
            List<MapPoint> verticies)
        {
            var length = geoCoordinates.Count;

            for (int i = 0; i < length - 1; i++)
            {
                if (geoCoordinates[i] == geoCoordinates[length - 1])
                {
                    length--;
                    break;
                }
            }

            //if (geoCoordinates[0] == geoCoordinates[length - 1])
            //    length--;

            for (int i = 0; i < length; i++)
            {
                var point = GeoProjection.ToMapCoordinate(center, geoCoordinates[i]);
                verticies.Add(point);
            }

            SortVertices(verticies);
        }

        public static void GetVerticies3D(GeoCoordinate center, HeightMap heightMap,
            List<GeoCoordinate> geoCoordinates, List<MapPoint> verticies)
        {
            var length = geoCoordinates.Count;

            for (int i = 0; i < length - 1; i++)
            {
                if (geoCoordinates[i] == geoCoordinates[length - 1])
                {
                    length--;
                    break;
                }
            }

            //if (geoCoordinates[0] == geoCoordinates[length - 1])
            //    length--;

            FillHeight(center, heightMap, geoCoordinates, verticies, length);

            SortVertices(verticies);
        }

        public static void FillHeight(GeoCoordinate center, HeightMap heightMap, List<GeoCoordinate> geoCoordinates,
            List<MapPoint> verticies, int length)
        {
            for (int i = 0; i < length; i++)
            {
                var point = GeoProjection.ToMapCoordinate(center, geoCoordinates[i]);
                point.Elevation = heightMap.LookupHeight(point);
                verticies.Add(point);
            }
        }

        /// <summary>
        /// Sorts verticies in clockwise order
        /// </summary>
        private static void SortVertices(List<MapPoint> verticies)
        {
            var direction = PointsDirection(verticies);

            switch (direction)
            {
                case PolygonDirection.Clockwise:
                    verticies.Reverse();
                    break;
                case PolygonDirection.CountClockwise:
                default:
                    // TODO need to understand what to do
                    //throw new NotImplementedException("Need to sort vertices!");
                    break;
            }
        }

        private static PolygonDirection PointsDirection(List<MapPoint> points)
        {
            int nCount = 0;
            int nPoints = points.Count;

            if (nPoints < 3)
                return PolygonDirection.Unknown;

            for (int i = 0; i < nPoints; i++)
            {
                int j = (i + 1) % nPoints;
                int k = (i + 2) % nPoints;

                double crossProduct = (points[j].X - points[i].X)
                    * (points[k].Y - points[j].Y);
                crossProduct = crossProduct - 
                    ((points[j].Y - points[i].Y)* (points[k].X - points[j].X));

                if (crossProduct > 0)
                    nCount++;
                else
                    nCount--;
            }

            if (nCount < 0)
                return PolygonDirection.CountClockwise;
            if (nCount > 0)
                return PolygonDirection.Clockwise;
            return PolygonDirection.Unknown;
        }


        public static int[] GetTriangles(List<MapPoint> verticies2D)
        {
            return Triangulator.Triangulate(verticies2D);
        }

        // TODO optimization: we needn't triangles for floor in case of building!
        public static int[] GetTriangles3D(List<MapPoint> verticies2D)
        {
            var verticiesLength = verticies2D.Count;
            
            var indecies = Triangulator.Triangulate(verticies2D);
            
            //var indecies = PolygonTriangulation.GetTriangles3D(verticies2D);
            
            var length = indecies.Length;

            // add top
            Array.Resize(ref indecies, length * 2);
            for (var i = 0; i < length; i++)
            {
                indecies[i + length] = indecies[i] + verticiesLength;
            }

            // process square faces
            var oldIndeciesLength = indecies.Length;
            var faceTriangleCount = verticiesLength * 6;
            Array.Resize(ref indecies, oldIndeciesLength + faceTriangleCount);

            int j = 0;
            for (var i = 0; i < verticiesLength; i++)
            {
                var nextPoint = i < (verticiesLength - 1) ? i + 1 : 0;
                indecies[i + oldIndeciesLength + j] = i;
                indecies[i + oldIndeciesLength + ++j] = nextPoint;
                indecies[i + oldIndeciesLength + ++j] = i + verticiesLength;

                indecies[i + oldIndeciesLength + ++j] = i + verticiesLength;
                indecies[i + oldIndeciesLength + ++j] = nextPoint;
                indecies[i + oldIndeciesLength + ++j] = nextPoint + verticiesLength;
            }

            return indecies;
        }

        internal enum PolygonDirection
        {
            Unknown,
            Clockwise,
            CountClockwise
        }

    }
}
