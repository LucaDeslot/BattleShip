using System.IO.Compression;

public class AttackResult
{
    public char PlayerAttackResult { get; set; } // 'M' pour manqué, 'H' pour touché
    public Coordinates IACoordinates { get;} = new Coordinates();
    public char IAAttackResult { get; set; } // 'M' pour manqué, 'H' pour touché
    public string? Winner { get; set; }
}
