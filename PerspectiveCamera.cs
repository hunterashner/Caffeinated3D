using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Caffeinated3D
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

        public Matrix RotationMatrix { get; set; }
        private float _cameraYaw;
        private float _cameraPitch;
        public Matrix ViewMatrix { get; private set; }
        public Matrix ProjectionMatrix { get; private set; }
        public Matrix WorldMatrix { get; private set; }
        float speed;
        float _sensitivity;

        public PerspectiveCamera()
        {
            Position = new Vector3(0, 0, -10);
            Rotation = new Vector3(0, 0, 0);
            WorldMatrix = Matrix.CreateTranslation(new Vector3(0, 0, 0));

            //basic projection setup, will abstract to user defined values when moving data to text files
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(70), 1920f / 1080f, 0.1f, 2000f);
            _cameraYaw = 0;
            _cameraPitch = 0;
            RotationMatrix = Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(_cameraYaw), MathHelper.ToRadians(_cameraPitch), MathHelper.ToRadians(0));
            Forward = Vector3.Transform(Vector3.Forward, RotationMatrix);
            Target = Position + Forward;
            ViewMatrix = Matrix.CreateLookAt(Position, Target, Vector3.Up);
            _sensitivity = 250.0f;
            speed = 12.5f;
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
            KeyboardState kstate = Keyboard.GetState();

            Vector3 newPos = Position;
            Vector3 newRotation = Rotation;

            if(gamePadState.IsConnected)
            {
                float leftThumbX = gamePadState.ThumbSticks.Left.X;
                float leftThumbY = gamePadState.ThumbSticks.Left.Y;
                float rightThumbX = gamePadState.ThumbSticks.Right.X;
                float rightThumbY = gamePadState.ThumbSticks.Right.Y;

                if (rightThumbX != 0)
                {
                    //newRotation += new Vector3(rightThumbX * speed / 5 * deltaTime, 0, 0);
                    _cameraYaw -= rightThumbX * _sensitivity * deltaTime;
                }
                if (rightThumbY != 0)
                {
                    //newRotation += new Vector3(0, rightThumbY * speed / 5 * deltaTime, 0);
                    _cameraPitch += rightThumbY * _sensitivity * deltaTime;
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
