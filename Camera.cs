using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Caffeinated3D
{
    /// <summary>
    /// Used for main camera creation and/or direct manipulation and
    /// state of global view matrix. Camera matrix can be passed in place
    /// of view matrix
    /// </summary>
    public class Camera
    {
        public Vector3 Position { get; private set; }
        public Vector3 Target { get; private set; }
        public Matrix ViewMatrix { get; private set; }

        private float _speed;

        private float _sensitivity;
        private float _yaw;
        private float _pitch;

        private Matrix TranslationMatrix;
        private Matrix rotationMatrix;

        public Camera(Vector3? startingPos, Vector3? startingTarget)
        {
            this.Position = startingPos ?? new Vector3(0, 70, 100);
            this.Target = startingTarget ?? new Vector3(0, 0, 0);
            //this.ViewMatrix = Matrix.CreateLookAt(this.Position, this.Target, Vector3.UnitY);
            this.ViewMatrix = Matrix.CreateLookAt(this.Position, this.Target, Vector3.UnitY);

            _speed = 50.0f;
        }

        public void Update(GameTime gameTime, float deltaTime)
        {
            #region //extremely basic camera controller for testing
            KeyboardState kstate = Keyboard.GetState();
            MouseState mstate = Mouse.GetState();


            Vector3 lookAt = new Vector3(this.Position.X, 45, this.Position.Z);
            if (kstate.IsKeyDown(Keys.A))
            {
                this.Position += Vector3.Left * _speed * deltaTime;
                this.ViewMatrix = Matrix.CreateLookAt(this.Position, lookAt, Vector3.UnitY);
            }

            if (kstate.IsKeyDown(Keys.D))
            {
                this.Position += Vector3.Right * _speed * deltaTime;
                this.ViewMatrix = Matrix.CreateLookAt(this.Position, lookAt, Vector3.UnitY);
            }

            if (kstate.IsKeyDown(Keys.W))
            {
                this.Position += Vector3.Forward * _speed * deltaTime;
                this.ViewMatrix = Matrix.CreateLookAt(this.Position, lookAt, Vector3.UnitY);
            }

            if (kstate.IsKeyDown(Keys.S))
            {
                this.Position += Vector3.Backward * _speed * deltaTime;
                this.ViewMatrix = Matrix.CreateLookAt(this.Position, lookAt, Vector3.UnitY);
            }
            #endregion
        }

        public void ProcessMouseMovement(float deltaX, float deltaY)
        {
            _yaw -= deltaX * _sensitivity;
            _pitch = deltaY * _sensitivity;

            if(_pitch > 89.0f) { _pitch = 89.0f; }
            if( _pitch < -89.0f) { _pitch = -89.0f; }

            UpdateTarget();
        }

        public void UpdateTarget()
        {
            float yawRadians = MathHelper.ToRadians(_yaw);
            float pitchRadians = MathHelper.ToRadians(_pitch);

            Vector3 updatedTarget;
            updatedTarget.X = -MathF.Sin(yawRadians) * MathF.Cos(pitchRadians);
            updatedTarget.Y = MathF.Sin(pitchRadians);
            updatedTarget.Z = -MathF.Cos(yawRadians) * MathF.Cos(pitchRadians);

            this.Target = updatedTarget;
        }
    }
}
