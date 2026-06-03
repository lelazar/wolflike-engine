// This class gives us a small reusable notification queue

using System.Collections.Generic;

namespace WolfLike.src.UI;

public class MessageLog
{
    private const int MAX_MESSAGES = 4;
    private const float DEFAULT_MESSAGE_DURATION_SECONDS = 2.0f;

    private readonly List<GameMessage> _messages = new();

    public IReadOnlyList<GameMessage> Messages => _messages;

    public void Add(string text)
    {
        Add(text, DEFAULT_MESSAGE_DURATION_SECONDS);
    }

    public void Add(string text, float duration)
    {
        if (string.IsNullOrWhiteSpace(text)) return;

        _messages.Add(new GameMessage(text, duration));

        while (_messages.Count > MAX_MESSAGES)
            _messages.RemoveAt(0);
    }

    public void Update(float deltaTime)
    {
        foreach (GameMessage message in _messages)
            message.Update(deltaTime);
        _messages.RemoveAll(message => message.IsExpired);
    }

    public void Clear()
    {
        _messages.Clear();
    }
}
