namespace WolfLike.src.UI;

public class GameMessage
{
    public string Text { get; }
    public float TimeRemaining { get; private set; }
    public float TotalDuration { get; }
    public bool IsExpired => TimeRemaining <= 0.0f;
    public float NormalizedLifetime
    {
        get
        {
            if (TotalDuration <= 0.0f)
                return 0.0f;
            return TimeRemaining / TotalDuration;
        }
    }

    public GameMessage(string text, float duration)
    {
        Text = text;
        TimeRemaining = duration;
        TotalDuration = duration;
    }

    public void Update(float deltaTime)
    {
        if (TimeRemaining > 0.0f)
            TimeRemaining -= deltaTime;
    }
}
