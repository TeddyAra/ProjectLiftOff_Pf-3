using System;
using GXPEngine;
using System.Drawing;
using System.Security.Cryptography;
using GXPEngine.Core;
using System.Collections.Generic;
using System.Linq;

public class MyGame : Game {
	// Variables that you can change
	int cameraSpeed = 5; // How fast the camera goes
	int roomSizeMin = 6; // Min of rooms
	int roomSizeMax = 12; // Max of rooms
	int hallwaySizeLongMin = 10; // Min of the long side of hallways
	int hallwaySizeLongMax = 20; // Max of the long side of hallways
	int hallwaySizeShortMin = 4; // Min of the short side of hallways
	int hallwaySizeShortMax = 6; // Max of the short side of hallways
	int roomAmount = 20; // How many rooms you want
	int tileSize = 8; // How many pixels the tiles are
	int branchChance = 15; // Chance of creating a new branch (1 in x)
	int hallwayChance = 3; // Chance of creating a hallway instead of a room (1 in x)
	int[] branchPositions = { 6, 12 }; // Preset rooms to branch out of (To get rid of the chance of not getting any branches at all)

	// Things that you can not change
	Camera camera;
	Random RNG = new Random();
	List<Room> rooms = new List<Room>();

	public MyGame() : base(800, 600, false) {
        targetFps = 60;

        camera = new Camera(0, 0, 800, 600);
        AddChild(camera);

		RoomGenerator();
		//RouteGenerator();
	}

	void Update() {
		CameraMove();

        // To test generation bugs based on chance (Turn this on and turn CameraMove() off)
        //GenerationTesting();
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
		// Generates first room
        Room startRoom = new Room(new Vector2(roomSizeMax * tileSize, roomSizeMax * tileSize), "startRoom", 0);
        startRoom.x = game.width / 2 - roomSizeMax / 2;
        startRoom.y = game.height / 2 - roomSizeMax / 2;
        AddChild(startRoom);
		rooms.Add(startRoom);

		// Variables used in for loop
		int facePicker = 1;
		Vector2 roomSize;
		Vector2 roomPos;
		string type;
		int roomNumber = 0;
		int roomAttempt = 0;

		for (int i = 1; i < roomAmount + 1; i++) {
			// Checks if last generation was a room (Makes sure two hallways can't generate next to each other), then randomly chooses between hallway and room
			// Also makes sure hallways are straight between two rooms (Room can't generate on the side of a hallway)
			// Also makes sure rooms that are in the branchPositions list can't be hallways
			if (roomNumber >= rooms.Count) roomNumber = rooms.Count - 1;
            if (rooms[roomNumber].type == "room" && !branchPositions.Contains(i) && i != roomAmount - 1) {
				facePicker = RNG.Next(1, 5);
                if (RNG.Next(1, hallwayChance + 1) == 1) {
					type = "hallway";
					if (facePicker == 1 || facePicker == 3) {
						roomSize = new Vector2(RNG.Next(hallwaySizeShortMin, hallwaySizeShortMax) * tileSize, RNG.Next(hallwaySizeLongMin, hallwaySizeLongMax) * tileSize);
					} else {
						roomSize = new Vector2(RNG.Next(hallwaySizeLongMin, hallwaySizeLongMax) * tileSize, RNG.Next(hallwaySizeShortMin, hallwaySizeShortMax) * tileSize);
					}
				} else {
					type = "room";
					roomSize = new Vector2(RNG.Next(roomSizeMin, roomSizeMax) * tileSize, RNG.Next(roomSizeMin, roomSizeMax) * tileSize);
				}
			} else {
				type = "room";
				roomSize = new Vector2(RNG.Next(roomSizeMin, roomSizeMax) * tileSize, RNG.Next(roomSizeMin, roomSizeMax) * tileSize);
			}

			// Overwrites previous code if this is the last room
			if (i == roomAmount) {
				roomSize = new Vector2(roomSizeMax * tileSize, roomSizeMax * tileSize);
				type = "endRoom";

				// Calculates furthest place from starting room
				double distance;
				double currentFurthestDistance = 0;

				for (int j = 0; j < roomAmount - 1; j++) {
					distance = Math.Sqrt(Math.Pow(rooms[0].x - rooms[j].x, 2)) + Math.Sqrt(Math.Pow(rooms[0].y - rooms[j].y, 2));
					if (distance > currentFurthestDistance) {
                        currentFurthestDistance = distance;
						roomNumber = j;
					}
                }
			}

			// Calculates room position
			roomPos = FaceCheck(roomSize, facePicker, roomNumber);

			// Checks for overlap
			if (!OverlapCheck(roomPos, roomSize)) {
                Room room = new Room(roomSize, type, i);
                room.x = roomPos.x;
                room.y = roomPos.y;
				roomNumber = i;

                AddChild(room);
				rooms.Add(room);
				roomAttempt = 0;

			// Prevents infinite loop (Tries 100 times before creating new branch)
			} else if (roomAttempt < 100) {
				i--;
				roomAttempt++;

			// Gives up after 100 tries and instead branches out
			} else {
				i--;
                do {
					roomNumber = RNG.Next(0, i);
				} while (rooms[roomNumber].type == "hallway");
            }

			// Chooses when to create new branch (Based on chance, predetermined branch rooms, and the previously generated room)
			if ((RNG.Next(1, branchChance + 1) == 1 || branchPositions.Contains(i)) && roomNumber != 0 && rooms[roomNumber].type != "hallway") {
				do {
					roomNumber = RNG.Next(0, i - 1);
				} while (rooms[roomNumber].type == "hallway");
			}
		}
	}

