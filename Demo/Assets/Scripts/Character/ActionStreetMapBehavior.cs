﻿using System;
using System.Reactive.Linq;
using Assets.Scripts.Console;
using Assets.Scripts.Console.Utils;
using Assets.Scripts.Demo;
using ActionStreetMap.Core;
using ActionStreetMap.Explorer;
using ActionStreetMap.Explorer.Bootstrappers;
using ActionStreetMap.Infrastructure.Bootstrap;
using ActionStreetMap.Infrastructure.Config;
using ActionStreetMap.Infrastructure.Dependencies;
using ActionStreetMap.Infrastructure.Diagnostic;
using ActionStreetMap.Infrastructure.IO;
using UnityEngine;
using Component = ActionStreetMap.Infrastructure.Dependencies.Component;

namespace Assets.Scripts.Character
{
    public class ActionStreetMapBehavior : MonoBehaviour
    {
        public float Delta = 10;

        private GameRunner _gameRunner;

        private ITrace _trace;

        private Vector2 _position2D;
        
        private DebugConsole _console;

        private event DataEventHandler<MapPoint> CharacterMove;

        // Use this for initialization
        private void Start()
        {
            Initialize();
        }

        // Update is called once per frame
        void Update () {
            if (Math.Abs(transform.position.x - _position2D.x) > Delta
                || Math.Abs(transform.position.z - _position2D.y) > Delta)
            {
                _position2D = new Vector2(transform.position.x, transform.position.z);
                if (CharacterMove != null)
                {
                    CharacterMove(this, new DataEventArgs<MapPoint>(
                        new MapPoint(transform.position.x, transform.position.z)));
                }
            }
        }


        #region Initialization

        private void Initialize()
        {
            // create and register DebugConsole inside Container
            var container = new Container();
            var messageBus = new MessageBus();
            var pathResolver = new WebPathResolver();
            InitializeConsole(container);
            try
            {
                var fileSystemService = new DemoWebFileSystemService(pathResolver);
                container.RegisterInstance(typeof(IPathResolver), pathResolver);
                container.RegisterInstance(typeof (IFileSystemService), fileSystemService);
                container.RegisterInstance<IConfigSection>(new ConfigSection(@"Config/settings.json", fileSystemService));

                // actual boot service
                container.Register(Component.For<IBootstrapperService>().Use<BootstrapperService>());

                // boot plugins
                container.Register(Component.For<IBootstrapperPlugin>().Use<InfrastructureBootstrapper>().Named("infrastructure"));
                container.Register(Component.For<IBootstrapperPlugin>().Use<OsmBootstrapper>().Named("osm"));
                container.Register(Component.For<IBootstrapperPlugin>().Use<TileBootstrapper>().Named("tile"));
                container.Register(Component.For<IBootstrapperPlugin>().Use<SceneBootstrapper>().Named("scene"));
                container.Register(Component.For<IBootstrapperPlugin>().Use<DemoBootstrapper>().Named("demo"));

                container.RegisterInstance(_trace);

                InitializeMessageBusListeners(messageBus);

                // interception
                //container.AllowProxy = true;
                //container.AutoGenerateProxy = true;
                //container.AddGlobalBehavior(new TraceBehavior(_trace));

                _gameRunner = new GameRunner(container, messageBus);
                _gameRunner.RunGame();
            }
            catch (Exception ex)
            {
                _console.LogMessage(new ConsoleMessage("Error running game:" + ex.ToString(), RecordType.Error, Color.red));
                throw;
            }

            // subscribe on position changes
            Observable.FromEventPattern<DataEventHandler<MapPoint>, DataEventArgs<MapPoint>>(h =>
               CharacterMove += h, h => CharacterMove -= h)
              .Do(e => _gameRunner.OnMapPositionChanged(e.EventArgs.Data))
              .Subscribe();
        }

        private void InitializeConsole(IContainer container)
        {
            var consoleGameObject = new GameObject("_DebugConsole_");
            _console = consoleGameObject.AddComponent<DebugConsole>();
            container.RegisterInstance(_console);
            _trace = new DebugConsoleTrace(_console);

            //_console.CommandManager.Register("scene", new SceneCommand(container));
        }

        private void InitializeMessageBusListeners(IMessageBus messageBus)
        {
            // NOTE not sure that these classes won't be collected during GC
            new DemoTileListener(messageBus, _trace);
        }

        #endregion
    }

    public delegate void DataEventHandler<T>(object sender, DataEventArgs<T> e);

    public class DataEventArgs<T> : EventArgs
    {
        public DataEventArgs(T data)
        {
            Data = data;
        }

        public T Data { get; protected set; }
    }

}