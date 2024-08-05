
/// <summary>
/// Stores all the data about the objects that are on our map
/// </summary>
public class GridData
{
    /// <summary>
    /// I have decided to reuse a singel structure.
    /// I bet you could condence it to a bigger data class.
    /// On the other hand this way it is easy to add new placement type
    /// ex OnWallObjects - paintings etc
    /// </summary>
    public PlacementGridData WallPlacementData { private set; get; }
    public PlacementGridData FloorPlacementData { private set; get;}
    public PlacementGridData ObjectPlacementData { private set; get;}
    public PlacementGridData InWallPlacementData { private set; get;}

    public GridData()
    {
        //Because of how we place walls (we assign them to belong to the cells right or bottom edge)
        //
        //   |_|_|_
        //   |_|_|_ <-- cell placement ends in the middle tile but to place wall we need the +1 in both X and Z axis
        //   |_|_|_
        //
        //We need to access the cell outside the grid visible area to assign them
        //That is why we add +1 to the max bounds of wall placement (not to floor or cell)
        FloorPlacementData = new(-10,9,-10,9);
        WallPlacementData = new(-10, 10, -10, 10);
        ObjectPlacementData = new(-10, 9, -10, 9);
        InWallPlacementData = new(-10, 10, -10, 10);
    }


}
