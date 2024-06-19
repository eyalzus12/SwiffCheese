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
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SwiffCheese.Exporting.Brushes.Internal;

// a modified copy of an internal ImageSharp class
public abstract class GradientBrushApplicator<TPixel> : BrushApplicator<TPixel>
    where TPixel : unmanaged, IPixel<TPixel>
{
    private static readonly TPixel _Transparent = Color.Transparent.ToPixel<TPixel>();

    private readonly ColorStop[] _colorStops;
    private readonly GradientRepetitionMode _repetitionMode;
    private readonly MemoryAllocator _allocator;
    private readonly int _scanlineWidth;
    private readonly ThreadLocalBlenderBuffers<TPixel> _blenderBuffers;
    private readonly PixelBlender<TPixel> _blender;
    private bool _isDisposed;

    protected GradientBrushApplicator(
        Configuration configuration,
        GraphicsOptions options,
        ImageFrame<TPixel> target,
        ColorStop[] colorStops,
        GradientRepetitionMode repetitionMode)
        : base(configuration, options, target)
    {
        _colorStops = colorStops;
        InsertionSort(_colorStops, (x, y) => x.Ratio.CompareTo(y.Ratio));
        _repetitionMode = repetitionMode;
        _scanlineWidth = target.Width;
        _allocator = configuration.MemoryAllocator;
        _blenderBuffers = new ThreadLocalBlenderBuffers<TPixel>(_allocator, _scanlineWidth);
        _blender = PixelOperations<TPixel>.Instance.GetPixelBlender(options);
    }

    internal TPixel this[int x, int y]
    {
        get
        {
            float positionOnCompleteGradient = PositionOnGradient(x + 0.5f, y + 0.5f);

            switch (_repetitionMode)
            {
                case GradientRepetitionMode.Repeat:
                    positionOnCompleteGradient %= 1;
                    break;
                case GradientRepetitionMode.Reflect:
                    positionOnCompleteGradient %= 2;
                    if (positionOnCompleteGradient > 1)
                        positionOnCompleteGradient = 2 - positionOnCompleteGradient;
                    break;
                case GradientRepetitionMode.DontFill:
                    if (positionOnCompleteGradient is > 1 or < 0)
                        return _Transparent;
                    break;
                case GradientRepetitionMode.None:
                default:
                    break;
            }

            (ColorStop from, ColorStop to) = GetGradientSegment(positionOnCompleteGradient);

            if (from.Color.Equals(to.Color))
            {
                return from.Color.ToPixel<TPixel>();
            }

            float onLocalGradient = (positionOnCompleteGradient - from.Ratio) / (to.Ratio - from.Ratio);

            return new Color(Vector4.Lerp((Vector4)from.Color, (Vector4)to.Color, onLocalGradient)).ToPixel<TPixel>();
        }
    }

    public override void Apply(Span<float> scanline, int x, int y)
    {
        Span<float> amounts = _blenderBuffers.AmountSpan[..scanline.Length];
        Span<TPixel> overlays = _blenderBuffers.OverlaySpan[..scanline.Length];
        float blendPercentage = Options.BlendPercentage;

        if (blendPercentage < 1)
        {
            for (int i = 0; i < scanline.Length; i++)
            {
                amounts[i] = scanline[i] * blendPercentage;
                overlays[i] = this[x + i, y];
            }
        }
        else
        {
            for (int i = 0; i < scanline.Length; i++)
            {
                amounts[i] = scanline[i];
                overlays[i] = this[x + i, y];
            }
        }

        Span<TPixel> destinationRow = Target.PixelBuffer.DangerousGetRowSpan(y).Slice(x, scanline.Length);
        _blender.Blend(Configuration, destinationRow, destinationRow, overlays, amounts);
    }

    protected abstract float PositionOnGradient(float x, float y);

    protected override void Dispose(bool disposing)
    {
        if (_isDisposed)
            return;

        base.Dispose(disposing);

        if (disposing) _blenderBuffers.Dispose();
        _isDisposed = true;
    }

    private (ColorStop From, ColorStop To) GetGradientSegment(float positionOnCompleteGradient)
    {
        ColorStop localGradientFrom = _colorStops[0];
        ColorStop localGradientTo = default;

        foreach (ColorStop colorStop in _colorStops)
        {
            localGradientTo = colorStop;
            if (colorStop.Ratio > positionOnCompleteGradient)
                break;
            localGradientFrom = localGradientTo;
        }

        return (localGradientFrom, localGradientTo);
    }

    private static void InsertionSort<T>(T[] collection, Comparison<T> comparison)
    {
        int count = collection.Length;
        for (int j = 1; j < count; j++)
        {
            T key = collection[j];

            int i = j - 1;
            for (; i >= 0 && comparison(collection[i], key) > 0; i--)
            {
                collection[i + 1] = collection[i];
            }

            collection[i + 1] = key;
        }
    }
}