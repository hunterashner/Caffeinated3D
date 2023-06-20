using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Caffeinated3D
{
    /// <summary>
    /// Basic class for all 3D Objects with functions for drawing updating
    /// position, rotation, matrix, etc.  Currently used for testing of 
    /// rendering passes and custom HLSL shader effects.
    /// </summary>
    public class C3DObject
    {
        public Vector3 Position { get; set; }
        public Vector3 Scale { get; set; }
        public Quaternion Rotation { get; set; }
        public Matrix TranslationMatrix { get; set; }
        public Matrix ScaleMatrix { get; set; }
        public Matrix RotationMatrix { get; set; }
        public Matrix Matrix3x3 { get; set; }

        private Model _model;
        private Effect _effect;

        public C3DObject(Model model, Effect effect)
        {
            _model = model;
            _effect = effect;
            Position = new Vector3(0, 0, 0);
            Scale = new Vector3(1, 1, 1);
            Rotation = new Quaternion(0, 0, 0, 0);

            TranslationMatrix = Matrix.CreateTranslation(Position);
            ScaleMatrix = Matrix.CreateScale(Scale);
            RotationMatrix = UpdateRotationMatrix(Rotation);

            //we will always need to multiply the world matrix by the matrix3x3 for accurate rendering
            Matrix3x3 = UpdateMatrix(ScaleMatrix, RotationMatrix, TranslationMatrix);
        }

        public void Update(GameTime gameTime, float deltaTime, float speed, Vector3 dir, Quaternion rot)
        {
            //idleobjectrotate in usage for testing
            //update scale, rotation, and translation here
            IdleObjectRotate(deltaTime, speed, rot);
            Move(deltaTime, speed, dir);
            Matrix3x3 = UpdateMatrix(ScaleMatrix, RotationMatrix, TranslationMatrix);
        }

        public void DrawWithBasicEffects(GameTime gameTime, Matrix world, Matrix view, Matrix projection)
        {
            foreach(ModelMesh mesh in _model.Meshes)
            {
                foreach(BasicEffect effect in mesh.Effects)
                {
                    effect.World = world * Matrix3x3;
                    effect.View = view;
                    effect.Projection = projection;
                }

                mesh.Draw();
            }
        }

        /*
         * Shader passed via effect to draw call, need to refactor to allow inheritence or batching of draws
         * especially for objects making usage of multiple shaders or reusing shaders; ex. lighting, shadows,etc.
         */
        public void DrawUsingCustomEffect(Effect effect, GameTime gameTime, Matrix world, Matrix view, Matrix projection)
        {
            foreach(ModelMesh mesh in _model.Meshes)
            {
                foreach(ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = effect;
                    effect.Parameters["WorldMatrix"].SetValue(world * Matrix3x3 * mesh.ParentBone.Transform);
                    effect.Parameters["ViewMatrix"].SetValue(view);
                    effect.Parameters["ProjectionMatrix"].SetValue(projection);
                    effect.Parameters["AmbienceColor"].SetValue(new Vector4(0.1f, 0.5f, 0.7f, 1.0f));
                }

                mesh.Draw();
            }
        }

        void Move(float deltaTime, float speed, Vector3 direction)
        {
            Position += direction * speed * deltaTime;
            TranslationMatrix = Matrix.CreateTranslation(Position);
        }

        void IdleObjectRotate(float deltaTime, float speed, Quaternion rot)
        {
            Rotation += rot * speed * deltaTime;
            RotationMatrix = Matrix.CreateRotationX(MathHelper.ToRadians(Rotation.X)) *
                             Matrix.CreateRotationY(MathHelper.ToRadians(Rotation.Y));
        }


        /// <summary>
        /// Updates the entire matrix 4x4 for the 3D object for easy usage
        /// when passing to shaders, takes a scalar matrix, rotational matrix,
        /// and translation matrix and multiplies them in the correct order
        /// </summary>
        /// <param name="scale"></param>
        /// <param name="rot"></param>
        /// <param name="pos"></param>
        /// <returns>Returns the updated matrix4x4 of the 3d object</returns>
        public Matrix UpdateMatrix(Matrix scale, Matrix rot, Matrix pos)
        {
            Matrix m4x4;

            m4x4 = scale * rot * pos;
            return m4x4;
        }

        /// <summary>
        /// Takes a quaternion and creates a rotation matrix based on the
        /// quaternion values
        /// </summary>
        /// <param name="rotation"></param>
        /// <returns>Returns a rotation matrix</returns>
        public Matrix UpdateRotationMatrix(Quaternion rotation)
        {
            Matrix rotMatrix;

            rotMatrix = Matrix.CreateRotationX(MathHelper.ToRadians(rotation.X)) *
                        Matrix.CreateRotationY(MathHelper.ToRadians(rotation.Y)) *
                        Matrix.CreateRotationZ(MathHelper.ToRadians(rotation.Z));

            return rotMatrix;
        }
    }
}
