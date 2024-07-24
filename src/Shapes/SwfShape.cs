using System;
using System.Linq;
using System.Collections.Generic;
using SwfLib.Shapes.Records;
using SwfLib.Shapes.FillStyles;
using SixLabors.ImageSharp;
using SwiffCheese.Edges;
using SwiffCheese.Exporting;
using SwiffCheese.Wrappers;

using EdgeMap = System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<SwiffCheese.Edges.IEdge>>;
using CoordMap = System.Collections.Generic.Dictionary<SixLabors.ImageSharp.Point, System.Collections.Generic.List<SwiffCheese.Edges.IEdge>>;
using Path = System.Collections.Generic.List<SwiffCheese.Edges.IEdge>;
using SwfLib.Data;

namespace SwiffCheese.Shapes;

public class SwfShape(DefineShapeXTag shape)
{
    private readonly List<ShapeRecord> _records = shape.ShapeRecords.ToList();
    private readonly List<FillStyle> _fillStyles = shape.FillStyles.ToList();
    private readonly List<LineStyle> _lineStyles = shape.LineStyles.ToList();

    private readonly List<EdgeMap> _fillEdgesMaps = [];
    private EdgeMap _currentFillEdgeMap = [];
    private readonly List<EdgeMap> _lineEdgeMaps = [];
    private EdgeMap _currentLineEdgeMap = [];
    private int _numGroups = 0;
    private readonly CoordMap _coordMap = [];
    private readonly CoordMap _reverseCoordMap = [];
    private bool _edgeMapsCreated = false;

    public void Export(IShapeExporter handler)
    {
        CreateEdgeMaps();
        handler.BeginShape();
        for (int i = 0; i < _numGroups; ++i)
        {
            ExportFillPath(handler, i);
            ExportLinePath(handler, i);
        }
        handler.EndShape();
    }

    private void ExportFillPath(IShapeExporter exporter, int groupIndex)
    {
        Path path = PathFromEdgeMap(_fillEdgesMaps[groupIndex]);
        if (path.Count == 0)
            return;
        Point pos = new(int.MaxValue, int.MaxValue);
        int fillStyleIdx = int.MaxValue;
        exporter.BeginFills();
        foreach (IEdge edge in path)
        {
            if (fillStyleIdx != edge.FillStyleIndex)
            {
                if (fillStyleIdx != int.MaxValue)
                    exporter.EndFill();

                fillStyleIdx = edge.FillStyleIndex;
                pos = new Point(int.MaxValue, int.MaxValue);

                if (fillStyleIdx == 0)
                    exporter.BeginFill(new SwfRGB(0, 0, 0));
                else
                {
                    FillStyle fillStyle = _fillStyles[fillStyleIdx - 1];
                    switch (fillStyle.Type)
                    {
                        case FillStyleType.SolidColor:
                            SolidFillStyle solidFillStyle = fillStyle.AsSolidFillStyle();
                            exporter.BeginFill(solidFillStyle.Color);
                            break;
                        case FillStyleType.LinearGradient:
                            LinearGradientFillStyle linearGradientFillStyle = fillStyle.AsLinearGradientFillStyle();
                            exporter.BeginLinearGradientFill(linearGradientFillStyle);
                            break;
                        case FillStyleType.RadialGradient:
                            RadialGradientFillStyle radialGradientFillStyle = fillStyle.AsRadialGradientFillStyle();
                            exporter.BeginRadialGradientFill(radialGradientFillStyle);
                            break;
                        case FillStyleType.FocalGradient:
                            FocalGradientFillStyle focalGradientFillStyle = fillStyle.AsFocalGradientFillStyle();
                            exporter.BeginFocalGradientFill(focalGradientFillStyle);
                            break;
                        case FillStyleType.RepeatingBitmap:
                        case FillStyleType.ClippedBitmap:
                        case FillStyleType.NonSmoothedRepeatingBitmap:
                        case FillStyleType.NonSmoothedClippedBitmap:
                            throw new NotImplementedException($"Unsupported fill style type {fillStyle.Type}");
                        default:
                            throw new ArgumentException($"Invalid fill style type {fillStyle.Type}");
                    }
                }
            }

            if (pos != edge.From)
                exporter.MoveTo(edge.From);

            if (edge is CurvedEdge cedge)
                exporter.CurveTo(cedge.Control, cedge.To);
            else
                exporter.LineTo(edge.To);

            pos = edge.To;
        }

        if (fillStyleIdx != int.MaxValue) exporter.EndFill();
        exporter.EndFills();
    }

