﻿// ----------------------------------------------------------------------- 
// <copyright file="EdgeEnumerator.cs" company="">
//     Triangle.NET code by Christian Woltering, http://triangle.codeplex.com/
// </copyright>
// ----------------------------------------------------------------------- 

using System.Collections.Generic;
using ActionStreetMap.Core.Polygons.Geometry;
using ActionStreetMap.Core.Polygons.Topology;

namespace ActionStreetMap.Core.Polygons.Meshing.Iterators
{
    /// <summary> Enumerates the edges of a triangulation. </summary>
    public class EdgeIterator : IEnumerator<Edge>
    {
        private IEnumerator<Triangle> triangles;
        private Otri tri = default(Otri);
        private Otri neighbor = default(Otri);
        private Osub sub = default(Osub);
        private Edge current;
        private Vertex p1, p2;

        /// <summary> Initializes a new instance of the <see cref="EdgeIterator"/> class. </summary>
        public EdgeIterator(Mesh mesh)
        {
            triangles = mesh.triangles.Values.GetEnumerator();
            triangles.MoveNext();

            tri.tri = triangles.Current;
            tri.orient = 0;
        }

        public Edge Current
        {
            get { return current; }
        }

        public void Dispose()
        {
            this.triangles.Dispose();
        }

        object System.Collections.IEnumerator.Current
        {
            get { return current; }
        }

        public bool MoveNext()
        {
            if (tri.tri == null)
            {
                return false;
            }

            current = null;

            while (current == null)
            {
                if (tri.orient == 3)
                {
                    if (triangles.MoveNext())
                    {
                        tri.tri = triangles.Current;
                        tri.orient = 0;
                    }
                    else
                    {
                        // Finally no more triangles 
                        return false;
                    }
                }

                tri.Sym(ref neighbor);

                if ((tri.tri.id < neighbor.tri.id) || (neighbor.tri.id == Triangle.EmptyID))
                {
                    p1 = tri.Org();
                    p2 = tri.Dest();

                    tri.Pivot(ref sub);

                    // Boundary mark of dummysub is 0, so we don't need to worry about that. 
                    current = new Edge(p1.id, p2.id, sub.seg.boundary);
                }

                tri.orient++;
            }

            return true;
        }

        public void Reset()
        {
            this.triangles.Reset();
        }
    }
}