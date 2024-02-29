using System.IO.Compression;

public class AttackResult
{
    public char PlayerAttackResult { get; set; } // 'M' pour manqué, 'H' pour touché
    public Coordinates IACoordinates { get; set;} = new Coordinates();
    public char IAAttackResult { get; set; } // 'M' pour manqué, 'H' pour touché
    public string? Winner { get; set; }
    
    public AttackResult(char playerAttackResult, Coordinates iaCoordinates, char iaAttackResult, string? winner)
    {
        PlayerAttackResult = playerAttackResult;
        IACoordinates = iaCoordinates;
        IAAttackResult = iaAttackResult;
        Winner = winner;
    }
}
