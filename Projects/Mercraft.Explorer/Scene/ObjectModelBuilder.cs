﻿using System;
using System.Collections.Generic;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Tiles;
using Mercraft.Core.Unity;
using Mercraft.Explorer.Helpers;

namespace Mercraft.Explorer.Scene
{
    public class ObjectModelBuilder: IModelBuilder
    {
        private readonly HashSet<long> _loadedModelIds;

        private readonly IEnumerable<IModelBuilder> _builders;
        private readonly IEnumerable<IModelBehaviour> _behaviours;

        public string Name { get { return "gameobject"; }}

        public ObjectModelBuilder(
            IEnumerable<IModelBuilder> builders,
            IEnumerable<IModelBehaviour> behaviours)
        {
            _loadedModelIds = new HashSet<long>();
            _builders = builders;
            _behaviours = behaviours;
        }

        public IGameObject BuildArea(Tile tile, Rule rule, Area area)
        {
            return ProcessGameObject(rule, area, b => b.BuildArea(tile, rule, area));
        }

        public IGameObject BuildWay(Tile tile, Rule rule, Way way)
        {
            return ProcessGameObject(rule, way, b => b.BuildWay(tile, rule, way));
        }

        private IGameObject ProcessGameObject(Rule rule, Model model, Func<IModelBuilder, IGameObject> goFunc)
        {
            if (!_loadedModelIds.Contains(model.Id))
            {
                var builder = rule.GetModelBuilder(_builders);
                if (builder == null)
                    return null;
                var gameObjectWrapper = goFunc(builder);

                var behaviour = rule.GetModelBehaviour(_behaviours);
                if (behaviour != null)
                    behaviour.Apply(gameObjectWrapper, model);

                return gameObjectWrapper;
            }

            return null;
        }
    }
}