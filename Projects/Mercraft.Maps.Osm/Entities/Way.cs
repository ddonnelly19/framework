﻿using System.Collections.Generic;
using Mercraft.Core;
using Mercraft.Maps.Osm.Visitors;

namespace Mercraft.Maps.Osm.Entities
{
    /// <summary>
    ///     Represents a simple way.
    /// </summary>
    public class Way : Element
    {
        /// <summary>
        ///     Holds the list of nodes.
        /// </summary>
        public List<long> NodeIds { get; set; }

        public List<Node> Nodes { get; set; }


        public override void Accept(IElementVisitor elementVisitor)
        {
            elementVisitor.VisitWay(this);
        }


        /// <summary>
        ///     Returns all the ponts in this way in the same order as the nodes.
        /// </summary>
        public void FillPoints(List<GeoCoordinate> coordinates)
        {
            for (int idx = 0; idx < Nodes.Count; idx++)
            {
                if (idx > 0 && Nodes[idx - 1].Coordinate == Nodes[idx].Coordinate)
                {
                    continue;
                }
                coordinates.Add(Nodes[idx].Coordinate);
            }
        }

        public bool IsPolygon
        {
            get { return Nodes.Count > 2; }
        }

        public bool IsComplete
        {
            get { return Nodes.Count == NodeIds.Count; }
        }

        public bool IsClosed
        {
            get { return Nodes[0].Id == Nodes[Nodes.Count - 1].Id; }
        }
    }
}