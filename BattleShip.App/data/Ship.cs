public class Ship
{
    public char Type { get; set; }
    public int StartRow { get; set; }
    public int StartCol { get; set; }
    public int CurrentSize { get; set; }
    public int Size { get; set; }
    public bool IsHorizontal { get; set; }
    
    public void fromGrpc(ShipGrpc shipGrpc) {
        Type = shipGrpc.Type[0];
        StartRow = shipGrpc.StartRow;
        StartCol = shipGrpc.StartCol;
        //TODO: CurrentSize
        Size = shipGrpc.Size;
        IsHorizontal = shipGrpc.IsHorizontal;
    }
}
