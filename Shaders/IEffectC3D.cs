using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Caffeinated3D.Shaders
{
    public interface IEffectC3D
    {
        Effect Shader { get; set; }
        void SetEffectParams(Matrix world, Matrix view, Matrix projection);
    }
}
