//
//      coded by un
//            --------------
//                     mindshifter.com
//

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mindshifter
{
    public static class Primitives
    {
        /// <summary>
        /// Draws a line from the specified start position to the specified end position
        /// </summary>
        /// <param name="device">The GraphicsDevice with which to draw</param>
        /// <param name="start">The start point of the line</param>
        /// <param name="end">The end point of the line</param>
        /// <param name="transform">The matrix by which to transform each vertex of the line</param>
        public static void DrawLine(GraphicsDevice device, Vector3 start, Vector3 end, Matrix transform)
        {
            Vector3[] vtx = new Vector3[] 
            { 
                Vector3.Transform(start, transform), 
                Vector3.Transform(end, transform) 
            };

            device.DrawUserPrimitives(PrimitiveType.LineList, vtx, 0, 1);
        }

        /// <summary>
        /// Draws a line from the specified start position to the specified end position
        /// </summary>
        /// <param name="device">The GraphicsDevice with which to draw</param>
        /// <param name="start">The start point of the line</param>
        /// <param name="end">The end point of the line</param>
        public static void DrawLine(GraphicsDevice device, Vector3 start, Vector3 end)
        {
            DrawLine(device, start, end, Matrix.Identity);
        }

        /// <summary>
        /// Draws a box centered at the specified center point with the specified axis extents in both
        /// negative and positive directions, giving side lengths of (extents * 2)
        /// </summary>
        /// <param name="device">The GraphicsDevice with which to draw the box</param>
        /// <param name="center">The local space center from which to draw the box</param>
        /// <param name="extents">The local space negative and positive extents of the box</param>
        /// <param name="transform">The matrix by which to transform each vertex of the box</param>
        public static void DrawBox(GraphicsDevice device, Vector3 center, Vector3 extents, Matrix transform)
        {
            Vector3[] vtx = new Vector3[]
            {
                Vector3.Transform(center + new Vector3(-extents.X, extents.Y, extents.Z), transform),
                Vector3.Transform(center + new Vector3(extents.X, extents.Y, extents.Z), transform),
                Vector3.Transform(center + new Vector3(extents.X, -extents.Y, extents.Z), transform),
                Vector3.Transform(center + new Vector3(-extents.X, -extents.Y, extents.Z), transform),
                Vector3.Transform(center + new Vector3(-extents.X, extents.Y, -extents.Z), transform),
                Vector3.Transform(center + new Vector3(extents.X, extents.Y, -extents.Z), transform),
                Vector3.Transform(center + new Vector3(extents.X, -extents.Y, -extents.Z), transform),
                Vector3.Transform(center + new Vector3(-extents.X, -extents.Y, -extents.Z), transform),
            };

            short[] ind = new short[]
            {
                // front
                0, 1, 2,    2, 3, 0,

                // right
                1, 5, 6,    6, 2, 1,

                // back
                5, 4, 7,    7, 6, 5,

                // left
                4, 0, 3,    3, 7, 4,

                // top
                4, 5, 1,    1, 0, 4,

                // bottom
                3, 2, 6,    6, 7, 3,
            };

            device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vtx, 0, 8, ind, 0, 12);
        }

        /// <summary>
        /// Draws a box centered at the specified center point with the specified axis extents in both
        /// negative and positive directions, giving side lengths of (extents * 2)
        /// </summary>
        /// <param name="device">The GraphicsDevice with which to draw the box</param>
        /// <param name="center">The local space center from which to draw the box</param>
        /// <param name="extents">The local space negative and positive extents of the box</param>
        public static void DrawBox(GraphicsDevice device, Vector3 center, Vector3 extents)
        {
            DrawBox(device, center, extents, Matrix.Identity);
        }

        /// <summary>
        /// Draws a cone with a base parallel to the XZ plane centered at the specified center point
        /// and a cone tip 'height' units above the base on the Y axis
        /// </summary>
        /// <param name="device">The device with which to draw the cone</param>
        /// <param name="center">The local space origin point of the base of the cone</param>
        /// <param name="radius">The radius of the base of the cone</param>
        /// <param name="height">The height from the base of the cone to the tip of the cone</param>
        /// <param name="segments">The number of segments with which to construct the base of the cone</param>
        /// <param name="transform">The matrix by which to transform each vertex of the cone</param>
        public static void DrawCone(GraphicsDevice device, Vector3 center, float radius, 
            float height, int segments, Matrix transform)
        {
            float ival = MathHelper.TwoPi / (float)segments;

            Vector3[] vtx = new Vector3[segments + 2];
            vtx[0] = Vector3.Transform(center + (Vector3.UnitY * height), transform);

            for (int i = 0; i < segments; ++i)
            {
                float rot = (float)i * ival;

                float a = radius * (float)Math.Cos(rot);
                float b = radius * (float)Math.Sin(rot);

                vtx[i + 1] = Vector3.Transform(center + new Vector3(a, 0, b), transform);
            }

            vtx[segments + 1] = vtx[1];

            device.DrawUserPrimitives(PrimitiveType.TriangleFan, vtx, 0, segments);
        }

        /// <summary>
        /// Draws a cone with a base parallel to the XZ plane centered at the specified center point
        /// and a cone tip 'height' units above the base on the Y axis
        /// </summary>
        /// <param name="device">The device with which to draw the cone</param>
        /// <param name="center">The local space origin point of the base of the cone</param>
        /// <param name="radius">The radius of the base of the cone</param>
        /// <param name="height">The height from the base of the cone to the tip of the cone</param>
        /// <param name="segments">The number of segments with which to construct the base of the cone</param>
        public static void DrawCone(GraphicsDevice device, Vector3 center, float radius,
            float height, int segments)
        {
            DrawCone(device, center, radius, height, segments, Matrix.Identity);
        }

        /// <summary>
        /// Draws a cylinder centered at the specified center point parallel with the Y axis, 
        /// with the specified radius and specified extents in both the negative and positive Y 
        /// directions resulting in a length of (extents * 2).
        /// </summary>
        /// <param name="device">The device with which to draw the cylinder</param>
        /// <param name="center">The local space center point of the cylinder</param>
        /// <param name="extents">The extents (half length) of the cylinder</param>
        /// <param name="radius">The radius of the cylinder</param>
        /// <param name="segments">The number of segments with which to construct the cylinder</param>
        /// <param name="transform">The matrix by which to transform each vertex of the cylinder</param>
        public static void DrawCylinder(GraphicsDevice device, Vector3 center, float extents, float radius, 
            int segments, Matrix transform)
        {
            float ival = MathHelper.TwoPi / (float)segments;

            Vector3[] vtx = new Vector3[(segments + 1) << 1];
            
            for (int i = 0; i < ((segments + 1) << 1); i += 2)
            {
                float rot = (float)i * ival;

                float a = radius * (float)Math.Cos(rot);
                float b = radius * (float)Math.Sin(rot);

                vtx[i + (((i % 2) == 0) ? (0) : (1))] 
                    = Vector3.Transform(center + new Vector3(a, extents, b), transform);
                vtx[i + (((i % 2) == 0) ? (1) : (0))] 
                    = Vector3.Transform(center + new Vector3(a, -extents, b), transform);
            }

            device.DrawUserPrimitives(PrimitiveType.TriangleStrip, vtx, 0, (segments) << 1);
        }

        /// <summary>
        /// Draws a cylinder centered at the specified center point parallel with the Y axis, 
        /// with the specified radius and specified extents in both the negative and positive Y 
        /// directions resulting in a length of (extents * 2).
        /// </summary>
        /// <param name="device">The device with which to draw the cylinder</param>
        /// <param name="center">The local space center point of the cylinder</param>
        /// <param name="extents">The extents (half length) of the cylinder</param>
        /// <param name="radius">The radius of the cylinder</param>
        /// <param name="segments">The number of segments with which to construct the cylinder</param>
        public static void DrawCylinder(GraphicsDevice device, Vector3 center, float extents, float radius,
            int segments)
        {
            DrawCylinder(device, center, extents, radius, segments, Matrix.Identity);
        }

        /// <summary>
        /// Draws a sphere centered at the specified center point with the specified radius
        /// </summary>
        /// <param name="device">The device with which to draw the sphere</param>
        /// <param name="center">The local space center point of the sphere</param>
        /// <param name="radius">The radius of the sphere</param>
        /// <param name="phi_seg">The phi (polar declination) segments of the sphere (180 degrees divided by segments)</param>
        /// <param name="theta_seg">The theta (polar azimuth) segments of the sphere (360 degrees divided by segments)</param>
        /// <param name="transform">The matrix by which to transform each vertex of the sphere</param>
        public static void DrawSphere(GraphicsDevice device, Vector3 center, float radius, int phi_seg, 
            int theta_seg, Matrix transform)
        {
            float delta_phi = MathHelper.Pi / (float)phi_seg;
            float delta_theta = MathHelper.TwoPi / (float)theta_seg;

            int num_vtx = phi_seg * theta_seg * 4;

            Vector3[] vtx = new Vector3[num_vtx];
            int[] ind = new int[(phi_seg * theta_seg) * 6];

            int ivtx = 0;
            int iind = 0;
            float phi = 0;
            float theta = 0;

            for (int pseg = 0; pseg < phi_seg; pseg++, phi += delta_phi)
            {
                for (int tseg = 0; tseg < theta_seg; tseg++, theta += delta_theta)
                {
                    float cphi = (float)Math.Cos(phi);
                    float sphi = (float)Math.Sin(phi);
                    float cphi_d = (float)Math.Cos(phi + delta_phi);
                    float sphi_d = (float)Math.Sin(phi + delta_phi);
                    float ctheta = (float)Math.Cos(theta);
                    float stheta = (float)Math.Sin(theta);
                    float ctheta_d = (float)Math.Cos(theta + delta_theta);
                    float stheta_d = (float)Math.Sin(theta + delta_theta);

                    vtx[ivtx].X = radius * sphi * ctheta;
                    vtx[ivtx].Z = radius * sphi * stheta;
                    vtx[ivtx].Y = radius * cphi;
                    vtx[ivtx] += center;

                    vtx[ivtx] = Vector3.Transform(vtx[ivtx], transform);
                    ivtx++;

                    vtx[ivtx].X = radius * sphi_d * ctheta;
                    vtx[ivtx].Z = radius * sphi_d * stheta;
                    vtx[ivtx].Y = radius * cphi_d;
                    vtx[ivtx] += center;

                    vtx[ivtx] = Vector3.Transform(vtx[ivtx], transform);
                    ivtx++;

                    vtx[ivtx].X = radius * sphi_d * ctheta_d;
                    vtx[ivtx].Z = radius * sphi_d * stheta_d;
                    vtx[ivtx].Y = radius * cphi_d;
                    vtx[ivtx] += center;

                    vtx[ivtx] = Vector3.Transform(vtx[ivtx], transform);
                    ivtx++;

                    vtx[ivtx].X = radius * sphi * ctheta_d;
                    vtx[ivtx].Z = radius * sphi * stheta_d;
                    vtx[ivtx].Y = radius * cphi;
                    vtx[ivtx] += center;

                    vtx[ivtx] = Vector3.Transform(vtx[ivtx], transform);
                    ivtx++;

                    ind[iind++] = ivtx - 4;
                    ind[iind++] = ivtx - 3;
                    ind[iind++] = ivtx - 2;
                    ind[iind++] = ivtx - 2;
                    ind[iind++] = ivtx - 1;
                    ind[iind++] = ivtx - 4;
                }
            }

            device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vtx, 0, num_vtx, 
                ind, 0, phi_seg * theta_seg * 2);
        }

        /// <summary>
        /// Draws a sphere centered at the specified center point with the specified radius
        /// </summary>
        /// <param name="device">The device with which to draw the sphere</param>
        /// <param name="center">The local space center point of the sphere</param>
        /// <param name="radius">The radius of the sphere</param>
        /// <param name="phi_seg">The phi (polar declination) segments of the sphere (180 degrees divided by segments)</param>
        /// <param name="theta_seg">The theta (polar azimuth) segments of the sphere (360 degrees divided by segments)</param>
        public static void DrawSphere(GraphicsDevice device, Vector3 center, float radius, int phi_seg,
            int theta_seg)
        {
            DrawSphere(device, center, radius, phi_seg, theta_seg, Matrix.Identity);
        }

        /// <summary>
        /// Draws a circle strip parallel to the XZ plane around the specified center point with
        /// the specified inner and outer radii
        /// </summary>
        /// <param name="device">The GraphicsDevice with which to draw the circle strip</param>
        /// <param name="center">The local space center point around which to draw the circle strip</param>
        /// <param name="inner_radius">The inner radius of the circle strip</param>
        /// <param name="outer_radius">The outer radius of the circle strip</param>
        /// <param name="segments">The number of segments with which to draw the circle strip</param>
        /// <param name="transform">The matrix by which to transform each vertex of the circle strip</param>
        public static void DrawCircleStrip(GraphicsDevice device, Vector3 center, float inner_radius, 
            float outer_radius, int segments, Matrix transform)
        {
            float ival = MathHelper.TwoPi / segments;

            Vector3[] vtx = new Vector3[((segments + 1) << 1)];

            for (int i = 0; i < (segments + 1) << 1; i += 2)
            {
                float rot = (float)i * ival;

                float a_i = inner_radius * (float)Math.Cos(rot);
                float b_i = inner_radius * (float)Math.Sin(rot);

                float a_o = outer_radius * (float)Math.Cos(rot);
                float b_o = outer_radius * (float)Math.Sin(rot);

                vtx[i + (((i % 2) == 0) ? (0) : (1))] 
                    = Vector3.Transform(center + new Vector3(a_i, 0, b_i), transform);
                vtx[i + (((i % 2) == 0) ? (1) : (0))] 
                    = Vector3.Transform(center + new Vector3(a_o, 0, b_o), transform);
            }

            device.DrawUserPrimitives(PrimitiveType.TriangleStrip, vtx, 0, (segments) << 1);
        }

        /// <summary>
        /// Draws a circle strip parallel to the XZ plane around the specified center point with
        /// the specified inner and outer radii
        /// </summary>
        /// <param name="device">The GraphicsDevice with which to draw the circle strip</param>
        /// <param name="center">The local space center point around which to draw the circle strip</param>
        /// <param name="inner_radius">The inner radius of the circle strip</param>
        /// <param name="outer_radius">The outer radius of the circle strip</param>
        /// <param name="segments">The number of segments with which to draw the circle strip</param>
        public static void DrawCircleStrip(GraphicsDevice device, Vector3 center, float inner_radius,
            float outer_radius, int segments)
        {
            DrawCircleStrip(device, center, inner_radius, outer_radius, segments, Matrix.Identity);
        }

        /// <summary>
        /// Draws a quad centered at the specified center point parallel to the XZ axis with the
        /// specified extents along the positive and negative X and Z axes resulting in side lengths
        /// of (extents * 2)
        /// </summary>
        /// <param name="device">The device with which to draw the quad</param>
        /// <param name="center">The local space center point at which to draw the quad</param>
        /// <param name="extents">The extents of the quad's sides on the positive and negative X and Z axes</param>
        /// <param name="transform">The matrix by which to transform each vertex of the quad</param>
        public static void DrawQuad(GraphicsDevice device, Vector3 center, Vector2 extents, Matrix transform)
        {
            Vector3[] vtx = new Vector3[]
            {
                Vector3.Transform(center + new Vector3(-extents.X, 0, -extents.Y), transform),
                Vector3.Transform(center + new Vector3(extents.X, 0, -extents.Y), transform),
                Vector3.Transform(center + new Vector3(extents.X, 0, extents.Y), transform),
                Vector3.Transform(center + new Vector3(-extents.X, 0, extents.Y), transform),
            };

            short[] ind = new short[] { 0, 1, 2, 2, 3, 0};

            device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vtx, 0, 4, ind, 0, 2);
        }

        /// <summary>
        /// Draws a quad centered at the specified center point parallel to the XZ axis with the
        /// specified extents along the positive and negative X and Z axes resulting in side lengths
        /// of (extents * 2)
        /// </summary>
        /// <param name="device">The device with which to draw the quad</param>
        /// <param name="center">The local space center point at which to draw the quad</param>
        /// <param name="extents">The extents of the quad's sides on the positive and negative X and Z axes</param>
        public static void DrawQuad(GraphicsDevice device, Vector3 center, Vector2 extents)
        {
            DrawQuad(device, center, extents, Matrix.Identity);
        }

        /// <summary>
        /// Draws a quad defined by the specified four vertices
        /// </summary>
        /// <param name="device">The device with which to draw the quad</param>
        /// <param name="vertex_one">The first vertex of the quad</param>
        /// <param name="vertex_two">The second vertex of the quad</param>
        /// <param name="vertex_three">The third vertex of the quad</param>
        /// <param name="vertex_four">The fourth vertex of the quad</param>
        /// <param name="transform">The matrix by which to transform each vertex of the quad</param>
        public static void DrawQuad(GraphicsDevice device, Vector3 vertex_one, Vector3 vertex_two,
            Vector3 vertex_three, Vector3 vertex_four, Matrix transform)
        {
            Vector3[] vtx = new Vector3[]
            {
                Vector3.Transform(vertex_one, transform),
                Vector3.Transform(vertex_two, transform),
                Vector3.Transform(vertex_three, transform),
                Vector3.Transform(vertex_four, transform),
            };

            short[] ind = new short[] { 0, 1, 2, 2, 3, 0 };

            device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vtx, 0, 4, ind, 0, 2);
        }

        /// <summary>
        /// Draws a quad defined by the specified four vertices
        /// </summary>
        /// <param name="device">The device with which to draw the quad</param>
        /// <param name="vertex_one">The first vertex of the quad</param>
        /// <param name="vertex_two">The second vertex of the quad</param>
        /// <param name="vertex_three">The third vertex of the quad</param>
        /// <param name="vertex_four">The fourth vertex of the quad</param>
        public static void DrawQuad(GraphicsDevice device, Vector3 vertex_one, Vector3 vertex_two,
            Vector3 vertex_three, Vector3 vertex_four)
        {
            DrawQuad(device, vertex_one, vertex_two, vertex_three, vertex_four, Matrix.Identity);
        }

        /// <summary>
        /// Draws a triangle defined by the specified three vertices
        /// </summary>
        /// <param name="device">The device with which to draw the triangle</param>
        /// <param name="vertex_one">The first vertex of the triangle</param>
        /// <param name="vertex_two">The second vertex of the triangle</param>
        /// <param name="vertex_three">The third vertex of the triangle</param>
        /// <param name="transform">The matrix by which to transform each vertex of the triangle</param>
        public static void DrawTriangle(GraphicsDevice device, Vector3 vertex_one, Vector3 vertex_two, 
            Vector3 vertex_three, Matrix transform)
        {
            Vector3[] vtx = new Vector3[]
            {
                Vector3.Transform(vertex_one, transform),
                Vector3.Transform(vertex_two, transform),
                Vector3.Transform(vertex_three, transform),
            };

            device.DrawUserPrimitives(PrimitiveType.TriangleList, vtx, 0, 1);
        }

        /// <summary>
        /// Draws a triangle defined by the specified three vertices
        /// </summary>
        /// <param name="device">The device with which to draw the triangle</param>
        /// <param name="vertex_one">The first vertex of the triangle</param>
        /// <param name="vertex_two">The second vertex of the triangle</param>
        /// <param name="vertex_three">The third vertex of the triangle</param>
        public static void DrawTriangle(GraphicsDevice device, Vector3 vertex_one, Vector3 vertex_two,
            Vector3 vertex_three)
        {
            DrawTriangle(device, vertex_one, vertex_two, vertex_three, Matrix.Identity);
        }
    }
}