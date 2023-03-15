namespace CommandLibrary;

[Serializable]
public class Command
{
    public CommandText Text { get; set; }
    public string? Param { get; set; }
}
