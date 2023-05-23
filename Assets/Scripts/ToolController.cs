using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class ToolController : Singleton<ToolController>
{
    List<Tilemap> tilemaps = new List<Tilemap>();

    private void Start()
    {
        List<Tilemap> maps = FindObjectsOfType<Tilemap>().ToList();

        maps.ForEach(map =>
        {
            if (map.name != "PreviewMap")
            {
                tilemaps.Add(map);
            }
        });
    }

    public void Eraser(Vector3Int position)
    {
        tilemaps.ForEach(map =>
        {
            map.SetTile(position, null);
        });
    }
}
