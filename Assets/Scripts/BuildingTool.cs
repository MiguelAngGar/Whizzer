using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum ToolType
{
    none,
    Eraser
}

[CreateAssetMenu(fileName ="Tool", menuName ="LevelBuilding/Create Tool")]
public class BuildingTool : BuildingObjectBase
{
    [SerializeField] private ToolType toolType;

    public void Use(Vector3Int position)
    {
        ToolController tc = ToolController.GetInstance();

        switch(toolType)
        {
            case ToolType.Eraser:
                tc.Eraser(position);
                break;
            default:
                Debug.Log("No se ha marcado el tipo de herramienta");
                break;
        }
    }
}
