using System.Collections.Generic;
using UnityEngine;

public class FreeObjectNewRotationPlacementStrategy : SelectionStrategy
{
    protected PlacementGridData wallPlacementData, inWallPlacementData;
    public FreeObjectNewRotationPlacementStrategy(PlacementGridData placementData, PlacementGridData wallPlacementData, PlacementGridData inWallPlacementData, GridManager gridManager) : base(placementData, gridManager)
    {
        this.wallPlacementData = wallPlacementData;
        this.inWallPlacementData = inWallPlacementData;
    }

    /// <summary>
    /// In this placement strategy first selection is the last selection. We just sent the onFinished event when we let go of the mouse button
    /// </summary>
    /// <param name="mousePosition"></param>
    /// <param name="selectionData"></param>
    public override void StartSelection(Vector3 mousePosition, SelectionData selectionData)
    {
        this.lastDetectedPosition.Reset();
        ModifySelection(mousePosition, selectionData);
    }

    public override bool ModifySelection(Vector3 mousePosition, SelectionData selectionData)
    {
        Vector3Int tempPos = gridManager.GetCellPosition(mousePosition, selectionData.PlacedItemData.objectPlacementType);
        if (lastDetectedPosition.TryUpdatingPositon(tempPos) || selectionData.OldRotation != selectionData.Rotation)
        {
            //Clear selection data
            selectionData.Clear();
            selectionData.OldRotation = selectionData.Rotation;
            Vector3 modifiedPosition = gridManager.GetWorldPosition(lastDetectedPosition.GetPosition());
            //if (selectionData.Rotation.eulerAngles.y == 90)
            //{
            //    selectionData.AddToPreviewPositions(modifiedPosition + new Vector3(0, 0, 0.5f));
            //}
            //else if (selectionData.Rotation.eulerAngles.y == 180)
            //{
            //    selectionData.AddToPreviewPositions(modifiedPosition + new Vector3(0.5f, 0, 0.5f));
            //}
            //else if (selectionData.Rotation.eulerAngles.y == 270)
            //{
            //    selectionData.AddToPreviewPositions(modifiedPosition + new Vector3(0.5f, 0, 0));
            //}
            selectionData.AddToWorldPositions(
                modifiedPosition 
                + gridManager.GetOffsetForRotation(Mathf.RoundToInt(selectionData.Rotation.eulerAngles.y)));

            selectionData.AddToGridPositions(gridManager.GetCellPosition(modifiedPosition,PlacementType.FreePlacedObject));

            List<Quaternion> rotations = new() { selectionData.Rotation};
            selectionData.SetGridCheckRotation(rotations);
            selectionData.SetObjectRotation(rotations);

            selectionData.PlacementValidity = ValidatePlacement(selectionData);


            return true;
        }
        return false;
    }
    public override Quaternion HandleRotation(Quaternion rotation, SelectionData selectionData)
    {
        //selectionData.OldRotation = selectionData.Rotation;
        selectionData.SetObjectRotation(new() { rotation });
        selectionData.SetGridCheckRotation(new() { rotation });
        
        return rotation;
    }
    protected override bool ValidatePlacement(SelectionData selectionData)
    {

        bool validity = PlacementValidator.CheckIfPositionsAreValid(
            selectionData.GetSelectedGridPositions(),
            placementData,
            selectionData.PlacedItemData.size,
            selectionData.GetSelectedPositionsGridRotation(),
            false);

        //Only if the previous check was TRUE
        if (validity)
        {
            //Checks if the placement position is free in the ObjectPlacementData
            validity = PlacementValidator.CheckIfPositionsAreFree(
            selectionData.GetSelectedGridPositions(),
            placementData,
            selectionData.PlacedItemData.size,
            selectionData.GetSelectedPositionsGridRotation(),
            false);
        }
        if (validity) //IN WALL Objects
        {
            //When placing objects we need to check if because of their size
            //ex 2x1 we are not trying to place the second tile inside a wall (crossing a wall)

            validity = PlacementValidator.CheckIfNotCrossingEdgeObject(
            selectionData.GetSelectedGridPositions(),
            inWallPlacementData,
            selectionData.PlacedItemData.size,
            selectionData.GetSelectedPositionsGridRotation(),
            false);
        }
        if (validity) // WALL objects
        {
            //When placing objects we need to check if because of their size
            //ex 2x1 we are not trying to place the second tile inside a wall (crossing a wall)
            validity = PlacementValidator.CheckIfNotCrossingEdgeObject(
            selectionData.GetSelectedGridPositions(),
            wallPlacementData,
            selectionData.PlacedItemData.size,
            selectionData.GetSelectedPositionsGridRotation(),
            false);
        }
        return validity;
    }

    /// <summary>
    /// Handles objects rotation. If object is 2x1 because of how our prefab is set up (we always start placement from the Bottom-Left corner of the object)
    /// we only allow the rotation to be 0 or 90. We can easily add "mirror" functionality to add this ability.
    /// This constraint is purely to keep the data storage easier.
    /// </summary>
    /// <param name="rotation"></param>
    /// <param name="selectionData"></param>
    /// <returns></returns>


    public override void FinishSelection(SelectionData selectionData)
    {
        lastDetectedPosition.Reset();
    }
}
