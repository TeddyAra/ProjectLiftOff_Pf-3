using System;
using GXPEngine;
using System.Drawing;
using System.Security.Cryptography;
using GXPEngine.Core;
using System.Collections.Generic;
using System.Linq;
using System.Drawing.Drawing2D;

public class MyGame : Game {
    float levelSpeed = 0.25f;

    Platform platform;
    Random RNG = new Random();
    Angel angel;
    Camera camera;
    public static List<Platform> platforms = new List<Platform>();
    Sprite background;

    public MyGame() : base(1920, 1080, false) {
        targetFps = 60;

        background = new Sprite("square.png");
        background.width = 1920;
        background.height = 43200;
        background.x = -(width / 2);
        background.y = -(height / 2);
        AddChild(background);

        platform = new Platform(200, 150, new Vector2(140, 50));
        AddChild(platform);
        platforms.Add(platform);

        platform = new Platform(200, 350, new Vector2(200, 50));
        AddChild(platform);
        platforms.Add(platform);

        platform = new Platform(200, 550, new Vector2(280, 50));
        AddChild(platform);
        platforms.Add(platform);

        platform = new Platform(400, 750, new Vector2(200, 50));
        AddChild(platform);
        platforms.Add(platform);

        camera = new Camera(0, 0, 1920, 1080);
        AddChild(camera);

        angel = new Angel(platforms, camera);
        AddChild(angel);
    }

    void Update() {
        camera.y += levelSpeed;
    }

    static void Main() {
		new MyGame().Start();
	}

	// Destroys everything
	void DestroyAll() {
		List<GameObject> children = GetChildren();
		foreach (GameObject child in children) {
			child.LateDestroy();
		}
    }
}