    private void ExportLinePath(IShapeExporter exporter, int groupIndex)
    {
        Path path = PathFromEdgeMap(_lineEdgeMaps[groupIndex]);
        if (path.Count == 0)
            return;
        Point pos = new(int.MaxValue, int.MaxValue);
        Point lastMove = pos;
        int lineStyleIndex = int.MaxValue;

        exporter.BeginLines();
        foreach (IEdge edge in path)
        {
            if (lineStyleIndex != edge.LineStyleIndex)
            {
                lineStyleIndex = edge.LineStyleIndex;
                pos = new Point(int.MaxValue, int.MaxValue);

                if (lineStyleIndex == 0)
                    exporter.LineStyle(0, new SwfRGB(0, 0, 0));
                else
                {
                    LineStyle lineStyle = _lineStyles[lineStyleIndex - 1];
                    exporter.LineStyle(lineStyle.Width, lineStyle.Color);
                }
            }

            if (pos != edge.From)
            {
                exporter.MoveTo(edge.From);
                lastMove = edge.From;
            }

            if (edge is CurvedEdge cedge)
                exporter.CurveTo(cedge.Control, cedge.To);
            else
                exporter.LineTo(edge.To);

            pos = edge.To;
        }
        exporter.EndLines(pos == lastMove);
    }

