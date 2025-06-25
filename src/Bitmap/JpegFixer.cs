using System.IO;

namespace SwiffCheese.Bitmap;

internal static class JpegFixer
{
    private const byte SOI = 0xD8;
    private const byte EOI = 0xD9;

    public static void FixJpeg(Stream input, Stream output)
    {
        bool prevEoi = false;

        int val = input.ReadByte();
        if (val == -1)
        {
            return;
        }
        if (val == 0xFF)
        {
            val = input.ReadByte();
            if (val == -1)
            {
                output.WriteByte(0xFF);
                return;
            }

            if (val != SOI && val != EOI)
            {
                //not a JPEG file, nor invalid header, proceed as is
                output.WriteByte(0xFF);
                output.WriteByte((byte)val);
                while ((val = input.ReadByte()) > -1)
                {
                    output.WriteByte((byte)val);
                }
                return;
            }

            //Check for erroneous header at the beginning, before first SOI marker
            if (val == EOI)
            {
                val = input.ReadByte();
                int val2 = input.ReadByte();
                if (val == 0xFF && val2 == SOI)
                {
                    val = input.ReadByte();
                    val2 = input.ReadByte();
                    if (val != 0xFF || val2 != SOI)
                    {
                        //not a JPEG file, proceed as is
                        output.WriteByte(0xFF);
                        output.WriteByte(EOI);
                        output.WriteByte(0xFF);
                        output.WriteByte(SOI);
                        if (val != -1)
                        {
                            output.WriteByte((byte)val);
                        }
                        if (val2 != -1)
                        {
                            output.WriteByte((byte)val2);
                        }
                        while ((val = input.ReadByte()) > -1)
                        {
                            output.WriteByte((byte)val);
                        }
                        return;
                    }
                }
                else
                {
                    //not a JPEG file, proceed as is
                    output.WriteByte(0xFF);
                    output.WriteByte(EOI);
                    if (val != -1)
                    {
                        output.WriteByte((byte)val);
                    }
                    if (val2 != -1)
                    {
                        output.WriteByte((byte)val2);
                    }
                    while ((val = input.ReadByte()) > -1)
                    {
                        output.WriteByte((byte)val);
                    }
                    return;
                }
            }
            output.WriteByte(0xFF);
            output.WriteByte(SOI);
        }
        else
        {
            //not a JPEG file, proceed as is
            output.WriteByte((byte)val);
            while ((val = input.ReadByte()) > -1)
            {
                output.WriteByte((byte)val);
            }
            return;
        }

        //main removing EOI+SOI
        while ((val = input.ReadByte()) > -1)
        {
            if (val == 0xFF)
            {
                val = input.ReadByte();
                if (val == 0)
                {
                    output.WriteByte(0xFF);
                    output.WriteByte((byte)val);
                    prevEoi = false;
                    continue;
                }

                if (val == SOI && prevEoi)
                {
                    //ignore, effectively removing EOI and SOI
                }
                else if (val == SOI)
                {
                    //second or more SOI in the file, remove that too
                }
                else if (prevEoi)
                {
                    output.WriteByte(0xFF);
                    output.WriteByte(EOI);
                    output.WriteByte(0xFF);
                    if (val != -1)
                    {
                        output.WriteByte((byte)val);
                    }
                }
                else if (val != EOI)
                {
                    output.WriteByte(0xFF);
                    if (val != -1)
                    {
                        output.WriteByte((byte)val);
                    }
                }

                if (val != -1 && JpegMarker.MarkerHasLength(val))
                {
                    int len1 = input.ReadByte();
                    if (len1 == -1)
                    {
                        break;
                    }
                    int len2 = input.ReadByte();
                    if (len2 == -1)
                    {
                        output.WriteByte((byte)len1);
                        break;
                    }
                    output.WriteByte((byte)len1);
                    output.WriteByte((byte)len2);
                    int len = (len1 << 8) + len2;
                    for (int i = 0; i < len - 2; i++)
                    {
                        int val2 = input.ReadByte();
                        if (val2 == -1)
                        {
                            goto exitLoop;
                        }
                        output.WriteByte((byte)val2);
                    }
                }

                prevEoi = val == EOI;
            }
            else
            {
                output.WriteByte((byte)val);
                prevEoi = false;
            }
        }
    exitLoop:

        if (prevEoi)
        {
            output.WriteByte(0xFF);
            output.WriteByte(EOI);
        }
    }
}

internal static class JpegMarker
{

