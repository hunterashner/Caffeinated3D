using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Caffeinated3D
{
    public class PerspectiveCamera
    {
        Vector3 position;
        Vector3 rotation;
        public Matrix ViewMatrix { get; private set; }
        float speed;

        public PerspectiveCamera()
        {
            position = new Vector3(0, 50, 50);
            rotation = new Vector3(0, 0, 0);
            ViewMatrix = Matrix.CreateLookAt(position, rotation, Vector3.UnitY);
            speed = 50.0f;
        }

        public void Update(GameTime gameTime, float deltaTime)
        {
            #region //extremely basic camera controller for testing
            KeyboardState kstate = Keyboard.GetState();

            Vector3 newPos = position;

            if (kstate.IsKeyDown(Keys.A))
            {
                newPos += Vector3.Left * speed * deltaTime;
            }

            if (kstate.IsKeyDown(Keys.D))
            {
                newPos += Vector3.Right * speed * deltaTime;
            }

            if (kstate.IsKeyDown(Keys.W))
            {
                newPos += Vector3.Forward * speed * deltaTime;
            }

            if (kstate.IsKeyDown(Keys.S))
            {
                newPos += Vector3.Backward * speed * deltaTime;
            }

            position = newPos;

            ViewMatrix = Matrix.CreateLookAt(newPos, position + Vector3.Forward, Vector3.Up);
            #endregion
        }
    }
}
