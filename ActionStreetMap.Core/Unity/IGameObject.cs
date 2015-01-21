﻿namespace ActionStreetMap.Core.Unity
{
    /// <summary>
    ///     Should hold Unity specific GameObject class. Actually, this is workaround which allows to use classes outside Unity environment
    /// </summary>
    public interface IGameObject
    {
        /// <summary>
        ///     Adds component to game object.
        /// </summary>
        /// <typeparam name="T">Type of component</typeparam>
        /// <param name="component">Component.</param>
        /// <returns>Component.</returns>
        T AddComponent<T>(T component);

        /// <summary>
        ///     Gets component of given type.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <returns>Component.</returns>
        T GetComponent<T>();

        /// <summary>
        ///     True, if gameobject has no components.
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        ///     Gets or sets name.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Sets parent game object.
        /// </summary>
        IGameObject Parent { set; }
    }
}