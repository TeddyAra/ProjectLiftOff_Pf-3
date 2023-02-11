using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using GXPEngine;
using GXPEngine.Core;

public class Room : EasyDraw {
    public Vector2 roomSize;

    public Room(Vector2 roomSize) : base(800, 600) {
        this.roomSize = roomSize;
    }

    void Update() {
        ClearTransparent();
        Fill(255, 255, 255, 0);
        Stroke(255, 255, 255);
        StrokeWeight(5);
        ShapeAlign(CenterMode.Min, CenterMode.Min);
        Rect(0, 0, roomSize.x, roomSize.y);
    }
}