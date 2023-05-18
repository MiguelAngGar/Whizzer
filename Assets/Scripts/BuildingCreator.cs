using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class BuildingCreator : Singleton<BuildingCreator>
{
    [SerializeField] Tilemap previewMap;

    PlayerInput playerInput;

    Vector2 mousePos;

    TileBase tileBase;
    BuildingObjectBase selectedObj;

    Vector3Int currentGridPosition;
    Vector3Int lastGridPosition;

    Camera _camera;

    protected override void Awake()
    {
        base.Awake();
        playerInput = new PlayerInput();
        _camera = Camera.main;
    }

    private void OnEnable()
    {
        playerInput.Enable();
        playerInput.Editor.Position.performed += OnMouseMove;
        playerInput.Editor.Click.performed += OnClick;
    }

    private void OnDisable()
    {
        playerInput.Disable();
        playerInput.Editor.Position.performed -= OnMouseMove;
        playerInput.Editor.Click.performed -= OnClick;
    }

    private void OnMouseMove(InputAction.CallbackContext ctx)
    {
        mousePos = ctx.ReadValue<Vector2>();
    }

    private void OnClick(InputAction.CallbackContext ctx)
    {

    }

    private BuildingObjectBase SelectedObj
    {
        set
        {
            selectedObj = value;
            tileBase = selectedObj != null ? selectedObj.TileBase : null;
            UpdatePreview();
        }
    }

    public void ObjectSelected (BuildingObjectBase obj)
    {
        SelectedObj = obj;

        // Set preview where mouse is
        // on click draw
        // on right click cancel drawing
    }

    private void Update()
    {
        // if something is selected - show preview
        if (selectedObj != null)
        {
            Vector3 pos = _camera.ScreenToWorldPoint(mousePos);
            Vector3Int gridPos = previewMap.WorldToCell(pos);

            if (gridPos != currentGridPosition)
            {
                lastGridPosition = currentGridPosition;
                currentGridPosition = gridPos;

                UpdatePreview();
            }
        }
    }

    private void UpdatePreview()
    {
        previewMap.SetTile(lastGridPosition, null);
        previewMap.SetTile(currentGridPosition, tileBase);
    }
}