    private void CreateEdgeMaps()
    {
        if (_edgeMapsCreated)
            return;
        Point position = Point.Empty;
        Point from;
        Point to;
        Point control;
        int fillStyleIndexOffset = 0;
        int lineStyleIndexOffset = 0;
        int currentFillStyleIndex0 = 0;
        int currentFillStyleIndex1 = 0;
        int currentLineStyleIndex = 0;
        Path subPath = [];
        _numGroups = 0;
        _fillEdgesMaps.Clear();
        _lineEdgeMaps.Clear();
        _currentFillEdgeMap.Clear();
        _currentLineEdgeMap.Clear();

        foreach (ShapeRecord shapeRecord in _records)
        {
            switch (shapeRecord.Type)
            {
                case ShapeRecordType.StyleChangeRecord:
                    StyleChangeRecord styleChangeRecord = shapeRecord.AsStyleChangeRecord();
                    if (styleChangeRecord.LineStyle is not null ||
                        styleChangeRecord.FillStyle0 is not null ||
                        styleChangeRecord.FillStyle1 is not null)
                    {
                        ProcessSubPath(subPath, currentLineStyleIndex, currentFillStyleIndex0, currentFillStyleIndex1);
                        subPath.Clear();
                    }

                    if (styleChangeRecord.StateNewStyles)
                    {
                        fillStyleIndexOffset = _fillStyles.Count;
                        lineStyleIndexOffset = _lineStyles.Count;
                        _fillStyles.AddRange(styleChangeRecord.FillStyles.ToList());
                        _lineStyles.AddRange(styleChangeRecord.LineStyles.ToList());
                    }

                    if (styleChangeRecord.LineStyle is not null && styleChangeRecord.LineStyle == 0 &&
                        styleChangeRecord.FillStyle0 is not null && styleChangeRecord.FillStyle0 == 0 &&
                        styleChangeRecord.FillStyle1 is not null && styleChangeRecord.FillStyle1 == 0)
                    {
                        CleanEdgeMap(_currentFillEdgeMap);
                        CleanEdgeMap(_currentLineEdgeMap);
                        _fillEdgesMaps.Add(_currentFillEdgeMap);
                        _lineEdgeMaps.Add(_currentLineEdgeMap);
                        //we must create new instead of Clear because the edge map lists hold a reference
                        _currentFillEdgeMap = [];
                        _currentLineEdgeMap = [];
                        currentLineStyleIndex = 0;
                        currentFillStyleIndex0 = 0;
                        currentFillStyleIndex1 = 0;
                        _numGroups++;
                    }
                    else
                    {
                        if (styleChangeRecord.LineStyle is not null)
                        {
                            currentLineStyleIndex = (int)styleChangeRecord.LineStyle;
                            if (currentLineStyleIndex > 0) currentLineStyleIndex += lineStyleIndexOffset;
                        }
                        if (styleChangeRecord.FillStyle0 is not null)
                        {
                            currentFillStyleIndex0 = (int)styleChangeRecord.FillStyle0;
                            if (currentFillStyleIndex0 > 0) currentFillStyleIndex0 += fillStyleIndexOffset;
                        }
                        if (styleChangeRecord.FillStyle1 is not null)
                        {
                            currentFillStyleIndex1 = (int)styleChangeRecord.FillStyle1;
                            if (currentFillStyleIndex1 > 0) currentFillStyleIndex1 += fillStyleIndexOffset;
                        }
                    }

                    if (styleChangeRecord.StateMoveTo)
                    {
                        position = new Point(styleChangeRecord.MoveDeltaX, styleChangeRecord.MoveDeltaY);
                    }
                    break;
                case ShapeRecordType.StraightEdge:
                    StraightEdgeShapeRecord straightEdgeRecord = shapeRecord.AsStraightEdgeRecord();
                    from = new Point(position.X, position.Y);
                    Size delta = new(straightEdgeRecord.DeltaX, straightEdgeRecord.DeltaY);
                    position += delta;
                    to = new Point(position.X, position.Y);
                    subPath.Add(new StraightEdge { From = from, To = to, LineStyleIndex = currentLineStyleIndex, FillStyleIndex = currentFillStyleIndex1 });
                    break;
                case ShapeRecordType.CurvedEdgeRecord:
                    CurvedEdgeShapeRecord curvedEdgeRecord = shapeRecord.AsCurvedEdgeRecord();
                    from = new Point(position.X, position.Y);
                    Size controlDelta = new(curvedEdgeRecord.ControlDeltaX, curvedEdgeRecord.ControlDeltaY);
                    control = position + controlDelta;
                    Size anchorDelta = new(curvedEdgeRecord.AnchorDeltaX, curvedEdgeRecord.AnchorDeltaY);
                    position = control + anchorDelta;
                    to = new Point(position.X, position.Y);
                    subPath.Add(new CurvedEdge { From = from, Control = control, To = to, LineStyleIndex = currentLineStyleIndex, FillStyleIndex = currentFillStyleIndex1 });
                    break;
                case ShapeRecordType.EndRecord:
                    ProcessSubPath(subPath, currentLineStyleIndex, currentFillStyleIndex0, currentFillStyleIndex1);
                    CleanEdgeMap(_currentFillEdgeMap);
                    CleanEdgeMap(_currentLineEdgeMap);
                    _fillEdgesMaps.Add(_currentFillEdgeMap);
                    _lineEdgeMaps.Add(_currentLineEdgeMap);
                    _numGroups++;
                    break;
                default:
                    throw new ArgumentException($"Invalid record type {shapeRecord.Type}");
            }
        }

        _edgeMapsCreated = true;
    }

    private void ProcessSubPath(Path subPath, int lineStyleIdx, int fillStyleIdx0, int fillStyleIdx1)
    {
        if (fillStyleIdx0 != 0)
        {
            if (!_currentFillEdgeMap.ContainsKey(fillStyleIdx0)) _currentFillEdgeMap[fillStyleIdx0] = [];
            _currentFillEdgeMap[fillStyleIdx0].AddRange(
                subPath.Select(e => e.ReverseWithStyle(fillStyleIdx0)).Reverse()
            );
        }

        if (fillStyleIdx1 != 0)
        {
            if (!_currentFillEdgeMap.ContainsKey(fillStyleIdx1)) _currentFillEdgeMap[fillStyleIdx1] = [];

            _currentFillEdgeMap[fillStyleIdx1].AddRange(subPath);
        }

        if (lineStyleIdx != 0)
        {
            if (!_currentLineEdgeMap.ContainsKey(lineStyleIdx)) _currentLineEdgeMap[lineStyleIdx] = [];

            _currentLineEdgeMap[lineStyleIdx].AddRange(subPath);
        }
    }

