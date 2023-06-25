using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Caffeinated3D.Rendering
{
    public class EffectManagerC3D
    {
        public List<EffectC3D> Effects = new List<EffectC3D>();

        public EffectManagerC3D() { }

        public void AddEffect(EffectC3D effect)
        {
            Effects.Add(effect);
        }

        public void RemoveEffect(EffectC3D effect)
        {
            Effects.Remove(effect);
        }

        public void SetEffectParams()
        {
            foreach(EffectC3D effect in Effects)
            {
                effect.SetEffectParameters();
            }
        }
    }
}
