using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Caffeinated3D.Rendering;

namespace Caffeinated3D.Primitives
{
    public class PlaneMesh
    {
        private GraphicsDevice _graphicsDevice;
        private EffectManagerC3D _effectManager;

        //shader fallback or for debugging
        private BasicEffect _basicEffect;
        private int _width;
        private int _height;
        private Vector3? _origin;

        private VertexPosition[] _vertices;
        private VertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer; 

        public PlaneMesh(GraphicsDevice graphicsDevice, int width, int height, Vector3? origin) 
        {
            _graphicsDevice = graphicsDevice;
            _effectManager = new EffectManagerC3D();
            _width = width;
            _height = height;
            _origin = origin;

            _basicEffect = new BasicEffect(_graphicsDevice);
        }

        public VertexPosition[] GenerateVertices()
        {
            VertexPosition[] vertices = new VertexPosition[_width * _height];

            int startingX = 0;
            int startingZ = 0;
            int counter = 0;
            for (int i = 0; i < _width; i++)
            {
                for (int j = 0; j < _height; j++)
                {
                    vertices[counter] = new VertexPosition(new Vector3(startingX, 0, startingZ));
                    startingZ++;
                    counter++;
                }
                startingZ = 0;
                startingX++;
            }

            return vertices;
        }

        public short[] GenerateIndices()
        {
            short[] indices = new short[(_width - 1) * (_height - 1) * 6];
            short current = 0;

            for (int y = 0; y < _height - 1; y++)
            {
                for (int x = 0; x < _width - 1; x++)
                {
                    short bl = (short)((y * _width) + x);
                    short br = (short)(bl + 1);
                    short tl = (short)(((y + 1) * _width) + x);
                    short tr = (short)(tl + 1);

                    indices[current++] = bl;
                    indices[current++] = tl;
                    indices[current++] = br;

                    indices[current++] = br;
                    indices[current++] = tl;
                    indices[current++] = tr;
                }
            }

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
            _vertexBuffer = new VertexBuffer(_graphicsDevice, typeof(VertexPosition), _vertices.Length, BufferUsage.WriteOnly);
            _vertexBuffer.SetData<VertexPosition>(_vertices);
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
                _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _width * _height / 2);
            }
        }

        public void AddEffect(EffectC3D effect)
        {
            _effectManager.AddEffect(effect);
        }
    }
}
