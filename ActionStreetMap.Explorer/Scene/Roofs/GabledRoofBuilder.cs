﻿using System;
using System.Collections.Generic;
using ActionStreetMap.Core;
using ActionStreetMap.Core.Scene;
using ActionStreetMap.Explorer.Geometry;
using ActionStreetMap.Explorer.Utils;
using ActionStreetMap.Infrastructure.Primitives;
using ActionStreetMap.Infrastructure.Utilities;
using ActionStreetMap.Unity.Wrappers;
using UnityEngine;

namespace ActionStreetMap.Explorer.Scene.Roofs
{
    /// <summary>
    ///     Builds gabled roof.
    ///     See http://wiki.openstreetmap.org/wiki/Key:roof:shape#Roof
    /// </summary>
    internal class GabledRoofBuilder : RoofBuilder
    {
        /// <inheritdoc />
        public override string Name { get { return "gabled"; } }

        /// <inheritdoc />
        public override bool CanBuild(Building building) { return true; }

        public override List<MeshData> Build(Building building)
        {
            throw new NotImplementedException();
        }

        ///// <inheritdoc />
        //public override List<MeshData> Build(Building building)
        //{
        //    var gradient = ResourceProvider.GetGradient(building.RoofColor);
        //    var context = new Context(gradient, ObjectPool);
        //    var roofOffset = building.Elevation + building.MinHeight + building.Height;
        //    var roofHeight = roofOffset + building.RoofHeight;

        //    var polygon = new Polygon(building.Footprint, roofOffset);

        //    // 1. detect the longest segment
        //    float length;
        //    var longestSegment = GetLongestSegment(building.Footprint, out length);
        
        //    // 2. get direction vector
        //    Vector3 ridgeDirection = longestSegment.End - longestSegment.Start;
        //    ridgeDirection.Normalize();

        //    // 3. get centroid
        //    var centroidPoint = PolygonUtils.GetCentroid(building.Footprint);
        //    var centroidVector = new Vector3(centroidPoint.X, longestSegment.Start.y, centroidPoint.Y);

        //    // 4. get something like center line
        //    Vector3 p1 = centroidVector + length * length * ridgeDirection;
        //    Vector3 p2 = centroidVector - length * length * ridgeDirection;
        //    var centerSegment = new Segment(p1, p2);

        //    // 5. detect segments which have intesection with center line
        //    MutableTuple<int, Vector3> firstIntersect;
        //    MutableTuple<int, Vector3> secondIntersect;
        //    DetectIntersectSegments(polygon, centerSegment, out firstIntersect, out secondIntersect);
        //    if (firstIntersect.Item1 == -1 || secondIntersect.Item1 == -1)
        //        throw new AlgorithmException(String.Format(Strings.GabledRoofGenFailed, building.Id));
        //    // move vertices up to make ridge
        //    firstIntersect.Item2 = new Vector3(firstIntersect.Item2.x, roofHeight, firstIntersect.Item2.z);
        //    secondIntersect.Item2 = new Vector3(secondIntersect.Item2.x, roofHeight, secondIntersect.Item2.z);

        //    // 6. process all segments and create vertices
        //    FillMeshData(polygon, firstIntersect, secondIntersect, context);

        //    context.Data.MaterialKey = building.RoofMaterial;
        //    return context.Data;
        //}

        //private Segment GetLongestSegment(List<MapPoint> footprint, out float length)
        //{
        //    var result = ObjectPool.NewList<MapPoint>();
        //    PolygonUtils.Simplify(footprint, result, 1, ObjectPool);
        //    var polygon = new Polygon(result);
        //    Segment longestSegment = default(Segment);
        //    length = 0;
        //    for (int i = 0; i < polygon.Segments.Length; i++)
        //    {
        //        var segment = polygon.Segments[i];
        //        var segmentLength = segment.GetLength();
        //        if (segmentLength > length)
        //        {
        //            longestSegment = segment;
        //            length = segmentLength;
        //        }
        //    }
        //    ObjectPool.StoreList(result);
        //    return longestSegment;
        //}

