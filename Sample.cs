using System;
using System.Collections.Generic;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SwfLib;
using SwfLib.Data;
using SwfLib.Tags;
using SwfLib.Tags.ControlTags;
using SwfLib.Tags.DisplayListTags;
using SwfLib.Tags.ShapeTags;
using SwiffCheese.Shapes;
using SwiffCheese.Wrappers;
using SwiffCheese.Utils;
using SwiffCheese.Exporting;

namespace SwiffCheese;

public class Sample
{
    public static void Main(string[] args)
    {
        string swfPath = args[0];
        string symbolName = args[1];
        string exportFolder = args[2];

        SwfFile swf;
        using(FileStream file = new(swfPath, FileMode.Open, FileAccess.Read))
        {
            swf = SwfFile.ReadFrom(file);
        }

        SymbolClassTag? symbolClass = null;

        //find symbol class
        foreach(SwfTagBase tag in swf.Tags)
        {
            if(tag is SymbolClassTag symbolClassTag)
            {
                symbolClass = symbolClassTag;
                break;
            }
        }

        if(symbolClass is null)
        {
            throw new Exception("No symbol class in swf");
        }

        //find sprite symbol id
        ushort? symbolId = null;
        foreach(SwfSymbolReference reference in symbolClass.References)
        {
            if(reference.SymbolName == symbolName)
            {
                symbolId = reference.SymbolID;
                break;
            }
        }

        if(symbolId is null)
        {
            throw new Exception("No matching symbol name found in swf");
        }

        //find define sprite tag
        DefineSpriteTag? sprite = null;
        foreach(SwfTagBase tag in swf.Tags)
        {
            if(tag is DefineSpriteTag st && st.SpriteID == symbolId)
            {
                sprite = st;
                break;
            }
        }

        if(sprite is null)
        {
            throw new Exception("No matching symbol id found in swf");
        }

        HashSet<ushort> shapeIds = [];
        //go over place object tags
        foreach(SwfTagBase tag in sprite.Tags)
        {
            if(tag is PlaceObjectBaseTag place)
            {
                shapeIds.Add(place.CharacterID);
            }
        }

        //find matching shape tags
        List<DefineShapeXTag> shapeTags = new(shapeIds.Count);
        foreach(SwfTagBase tag in swf.Tags)
        {
            if(tag is ShapeBaseTag shape && shapeIds.Contains(shape.ShapeID))
            {
                shapeTags.Add(new DefineShapeXTag(shape));
            }
        }

        //go over shapes
        foreach(DefineShapeXTag shape in shapeTags)
        {
            SwfShape compiledShape = new(shape);
            int width = shape.ShapeBounds.Width();
            int height = shape.ShapeBounds.Height();
            Image<Rgba32> image = new(width, height, Color.Transparent.ToPixel<Rgba32>());
            ImageSharpShapeExporter exporter = new(image, new Size(-shape.ShapeBounds.XMin, -shape.ShapeBounds.YMin));
            compiledShape.Export(exporter);
            using FileStream file = new(Path.Join(exportFolder, $"{symbolName}{shape.ShapeID}.png"), FileMode.Create, FileAccess.Write);
            ImageExtensions.SaveAsPng(image, file);
        }
    }
}