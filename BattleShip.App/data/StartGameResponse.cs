public class StartGameResponse
{
    public Guid Id { get; set; }
    public List<Ship> Ships { get; set; }
    
    public int gridSize { get; set; }
}
