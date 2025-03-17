using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapVisualizer : MonoBehaviour
{
    [SerializeField] private Tilemap floorTilemap, wallTilemap, minimap;
    [SerializeField] private TileBase floorTile, wallTile;
    [SerializeField] private TileBase minimapFloor, minimapWall;
    [SerializeField] private GameObject doorParent; // up/down, left, right
    [SerializeField] private GameObject[] doors; // front. side

    public void VisualizeLayout(HashSet<Vector2Int> floor)
    {
        PlaceTiles(floorTilemap, floorTile, floor);
        DrawMinimap(floor, minimapFloor);
        HashSet<Vector2Int> wall = GetAroundFloor(floor, 1);
        PlaceTiles(wallTilemap, wallTile, wall);
        wall = ConvertWallToMinimap(floor);
        DrawMinimap(wall, minimapWall);
    }

    private void PlaceTiles(Tilemap tilemap, TileBase tile, HashSet<Vector2Int> positions)
    {
        foreach (var pos in positions)
        {
            PlaceTile(tilemap, tile, pos);
        }
    }

    private void PlaceTile(Tilemap floorTilemap, TileBase tile, Vector2Int pos)
    {
        floorTilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), tile);
    }

    public void ClearMap()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
        minimap.ClearAllTiles();
        //clear doors
        while (doorParent.transform.childCount > 0) {
            DestroyImmediate(doorParent.transform.GetChild(0).gameObject);
        }
    }

    public void DrawMinimap(HashSet<Vector2Int> floor, TileBase tile)
    {
        foreach (var pos in floor)
        {
            // PlaceTile(minimap, tile, pos);
            MinimapData.minimapTiles[pos] = tile;
        }
    }

    private HashSet<Vector2Int> GetAroundFloor(HashSet<Vector2Int> floorPositions, int bottomHeight)
    {
        HashSet<Vector2Int> offsetBottomFloor = new HashSet<Vector2Int>();
        foreach (var pos in floorPositions)
        {
            for(int i = 0; i < bottomHeight; i++)
            {
                if (floorPositions.Contains(pos + Vector2Int.up*(i+1))) //move entire floor up 
                {
                    offsetBottomFloor.Add(pos + Vector2Int.up*(i+1));
                }
            }

        }
        
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();
        foreach (var pos in offsetBottomFloor) //get tiles around offseted floor
        {
            foreach (var direction in Direction2D.AllDirections)
            {
                if (!offsetBottomFloor.Contains(pos + direction))
                {
                    if(direction.Equals(Vector2Int.right) || direction.Equals(Vector2Int.down))
                    {
                        wallPositions.Add(pos + direction*2);
                    } else {
                        wallPositions.Add(pos + direction);
                    }
                }
            }
        }
        return wallPositions;
    }

    private HashSet<Vector2Int> ConvertWallToMinimap(HashSet<Vector2Int> floorPositions)
    {
        HashSet<Vector2Int> minimapWallPositions = new HashSet<Vector2Int>();
        foreach (var pos in floorPositions) //get tiles around offseted floor
        {
            foreach (var direction in Direction2D.AllDirections)
            {
                if (!floorPositions.Contains(pos + direction))
                {
                    minimapWallPositions.Add(pos + direction);
                }
            }
        }
        return minimapWallPositions;
    }

}