        //private void DetectIntersectSegments(Polygon polygon, Segment centerSegment, 
        //    out MutableTuple<int, Vector3> firstIntersect, 
        //    out MutableTuple<int, Vector3> secondIntersect)
        //{
        //    firstIntersect = new MutableTuple<int, Vector3>(-1, new Vector3());
        //    secondIntersect = new MutableTuple<int, Vector3>(-1, new Vector3());
        //    for (int i = 0; i < polygon.Segments.Length; i++)
        //    {
        //        var segment = polygon.Segments[i];
        //        if (SegmentUtils.Intersect(segment, centerSegment))
        //        {
        //            var intersectionPoint = SegmentUtils.IntersectionPoint(segment, centerSegment);
        //            if (firstIntersect.Item1 == -1)
        //            {
        //                firstIntersect.Item1 = i;
        //                firstIntersect.Item2 = intersectionPoint;
        //            }
        //            else
        //            {
        //                secondIntersect.Item1 = i;
        //                secondIntersect.Item2 = intersectionPoint;
        //                break;
        //            }
        //        }
        //    }
        //}

        //private void FillMeshData(Polygon polygon, MutableTuple<int, Vector3> firstIntersect,
        //    MutableTuple<int, Vector3> secondIntersect, Context context)
        //{
        //    var count = polygon.Segments.Length;
        //    int i = secondIntersect.Item1;
        //    Vector3 startRidgePoint = default(Vector3);
        //    do 
        //    {
        //        var segment = polygon.Segments[i];
        //        var nextIndex = i == count - 1 ? 0 : i + 1;
        //        // front faces
        //        if (i == firstIntersect.Item1 || i == secondIntersect.Item1)
        //        {
        //            startRidgePoint = i == firstIntersect.Item1 ? firstIntersect.Item2 : secondIntersect.Item2;
        //            AddTriangle(segment.Start, startRidgePoint, segment.End, context);
        //            i = nextIndex;
        //            continue;
        //        }
        //        // side faces
        //        Vector3 endRidgePoint;
        //        if (nextIndex == firstIntersect.Item1 || nextIndex == secondIntersect.Item1)
        //            endRidgePoint = nextIndex == firstIntersect.Item1 ? firstIntersect.Item2 : secondIntersect.Item2;
        //        else
        //            endRidgePoint = GetPointOnLine(firstIntersect.Item2, secondIntersect.Item2, segment.End);

        //        AddTrapezoid(segment.Start, segment.End, endRidgePoint, startRidgePoint, context);

        //        startRidgePoint = endRidgePoint;
        //        i = nextIndex;
        //    } while (i != secondIntersect.Item1);
        //}

        //private void AddTriangle(Vector3 first, Vector3 second, Vector3 third, Context context)
        //{
        //    var color = GradientUtils.GetColor(context.Gradient, first, 0.2f);

        //    context.Data.AddTriangle(
        //        new MapPoint(first.x, first.z, first.y),
        //        new MapPoint(second.x, second.z, second.y),
        //        new MapPoint(third.x, third.z, third.y),
        //        color);
        //}

        //private void AddTrapezoid(Vector3 rightStart, Vector3 leftStart, Vector3 leftEnd, Vector3 rightEnd, Context context)
        //{
        //    AddTriangle(rightStart, leftEnd, leftStart, context);
        //    AddTriangle(leftEnd, rightStart, rightEnd, context);
        //}

        ///// <summary>
        /////     Gets point on line. 
        /////     See http://stackoverflow.com/questions/5227373/minimal-perpendicular-vector-between-a-point-and-a-line
        ///// </summary>
        //private Vector3 GetPointOnLine(Vector3 a, Vector3 b, Vector3 p)
        //{
        //    var d = (a - b).normalized;
        //    var x = a + Vector3.Dot(p - a, d) * d;
        //    return x;
        //}

        //#region Nested classes

        //private sealed class Context
        //{
        //    public MeshData Data;
        //    public GradientWrapper Gradient;

        //    public Context(GradientWrapper gradient, IObjectPool objectPool)
        //    {
        //        Data =  objectPool.CreateMeshData();
        //        Gradient = gradient;
        //   }
        //}

        //#endregion
    }
}
