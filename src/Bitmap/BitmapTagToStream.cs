using System;
using System.IO;
using SwfLib.Tags.BitmapTags;

namespace SwiffCheese.Bitmap;

internal static class BitmapTagToStream
{
    public static Stream TagToStream(BitmapBaseTag bitmapTag)
    {
        throw new NotImplementedException();
    }

    private static Stream JpegBaseTagToStream(DefineBitsJpegTagBase jpegBase)
    {
        MemoryStream fix = new();
        using (MemoryStream data = new(jpegBase.ImageData))
            JpegFixer.FixJpeg(data, fix);
        return fix;
    }

    private static Stream JpegAlphaBaseTagToStream(DefineBitsJpegAlphaBase jpegAlphaBase)
    {
        using Stream alphaless = JpegBaseTagToStream(jpegAlphaBase);
        
    }

    private static Stream Jpeg2TagToStream(DefineBitsJPEG2Tag jpeg2)
    {
        return JpegBaseTagToStream(jpeg2);
    }

    private static Stream Jpeg3TagToStream(DefineBitsJPEG3Tag jpeg3)
    {
        using Stream raw = JpegBaseTagToStream(jpeg3);

    }
}