using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Caffeinated3D.Rendering
{
    public class SkyboxShaderC3D : EffectC3D
    {
        private GraphicsDevice _graphicsDevice;
        private PerspectiveCamera _camera;
        private TextureCube _textureCube;

        private Matrix _world;
        private Matrix _view;
        private Matrix _projection;
        private Vector3 _cameraPosition;

        float _skyboxSize;
        public SkyboxShaderC3D(GraphicsDevice graphicsDevice, byte[] effectCode, PerspectiveCamera camera, TextureCube tex) : base(graphicsDevice, effectCode)
        {
            _graphicsDevice = graphicsDevice;
            _camera = camera;
            _textureCube = tex;

            _skyboxSize = 1f;
        }

        public SkyboxShaderC3D(GraphicsDevice graphicsDevice, byte[] effectCode, int index, int count) : base(graphicsDevice, effectCode, index, count)
        {
        }

        protected SkyboxShaderC3D(Effect cloneSource) : base(cloneSource)
        {
        }

        public override void SetEffectParameters()
        {
            RefreshParams();
            try
            {
                Parameters["World"].SetValue(Matrix.CreateScale(_skyboxSize) * Matrix.CreateTranslation(_cameraPosition));
                Parameters["View"].SetValue(_view);
                Parameters["Projection"].SetValue(_projection);
                Parameters["SkyboxTexture"].SetValue(_textureCube);
                Parameters["CameraPosition"].SetValue(_cameraPosition);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void RefreshParams()
        {
            _world = _camera.WorldMatrix;
            _view = _camera.ViewMatrix;
            _projection = _camera.ProjectionMatrix;
            _cameraPosition = _camera.Position;
        }
    }
}
