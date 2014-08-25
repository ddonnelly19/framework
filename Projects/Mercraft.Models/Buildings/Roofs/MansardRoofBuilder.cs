﻿using System;
using System.Collections.Generic;
using System.Linq;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.World.Buildings;
using Mercraft.Models.Geometry;
using UnityEngine;

namespace Mercraft.Models.Buildings.Roofs
{
    public class MansardRoofBuilder: IRoofBuilder
    {
        public string Name { get { return "mansard"; } }
        public MeshData Build(Building building, BuildingStyle style)
        {
            var polygon = new Polygon(building.Footprint);
            var offset = 2f; // TODO
            var roofHeight = 3f;
            var verticies3D = GetVerticies3D(polygon, building.BottomOffset + building.Height, offset, roofHeight);

            return new MeshData()
            {
                Vertices = verticies3D,
                Triangles = GetTriangles(building.Footprint),
                UV = GetUV(building.Footprint),
                TextureKey = style.Roof.Texture,
                MaterialKey = style.Roof.Material
            };
        }

        private Vector3[] GetVerticies3D(Polygon polygon, float top, float offset, float roofHeight)
        {
            var verticies = new List<Vector3>(polygon.Verticies.Length * 2);
            var topVerticies = new List<Vector3>(polygon.Verticies.Length);
            for (int i = 0; i < polygon.Segments.Length; i++)
            {
                var previous = i == 0 ? polygon.Segments.Length - 1 : i - 1;
                var nextIndex = i == polygon.Segments.Length - 1 ? 0 : i + 1;

                var segment1 = polygon.Segments[previous];
                var segment2 = polygon.Segments[i];
                var segment3 = polygon.Segments[nextIndex];

                var parallel1 = SegmentUtils.GetParallel(segment1, offset);
                var parallel2 = SegmentUtils.GetParallel(segment2, offset);
                var parallel3 = SegmentUtils.GetParallel(segment3, offset);

                var ip1 = SegmentUtils.IntersectionPoint(parallel1, parallel2);
                var ip2 = SegmentUtils.IntersectionPoint(parallel2, parallel3);

                // TODO check whether offset is correct for intersection

                verticies.Add(new Vector3(segment1.End.x, top, segment1.End.y));
                verticies.Add(new Vector3(ip1.x, top + roofHeight, ip1.y));
                verticies.Add(new Vector3(segment2.End.x, top, segment2.End.y));
                verticies.Add(new Vector3(ip2.x, top + roofHeight, ip2.y));

                topVerticies.Add(new Vector3(ip1.x, top + roofHeight, ip1.y));
            }
            verticies.AddRange(topVerticies);
            return verticies.ToArray();
        }

        private int[] GetTriangles(MapPoint[] footprint)
        {
            var triangles = new List<int>();
            for (int i = 0; i < footprint.Length; i++)
            {
                var offset = i * 4;
                triangles.AddRange(new int[]
                {
                    0 + offset, 2 + offset, 1 + offset,
                    3 + offset, 1 + offset, 2 + offset
                });
            }

            var topPartIndecies = Triangulator.Triangulate(footprint);
            var vertCount = footprint.Length * 4;
            triangles.AddRange(topPartIndecies.Select(i => i + vertCount));

            return triangles.ToArray();
        }

        private Vector2[] GetUV(MapPoint[] footprint)
        {
            var uv = new Vector2[footprint.Length * 5];

            for (int i = 0; i < uv.Length; i++)
            {
                uv[i] = new Vector2(0, 0);
            }

            return uv;
        }
    }
}