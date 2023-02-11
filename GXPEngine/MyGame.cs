using System;
using GXPEngine;
using System.Drawing;
using System.Security.Cryptography;
using GXPEngine.Core;
using System.Collections.Generic;

public class MyGame : Game {
	int cameraSpeed = 3;
	int roomSizeMin = 25;
	int roomSizeMax = 100;
	int roomAmount = 20;
	int roomAttempt = 0;

	Camera camera;
	Random RNG = new Random();
	List<Room> rooms = new List<Room>();

	public MyGame() : base(800, 600, false) {
        camera = new Camera(0, 0, 800, 600);
        AddChild(camera);

		RoomGenerator();
		RouteGenerator();

		targetFps = 60;
	}

	void Update() {
		CameraMove();
	}

	static void Main() {
		new MyGame().Start();
	}

	void CameraMove() { 
		if (Input.GetKey(Key.W)) {
			camera.y -= cameraSpeed;
		}
		if (Input.GetKey(Key.S)) {
			camera.y += cameraSpeed;
		}
		if (Input.GetKey(Key.A)) {
			camera.x -= cameraSpeed;
		}
		if (Input.GetKey(Key.D)) {
			camera.x += cameraSpeed;
		}
	}

	void RoomGenerator() {
        Room startRoom = new Room(new Vector2(roomSizeMax, roomSizeMax));
        startRoom.x = game.width / 2 - roomSizeMax / 2;
        startRoom.y = game.height / 2 - roomSizeMax / 2;
        AddChild(startRoom);
		rooms.Add(startRoom);

		int facePicker;
		Vector2 roomSize;
		Vector2 roomPos;

        for (int i = 0; i < roomAmount; i++) {
            Console.WriteLine(i);
            roomSize = new Vector2(RNG.Next(roomSizeMin, roomSizeMax), RNG.Next(roomSizeMin, roomSizeMax));
			if (i == roomAmount - 1) roomSize = new Vector2(roomSizeMax, roomSizeMax);
            facePicker = RNG.Next(1, 5);
			roomPos = FaceCheck(roomSize, facePicker);

			// Checks for overlap
			if (!OverlapCheck(roomPos, roomSize)) {
                Room room = new Room(roomSize);
                room.x = roomPos.x;
                room.y = roomPos.y;

                AddChild(room);
				rooms.Add(room);
			// Prevents infinite loop
			} else if (roomAttempt < 500) {
				i--;
				roomAttempt++;
			}
		}
	}

	void RouteGenerator() {
		// Creates red line between rooms
		for (int i = 0; i < rooms.Count - 1; i++) {
            EasyDraw route = new EasyDraw(roomSizeMax * 2, roomSizeMax * 2);
            route.Stroke(255, 0, 0);
            route.StrokeWeight(5);
			route.x = rooms[i].x + rooms[i].roomSize.x / 2 - roomSizeMax;
			route.y = rooms[i].y + rooms[i].roomSize.y / 2 - roomSizeMax;

			if (rooms[i].x + rooms[i].roomSize.x / 2 < rooms[i + 1].x + rooms[i + 1].roomSize.x / 2) {
				route.Line(roomSizeMax, roomSizeMax, roomSizeMax + rooms[i].roomSize.x / 2 + rooms[i + 1].roomSize.x / 2, roomSizeMax);
			} else if (rooms[i].x + rooms[i].roomSize.x / 2 > rooms[i + 1].x + rooms[i + 1].roomSize.x / 2) {
                route.Line(roomSizeMax, roomSizeMax, roomSizeMax - rooms[i].roomSize.x / 2 - rooms[i + 1].roomSize.x / 2, roomSizeMax);
            } else if (rooms[i].y + rooms[i].roomSize.y / 2 < rooms[i + 1].y + rooms[i + 1].roomSize.y / 2) {
                route.Line(roomSizeMax, roomSizeMax, roomSizeMax, roomSizeMax + rooms[i].roomSize.y / 2 + rooms[i + 1].roomSize.y / 2);
            } else {
                route.Line(roomSizeMax, roomSizeMax, roomSizeMax, roomSizeMax - rooms[i].roomSize.y / 2 - rooms[i + 1].roomSize.y / 2);
            }
            AddChild(route);
        }
	}

	Vector2 FaceCheck(Vector2 roomSize, int facePicker) { 
		// Randomly chooses which way to generate (1 = up, 2 = right, 3 = down, 4 (else) = left) and returns top-left corner of new room
		if (facePicker == 1) {
			return new Vector2(rooms[rooms.Count - 1].x + rooms[rooms.Count - 1].roomSize.x / 2 - roomSize.x / 2, rooms[rooms.Count - 1].y - roomSize.y);
		} else if (facePicker == 2) {
            return new Vector2(rooms[rooms.Count - 1].roomSize.x + rooms[rooms.Count - 1].x, rooms[rooms.Count - 1].y + rooms[rooms.Count - 1].roomSize.y / 2 - roomSize.y / 2);
		} else if (facePicker == 3) {
            return new Vector2(rooms[rooms.Count - 1].x + rooms[rooms.Count - 1].roomSize.x / 2 - roomSize.x / 2, rooms[rooms.Count - 1].y + rooms[rooms.Count-1].roomSize.y);
        } else {
            return new Vector2(rooms[rooms.Count - 1].x - roomSize.x, rooms[rooms.Count - 1].y + rooms[rooms.Count - 1].roomSize.y / 2 - roomSize.y / 2);
        }
	}

	Boolean OverlapCheck(Vector2 pos, Vector2 size) {
		// Top-left and bottom-right of room 1
		Vector2 pos1 = pos;
		Vector2 pos2 = new Vector2(pos.x + size.x, pos.y + size.y);

		// Top-left and bottom-right of other rooms in for loop
		Vector2 pos3;
		Vector2 pos4;

		for (int i = 0; i < rooms.Count; i++) {
            Console.WriteLine("Test");
            pos3 = new Vector2(rooms[i].x, rooms[i].y);
			pos4 = new Vector2(rooms[i].x + rooms[i].roomSize.x, rooms[i].y + rooms[i].roomSize.y);

			// Checks for overlap
			if ((pos4.x > pos1.x && pos3.x < pos2.x) && (pos4.y > pos1.y && pos3.y < pos2.y)) return true;
        }

		return false;
	}
}