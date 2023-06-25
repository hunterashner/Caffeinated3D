using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caffeinated3D.Rendering
{
    /// <summary>
    /// Custom effects, allows custom effect paramters to be set based on each unique shader/effect
    /// </summary>
    public class EffectC3D : Effect
    {
        public EffectC3D(GraphicsDevice graphicsDevice, byte[] effectCode) : base(graphicsDevice, effectCode)
        {

        }

        public EffectC3D(GraphicsDevice graphicsDevice, byte[] effectCode, int index, int count) : base(graphicsDevice, effectCode, index, count)
        {

        }

        protected EffectC3D(Effect cloneSource) : base(cloneSource)
        {

        }

        /// <summary>
        /// Try set the effect parameters otherwise throw compile time exception
        /// </summary>
        public virtual void SetEffectParameters()
        {
            //set effect parameters here
            try
            {

            }
            catch(Exception e)
            {
                throw e;
            }
        }
    }
}
