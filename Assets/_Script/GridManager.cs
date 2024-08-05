using UnityEngine;

/// <summary>
/// Helper class that connects the Grid component and Grid shader and allows other scripts to access the data from the grid
/// </summary>
public class GridManager : MonoBehaviour
{
    [SerializeField]
    private Grid grid;
    [SerializeField]
    private Renderer gridRenderer;

    [SerializeField]
    private Vector3 gridCellSize;
    private Vector3 halfGridCellSize;

    [SerializeField]
    private string cellSizeParameter = "_GridSize";

    private void Start()
    {
        grid.cellSize = gridCellSize;
        halfGridCellSize = gridCellSize / 2f;

        gridRenderer.material.SetVector(cellSizeParameter, new Vector2(1 / gridCellSize.x, 1 / gridCellSize.z));
    }

    public Vector3 GetOffsetForRotation(int rotationValue)
    {
        if (rotationValue == 90)
        {
            return new Vector3(0, 0, gridCellSize.z);
        }
        else if (rotationValue == 180)
        {
            return new Vector3(gridCellSize.x, 0, gridCellSize.z);
        }
        else if (rotationValue == 270)
        {
            return new Vector3(gridCellSize.x, 0, 0);
        }
        return Vector3.zero;
    }
    public Vector3Int GetCellPosition(Vector3 worldPosition, PlacementType placementType)
    {
        if (placementType.IsEdgePlacement())
            worldPosition += halfGridCellSize;
        return grid.WorldToCell(worldPosition);
    }

    public Vector3 GetWorldPosition(Vector3Int cellPosition)
    {
        //we subtract y value because the grid is placed a bit higher
        //to make if show on top of the floor tiles that we place
        return grid.CellToWorld(cellPosition);
    }

    public Vector3 GetCenterPositionForCell(Vector3Int cellPosition)
    {
        return GetWorldPosition(cellPosition) + halfGridCellSize;
    }

    public void ToggleGrid(bool value)
    {
        gridRenderer.gameObject.SetActive(value);
    }
}

/// <summary>
/// Placement types. A better idea would be to try to create objects from it ex using ScriptableObjects.
/// Still enum works well for a prototype.
/// </summary>
public enum PlacementType
{
    None,
    Floor,
    Wall,
    InWalls,
    NearWallObject,
    FreePlacedObject
}

/// <summary>
/// Because of the limitation of using enum the end result is that you need extensions methods
/// since you can't easily add more data to an enum. This way I can reliably access the additional data
/// without having to check each if / switch statement where I have used the enum.
/// </summary>
public static class PlacementTypeExtensions
{
    public static bool IsEdgePlacement(this PlacementType placementType)
    => placementType switch
        {
            PlacementType.Wall => true,
            PlacementType.InWalls => true,
            _ => false
        };
    public static bool IsObjectPlacement(this PlacementType placementType)
    => placementType switch
    {
        PlacementType.FreePlacedObject => true,
        PlacementType.NearWallObject => true,
        _ => false
    };
}