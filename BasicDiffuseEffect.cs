using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caffeinated3D
{
    public class BasicDiffuseEffect : IEffectC3D
    {
        private bool _paramsSet;
        public void ApplyEffect()
        {
            if(!_paramsSet) 
            {
                throw new Exception(
                    $@"Can not apply shader of type ${GetType()}, check params"
                    );
            }

            //apply the effect

            /*
             * reset the parameters as they will need to be updated to reflect
             * changes to light direction, world/view matrices etc.
             * 
             */
            _paramsSet = false;
        }

        public void SetEffectParams()
        {
            try
            {
                //set effect params
            }
            catch (Exception e)
            {
                //
            }

            _paramsSet = true;
        }
    }
}
