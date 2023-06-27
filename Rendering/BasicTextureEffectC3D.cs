using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caffeinated3D.Rendering
{
    public class BasicTextureEffectC3D : EffectC3D
    {
        private PerspectiveCamera _camera;
        private Matrix _world;
        private Matrix _view;
        private Matrix _projection;
        private Matrix _objectPosition;
        private Matrix _objectRotation;
        private Matrix _objectScale;
        private Texture2D _uv;
        public BasicTextureEffectC3D(GraphicsDevice graphicsDevice,
            byte[] effectCode,
            PerspectiveCamera camera,
            Texture2D uv
            ) : base(graphicsDevice, effectCode)
        {
            _camera = camera;
            _uv = uv;
        }

        public BasicTextureEffectC3D(GraphicsDevice graphicsDevice, byte[] effectCode, int index, int count) : base(graphicsDevice, effectCode, index, count)
        {
        }

        protected BasicTextureEffectC3D(Effect cloneSource) : base(cloneSource)
        {
        }

        public override void SetEffectParameters()
        {
            RefreshParams();
            try
            {
                Parameters["WorldMatrix"].SetValue(_world);
                Parameters["ViewMatrix"].SetValue(_view);
                Parameters["ProjectionMatrix"].SetValue(_projection);
                Parameters["AmbienceColor"].SetValue(new Vector4(0.1f, 0.2f, 0.7f, 1.0f));
                Parameters["WorldInverseTransposeMatrix"].SetValue(Matrix.Invert(Matrix.Transpose(_world)));
                Parameters["DiffuseLightDirection"].SetValue(new Vector3(-1.0f, 0.0f, 0.0f));
                Parameters["DiffuseColor"].SetValue(new Vector4(1.0f, 1.0f, 1.0f, 1.0f));
                Parameters["ModelTexture"].SetValue(_uv);
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
        }
    }
}
