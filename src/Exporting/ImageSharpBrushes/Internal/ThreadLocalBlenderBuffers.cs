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
using System.Buffers;
using System.Threading;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SwiffCheese.Exporting.Brushes.Internal;

// a modified copy of an internal ImageSharp class
public class ThreadLocalBlenderBuffers<TPixel>(MemoryAllocator allocator, int scanlineWidth, bool amountBufferOnly = false) : IDisposable
    where TPixel : unmanaged, IPixel<TPixel>
{
    private readonly ThreadLocal<BufferOwner> _data = new(() => new BufferOwner(allocator, scanlineWidth, amountBufferOnly), true);

    public Span<float> AmountSpan => _data.Value!.AmountSpan;
    public Span<TPixel> OverlaySpan => _data.Value!.OverlaySpan;

    public void Dispose()
    {
        foreach (BufferOwner d in _data.Values)
            d.Dispose();
        _data.Dispose();
        GC.SuppressFinalize(this);
    }

    private sealed class BufferOwner(MemoryAllocator allocator, int scanlineLength, bool amountBufferOnly) : IDisposable
    {
        private readonly IMemoryOwner<float> _amountBuffer = allocator.Allocate<float>(scanlineLength);
        private readonly IMemoryOwner<TPixel>? _overlayBuffer = amountBufferOnly ? null : allocator.Allocate<TPixel>(scanlineLength);
        public Span<float> AmountSpan => _amountBuffer.Memory.Span;
        public Span<TPixel> OverlaySpan => _overlayBuffer is not null ? _overlayBuffer.Memory.Span : [];

        public void Dispose()
        {
            _amountBuffer.Dispose();
            _overlayBuffer?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}