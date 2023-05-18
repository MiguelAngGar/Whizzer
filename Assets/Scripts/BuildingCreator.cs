using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class BuildingCreator : Singleton<BuildingCreator>
{
    [SerializeField] Tilemap previewMap;
    [SerializeField] Tilemap defaultMap;

    PlayerInput playerInput;
    bool isDragging = false;
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
        playerInput.Editor.Click.started += OnDragStarted;
        playerInput.Editor.Click.canceled += OnDragCancelled;
    }

    private void OnDisable()
    {
        playerInput.Disable();
        playerInput.Editor.Position.performed -= OnMouseMove;
        playerInput.Editor.Click.performed -= OnClick;
        playerInput.Editor.Click.started -= OnDragStarted;
        playerInput.Editor.Click.canceled -= OnDragCancelled;
    }

    private void OnDragStarted(InputAction.CallbackContext ctx)
    {
        isDragging = true;
    }
    private void OnDragCancelled(InputAction.CallbackContext ctx)
    {
        isDragging = false;
    }

    private void OnMouseMove(InputAction.CallbackContext ctx)
    {
        mousePos = ctx.ReadValue<Vector2>();

        if (isDragging)
        {
            if (selectedObj != null && !EventSystem.current.IsPointerOverGameObject())
                HandleDrawing();
        }
    }

    private void OnClick(InputAction.CallbackContext ctx)
    {
        if (selectedObj != null && !EventSystem.current.IsPointerOverGameObject())
            HandleDrawing();
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

    public void ObjectSelected(BuildingObjectBase obj)
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

    private void HandleDrawing()
    {
        DrawItem();
    }

    private void DrawItem()
    {
        defaultMap.SetTile(currentGridPosition, tileBase);
    }
}
