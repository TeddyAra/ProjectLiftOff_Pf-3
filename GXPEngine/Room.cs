using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using GXPEngine;
using GXPEngine.Core;

public class Room : EasyDraw {
    public Vector2 roomSize;
    public string type;
    int roomNumber;

    public Room(Vector2 roomSize, string type, int roomNumber) : base(800, 600) {
        this.roomSize = roomSize;
        this.type = type;
        this.roomNumber = roomNumber;
    }

    void Update() {
        ClearTransparent();

        // Gives different stroke colour depending on room type
        if (type == "room") Stroke(255, 255, 255);
        else if (type == "hallway") Stroke(0, 0, 255);
        else if (type == "startRoom") Stroke(255, 0, 0);
        else if (type == "endRoom") Stroke(0, 255, 0);

        // Creates the room itself
        Fill(255, 255, 255, 0);
        StrokeWeight(5);
        ShapeAlign(CenterMode.Min, CenterMode.Min);
        Rect(0, 0, roomSize.x, roomSize.y);

        // Shows room number
        Fill(255, 255, 255, 255);
        TextSize(20);
        TextAlign(CenterMode.Center, CenterMode.Center);
        Text(roomNumber.ToString(), roomSize.x / 2, roomSize.y / 2);
    }
}