﻿using System;
using System.Collections.Generic;
using PlaneSimulator.Graphics;
using PlaneSimulator.Toolkit.IO;
using SharpDX.Windows;

namespace PlaneSimulator
{
    public class Game
    {
        private readonly Timer _timer;
        private readonly Airplane _playerPlane;
        private readonly Renderer _renderer;
        private readonly List<GameComponent> _gameComponents;
        private readonly World _world;
        private bool _newRegisteredElement = false;

        public Game()
        {
            _gameComponents = new List<GameComponent>();
            _timer = new Timer();
            _renderer = new Renderer();

            _world = new World();
            _playerPlane = AirplaneFactory.Create(_world);
            _playerPlane.Initialize(1000, 200);

            Register(new MonitoringHeader(this, _renderer));
            Register(new FlightRecorder(this, _timer, _playerPlane));
        }

        public void Register(GameComponent item)
        {
            _newRegisteredElement = true;
            _gameComponents.Add(item);
            if (item is RenderableGameComponent)
                _renderer.Register(item as RenderableGameComponent);
        }

        public void Run()
        {
            RenderLoop.Run(_renderer.Form, () =>
            {
                double delta = _timer.Tick();
                if (_newRegisteredElement)
                {
                    _gameComponents.Sort();
                    _newRegisteredElement = false;
                }

                foreach (GameComponent item in _gameComponents)
                    if(item.Enabled)
                        item.Update(delta);

                if (_playerPlane.IsCrashed())
                    Exit();

                _renderer.Render();
            });
        }

        private void Exit()
        {
            _renderer.Form.Close();
        }

        public void Dispose()
        {
            _renderer.Dispose();
            foreach (GameComponent item in _gameComponents)
                item.Dispose();
        }
    }
}
