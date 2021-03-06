﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct2D1;
using System.Windows;
using System.Drawing;
using Brushes = System.Windows.Media.Brushes;
using MatrixTransform = System.Windows.Media.MatrixTransform;

namespace IronPlot
{
    /// <summary>
    /// A Direct2D Path that is plotted multiple times at different locations.
    /// In other words, this is a scatter plot.
    /// </summary>
    public class DirectPathScatter : DirectPath
    {
        private Curve curve;
        public Curve Curve { get { return curve; } set { curve = value; } }

        public double xOffsetMarker;
        public double yOffsetMarker;

        private MatrixTransform graphToCanvas;
        public MatrixTransform GraphToCanvas { get { return graphToCanvas; } set { graphToCanvas = value; } }

        public void RenderScatterGeometry(RenderTarget renderTarget)
        {
            var x = curve.x;
            var y = curve.y;
            int length = curve.PointsCount;
            double xScale, xOffset, yScale, yOffset;
            xScale = graphToCanvas.Matrix.M11;
            xOffset = graphToCanvas.Matrix.OffsetX - this.xOffsetMarker;
            yScale = graphToCanvas.Matrix.M22;
            yOffset = graphToCanvas.Matrix.OffsetY - this.yOffsetMarker;
            bool[] include = curve.includeMarker;
            StrokeStyleProperties properties = new StrokeStyleProperties();
            properties.LineJoin = LineJoin.MiterOrBevel;
            StrokeStyle strokeStyle = new StrokeStyle(renderTarget.Factory, properties);
            for (int i = 0; i < length; ++i)
            {
                if (include[i])
                {
                    renderTarget.Transform = (Matrix3x2)Matrix.Translation((float)(x[i] * xScale + xOffset), (float)(y[i] * yScale + yOffset), 0);
                    renderTarget.FillGeometry(Geometry, FillBrush);
                    renderTarget.DrawGeometry(Geometry, Brush, (float)StrokeThickness, strokeStyle);
                }
            }
            renderTarget.Transform = Matrix3x2.Identity;
        }

        public void SetGeometry(MarkersType markersType, double markersSize)
        { 
            if (Geometry != null)
            {
                Geometry.Dispose();
                Geometry = null;
            }
            if (Factory == null) return;
            float width = (float)Math.Abs(markersSize);
            float height = (float)Math.Abs(markersSize);
            this.xOffsetMarker = 0; // width / 2;
            this.yOffsetMarker = 0; // height / 2;
            Geometry = MarkerGeometriesD2D.MarkerGeometry(markersType, Factory, width, height);
           
        }
    }
}
