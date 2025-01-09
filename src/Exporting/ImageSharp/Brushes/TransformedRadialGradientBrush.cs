/*
Copyright 2024 AllHailCheese

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Numerics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SwiffCheese.Exporting.ImageSharp.Brushes.Internal;

namespace SwiffCheese.Exporting.ImageSharp.Brushes;

// a modified copy of ImageSharp's RadialGradientBrush, with support for a gradient transformation and a focal point.
public sealed class TransformedRadialGradientBrush(
    PointF center,
    PointF focus,
    float radius,
    Matrix3x2 transform,
    GradientRepetitionMode repetitionMode,
    params ColorStop[] colorStops
) : GradientBrush(repetitionMode, colorStops)
{
    private readonly PointF _center = center;
    private readonly PointF _focus = focus;
    private readonly float _radius = radius;
    private readonly Matrix3x2 _inverseTransformation = Matrix3x2.Invert(transform, out Matrix3x2 inverse) ? inverse : throw new ArgumentException("Gradient transformation must be invertible");


    public override bool Equals(Brush? other)
    {
        if (other is TransformedRadialGradientBrush brush)
        {
            return base.Equals(other)
                && _center.Equals(brush._center)
                && _focus.Equals(_focus)
                && _radius.Equals(brush._radius)
                && _inverseTransformation.Equals(brush._inverseTransformation);
        }

        return false;
    }

    public override int GetHashCode()
        => HashCode.Combine(base.GetHashCode(), _center, _focus, _radius, _inverseTransformation);

    public override BrushApplicator<TPixel> CreateApplicator<TPixel>(
        Configuration configuration,
        GraphicsOptions options,
        ImageFrame<TPixel> source,
        RectangleF region) =>
        new TransformedRadialGradientBrushApplicator<TPixel>(
            configuration,
            options,
            source,
            _center,
            _focus,
            _radius,
            _inverseTransformation,
            ColorStops,
            RepetitionMode
        );

    private sealed class TransformedRadialGradientBrushApplicator<TPixel>(
        Configuration configuration,
        GraphicsOptions options,
        ImageFrame<TPixel> target,
        PointF center,
        PointF focus,
        float radius,
        Matrix3x2 inverseTransformation,
        ColorStop[] colorStops,
        GradientRepetitionMode repetitionMode
    ) : GradientBrushApplicator<TPixel>(configuration, options, target, colorStops, repetitionMode)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        private readonly PointF _center = center;
        private readonly PointF _focus = focus;
        private readonly float _radius = radius;
        private readonly Matrix3x2 _inverseTransformation = inverseTransformation;

        // implemented like RadialGradientBrushSVGStyle
        // from https://github.com/arklumpus/VectSharp/blob/master/VectSharp.Raster.ImageSharp/ImageSharpContext.cs
        protected override float PositionOnGradient(float x, float y)
        {
            Vector2 vec = Vector2.Transform(new Vector2(x, y), _inverseTransformation);
            float a = MathF.Pow(_radius, 2) - Vector2.DistanceSquared(_center, _focus);
            float c = Vector2.DistanceSquared(vec, _focus);
            float halfB = -((vec.X - _focus.X) * (_center.X - _focus.X) + (vec.Y - _focus.Y) * (_center.Y - _focus.Y));
            float sqrt = MathF.Sqrt(halfB * halfB + a * c);

            float tbr1 = (halfB - sqrt) / a;
            float tbr2 = (halfB + sqrt) / a;

            if (tbr1 >= 0 && tbr2 < 0)
            {
                return tbr1;
            }
            else if (tbr1 < 0 && tbr2 >= 0)
            {
                return tbr2;
            }
            else if (tbr1 < 0 && tbr2 < 0)
            {
                return 0;
            }
            else
            {
                return MathF.Min(tbr1, tbr2);
            }
        }

        public override void Apply(Span<float> scanline, int x, int y)
        {
            base.Apply(scanline, x, y);
        }
    }
}