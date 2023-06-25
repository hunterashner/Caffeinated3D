using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Caffeinated3D.Rendering
{
    /// <summary>
    /// Convex hull used for getting the bounds of a vertex buffer array
    /// and for triangulating the vertices of said array.
    /// </summary>
    public class ConvexHull
    {
        public static List<Vector3> ComputeConvexHull(List<Vector3> points)
        {
            if (points.Count < 3)
                throw new ArgumentException("At least three points are required to compute the Convex Hull.");

            // Find the point with the lowest y-coordinate (and leftmost x-coordinate in case of a tie)
            Vector3 startPoint = points.Aggregate((p1, p2) =>
            {
                if (p1.Z < p2.Z || (p1.Z == p2.Z && p1.X < p2.X))
                    return p1;
                return p2;
            });

            // Sort the points based on their polar angle with respect to the startPoint
            List<Vector3> sortedPoints = points.OrderBy(p => GetPolarAngle(startPoint, p)).ToList();

            // Create a stack to store the points of the Convex Hull
            Stack<Vector3> convexHull = new Stack<Vector3>();
            convexHull.Push(sortedPoints[0]);
            convexHull.Push(sortedPoints[1]);

            for (int i = 2; i < sortedPoints.Count; i++)
            {
                while (convexHull.Count > 1 && !IsLeftTurn(convexHull.ElementAt(1), convexHull.Peek(), sortedPoints[i]))
                {
                    convexHull.Pop();
                }

                convexHull.Push(sortedPoints[i]);
            }

            return convexHull.Reverse().ToList();
        }

        public static List<Vector3> TriangulateConvexHull(List<Vector3> convexHull)
        {
            List<Vector3> triangles = new List<Vector3>();

            if (convexHull.Count < 3)
                throw new ArgumentException("Convex Hull must have at least three points to perform triangulation.");

            int n = convexHull.Count;

            if (n == 3)
            {
                triangles.AddRange(convexHull);
                return triangles;
            }

            int[] indices = new int[n];

            for (int i = 0; i < n; i++)
            {
                indices[i] = i;
            }

            int currentIndex = 0;
            int nextIndex = 0;
            int prevIndex = 0;

            while (n > 3)
            {
                prevIndex = (currentIndex + n - 1) % n;
                nextIndex = (currentIndex + 1) % n;

                if (IsEar(convexHull, prevIndex, currentIndex, nextIndex))
                {
                    triangles.Add(convexHull[prevIndex]);
                    triangles.Add(convexHull[currentIndex]);
                    triangles.Add(convexHull[nextIndex]);

                    // Remove the ear tip from the polygon
                    convexHull.RemoveAt(currentIndex);
                    indices = indices.Where((val, idx) => idx != currentIndex).ToArray();
                    n--;
                }
                else
                {
                    currentIndex = (currentIndex + 1) % n;
                }
            }

            // Add the last triangle
            //triangles.Add(convexHull[indices[0]]);
            //triangles.Add(convexHull[indices[1]]);
            //triangles.Add(convexHull[indices[2]]);

            return triangles;
        }

        private static float GetPolarAngle(Vector3 referencePoint, Vector3 targetPoint)
        {
            return (float)MathF.Atan2(targetPoint.Z - referencePoint.Z, targetPoint.X - referencePoint.X);
        }

        private static bool IsLeftTurn(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            return ((p2.X - p1.X) * (p3.Z - p1.Z) - (p3.X - p1.X) * (p2.Z - p1.Z)) > 0;
        }

        private static bool IsEar(List<Vector3> polygon, int prevIndex, int currentIndex, int nextIndex)
        {
            Vector3 p1 = polygon[prevIndex];
            Vector3 p2 = polygon[currentIndex];
            Vector3 p3 = polygon[nextIndex];

            for (int i = 0; i < polygon.Count; i++)
            {
                if (i != prevIndex && i != currentIndex && i != nextIndex)
                {
                    if (PointInTriangle(polygon[i], p1, p2, p3))
                        return false;
                }
            }

            return true;
        }

        private static bool PointInTriangle(Vector3 point, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            bool b1 = IsLeftTurn(p1, p2, point);
            bool b2 = IsLeftTurn(p2, p3, point);
            bool b3 = IsLeftTurn(p3, p1, point);

            return (b1 == b2) && (b2 == b3);
        }

        //public static void Main(string[] args)
        //{
        //    // Example usage
        //    List<Vector3> points = new List<Vector3>
        //    {
        //        new Vector3(1, 1),
        //        new Vector3(2, 3),
        //        new Vector3(4, 2),
        //        new Vector3(3, 1),
        //        new Vector3(5, 1),
        //        new Vector3(3, 4),
        //        new Vector3(2, 5)
        //    };

        //    List<Vector3> convexHull = ComputeConvexHull(points);
        //    List<Vector3> triangles = TriangulateConvexHull(convexHull);

        //    Console.WriteLine("Convex Hull Points:");
        //    foreach (Vector3 point in convexHull)
        //    {
        //        Console.WriteLine(point);
        //    }

        //    Console.WriteLine("\nTriangulated Points:");
        //    foreach (Vector3 point in triangles)
        //    {
        //        Console.WriteLine(point);
        //    }
        //}
    }
}
