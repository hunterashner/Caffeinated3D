using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caffeinated3D
{
    public class C3DObjectEffectManager
    {
        public List<IEffectC3D> Effects;
        public C3DObjectEffectManager()
        {
            Effects = new List<IEffectC3D>();
        }

        public void AddEffect(IEffectC3D effect)
        {
            Effects.Add(effect);
        }

        public void RemoveEffect(IEffectC3D effect)
        {
            Effects.Remove(effect);
        }

        public void SetEffectParams(Matrix world, Matrix view, Matrix projection)
        {
            foreach(var effect in Effects)
            {
                effect.SetEffectParams(world, view, projection);
            }
        }
    }
}
