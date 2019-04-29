using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Diagnostics;
using System.Drawing;
using Torches.ECS;

namespace Torches
{
    public class Zone
    {
        public const int Width = 16;
        public const int Height = 8;

        public int X { get; protected set; }
        public int Y { get; protected set; }

        // List of tiles, indexed with tiles[y, x].
        public Tile[,] Tiles { get; set; }
        public List<Entity> Entities { get; set; }

        // Represents if a door is open in a zone, start from the right and go counterclockwise
        // (ie. right, up, left, down).
        public bool[] Doors { get; set; }

        private World World { get; }

        public Zone(World world)
        {
            World = world;
            Tiles = new Tile[Height, Width];
            Entities = new List<Entity>();
            X = 0;
            Y = 0;
        }

        public Zone(int x, int y, World world)
        {
            Tiles = new Tile[Height, Width];
            Entities = new List<Entity>();
            X = x;
            Y = y;
            World = world;
        }

        public Zone(int x, int y, Tile[,] tiles, List<Entity> entities, World world)
        {
            X = x;
            Y = y;
            Tiles = tiles;
            Entities = entities;
            World = world;
        }

        public void AddTile(int x, int y, Tile tile)
        {
            // Make sure tile is within the bounds of the zone
            if(x >= 0 && x < Width && y >= 0 && y < Height)
            {
                Tiles[y, x] = tile;
            }
        }

        public bool IsSolidAt(int x, int y)
        {
            // Check if coordinate is within zone
            if (x < 0 || x >= Width || y < 0 || y >= Height)
            {
                return true;
            }

            // Check if the tile at coord is solid
            if (Tiles[y, x].IsSolid)
            {
                return true;
            }
            
            // Check if a solid entity exists at the coord
            foreach (Entity e in Entities)
            {
                if (e.HasFlag(EntityFlags.Solid))
                {
                    if (e.GetComponent<ECS.Position>().x == x && e.GetComponent<ECS.Position>().y == y)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public Entity GetEntityAt(int x, int y)
        {
            // Check if coordinate is within zone
            if (x < 0 || x >= Width || y < 0 || y >= Height)
            {
                return null;
            }

            // Check if an entity exists at the coord
            foreach (Entity e in Entities)
            {
                if(e.HasComponent<ECS.Position>() && !e.Removed)
                {
                    if (e.GetComponent<ECS.Position>().x == x && e.GetComponent<ECS.Position>().y == y)
                    {
                        return e;
                    }
                }
            }

            return null;
        }

        public Entity GetEntityAt(Position p)
        {
            return GetEntityAt(p.x, p.y);
        }

        public Tile GetTileAt(int x, int y)
        {
            // Check if coordinate is within zone
            if (x < 0 || x >= Width || y < 0 || y >= Height)
            {
                return null;
            }

            return Tiles[y, x];
        }

        public Tile GetTileAt(Position p)
        {
            return GetTileAt(p.x, p.y);
        }

        // Remove all entities that are scheduled to be removed.
        public void Update()
        {
            // Render entities and tiles below removed entities.
            foreach(Entity removed in Entities.Where(x => x.Removed))
            {
                if (removed.HasComponent<Position>())
                {
                    RenderTile(removed.GetComponent<Position>().x, removed.GetComponent<Position>().y);

                    if(World.GetPlayer().GetComponent<Position>().x == removed.GetComponent<Position>().x
                        && World.GetPlayer().GetComponent<Position>().y == removed.GetComponent<Position>().y)
                    {
                        Renderer.RenderEntity(World.GetPlayer());
                    }
                    else
                    {
                        Entity entity = GetEntityAt(removed.GetComponent<Position>().x, removed.GetComponent<Position>().y);
                        if (entity != null)
                        {
                            if (!entity.Removed)
                            {
                                Renderer.RenderEntity(entity);
                            }
                        }
                    }
                }
            }

            // Remove entities from list.
            Entities.RemoveAll(x => x.Removed);
        }

        public void RenderAll()
        {
            // Render tiles.
            Console.CursorVisible = false;
            for (int yi = 0; yi < Height; yi++)
            {
                for (int xi = 0; xi < Width; xi++)
                {
                    RenderTile(xi, yi);
                }
            }

            // Render entities.
            foreach(Entity e in Entities)
            {
                Renderer.RenderEntity(e);
            }
            Console.CursorVisible = true;
            
            RenderDoors();
        }

        public void RenderTile(int x, int y)
        {
            if(IsPos(0, -3))
            {
                Renderer.PrintAt(Constants.MapX + x, Constants.MapY + (Height - y - 1), Tiles[y, x].Symbol, Color.Gray);
            }
            else
            {
                Renderer.PrintAt(Constants.MapX + x, Constants.MapY + (Height - y - 1), Tiles[y, x].Symbol, Color.LightGray);
            }
        }

        public void RenderTile(Position p)
        {
            RenderTile(p.x, p.y);
        }

        public void RenderDoors()
        {
            // Right door.
            if (Doors[0])
            {
                Renderer.PrintAt(Constants.MapX + Width, Constants.MapY + Height / 2 - 1, ':', Color.LightGray);
            }
            else
            {
                Renderer.PrintAt(Constants.MapX + Width, Constants.MapY + Height / 2 - 1, '|', Color.LightGray);
            }

            // Top door.
            if (Doors[1])
            {
                Renderer.PrintAt(Constants.MapX + Width / 2 - 1, Constants.MapY - 1, '.', Color.LightGray);
                Renderer.PrintAt(Constants.MapX + Width / 2    , Constants.MapY - 1, '.', Color.LightGray);
            }
            else
            {
                Renderer.PrintAt(Constants.MapX + Width / 2 - 1, Constants.MapY - 1, '-', Color.LightGray);
                Renderer.PrintAt(Constants.MapX + Width / 2    , Constants.MapY - 1, '-', Color.LightGray);
            }

            // Left door.
            if (Doors[2])
            {
                Renderer.PrintAt(Constants.MapX - 1, Constants.MapY + Height / 2 - 1, ':', Color.LightGray);
            }
            else
            {
                Renderer.PrintAt(Constants.MapX - 1, Constants.MapY + Height / 2 - 1, '|', Color.LightGray);
            }

            // Bottom door.
            if (Doors[3])
            {
                Renderer.PrintAt(Constants.MapX + Width / 2 - 1, Constants.MapY + Height, '.', Color.LightGray);
                Renderer.PrintAt(Constants.MapX + Width / 2    , Constants.MapY + Height, '.', Color.LightGray);
            }
            else
            {
                Renderer.PrintAt(Constants.MapX + Width / 2 - 1, Constants.MapY + Height, '-', Color.LightGray);
                Renderer.PrintAt(Constants.MapX + Width / 2    , Constants.MapY + Height, '-', Color.LightGray);
            }
        }

        public bool IsPos(int x, int y)
        {
            return X == x && Y == y;
        }
    }
}
