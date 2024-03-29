public class StartGameResponse
{
    public Guid Id { get; set; }
    public List<Ship> Ships { get; set; }
    
    public int gridSize { get; set; }
    
    public StartGameResponse(Guid id, List<Ship> ships, int gridSize)
    {
        Id = id;
        Ships = ships;
        this.gridSize = gridSize;
    }
}