    public const byte SOF0 = 0xC0; //Start of Frame 0
    public const byte SOF1 = 0xC1; //Start of Frame 1
    public const byte SOF2 = 0xC2; //Start of Frame 2
    public const byte SOF3 = 0xC3; //Start of Frame 3

    public const byte DHT = 0xC4; //Define Huffman Table

    public const byte SOF5 = 0xC5; //Start of Frame 5
    public const byte SOF6 = 0xC6; //Start of Frame 6
    public const byte SOF7 = 0xC7; //Start of Frame 7

    public const byte JPG = 0xC8; //JPEG Extensions

    public const byte SOF9 = 0xC9; //Start of Frame 9
    public const byte SOF10 = 0xCA; //Start of Frame 10
    public const byte SOF11 = 0xCB; //Start of Frame 11

    public const byte DAC = 0xCC; //Define Arithmetic Coding

    public const byte SOF13 = 0xCD; //Start of Frame 13
    public const byte SOF14 = 0xCE; //Start of Frame 14
    public const byte SOF15 = 0xCF; //Start of Frame 15

    public const byte RST0 = 0xD0; //Restart Marker 0
    public const byte RST1 = 0xD1; //Restart Marker 1
    public const byte RST2 = 0xD2; //Restart Marker 2
    public const byte RST3 = 0xD3; //Restart Marker 3
    public const byte RST4 = 0xD4; //Restart Marker 4
    public const byte RST5 = 0xD5; //Restart Marker 5
    public const byte RST6 = 0xD6; //Restart Marker 6
    public const byte RST7 = 0xD7; //Restart Marker 7

    public const byte SOI = 0xD8; //Start of Image
    public const byte EOI = 0xD9; //End of Image

    public const byte SOS = 0xDA; //Start of Scan
    public const byte DQT = 0xDB; //Define Quantization Table
    public const byte DNL = 0xDC; //Define Number of Lines
    public const byte DRI = 0xDD; //Define Restart Interval
    public const byte DHP = 0xDE; //Define Hierarchical Progression
    public const byte EXP = 0xDF; //Expand Reference Component

    public const byte APP0 = 0xE0; //Application Segment 0
    public const byte APP1 = 0xE1; //Application Segment 1
    public const byte APP2 = 0xE2; //Application Segment 2
    public const byte APP3 = 0xE3; //Application Segment 3
    public const byte APP4 = 0xE4; //Application Segment 4
    public const byte APP5 = 0xE5; //Application Segment 5
    public const byte APP6 = 0xE6; //Application Segment 6
    public const byte APP7 = 0xE7; //Application Segment 7
    public const byte APP8 = 0xE8; //Application Segment 8
    public const byte APP9 = 0xE9; //Application Segment 9
    public const byte APP10 = 0xEA; //Application Segment 10
    public const byte APP11 = 0xEB; //Application Segment 11
    public const byte APP12 = 0xEC; //Application Segment 12
    public const byte APP13 = 0xED; //Application Segment 13
    public const byte APP14 = 0xEE; //Application Segment 14
    public const byte APP15 = 0xEF; //Application Segment 15

    public const byte JPG0 = 0xF0; //JPEG Extension 0
    public const byte JPG1 = 0xF1; //JPEG Extension 1
    public const byte JPG2 = 0xF2; //JPEG Extension 2
    public const byte JPG3 = 0xF3; //JPEG Extension 3
    public const byte JPG4 = 0xF4; //JPEG Extension 4
    public const byte JPG5 = 0xF5; //JPEG Extension 5
    public const byte JPG6 = 0xF6; //JPEG Extension 6
    public const byte JPG7 = 0xF7; //JPEG Extension 7
    public const byte SOF48 = 0xF7; //JPEG-LS
    public const byte JPG8 = 0xF8; //JPEG Extension 8
    public const byte LSE = 0xF8; //JPEG Extension 8
    public const byte JPG9 = 0xF9; //JPEG Extension 9
    public const byte JPG10 = 0xFA; //JPEG Extension 10
    public const byte JPG11 = 0xFB; //JPEG Extension 11
    public const byte JPG12 = 0xFC; //JPEG Extension 12
    public const byte JPG13 = 0xFD; //JPEG Extension 13   
    public const byte COM = 0xFE; //Comment       

    public static bool MarkerHasLength(int marker)
    {
        return marker != 0
                && marker != SOI
                && marker != EOI
                && marker != RST0
                && marker != RST1
                && marker != RST2
                && marker != RST3
                && marker != RST4
                && marker != RST5
                && marker != RST6
                && marker != RST7;
    }
}
