using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using GXPEngine;
using GXPEngine.Core;

public class Platform : EasyDraw {
    Vector2 size;

    public Platform(int x, int y, Vector2 size) : base(280, 50) {
        SetOrigin(width / 2, height / 2);
        this.x = x;
        this.y = y;
        this.size = size;
    }

    void Update() {
        ClearTransparent();
        StrokeWeight(2);
        Stroke(255, 0, 0);
        Fill(255, 0, 0);
        Rect(width / 2, height / 2, size.x, size.y);
    }
}
