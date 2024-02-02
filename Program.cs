using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SwfLib;
using SwfLib.Data;
using SwfLib.Tags;
using SwfLib.Tags.ControlTags;
using SwfLib.Tags.DisplayListTags;
using SwfLib.Tags.ShapeTags;
using SwiffCheese;
using SwiffCheese.Wrappers;

string swfPath = args[0];
string symbolName = args[1];

SwfFile swf;
using(FileStream file = new(swfPath, FileMode.Open))
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

List<ushort> shapeIds = new();
//go over place object tags
foreach(SwfTagBase tag in sprite.Tags)
{
    if(tag is PlaceObjectBaseTag place)
    {
        shapeIds.Add(place.CharacterID);
    }
}

foreach(ushort shapeId in shapeIds)
{
    foreach(SwfTagBase tag in swf.Tags)
    {
        if(tag is ShapeBaseTag shape && shape.ShapeID == shapeId)
        {
            SwfShape compiledShape = new(new DefineShapeXTag(shape));
            using TestGame game = new(compiledShape);
            game.Run();
        }
    }
}

public class TestGame : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private LilyPathShapeExporter? _exporter;
    private readonly SwfShape _shape;

    public TestGame(SwfShape shape)
    {
        _graphics = new(this);
        IsMouseVisible = true;
        Window.AllowUserResizing = true;
        _shape = shape;
    }
    
    protected override void Draw(GameTime gameTime)
    {
        if(_exporter is null)
        {
            Console.WriteLine("gonna export");
            _exporter = new LilyPathShapeExporter(GraphicsDevice);
            _shape.Export(_exporter);
            Console.WriteLine("got past exporting");
            int w = GraphicsDevice.PresentationParameters.BackBufferWidth;
            int h = GraphicsDevice.PresentationParameters.BackBufferHeight;
            int[] backBuffer = new int[w * h];
            GraphicsDevice.GetBackBufferData(backBuffer);
            using Texture2D texture = new(GraphicsDevice, w, h, false, GraphicsDevice.PresentationParameters.BackBufferFormat);
            texture.SetData(backBuffer);
            using FileStream file = new("C:/Users/eyalz/Desktop/test.png", FileMode.Create);
            texture.SaveAsPng(file, w, h);
        }
    }
}