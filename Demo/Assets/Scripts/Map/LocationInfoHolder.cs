﻿using System;
using Mercraft.Core.Scene;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Unity;
using Mercraft.Maps.Osm.Helpers;
using UnityEngine;
using LocationInfo = Mercraft.Core.LocationInfo;

namespace Assets.Scripts.Map
{
    /// <summary>
    /// Attached to every osm object and provides the way to find LocationInfo
    /// associated with target object
    /// </summary>
    public class LocationInfoHolder : MonoBehaviour, IModelBehaviour
    {
        public LocationInfo LocationInfo { get; private set; }

        /// <summary>
        /// Gets "center" coordinate of object
        /// </summary>
        public Vector3 Center { get; private set; }

        public string Name { get; private set; }

        public void Apply(IGameObject go, Model model)
        {
            LocationInfo = LocationInfoExtractor.Extract(model.Tags);
            if (!String.IsNullOrEmpty(LocationInfo.Street))
            {
                // attach OSM tag
                go.GetComponent<GameObject>().tag = Consts.OsmTag;

                // calculate Position to help determine the nearest object to Character
                // with available LocationInfo
                var meshFilter = gameObject.GetComponentInChildren<MeshFilter>();

                // TODO investigate such cases
                // first: for empty models
                if (meshFilter == null || meshFilter.mesh == null) 
                    return;

                Center = FindCenter(meshFilter.mesh.vertices);
            }
        }

        private static Vector3 FindCenter(Vector3[] polygon)
        {
            Vector3 center = Vector3.zero;
            foreach (Vector3 v3 in polygon)
            {
                center += v3;
            }
            return center / polygon.Length;
        }

    }
}