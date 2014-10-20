﻿using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Unity;
using Mercraft.Core.World;
using Mercraft.Explorer.Helpers;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Infrastructure.Utilities;
using Mercraft.Models.Roads;
using Mercraft.Models.Utils;
using UnityEngine;

namespace Mercraft.Explorer.Scene.Builders
{
    /// <summary>
    ///     Provides logic to build details.
    /// </summary>
    public class DetailModelBuilder: ModelBuilder
    {
        private readonly IResourceProvider _resourceProvider;

        /// <inheritdoc />
        public override string Name
        {
            get { return "detail"; }
        }

        /// <summary>
        ///     Creates DetailModelBuilder.
        /// </summary>
        [Dependency]
        public DetailModelBuilder(WorldManager worldManager, IGameObjectFactory gameObjectFactory,
            IResourceProvider resourceProvider, IObjectPool objectPool) :
            base(worldManager, gameObjectFactory, objectPool)
        {
            _resourceProvider = resourceProvider;
        }

        /// <inheritdoc />
        public override IGameObject BuildNode(Tile tile, Rule rule, Node node)
        {
            var mapPoint = GeoProjection.ToMapCoordinate(tile.RelativeNullPoint, node.Point);
            if (!tile.Contains(mapPoint, 0))
                return null;

            var detail = rule.GetDetail();
            var zIndex = rule.GetZIndex();
            mapPoint.Elevation = tile.HeightMap.LookupHeight(mapPoint);

            // TODO check this
            //WorldManager.AddModel(node.Id);

            return BuildObject(tile, rule, node, mapPoint, zIndex, detail);
        }

        /// <summary>
        ///     Process unity specific data.
        /// </summary>
        protected virtual IGameObject BuildObject(Tile tile, Rule rule, Node node, MapPoint mapPoint, 
            float zIndex, string detail)
        {
            var prefab = _resourceProvider.GetGameObject(detail);
            var gameObject = (GameObject)Object.Instantiate(prefab);
            if (rule.IsRoadFix())
            {
                gameObject.AddComponent<RoadFixBehavior>().RotationOffset = rule.GetDetailRotation();
            }

            gameObject.transform.position = new Vector3(mapPoint.X, mapPoint.Elevation + zIndex, mapPoint.Y);

            // TODO add detail to worldManager
            // TODO actually, sometimes we have to rotate device correctly,
            // need to find way how to do this

            var gameObjectWrapper = GameObjectFactory.Wrap("detail " + node, gameObject);
            gameObjectWrapper.Parent = tile.GameObject;

            return gameObjectWrapper;
        }
    }
}
