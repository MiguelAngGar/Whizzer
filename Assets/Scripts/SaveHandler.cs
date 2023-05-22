using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class SaveHandler : MonoBehaviour
{
    Dictionary<string, Tilemap> tilemaps = new Dictionary<string, Tilemap>();
    // Dictionary for saving
    Dictionary<TileBase, BuildingObjectBase> tileBaseToBuildingObject = new Dictionary<TileBase, BuildingObjectBase>();
    //Dictionary for loading
    Dictionary<string, TileBase> guidToTileBase = new Dictionary<string, TileBase>();

    [SerializeField] BoundsInt bounds;
    [SerializeField] string filename = "tilemapData.json";

    private void Start()
    {
        InitTilemaps();
        InitTileReferences();
    }

    private void InitTileReferences()
    {
        BuildingObjectBase[] buildables = Resources.LoadAll<BuildingObjectBase>("Scriptables/Buildables");

        Debug.Log(buildables);

        foreach (BuildingObjectBase buildable in buildables)
        {
            if (!tileBaseToBuildingObject.ContainsKey(buildable.TileBase))
            {
                tileBaseToBuildingObject.Add(buildable.TileBase, buildable);
                guidToTileBase.Add(buildable.name, buildable.TileBase);
            }
            else
            {
                Debug.LogError("El TileBase: '" + buildable.TileBase.name + "' ya está siendo usado por: '" + tileBaseToBuildingObject[buildable.TileBase].name + "'");
            }
        }
    }

    private void InitTilemaps()
    {
        Tilemap[] maps = FindObjectsOfType<Tilemap>();

        foreach (var map in maps)
        {
            tilemaps.Add(map.name, map);
        }
    }

    public void OnSave() {
        List<TilemapData> data = new List<TilemapData>();

        foreach (var tm in tilemaps)
        {
            if (tm.Key != "PreviewMap")
            {
                TilemapData mapData = new TilemapData();
                mapData.key = tm.Key;

                for (int x = bounds.xMin; x < bounds.xMax; x++)
                {
                    for (int y = bounds.yMin; y < bounds.yMax; y++)
                    {
                        Vector3Int pos = new Vector3Int(x, y, 0);
                        TileBase tile = tm.Value.GetTile(pos);

                        if (tile != null && tileBaseToBuildingObject.ContainsKey(tile))
                        {
                            string guid = tileBaseToBuildingObject[tile].name;
                            TileInfo ti = new TileInfo(guid, pos);
                            mapData.tiles.Add(ti);
                        }
                    }
                }

                data.Add(mapData);
            }
        }

        FileHandler.SaveToJSON<TilemapData>(data, filename);
    }

    public void OnLoad() {
        List<TilemapData> data = FileHandler.ReadListFromJSON<TilemapData>(filename);

        foreach(var mapData in data)
        {
            if (!tilemaps.ContainsKey(mapData.key))
            {
                Debug.LogError("Hay datos guardados para el tilemap: '" + mapData.key + "', pero este no existe. Este no se reproducirá.");
                continue;
            }

            var map = tilemaps[mapData.key];

            map.ClearAllTiles();

            if (mapData.tiles != null && mapData.tiles.Count > 0)
            {
                foreach(var tile in mapData.tiles)
                {
                    if (guidToTileBase.ContainsKey(tile.guidForBuildable))
                    {
                        map.SetTile(tile.pos, guidToTileBase[tile.guidForBuildable]);
                    } else
                    {
                        Debug.LogError("La referencia " + tile.guidForBuildable + " no existe.");
                    }
                }
            }
        }
    }

}

[Serializable]
public class TilemapData
{
    public string key;
    public List<TileInfo> tiles = new List<TileInfo>();
}

[Serializable]
public class TileInfo
{
    public string guidForBuildable;
    public Vector3Int pos;

    public TileInfo(string guid, Vector3Int pos)
    {
        guidForBuildable = guid;
        this.pos = pos;
    }
}