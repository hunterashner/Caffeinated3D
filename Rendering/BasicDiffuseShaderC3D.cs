using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caffeinated3D.Rendering
{
    public class BasicDiffuseShaderC3D : EffectC3D
    {
        private PerspectiveCamera _camera;
        private 

        Matrix _world;
        Matrix _view;
        Matrix _projection;
        public BasicDiffuseShaderC3D(GraphicsDevice graphicsDevice, 
            byte[] effectCode, 
            PerspectiveCamera camera
            ) : base(graphicsDevice, effectCode)
        {
            _camera = camera;
        }

        public BasicDiffuseShaderC3D(GraphicsDevice graphicsDevice, byte[] effectCode, int index, int count) : base(graphicsDevice, effectCode, index, count)
        {
        }

        protected BasicDiffuseShaderC3D(Effect cloneSource) : base(cloneSource)
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
