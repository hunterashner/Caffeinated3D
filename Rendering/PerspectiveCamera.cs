using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Caffeinated3D.Rendering
{
    /// <summary>
    /// Camera abstraction of View and Projection matrices with a simple free move camera without gravity.
    /// Currently holds a World matrix for simplicity of access but does not make manipulations to the world.
    /// </summary>
    public class PerspectiveCamera
    {
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Target { get; set; }
        public Vector3 Forward { get; set; }
        public Vector3 Up { get; set; }
        public Vector3 Right { get; set; }
        public Matrix ViewMatrix { get; private set; }
        public Matrix ProjectionMatrix { get; private set; }
        public Matrix WorldMatrix { get; private set; }
        public Matrix RotationMatrix { get; set; }

        private float _cameraYaw;
        private float _cameraPitch;
        private int lastMouseX;
        private int lastMouseY;

        private float speed;
        private float _sensitivityGamepad;
        private float _sensitivityMouse;

        public PerspectiveCamera()
        {
            Position = new Vector3(50, 1, 50);
            Rotation = new Vector3(0, 0, 0);
            WorldMatrix = Matrix.CreateTranslation(new Vector3(0, 0, 0));

            //basic projection setup, will abstract to user defined values when moving data to text files
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(70), 1920f / 1080f, 0.1f, 2000f);
            _cameraYaw = 0;
            _cameraPitch = 0;
            RotationMatrix = Matrix.CreateFromYawPitchRoll(
                MathHelper.ToRadians(_cameraYaw), 
                MathHelper.ToRadians(_cameraPitch), 
                MathHelper.ToRadians(0));
            Forward = Vector3.Transform(Vector3.Forward, RotationMatrix);
            Target = Position + Forward;
            ViewMatrix = Matrix.CreateLookAt(Position, Target, Vector3.Up);
            _sensitivityGamepad = 200.0f;
            _sensitivityMouse = 10.0f;
            speed = 7.5f;
        }


        /// <summary>
        /// Basic camera controller script, currently designing most for gamepad, willupdate keyboard
        /// input soon.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="deltaTime"></param>
        public void Update(GameTime gameTime, float deltaTime)
        {
            #region //extremely basic camera controller for testing

            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            if (gamePadState.IsConnected)
            {
                //alert that controller input has been detected
            }

            Vector3 newPos = Position;
            Vector3 newRotation = Rotation;

            if (gamePadState.IsConnected)
            {
                float leftThumbX = gamePadState.ThumbSticks.Left.X;
                float leftThumbY = gamePadState.ThumbSticks.Left.Y;
                float rightThumbX = gamePadState.ThumbSticks.Right.X;
                float rightThumbY = gamePadState.ThumbSticks.Right.Y;

                if (rightThumbX != 0)
                {
                    //newRotation += new Vector3(rightThumbX * speed / 5 * deltaTime, 0, 0);
                    _cameraYaw -= rightThumbX * _sensitivityGamepad * deltaTime;
                }
                if (rightThumbY != 0)
                {
                    //newRotation += new Vector3(0, rightThumbY * speed / 5 * deltaTime, 0);
                    _cameraPitch += rightThumbY * _sensitivityGamepad * deltaTime;
                }

                _cameraYaw = (_cameraYaw + 180f) % 360f - 180f;
                _cameraPitch = MathHelper.Clamp(_cameraPitch, -89.9f, 89.9f);

                if (leftThumbX != 0)
                {
                    //newPos += new Vector3(leftThumbX * speed * deltaTime, 0, 0);
                    //newPos += Forward * Vector3.Right * speed * deltaTime;
                    newPos += Right * leftThumbX * speed * deltaTime;
                }
                if (leftThumbY != 0)
                {
                    newPos += Forward * leftThumbY * speed * deltaTime;
                }

                if (gamePadState.IsButtonDown(Buttons.A))
                {
                    newPos += new Vector3(0, 1 * speed / 4 * deltaTime, 0);
                }

                if (gamePadState.IsButtonDown(Buttons.B))
                {
                    newPos += new Vector3(0, -1 * speed / 4 * deltaTime, 0);
                }
            }
            else
            {
                KeyboardState kstate = Keyboard.GetState();
                MouseState mstate = Mouse.GetState();

                int currentMouseX = mstate.X;
                int currentMouseY = mstate.Y;

                if(currentMouseX != lastMouseX)
                {
                    int deltaX = currentMouseX - lastMouseX;
                    _cameraYaw -= deltaX * _sensitivityMouse * deltaTime;
                }

                if (currentMouseY != lastMouseY)
                {
                    int deltaY = currentMouseY - lastMouseY;
                    _cameraPitch -= deltaY * _sensitivityMouse * deltaTime;
                }

                _cameraYaw = (_cameraYaw + 180f) % 360f - 180f;
                _cameraPitch = MathHelper.Clamp(_cameraPitch, -89.9f, 89.9f);


                if (kstate.IsKeyDown(Keys.A))
                {
                    newPos += Right * Vector3.Left * speed * deltaTime;
                }

                if (kstate.IsKeyDown(Keys.D))
                {
                    newPos += Right * Vector3.Right * speed * deltaTime;
                }

                if (kstate.IsKeyDown(Keys.W))
                {
                    newPos -= Forward * Vector3.Forward * speed * deltaTime;
                }

                if (kstate.IsKeyDown(Keys.S))
                {
                    newPos -= Forward * Vector3.Backward * speed * deltaTime;
                }

                lastMouseX = currentMouseX;
                lastMouseY = currentMouseY;
            }

            Position = newPos;


            //updates all matrices accordingly to previous camera and movement inputs
            RotationMatrix = Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(_cameraYaw), MathHelper.ToRadians(_cameraPitch), MathHelper.ToRadians(0));
            Up = Vector3.Transform(Vector3.Up, RotationMatrix);
            Forward = Vector3.Transform(Vector3.Forward, RotationMatrix);
            Right = Vector3.Cross(Forward, Up);
            Target = Position + Forward;
            ViewMatrix = Matrix.CreateLookAt(newPos, Target, Up);
            #endregion
        }
    }
}