	/* Can't properly generate paths with hallways for now
	void RouteGenerator() {
		// Creates red line between rooms
		for (int i = 0; i < rooms.Count - 1; i++) {
			EasyDraw route = new EasyDraw(roomSizeMax * 2 * tileSize, roomSizeMax * 2 * tileSize);
            route.Stroke(255, 0, 0);
            route.StrokeWeight(5);
            route.x = rooms[i].x + rooms[i].roomSize.x / 2 - roomSizeMax * tileSize;
			route.y = rooms[i].y + rooms[i].roomSize.y / 2 - roomSizeMax * tileSize;

			if (rooms[i].x + rooms[i].roomSize.x / 2 < rooms[i + 1].x + rooms[i + 1].roomSize.x / 2) {
				route.Line(roomSizeMax * tileSize, roomSizeMax * tileSize, roomSizeMax * tileSize + rooms[i].roomSize.x / 2 + rooms[i + 1].roomSize.x / 2, roomSizeMax * tileSize);
			} else if (rooms[i].x + rooms[i].roomSize.x / 2 > rooms[i + 1].x + rooms[i + 1].roomSize.x / 2) {
				route.Line(roomSizeMax * tileSize, roomSizeMax * tileSize, roomSizeMax * tileSize - rooms[i].roomSize.x / 2 - rooms[i + 1].roomSize.x / 2, roomSizeMax * tileSize);
			} else if (rooms[i].y + rooms[i].roomSize.y / 2 < rooms[i + 1].y + rooms[i + 1].roomSize.y / 2) {
				route.Line(roomSizeMax * tileSize, roomSizeMax * tileSize, roomSizeMax * tileSize, roomSizeMax * tileSize + rooms[i].roomSize.y / 2 + rooms[i + 1].roomSize.y / 2);
			} else {
                route.Line(roomSizeMax * tileSize, roomSizeMax * tileSize, roomSizeMax * tileSize, roomSizeMax * tileSize - rooms[i].roomSize.y / 2 - rooms[i + 1].roomSize.y / 2);
            }
            AddChild(route);
        }
	}
	*/

	Vector2 FaceCheck(Vector2 roomSize, int facePicker, int roomNumber) { 
		// Randomly chooses which way to generate (1 = up, 2 = right, 3 = down, 4 (else) = left) and returns position of top-left corner of new room
		if (facePicker == 1) {
			return new Vector2(rooms[roomNumber].x + rooms[roomNumber].roomSize.x / 2 - roomSize.x / 2, rooms[roomNumber].y - roomSize.y);
		} else if (facePicker == 2) {
            return new Vector2(rooms[roomNumber].roomSize.x + rooms[roomNumber].x, rooms[roomNumber].y + rooms[roomNumber].roomSize.y / 2 - roomSize.y / 2);
		} else if (facePicker == 3) {
            return new Vector2(rooms[roomNumber].x + rooms[roomNumber].roomSize.x / 2 - roomSize.x / 2, rooms[roomNumber].y + rooms[roomNumber].roomSize.y);
        } else {
            return new Vector2(rooms[roomNumber].x - roomSize.x, rooms[roomNumber].y + rooms[roomNumber].roomSize.y / 2 - roomSize.y / 2);
        }
	}

	Boolean OverlapCheck(Vector2 pos, Vector2 size) {
		// Top-left and bottom-right of room 1
		Vector2 pos1 = pos;
		Vector2 pos2 = new Vector2(pos.x + size.x, pos.y + size.y);

		// Top-left and bottom-right of other rooms in for loop
		Vector2 pos3;
		Vector2 pos4;

        // Returns true if any rectangles overlap
        for (int i = 0; i < rooms.Count; i++) {
            pos3 = new Vector2(rooms[i].x, rooms[i].y);
			pos4 = new Vector2(rooms[i].x + rooms[i].roomSize.x, rooms[i].y + rooms[i].roomSize.y);

			// Checks for overlap
			if ((pos4.x > pos1.x && pos3.x < pos2.x) && (pos4.y > pos1.y && pos3.y < pos2.y)) return true;
        }

		return false;
	}

	// Destroys everything
	void DestroyAll() {
		List<GameObject> children = GetChildren();
		foreach (GameObject child in children) {
			child.LateDestroy();
		}
        //Doesn't work for some reason? 
        //Array.Clear(rooms, 0, rooms.Count);
    }

    void GenerationTesting() {
		Console.WriteLine("Reset");
		DestroyAll();
		RoomGenerator();
        camera = new Camera(0, 0, 800, 600);
        AddChild(camera);
    }
}