﻿using System;
using System.Collections.Generic;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Unity;
using Mercraft.Core.World;
using Mercraft.Explorer.Helpers;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Infrastructure.Utilities;
using Mercraft.Models.Geometry.ThickLine;
using Mercraft.Models.Utils;
using UnityEngine;

namespace Mercraft.Explorer.Scene.Builders
{
    /// <summary>
    ///     Provides logic to build various barriers.
    /// </summary>
    public class BarrierModelBuilder: ModelBuilder
    {
        private readonly IResourceProvider _resourceProvider;
        private readonly DimenLineBuilder _dimenLineBuilder = new DimenLineBuilder(2);
        private readonly List<LineElement> _lines = new List<LineElement>(1);

        /// <inheritdoc />
        public override string Name
        {
            get { return "barrier"; }
        }

        /// <summary>
        ///     Creates BarrierModelBuilder.
        /// </summary>
        [Dependency]
        public BarrierModelBuilder(WorldManager worldManager, IGameObjectFactory gameObjectFactory, 
            IResourceProvider resourceProvider, IObjectPool objectPool) :
            base(worldManager, gameObjectFactory, objectPool)
        {
            _resourceProvider = resourceProvider;
        }

        /// <inheritdoc />
        public override IGameObject BuildWay(Tile tile, Rule rule, Way way)
        {
            if (way.Points.Count < 2)
            {
                Trace.Warn(ErrorStrings.InvalidPolyline);
                return null;
            }

            var gameObjectWrapper = GameObjectFactory.CreateNew(String.Format("{0} {1}", Name, way));

            var points = ObjectPool.NewList<MapPoint>();
            PointHelper.GetVerticies3D(tile.RelativeNullPoint, tile.HeightMap, way.Points, points);
            
            // reuse lines
            _lines.Clear();
            _lines.Add(new LineElement(points, rule.GetWidth()));

            _dimenLineBuilder.Height = rule.GetHeight();
            _dimenLineBuilder.Build(tile.HeightMap, _lines, 
                (p, t, u) => BuildObject(gameObjectWrapper, rule, p, t, u));
            _lines.Clear();

            ObjectPool.Store(points);

            return gameObjectWrapper;
        }

        /// <summary>
        ///     Process unity specific data.
        /// </summary>
        protected virtual void BuildObject(IGameObject gameObjectWrapper, Rule rule,
            List<Vector3> p, List<int> t, List<Vector2> u)
        {
            var gameObject = gameObjectWrapper.GetComponent<GameObject>();

            Mesh mesh = new Mesh();
            mesh.vertices = p.ToArray();
            mesh.triangles = t.ToArray();
            mesh.uv = u.ToArray();
            mesh.RecalculateNormals();

            var meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;

            var renderer = gameObject.AddComponent<MeshRenderer>();
            renderer.material = rule.GetMaterial(_resourceProvider);
            renderer.material.mainTexture = rule.GetTexture(_resourceProvider);
        }
    }
}
