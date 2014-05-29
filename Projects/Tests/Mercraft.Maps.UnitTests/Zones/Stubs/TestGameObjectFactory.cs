﻿using System;
using Mercraft.Core.Unity;
using UnityEngine;

namespace Mercraft.Maps.UnitTests.Zones.Stubs
{
    public class TestGameObjectFactory: IGameObjectFactory
    {
        public IGameObject CreateNew(string name)
        {
            return new TestGameObject();
        }

        public IGameObject CreatePrimitive(string name, PrimitiveType type)
        {
            return new TestGameObject();
        }
    }
}
