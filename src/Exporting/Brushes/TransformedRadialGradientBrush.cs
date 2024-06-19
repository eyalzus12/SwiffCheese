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
using SwiffCheese.Exporting.Brushes.Internal;

namespace SwiffCheese.Exporting.Brushes;

// a modified copy of ImageSharp's RadialGradientBrush
public sealed class TransformedRadialGradientBrush(
    PointF center,
    float radius,
    Matrix3x2 transformation,
    GradientRepetitionMode repetitionMode,
    params ColorStop[] colorStops
) : GradientBrush(repetitionMode, colorStops)
{
    private readonly PointF _center = center;
    private readonly float _radius = radius;
    private readonly Matrix3x2 _transformation = transformation;


    public override bool Equals(Brush? other)
    {
        if (other is TransformedRadialGradientBrush brush)
        {
            return base.Equals(other)
                && _center.Equals(brush._center)
                && _radius.Equals(brush._radius)
                && _transformation.Equals(brush._transformation);
        }

        return false;
    }

    public override int GetHashCode()
        => HashCode.Combine(base.GetHashCode(), _center, _radius, _transformation);

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
            _radius,
            _transformation,
            ColorStops,
            RepetitionMode
        );

    private sealed class TransformedRadialGradientBrushApplicator<TPixel>(
        Configuration configuration,
        GraphicsOptions options,
        ImageFrame<TPixel> target,
        PointF center,
        float radius,
        Matrix3x2 transformation,
        ColorStop[] colorStops,
        GradientRepetitionMode repetitionMode
    ) : GradientBrushApplicator<TPixel>(configuration, options, target, colorStops, repetitionMode)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        private readonly PointF _center = center;
        private readonly float _radius = radius;
        private readonly Matrix3x2 _transformation = transformation;

        protected override float PositionOnGradient(float x, float y)
        {
            Matrix3x2.Invert(_transformation, out Matrix3x2 inverse);
            float distance = Vector2.Distance(_center, Vector2.Transform(new Vector2(x, y), inverse));
            return distance / _radius;
        }

        public override void Apply(Span<float> scanline, int x, int y)
        {
            base.Apply(scanline, x, y);
        }
    }
}