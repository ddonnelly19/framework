﻿using System;
using ActionStreetMap.Explorer.Utils;
using ActionStreetMap.Unity.Wrappers;
using UnityEngine;

namespace ActionStreetMap.Explorer.Scene.Facades
{
    internal class EmptyWallBuilder
    {
        private int _vertStartIndex;
        private float _height = 10;
        private float _minHeight = 0;
        private float _minStepWidth = 2f;
        private float _minStepHeight = 2;
        private GradientWrapper _gradient;

        private Vector3[] _vertices;
        private int[] _triangles;
        private Color[] _colors;

        #region Setters

        public EmptyWallBuilder SetMeshData(MeshData meshData)
        {
            _vertices = meshData.Vertices;
            _triangles = meshData.Triangles;
            _colors = meshData.Colors;
            return this;
        }

        public EmptyWallBuilder SetStartIndex(int index)
        {
            _vertStartIndex = index;
            return this;
        }

        public EmptyWallBuilder SetHeight(float height)
        {
            _height = height;
            return this;
        }

        public EmptyWallBuilder SetMinHeight(float minHeight)
        {
            _minHeight = minHeight;
            return this;
        }

        public EmptyWallBuilder SetStepWidth(float stepWidth)
        {
            _minStepWidth = stepWidth;
            return this;
        }

        public EmptyWallBuilder SetStepHeight(float stepHeight)
        {
            _minStepHeight = stepHeight;
            return this;
        }

        public EmptyWallBuilder SetGradient(GradientWrapper gradient)
        {
            _gradient = gradient;
            return this;
        }

        #endregion

        public int CalculateVertexCount(Vector3 start, Vector3 end)
        {
            var direction = (end - start);
            direction.Normalize();

            var distance = Vector3.Distance(start, end);

            var xIterCount = (int)Math.Ceiling(distance / _minStepWidth);
            var yIterCount = (int)Math.Ceiling(_height / _minStepHeight);

            return xIterCount * yIterCount * 12;
        }

        public void Build(Vector3 start, Vector3 end)
        {
            var direction = (end - start);
            direction.Normalize();

            var distance = Vector3.Distance(start, end);

            var xIterCount = (int)Math.Ceiling(distance / _minStepWidth);
            float xStep = distance / xIterCount;

            var yIterCount = (int)Math.Ceiling(_height / _minStepHeight);
            float yStep = _height / yIterCount;

            var vertCount = xIterCount * yIterCount * 12;

            for (int j = _vertStartIndex; j < _vertStartIndex + yIterCount; j++)
            {
                var yStart = _minHeight + j * yStep;
                var yEnd = yStart + yStep;

                for (int i = 0; i < xIterCount; i++)
                {
                    var startIndex = (12 * xIterCount) * j + i * 12;

                    var x1 = start + direction * (i * xStep);
                    var middle = x1 + direction * (0.5f * xStep);
                    var x2 = x1 + direction * xStep;

                    BuildPlane(x1, middle, x2, yStart, yEnd, startIndex, vertCount);
                }
            }
        }

        protected void BuildPlane(Vector3 x1, Vector3 middle, Vector3 x2,
            float yStart, float yEnd, int startIndex, int vertCount)
        {
            // NOTE taking into account performance, don't want to split
            // this huge function into smaller ones

            var p0 = new Vector3(x1.x, yStart, x1.z);
            var p1 = new Vector3(x2.x, yStart, x2.z);
            var p2 = new Vector3(x2.x, yEnd, x2.z);
            var p3 = new Vector3(x1.x, yEnd, x1.z);

            var pc = new Vector3(middle.x, yStart + (yEnd - yStart) / 2, middle.z);

            var count = startIndex;

            #region Vertices
            _vertices[count] = p3;
            _vertices[count + vertCount] = p3;
            _vertices[++count] = p0;
            _vertices[count + vertCount] = p0;
            _vertices[++count] = pc;
            _vertices[count + vertCount] = pc;

            _vertices[++count] = p0;
            _vertices[count + vertCount] = p0;
            _vertices[++count] = p1;
            _vertices[count + vertCount] = p1;
            _vertices[++count] = pc;
            _vertices[count + vertCount] = pc;

            _vertices[++count] = p1;
            _vertices[count + vertCount] = p1;
            _vertices[++count] = p2;
            _vertices[count + vertCount] = p2;
            _vertices[++count] = pc;
            _vertices[count + vertCount] = pc;

            _vertices[++count] = p2;
            _vertices[count + vertCount] = p2;
            _vertices[++count] = p3;
            _vertices[count + vertCount] = p3;
            _vertices[++count] = pc;
            _vertices[count + vertCount] = pc;
            #endregion

            #region Triangles
            // triangles for outer part
            for (int i = startIndex; i < startIndex + 12; i++)
                _triangles[i] = i;

            var lastIndex = startIndex + vertCount + 12;
            for (int i = startIndex + vertCount; i < lastIndex; i++)
            {
                var rest = i % 3;
                _triangles[i] = rest == 0 ? i : (rest == 1 ? i + 1 : i - 1);
            }
            #endregion

            #region Colors
            count = startIndex;
            var color = GetColor(p3);
            _colors[count] = color;
            _colors[count + vertCount] = color;
            _colors[++count] = color;
            _colors[count + vertCount] = color;
            _colors[++count] = color;
            _colors[count + vertCount] = color;

            color = GetColor(p0);
            _colors[++count] = color;
            _colors[count + vertCount] = color;
            _colors[++count] = color;
            _colors[count + vertCount] = color;
            _colors[++count] = color;
            _colors[count + vertCount] = color;

            color = GetColor(p1);
            _colors[++count] = color;
            _colors[count + vertCount] = color;
            _colors[++count] = color;
            _colors[count + vertCount] = color;
            _colors[++count] = color;
            _colors[count + vertCount] = color;

            color = GetColor(p2);
            _colors[++count] = color;
            _colors[count + vertCount] = color;
            _colors[++count] = color;
            _colors[count + vertCount] = color;
            _colors[++count] = color;
            _colors[count + vertCount] = color;
            #endregion
        }

        protected Color GetColor(Vector3 point)
        {
            var value = (Noise.Perlin3D(point, .3f) + 1f) / 2f;
            return _gradient.Evaluate(value);
        }
    }
}
