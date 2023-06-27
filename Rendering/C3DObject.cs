using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Caffeinated3D.Shaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Caffeinated3D.Rendering
{
    /// <summary>
    /// Basic class for all 3D Objects with functions for drawing updating
    /// Position, Rotation, matrix, etc.  Currently used for testing of 
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

        private C3DObjectEffectManager _effectManager;


        /// <summary>
        /// Basic constructor that takes object model and a custom shader to draw with.
        /// More constructors will be added to handle different starting positions,
        /// and removing shader requirements as they can be added and removed using the
        /// newly added effect manager.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="effect"></param>
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

            _effectManager = new C3DObjectEffectManager();
        }


        /// <summary>
        /// adds new shader to list of effects that will be iterated through for this objects 
        /// draw call.
        /// </summary>
        /// <param name="effect"></param>
        public void AddEffect(IEffectC3D effect)
        {
            _effectManager.AddEffect(effect);
        }

        public void Update(GameTime gameTime, float deltaTime, float speed, Vector3 dir, Quaternion rot)
        {
            //idleobjectrotate in usage for testing
            //update scale, Rotation, and translation here
            IdleObjectRotate(deltaTime, speed, rot);
            Move(deltaTime, speed, dir);
            Matrix3x3 = UpdateMatrix(ScaleMatrix, RotationMatrix, TranslationMatrix);
        }


        /// <summary>
        /// Draws object using monogame's basicEffect class, currently used exclusively for testing
        /// draws calls vs those using custom shaders for debugging.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="world"></param>
        /// <param name="view"></param>
        /// <param name="projection"></param>
        public void DrawWithBasicEffects(GameTime gameTime, Matrix world, Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in _model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = world * Matrix3x3;
                    effect.View = view;
                    effect.Projection = projection;
                }

                mesh.Draw();
            }
        }

        /// <summary>
        /// Shader passed via effect to draw call, need to refactor to allow inheritence or batching of draws
        /// especially for objects making usage of multiple shaders or reusing shaders; ex.lighting, shadows,etc.
        /// 
        /// Totally useless draw call, only using it for testing different shaders, totally overhauling
        /// shader/effect processing and rendering.
        /// </summary>
        /// <param name="effect"></param>
        /// <param name="gameTime"></param>
        /// <param name="world"></param>
        /// <param name="view"></param>
        /// <param name="projection"></param>
        public void DrawUsingCustomEffect(Effect effect, GameTime gameTime, Matrix world, Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in _model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    //Matrix worldInverseTranspose = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform * world));
                    part.Effect = effect;
                    effect.Parameters["WorldMatrix"].SetValue(world * Matrix.CreateTranslation(Position));
                    effect.Parameters["ViewMatrix"].SetValue(view);
                    effect.Parameters["ProjectionMatrix"].SetValue(projection);
                    effect.Parameters["AmbienceColor"].SetValue(new Vector4(0.1f, 0.2f, 0.7f, 1.0f));
                    effect.Parameters["WorldInverseTransposeMatrix"].SetValue(Matrix.Invert(Matrix.Transpose(world)));
                    effect.Parameters["DiffuseLightDirection"].SetValue(new Vector3(-1.0f, 0.0f, 0.0f));
                    effect.Parameters["DiffuseColor"].SetValue(new Vector4(1.0f, 1.0f, 1.0f, 1.0f));
                }

                mesh.Draw();
            }
        }


        /// <summary>
        /// Calls drawing for the 3D objects effect manager
        /// </summary>
        public void DrawWithEffectManager()
        {

        }

        /// <summary>
        /// Updates objects vec3 Position and translation matrix
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <param name="speed"></param>
        /// <param name="direction"></param>
        void Move(float deltaTime, float speed, Vector3 direction)
        {
            Position += direction * speed * deltaTime;
            TranslationMatrix = Matrix.CreateTranslation(Position);
        }

        /// <summary>
        /// Test method, applies a linear Rotation along the y axis to 3D object
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <param name="speed"></param>
        /// <param name="rot"></param>
        void IdleObjectRotate(float deltaTime, float speed, Quaternion rot)
        {
            Rotation += rot * speed * deltaTime;
            RotationMatrix = Matrix.CreateRotationY(MathHelper.ToRadians(Rotation.Y));
        }


        /// <summary>
        /// Updates the entire matrix 3x3 for the 3D object for easy usage
        /// when passing to shaders, takes a scalar matrix, rotational matrix,
        /// and translation matrix and multiplies them in the correct order
        /// </summary>
        /// <param name="scale"></param>
        /// <param name="rot"></param>
        /// <param name="pos"></param>
        /// <returns>Returns the updated matrix3x3 of the 3d object</returns>
        public static Matrix UpdateMatrix(Matrix scale, Matrix rot, Matrix pos)
        {
            Matrix m3x3;

            m3x3 = scale * rot * pos;
            return m3x3;
        }

        /// <summary>
        /// Takes a quaternion and creates a Rotation matrix based on the
        /// quaternion values
        /// considering moving static methods to helper util
        /// </summary>
        /// <param name="rotation"></param>
        /// <returns>Returns a Rotation matrix</returns>
        public static Matrix UpdateRotationMatrix(Quaternion rotation)
        {
            Matrix rotMatrix;

            rotMatrix = Matrix.CreateRotationX(MathHelper.ToRadians(rotation.X)) *
                        Matrix.CreateRotationY(MathHelper.ToRadians(rotation.Y)) *
                        Matrix.CreateRotationZ(MathHelper.ToRadians(rotation.Z));

            return rotMatrix;
        }

        /// <summary>
        /// Takes a vector3 and creates a Rotation matrix based on the
        /// vector3 values
        /// considering moving static methods to helper util
        /// </summary>
        /// <param name="rotation"></param>
        /// <returns>Returns a Rotation matrix</returns>
        public static Matrix UpdateRotationMatrix(Vector3 rotation)
        {
            Matrix rotMatrix;

            rotMatrix = Matrix.CreateRotationX(MathHelper.ToRadians(rotation.X)) *
                        Matrix.CreateRotationY(MathHelper.ToRadians(rotation.Y)) *
                        Matrix.CreateRotationZ(MathHelper.ToRadians(rotation.Z));

            return rotMatrix;
        }
    }
}
