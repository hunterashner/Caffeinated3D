using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Caffeinated3D.Rendering;
using System;

namespace Caffeinated3D.Primitives
{
    /// <summary>
    /// Creates a primitive plane of width vertices wide, and height vertices tall
    /// </summary>
    public class PlaneMesh
    {
        private GraphicsDevice _graphicsDevice;
        private EffectManagerC3D _effectManager;

        //shader fallback or for debugging
        private BasicEffect _basicEffect;

        private int _width;
        private int _height;
        private Vector3? _origin;

        private VertexPositionNormalTexture[] _vertices;
        private VertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;
        private float _indiceCount;

        public PlaneMesh(GraphicsDevice graphicsDevice, int width, int height, Vector3? origin, EffectC3D effectC3D) 
        {
            _graphicsDevice = graphicsDevice;
            _effectManager = new EffectManagerC3D();

            _width = width;
            _height = height;
            _origin = origin;

            _basicEffect = new BasicEffect(_graphicsDevice);
        }


        /// <summary>
        /// Generates equidistant vertices for a flat plane width wide and height high.
        /// </summary>
        /// <returns>Array of vertex positions for entire plane.</returns>
        public VertexPositionNormalTexture[] GenerateVertices()
        {
            VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[_width * _height];

            /*
             * wrap factor is the amount of uv's to generate when generating vertices
             * this is helpful when passing a texture that is intended to wrapped
             * and not stretch to cover the entire plane. refactor later with custom
             * functions for wrapping.
             */
            int wrapFactor = 64;
            int startingX = 0;
            int startingZ = 0;
            int counter = 0;
            for (int i = 0; i < _width; i++)
            {
                for (int j = 0; j < _height; j++)
                {
                    vertices[counter] = new VertexPositionNormalTexture(
                        new Vector3(startingX, 0, startingZ),
                        Vector3.Up,
                        new Vector2((float)i / (_width - 1) * wrapFactor, (float)j / (_height -1) * wrapFactor));

                    startingZ++;
                    counter++;
                }
                startingZ = 0;
                startingX++;
            }

            return vertices;
        }

        /// <summary>
        /// Generates indices for each triangle on the plane
        /// </summary>
        ///// <returns>short array of</returns>
        public short[] GenerateIndices()
        {
            long overflowDebug = 0;
            short[] indices = new short[(_width - 1) * (_height - 1) * 6];
            long current = 0;

            for (int y = 0; y < _height - 1; y++)
            {
                for (int x = 0; x < _width - 1; x++)
                {
                    short bl = (short)((y * _width) + x);
                    short br = (short)(bl + 1);
                    short tl = (short)(((y + 1) * _width) + x);
                    short tr = (short)(tl + 1);

                    indices[current++] = tl;
                    indices[current++] = bl;
                    indices[current++] = br;

                    indices[current++] = tl;
                    indices[current++] = br;
                    indices[current++] = tr;
                }
            }

            _indiceCount = indices.Length;
            return indices;
        }

        public void SetIndexBufferData()
        {
            short[] indices = GenerateIndices();
            _indexBuffer = new IndexBuffer(_graphicsDevice, typeof(short), indices.Length, BufferUsage.WriteOnly);
            _indexBuffer.SetData(indices);
        }

        public void SetVertexBufferData()
        {
            _vertices = GenerateVertices();
            _vertexBuffer = new VertexBuffer(_graphicsDevice, typeof(VertexPositionNormalTexture), _vertices.Length, BufferUsage.WriteOnly);
            _vertexBuffer.SetData<VertexPositionNormalTexture>(_vertices);
        }

        public void Draw()
        {
            SetVertexBufferData();
            SetIndexBufferData();

            _graphicsDevice.SetVertexBuffer(_vertexBuffer);
            _graphicsDevice.Indices = _indexBuffer;

            RasterizerState rState = new RasterizerState();
            rState.CullMode = CullMode.None;
            _graphicsDevice.RasterizerState = rState;

            _effectManager.SetEffectParams();

            foreach(Effect effect in _effectManager.Effects)
            {
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _width * _height / 2);
                }
            }
        }

        public void DrawWireframeWithBasicEffect(GameTime gameTime, Matrix world, Matrix view, Matrix projection)
        {
            SetVertexBufferData();
            SetIndexBufferData();

            _basicEffect.World = world;
            _basicEffect.View = view;
            _basicEffect.Projection = projection;
            _basicEffect.VertexColorEnabled = true;

            _graphicsDevice.SetVertexBuffer(_vertexBuffer);
            _graphicsDevice.Indices = _indexBuffer;

            RasterizerState rState = new RasterizerState();
            rState.CullMode = CullMode.None;
            rState.FillMode = FillMode.WireFrame;
            _graphicsDevice.RasterizerState = rState;

            foreach(EffectPass pass in _basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, (int)_indiceCount);
            }
        }

        public void DrawWithEffectManager(GameTime gameTime, Matrix world, Matrix view, Matrix projection)
        {
            SetVertexBufferData();
            SetIndexBufferData();

            _effectManager.SetEffectParams();

            _graphicsDevice.SetVertexBuffer(_vertexBuffer);
            _graphicsDevice.Indices = _indexBuffer;

            RasterizerState rState = new RasterizerState();
            rState.CullMode = CullMode.None;
            rState.FillMode = FillMode.Solid;
            _graphicsDevice.RasterizerState = rState;

            foreach (Effect effect in _effectManager.Effects) 
            {
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, (int)_indiceCount);
                }
            }
        }

        public void DrawWithEffectParam(GameTime gameTime, Matrix world, Matrix view, Matrix projection, Texture2D texture, Effect effect)
        {
            SetVertexBufferData();
            SetIndexBufferData();

            effect.Parameters["WorldMatrix"].SetValue(world);
            effect.Parameters["ViewMatrix"].SetValue(view);
            effect.Parameters["ProjectionMatrix"].SetValue(projection);
            effect.Parameters["ModelTexture"].SetValue(texture);
            //effect.Parameters["AmbienceColor"].SetValue(new Vector4(0.1f, 0.2f, 0.7f, 1.0f));
            //effect.Parameters["WorldInverseTransposeMatrix"].SetValue(Matrix.Invert(Matrix.Transpose(world)));
            //effect.Parameters["DiffuseLightDirection"].SetValue(new Vector3(-1.0f, 0.0f, 0.0f));
            //effect.Parameters["DiffuseColor"].SetValue(new Vector4(1.0f, 1.0f, 1.0f, 1.0f));
            //effect.Parameters["ModelTexture"].SetValue(texture);

            _graphicsDevice.SetVertexBuffer(_vertexBuffer);
            _graphicsDevice.Indices = _indexBuffer;

            RasterizerState rState = new RasterizerState();
            rState.CullMode = CullMode.None;
            rState.FillMode = FillMode.Solid;
            _graphicsDevice.RasterizerState = rState;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, (int)_indiceCount);
            }
        }

        public void AddEffect(EffectC3D effect)
        {
            _effectManager.AddEffect(effect);
        }
    }
}
