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

// a modified copy of ImageSharp's LinearGradientBrush, with support for a gradient transformation.
public sealed class TransformedLinearGradientBrush(
    PointF start,
    PointF end,
    Matrix3x2 transform,
    GradientRepetitionMode repetitionMode,
    params ColorStop[] colorStops
) : GradientBrush(repetitionMode, colorStops)
{
    private readonly PointF _start = start;
    private readonly PointF _end = end;
    private readonly Matrix3x2 _inverseTransformation = Matrix3x2.Invert(transform, out Matrix3x2 inverse) ? inverse : throw new ArgumentException("Gradient transformation must be invertible");

    public override bool Equals(Brush? other)
    {
        if (other is TransformedLinearGradientBrush brush)
        {
            return base.Equals(other)
                && _start.Equals(brush._start)
                && _end.Equals(brush._end)
                && _inverseTransformation.Equals(brush._inverseTransformation);
        }

        return false;
    }

    public override int GetHashCode()
        => HashCode.Combine(base.GetHashCode(), _start, _end, _inverseTransformation);

    public override BrushApplicator<TPixel> CreateApplicator<TPixel>(
        Configuration configuration,
        GraphicsOptions options,
        ImageFrame<TPixel> source,
        RectangleF region) =>
        new TransformedLinearGradientBrushApplicator<TPixel>(
            configuration,
            options,
            source,
            _start,
            _end,
            _inverseTransformation,
            ColorStops,
            RepetitionMode
        );

    private sealed class TransformedLinearGradientBrushApplicator<TPixel> : GradientBrushApplicator<TPixel>
        where TPixel : unmanaged, IPixel<TPixel>
    {
        private readonly PointF _start;
        private readonly PointF _end;
        private readonly Matrix3x2 _inverseTransformation;
        private readonly float _alongX;
        private readonly float _alongY;
        private readonly float _acrossY;
        private readonly float _acrossX;
        private readonly float _alongsSquared;
        private readonly float _length;

        public TransformedLinearGradientBrushApplicator(
            Configuration configuration,
            GraphicsOptions options,
            ImageFrame<TPixel> source,
            PointF start,
            PointF end,
            Matrix3x2 inverseTransformation,
            ColorStop[] colorStops,
            GradientRepetitionMode repetitionMode
        ) : base(configuration, options, source, colorStops, repetitionMode)
        {
            _start = start;
            _end = end;
            _inverseTransformation = inverseTransformation;

            _alongX = _end.X - _start.X;
            _alongY = _end.Y - _start.Y;

            _acrossX = _alongY;
            _acrossY = -_alongX;

            _alongsSquared = (_alongX * _alongX) + (_alongY * _alongY);
            _length = MathF.Sqrt(_alongsSquared);
        }

        protected override float PositionOnGradient(float x, float y)
        {
            Vector2 vec = Vector2.Transform(new Vector2(x, y), _inverseTransformation);
            x = vec.X; y = vec.Y;

            if (_acrossX == 0)
            {
                return (x - _start.X) / (_end.X - _start.X);
            }
            else if (_acrossY == 0)
            {
                return (y - _start.Y) / (_end.Y - _start.Y);
            }
            else
            {
                float deltaX = x - _start.X;
                float deltaY = y - _start.Y;
                float k = ((_alongY * deltaX) - (_alongX * deltaY)) / _alongsSquared;

                float x4 = x - (k * _alongY);
                float y4 = y + (k * _alongX);

                float distance = Vector2.Distance(_start, new(x4, y4));

                return distance / _length;
            }
        }
    }
}