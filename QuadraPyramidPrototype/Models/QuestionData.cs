namespace QuadraPyramidPrototype.Models;

// Property names match the JSON names (JSON loading ignores letter case).
public class LevelData
{
    public int Level { get; set; }
    public string Title { get; set; } = "";
    public QuestionData[] Questions { get; set; } = Array.Empty<QuestionData>();
}

public class QuestionData
{
    public int Id { get; set; }
    public string Question { get; set; } = "";
    public string[] Choices { get; set; } = Array.Empty<string>();
    public int Answer { get; set; }
    public string Explanation { get; set; } = "";
}
