using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caffeinated3D
{
    public class C3DObjectEffectManager
    {
        private List<IEffectC3D> _effects;
        public C3DObjectEffectManager()
        {
            _effects = new List<IEffectC3D>();
        }

        public void AddEffect(IEffectC3D effect)
        {
            _effects.Add(effect);
        }

        public void RemoveEffect(IEffectC3D effect)
        {
            _effects.Remove(effect);
        }

        public void ApplyEffects()
        {
            foreach(var effect in _effects)
            {
                effect.ApplyEffect();
            }
        }
    }
}
