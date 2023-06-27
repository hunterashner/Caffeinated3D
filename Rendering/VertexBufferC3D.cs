using Caffeinated3D.Shaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Caffeinated3D.Rendering
{
    public class VertexBufferC3D
    {
        GraphicsDevice _graphicsDevice;
        private C3DObjectEffectManager _shaderManager;
        private VertexBuffer _vertexBuffer;
        private IEffectC3D _shader;
        private Matrix _world;
        private Matrix _view;
        private Matrix _projection;

        public VertexBufferC3D(
            GraphicsDevice graphicsDevice,
            C3DObjectEffectManager shaderManager,
            IEffectC3D shader)
        {
            _graphicsDevice = graphicsDevice;
            _shaderManager = shaderManager;
            _shader = shader;

            _shaderManager.AddEffect(shader);
        }

        public Vector3[] GeneratePrimitiveTriangle(Point origin)
        {
            Vector3[] triangleVertices = new Vector3[3];

            triangleVertices[0] = new Vector3(origin.X - 10, 0, 0);
            triangleVertices[1] = new Vector3(origin.X + 10, 0, 0);
            triangleVertices[2] = new Vector3(0, origin.Y + 10, 0);

            return triangleVertices;
        }

        public VertexPosition[] GenerateVertexPositions(int amount)
        {
            VertexPosition[] vertexPositions = new VertexPosition[amount];

            return vertexPositions;
        }


        public VertexBuffer GenerateTriangleVertices(Color color, Point point)
        {
            VertexPositionColor[] vertices = new VertexPositionColor[3];

            //using Vector3s instead of vertexPositions... same thing?
            //VertexPosition[] positions = GenerateVertexPositions(amount);
            Vector3[] positions = GeneratePrimitiveTriangle(point);


            for (int i = 0; i < 3; i++)
            {
                vertices[i] = new VertexPositionColor(positions[i], color);
            }

            VertexBuffer buffer;
            buffer = new VertexBuffer(_graphicsDevice, typeof(VertexPositionColor), 3, BufferUsage.WriteOnly);
            buffer.SetData<VertexPositionColor>(vertices);
            return buffer;
        }

        public void Draw(VertexBuffer buffer, Matrix world, Matrix view, Matrix projection)
        {
            //apply parameters to each shader in list
            _shaderManager.SetEffectParams(world, view, projection);

            //bind buffer and choose draw options
            _graphicsDevice.SetVertexBuffer(buffer);
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            _graphicsDevice.RasterizerState = rasterizerState;

            foreach (EffectPass pass in _shader.Shader.CurrentTechnique.Passes)
            {
                pass.Apply();
                _graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 1);
            }
        }
    }

}
