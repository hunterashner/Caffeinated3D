using Caffeinated3D.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caffeinated3D.Diagnostics
{
    public struct DState
    {
        public bool CameraCoords;
        public bool FPS;
        public bool EngineVersion;
        public Color TextColor;
    }
    public class DiagnosticsHUD
    {
        public DState State { get; set; }
        private SpriteBatch _spriteBatch;
        private SpriteFont _font;
        private PerspectiveCamera _perspectiveCamera;
        private string _versionNumber;
        public bool Enabled { get; set; }
        public DiagnosticsHUD(SpriteBatch spriteBatch, SpriteFont font, PerspectiveCamera camera, DState state)
        {
            _spriteBatch = spriteBatch;
            _font = font;
            _perspectiveCamera = camera;
            State = state;
            Enabled = true;
            _versionNumber = "0.0.01";
        }

        public void Draw(GameTime gameTime)
        {
            if (!Enabled) return;
            if (State.CameraCoords) 
            {
                _spriteBatch.DrawString(
                    _font, 
                    $"Coords : {String.Format("{0:0.00}", _perspectiveCamera.Position.ToNumerics())}", 
                    new Vector2(50, 1050), 
                    State.TextColor);
            }

            if (State.FPS)
            {
                //draw FPS
            }

            if (State.EngineVersion)
            {
                _spriteBatch.DrawString(
                    _font,
                    $"Caffeinated3D v{_versionNumber}",
                    new Vector2(50, 30),
                    State.TextColor);
            }
        }
    }
}
