using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caffeinated3D.Shaders
{
    public class BasicDiffuseEffect : IEffectC3D
    {
        public Effect Shader { get; set; }

        public BasicDiffuseEffect(Effect shader)
        {
            Shader = shader;
        }

        public void SetEffectParams(Matrix world, Matrix view, Matrix projection)
        {
            try
            {
                Shader.Parameters["WorldMatrix"].SetValue(world);
                Shader.Parameters["ViewMatrix"].SetValue(view);
                Shader.Parameters["ProjectionMatrix"].SetValue(projection);
                Shader.Parameters["AmbienceColor"].SetValue(new Vector4(0.1f, 0.2f, 0.7f, 1.0f));
                Shader.Parameters["WorldInverseTransposeMatrix"].SetValue(Matrix.Invert(Matrix.Transpose(world)));
                Shader.Parameters["DiffuseLightDirection"].SetValue(new Vector3(-1.0f, 0.0f, 0.0f));
                Shader.Parameters["DiffuseColor"].SetValue(new Vector4(1.0f, 1.0f, 1.0f, 1.0f));
            }
            catch (Exception e)
            {
                throw new Exception($@"Can not apply shader of type ${GetType()}, check params");
            }
        }
    }
}
