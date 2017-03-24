using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;

namespace WindowsGame1
{
	public static class WorldGenerator
	{		
		private static int _worldSize = Config.Players * Config.WorldTilesPerPlayer;
		private static Random _random;
		private static Tile[,] _walls;
		private static Tile[,] _floors;
		private static bool[,] _boolWalls;
		private static Item[] _items;
		private static World _world;

		public static Arena GenerateWorld(int playerCount)
		{
			_random = new Random(Config.Seed);
			_world = new World(Vector2.Zero);
			_walls = new Tile[_worldSize, _worldSize];
			_floors = new Tile[_worldSize, _worldSize];
			_boolWalls = new bool[_worldSize, _worldSize];
			FillWorld();
			_items = new Item[Config.Players * Config.AmmoPerPlayer];
			CarveRooms();
			PlaceWallsInWorld();
			for (var i = 0; i < Config.Players * Config.AmmoPerPlayer; i++)
			{
				AddItem(i);
			}
			return new Arena(_world, _walls, _floors, _items, _worldSize);
		}

		private static void AddItem(int index)
		{
			var itemPlaced = false;
			do
			{
				var x = _random.Next(2, _worldSize - 2);
				var y = _random.Next(2, _worldSize - 2);
				if (!_boolWalls[x, y])
				{
					DamageType dtype = DamageType.Normal;
					Texture2D texture = Loader.LoadGameTexture("WorldObjects/Items/NormalAmmo");
					switch (_random.Next(4))
					{
						case 0:
						dtype = DamageType.Normal;
						texture = Loader.LoadGameTexture("WorldObjects/Items/NormalAmmo");
						_items[index] = new Item(_world, new Vector2(x * Config.TileSize - (Config.TileSize / 2), y * Config.TileSize - (Config.TileSize / 2)), false, 30, texture, dtype);
						break;
						case 1:
						dtype = DamageType.Poison;
						texture = Loader.LoadGameTexture("WorldObjects/Items/PoisonAmmo");
						_items[index] = new Item(_world, new Vector2(x * Config.TileSize - (Config.TileSize / 2), y * Config.TileSize - (Config.TileSize / 2)), false, 30, texture, dtype);
						break;
						case 2:
						dtype = DamageType.Water;
						texture = Loader.LoadGameTexture("WorldObjects/Items/WaterAmmo");
						_items[index] = new Item(_world, new Vector2(x * Config.TileSize - (Config.TileSize / 2), y * Config.TileSize - (Config.TileSize / 2)), false, 30, texture, dtype);
						break;
						case 3:
						texture = Loader.LoadGameTexture("WorldObjects/Items/healthpack");
						_items[index] = new Item(_world, new Vector2(x * Config.TileSize - (Config.TileSize / 2), y * Config.TileSize - (Config.TileSize / 2)), false, texture, Config.AdditionalHealth);
						break;
					}
					itemPlaced = true;
				}
			}
			while (!itemPlaced);
		}

		private static void PlaceWallsInWorld()
		{
			for (var y = 0; y < _worldSize; y++)
			{
				for (var x = 0; x < _worldSize; x++)
				{
					if (_boolWalls[x, y])
					{
						var outerWall = false;
						if (x == 0 || x == _worldSize - 1 || y == 0 || y == _worldSize - 1)
						{
							outerWall = true;
						}
						_walls[x, y] = new Tile(_world, new Vector2(x * Config.TileSize, y * Config.TileSize), outerWall, _random.Next(4), true);
					}
					else
					{
						_floors[x, y] = new Tile(_world, new Vector2(x * Config.TileSize, y * Config.TileSize), false, _random.Next(4), false);
					}
				}
			}
		}

		private static void FillWorld()
		{
			for (var y = 0; y < _worldSize; y++)
			{
				for (var x = 0; x < _worldSize; x++)
				{
					_boolWalls[x, y] = true;
				}
			}
		}

		private static void CarveRooms()
		{
			var meetingRoomSize = (int)(_worldSize - 4); // offset of two tiles on each side.
			var meetingRoomLocation = (_worldSize - meetingRoomSize) / 2;
			for (var y = 0; y < meetingRoomSize; y++)
			{
				for (var x = 0; x < meetingRoomSize; x++)
				{
					_boolWalls[x + meetingRoomLocation, y + meetingRoomLocation] = (_random.Next(100) < 90) ? false : true;
				}
			}
		}
	}
}