    private void CleanEdgeMap(EdgeMap edgeMap)
    {
        foreach ((int styleIdx, Path subPath) in edgeMap)
        {
            if (subPath.Count == 0) continue;
            IEdge? prevEdge = null;
            IEdge? edge;
            Path tmpPath = [];
            CreateCoordMap(subPath);
            CreateReverseCoordMap(subPath);
            while (subPath.Count > 0)
            {
                int index = 0;
                while (index < subPath.Count)
                {
                    if (prevEdge is not null)
                    {
                        if (prevEdge.To != subPath[index].From)
                        {
                            edge = FindNextEdgeInCoordMap(prevEdge);
                            if (edge is not null)
                            {
                                index = subPath.IndexOf(edge);
                            }
                            else
                            {
                                IEdge? revEdge = FindNextEdgeInReverseCoordMap(prevEdge);
                                if (revEdge is not null)
                                {
                                    index = subPath.IndexOf(revEdge);
                                    IEdge r = revEdge.ReverseWithStyle(revEdge.FillStyleIndex);
                                    UpdateEdgeInCoordMap(revEdge, r);
                                    UpdateEdgeInReverseCoordMap(revEdge, r);
                                    subPath[index] = r;
                                }
                                else
                                {
                                    index = 0;
                                    prevEdge = null;
                                }
                            }

                            continue;
                        }
                    }

                    edge = subPath[index];
                    subPath.RemoveAt(index);
                    tmpPath.Add(edge);
                    RemoveEdgeFromCoordMap(edge);
                    RemoveEdgeFromReverseCoordMap(edge);
                    prevEdge = edge;
                }
            }
            edgeMap[styleIdx] = tmpPath;
        }
    }

    private IEdge? FindNextEdgeInCoordMap(IEdge edge)
    {
        Point key = edge.To;
        if (!_coordMap.TryGetValue(key, out Path? path))
            return null;
        if (path.Count == 0)
            return null;
        return path[0];
    }

    private IEdge? FindNextEdgeInReverseCoordMap(IEdge edge)
    {
        Point key = edge.To;
        if (!_reverseCoordMap.TryGetValue(key, out Path? path))
            return null;
        if (path.Count == 0)
            return null;
        return path[0];
    }

    private void RemoveEdgeFromCoordMap(IEdge edge)
    {
        Point key = edge.From;
        if (_coordMap.TryGetValue(key, out Path? value))
        {
            if (value.Count == 1) _coordMap.Remove(key);
            else _coordMap[key].Remove(edge);
        }
    }

    private void RemoveEdgeFromReverseCoordMap(IEdge edge)
    {
        Point key = edge.To;
        if (_reverseCoordMap.TryGetValue(key, out Path? value))
        {
            if (value.Count == 1) _reverseCoordMap.Remove(key);
            else _reverseCoordMap[key].Remove(edge);
        }
    }

    private void CreateCoordMap(Path path)
    {
        _coordMap.Clear();
        for (int i = 0; i < path.Count; ++i)
        {
            Point key = path[i].From;
            if (!_coordMap.ContainsKey(key)) _coordMap[key] = [];
            _coordMap[key].Add(path[i]);
        }
    }

    private void CreateReverseCoordMap(Path path)
    {
        _reverseCoordMap.Clear();
        for (int i = 0; i < path.Count; ++i)
        {
            Point key = path[i].To;
            if (!_reverseCoordMap.ContainsKey(key)) _reverseCoordMap[key] = [];
            _reverseCoordMap[key].Add(path[i]);
        }
    }

    private void UpdateEdgeInCoordMap(IEdge edge, IEdge newEdge)
    {
        Point key1 = edge.From;
        _coordMap[key1].Remove(edge);
        Point key2 = newEdge.From;
        if (!_coordMap.ContainsKey(key2)) _coordMap[key2] = [];
        _coordMap[key2].Add(newEdge);
    }

    private void UpdateEdgeInReverseCoordMap(IEdge edge, IEdge newEdge)
    {
        Point key1 = edge.To;
        _reverseCoordMap[key1].Remove(edge);
        Point key2 = newEdge.To;
        if (!_reverseCoordMap.ContainsKey(key2)) _reverseCoordMap[key2] = [];
        _reverseCoordMap[key2].Add(newEdge);
    }

    private static Path PathFromEdgeMap(EdgeMap edgeMap) => edgeMap.Keys.OrderBy(i => i).SelectMany(i => edgeMap[i]).ToList();
}
