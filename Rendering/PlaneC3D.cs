using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Caffeinated3D.Rendering
{
    /// <summary>
    /// Create a flat primitve plane for drawing to the screen or manipulate 
    /// the plane by its vertices.  Considering adding qhull c++ dll for fancier
    /// triangulation strategies.
    /// </summary>
    public class PlaneC3D
    {
        public EffectManagerC3D EffectManager { get; set; }
        private GraphicsDevice _gd;
        private VertexBuffer _vertexBuffer;
        private Vector3 _origin;
        private Point _dimensions;

        public PlaneC3D(GraphicsDevice gd, EffectC3D shader, Vector3 origin, Point dimensions) 
        {
            _gd = gd;
            _origin = origin;
            _dimensions = dimensions;
            EffectManager= new EffectManagerC3D();
            EffectManager.AddEffect(shader);

            GenerateVertices(origin, dimensions);
            GenerateVertexBuffer(origin, dimensions);
        }


        /// <summary>
        /// Generates the vertices required by the vertex buffer to 
        /// render a flat plane in 3D space.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="dimensions"></param>
        /// <returns></returns>
        public Vector3[] GenerateVertices(Vector3 origin, Point dimensions)
        {
            Vector3[] vertices = new Vector3[dimensions.X * dimensions.Y];

            int startingX = 0;
            int startingZ = 0;
            int counter = 0;
            for (int i = 0; i < dimensions.X; i++)
            {
                for(int j = 0; j < dimensions.Y; j++)
                {
                    vertices[counter] = new Vector3(startingX, 0, startingZ);
                    startingZ++;
                    counter++;
                }
                startingZ = 0;
                startingX++;
            }

            return vertices;
        }


        /// <summary>
        /// Generates triangles... basically reorders and eliminates uneeded vertices
        /// for passing full triangle vertices to vertexbuffer
        /// </summary>
        /// <param name="vertices"></param>
        /// <returns></returns>
        public Vector3[] GenerateTriangles(Vector3[] vertices)
        {
            List<Vector3> verticesList = new List<Vector3>();
            for (int i = 0; i < vertices.Length; i++)
            {
                verticesList.Add(vertices[i]);
            }

            List<Vector3> convexHull = ConvexHull.ComputeConvexHull(verticesList);
            List<Vector3> triangles = ConvexHull.TriangulateConvexHull(convexHull);
            Vector3[] trianglesArray = new Vector3[triangles.Count];

            for (int i = 0; i < triangles.Count; i++)
            {
                trianglesArray[i] = triangles.ElementAt(i);
            }

            return trianglesArray;
        }

        /// <summary>
        /// Setup a vertexbuffer that will be bound in the draw method to 
        /// render the plane in 3D space.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="dimensions"></param>
        public void GenerateVertexBuffer(Vector3 origin, Point dimensions)
        {
            Vector3[] vertices = GenerateVertices(origin, dimensions);
            Vector3[] triangles = GenerateTriangles(vertices);

            VertexPositionColor[] vertexPositionColors = new VertexPositionColor[triangles.Length];
            
            for(int i = 0; i < vertexPositionColors.Length; i++)
            {
                vertexPositionColors[i] = new VertexPositionColor(triangles[i], Color.Pink);
            }
            
            VertexBuffer vertexBuffer = new VertexBuffer(_gd, typeof(VertexPositionColor), vertexPositionColors.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData(vertexPositionColors);
            _vertexBuffer = vertexBuffer;
        }


        /// <summary>
        /// Draw the plane with any shaders from the effect manager
        /// </summary>
        /// <param name="gameTime"></param>
        public void Draw(GameTime gameTime)
        {
            EffectManager.SetEffectParams();

            //bind buffer and choose draw options
            _gd.SetVertexBuffer(_vertexBuffer);
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            _gd.RasterizerState = rasterizerState;

            foreach(Effect effect in EffectManager.Effects)
            {
                foreach(EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    //last param needs to be altered if depending on primitive type
                    _gd.DrawPrimitives(PrimitiveType.TriangleList, 0, _vertexBuffer.VertexCount);
                }
            }
        }
    }
}
