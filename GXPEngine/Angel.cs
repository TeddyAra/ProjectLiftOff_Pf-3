using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using GXPEngine;
using GXPEngine.Core;

public class Angel : Sprite {
    int moveRange = 100;

    List<Platform> platforms;
    int currentPlatform = 0;
    Camera camera;

    public Angel(List<Platform> platforms, Camera camera) : base("triangle.png") {
        SetOrigin(width / 2, height / 2);
        this.platforms = platforms;
        this.camera = camera;
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) MovementCheck();

        Console.WriteLine(game.height / 2 - platforms[0].y - camera.y);

        // Sets player position
        x = platforms[currentPlatform].x;
        y = platforms[currentPlatform].y;
    }

    void MovementCheck() {
        for (int i = 0; i < platforms.Count; i++) {
            if (Math.Sqrt(Math.Pow((Input.mouseX - game.width / 2 - platforms[i].x), 2) + Math.Pow((Input.mouseY - game.height / 2 - platforms[i].y - camera.y), 2)) < moveRange) currentPlatform = i;
        }
    }
}