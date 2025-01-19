using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using SwfLib;
using SwfLib.Data;
using SwfLib.Tags;
using SwfLib.Tags.ControlTags;
using SwfLib.Tags.DisplayListTags;
using SwfLib.Tags.ShapeTags;
using SwiffCheese.Exporting.Svg;
using SwiffCheese.Math;
using SwiffCheese.Shapes;
using SwiffCheese.Wrappers;

namespace SwiffCheese;

public class Sample
{
    public static void Main(string[] args)
    {
        string swfPath = args[0];
        string symbolName = args[1];
        string exportFolder = args[2];

        SwfFile swf;
        using (FileStream file = new(swfPath, FileMode.Open, FileAccess.Read))
        {
            swf = SwfFile.ReadFrom(file);
        }

        SymbolClassTag? symbolClass = null;

        //find symbol class
        foreach (SwfTagBase tag in swf.Tags)
        {
            if (tag is SymbolClassTag symbolClassTag)
            {
                symbolClass = symbolClassTag;
                break;
            }
        }

        if (symbolClass is null)
        {
            throw new Exception("No symbol class in swf");
        }

        //find sprite symbol id
        ushort? symbolId = null;
        foreach (SwfSymbolReference reference in symbolClass.References)
        {
            if (reference.SymbolName == symbolName)
            {
                symbolId = reference.SymbolID;
                break;
            }
        }

        if (symbolId is null)
        {
            throw new Exception("No matching symbol name found in swf");
        }

        //find define sprite tag
        DefineSpriteTag? sprite = null;
        foreach (SwfTagBase tag in swf.Tags)
        {
            if (tag is DefineSpriteTag st && st.SpriteID == symbolId)
            {
                sprite = st;
                break;
            }
        }

        if (sprite is null)
        {
            throw new Exception("No matching symbol id found in swf");
        }

        HashSet<ushort> shapeIds = [];
        //go over place object tags
        foreach (SwfTagBase tag in sprite.Tags)
        {
            if (tag is PlaceObjectBaseTag place)
            {
                shapeIds.Add(place.CharacterID);
            }
        }

        //find matching shape tags
        List<DefineShapeXTag> shapeTags = new(shapeIds.Count);
        foreach (SwfTagBase tag in swf.Tags)
        {
            if (tag is ShapeBaseTag shape && shapeIds.Contains(shape.ShapeID))
            {
                shapeTags.Add(new DefineShapeXTag(shape));
            }
        }

        //go over shapes
        foreach (DefineShapeXTag shape in shapeTags)
        {
            int x = shape.ShapeBounds.XMin;
            int y = shape.ShapeBounds.YMin;
            Vector2I pos = new(x, y);
            int w = shape.ShapeBounds.XMax - shape.ShapeBounds.XMin;
            int h = shape.ShapeBounds.YMax - shape.ShapeBounds.YMin;
            Vector2I size = new(w, h);

            SwfShape compiledShape = new(shape);
            SvgShapeExporter exporter = new(pos, size, 20);
            compiledShape.Export(exporter);
            exporter.Document.Root?.SetAttributeValue("shape-rendering", "crispEdges");

            using FileStream file = new(Path.Join(exportFolder, $"{symbolName}{shape.ShapeID}.svg"), FileMode.Create, FileAccess.Write);
            using XmlWriter writer = XmlWriter.Create(file, new()
            {
                OmitXmlDeclaration = true,
                NewLineChars = "\n",
                Indent = true,
                IndentChars = "  ",
                Encoding = new UTF8Encoding(false),
            });
            exporter.Document.WriteTo(writer);
        }
    }
}