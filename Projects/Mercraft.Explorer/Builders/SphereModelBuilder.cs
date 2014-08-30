﻿using System;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Unity;
using Mercraft.Explorer.Helpers;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Models.Terrain;
using UnityEngine;

namespace Mercraft.Explorer.Builders
{
    public class SphereModelBuilder : ModelBuilder
    {
        public override string Name
        {
            get { return "sphere"; }
        }

        [Dependency]
        public SphereModelBuilder(IGameObjectFactory gameObjectFactory)
            : base(gameObjectFactory)
        {
        }

        public override IGameObject BuildArea(GeoCoordinate center, HeightMap heightMap, Rule rule, Area area)
        {
            base.BuildArea(center, heightMap, rule, area);
            return BuildSphere(center, heightMap, area, area.Points, rule);
        }

        public override IGameObject BuildWay(GeoCoordinate center, HeightMap heightMap,  Rule rule, Way way)
        {
            base.BuildWay(center, heightMap, rule, way);
            // TODO is it applied to way?
            return BuildSphere(center, heightMap, way, way.Points, rule);
        }

        private IGameObject BuildSphere(GeoCoordinate center, HeightMap heightMap, Model model, GeoCoordinate[] points, Rule rule)
        {
            var circle = CircleHelper.GetCircle(center, points);
            var diameter = circle.Item1;
            var sphereCenter = circle.Item2;

            IGameObject gameObjectWrapper = GameObjectFactory.CreatePrimitive(String.Format("Spfere {0}", model),
                UnityPrimitiveType.Sphere);
            var sphere = gameObjectWrapper.GetComponent<GameObject>();

            sphere.AddComponent<MeshRenderer>();
            sphere.renderer.material = rule.GetMaterial();
            sphere.renderer.material.color = rule.GetFillColor();

            var minHeight = rule.GetMinHeight();
            sphere.transform.localScale = new Vector3(diameter, diameter, diameter);
            sphere.transform.position = new Vector3(sphereCenter.X, minHeight + diameter/2, sphereCenter.Y);

            return gameObjectWrapper;
        }
    }
}