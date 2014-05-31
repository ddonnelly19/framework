﻿using Mercraft.Core.Unity;
using UnityEngine;

namespace Mercraft.Explorer.Infrastructure
{
    public class GameObjectFactory : IGameObjectFactory
    {
        public virtual IGameObject CreateNew(string name)
        {
            return new UnityGameObject(name);
        }

        public virtual IGameObject CreatePrimitive(string name, UnityPrimitiveType type)
        {
            return new UnityGameObject(name, GetPrimitive(type));
        }

        private GameObject GetPrimitive(UnityPrimitiveType type)
        {
            switch (type)
            {
                case UnityPrimitiveType.Capsule:
                    return GameObject.CreatePrimitive(PrimitiveType.Capsule);
                case UnityPrimitiveType.Cube:
                    return GameObject.CreatePrimitive(PrimitiveType.Cube);
                case UnityPrimitiveType.Cylinder:
                    return GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                case UnityPrimitiveType.Plane:
                    return GameObject.CreatePrimitive(PrimitiveType.Plane);
                case UnityPrimitiveType.Quad:
                    return GameObject.CreatePrimitive(PrimitiveType.Quad);
                case UnityPrimitiveType.Sphere:
                    return GameObject.CreatePrimitive(PrimitiveType.Sphere);
            }
            return null;
        }
    }
}