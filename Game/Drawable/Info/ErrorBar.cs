using Godot;

public class ErrorBar : IDrawable
{
    private struct ErrorPoint
    {
        public float Delta;
        public Color Color;

        public ErrorPoint(float delta, Color color)
        {
            Delta = delta;
            Color = color;
        }
    }

    private Color _maxColor = new Color(0f, 0.975f, 1f);
    private Color _goodColor = new Color(0.024f, 1f, 0f);
    private Color _badColor = new Color(1f, 0.173f, 0.886f);
    private Color _missColor = new Color(1f, 0.94f, 0.94f);

    private Vector2 _position;
    private Align _horizontalAlign;
    private Align _verticalAlign;

    private ErrorPoint[] _errorPoints;
    private int _limit;
    private int currentIndex = 0;

    public Vector2 ExactPosition;

    public ErrorBar(int limit, Align horizontal, Align vertical)
    {
        _errorPoints = new ErrorPoint[limit];
        _limit = limit;
        _horizontalAlign = horizontal;
        _verticalAlign = vertical;
    }

    public void Update(float deltaTime, JudgeScoreSystem.JudgeName judgeName)
    {
        currentIndex += 1;
        if (currentIndex == _limit)
            currentIndex = 0;

        switch(judgeName)
        {
            case (JudgeScoreSystem.JudgeName.Max):
                AddPoint(deltaTime, _maxColor);
                break;
            case (JudgeScoreSystem.JudgeName.Good):
                AddPoint(deltaTime, _goodColor);
                break;
            case (JudgeScoreSystem.JudgeName.Bad):
                AddPoint(deltaTime, _badColor);
                break;
            case (JudgeScoreSystem.JudgeName.Miss):
                AddPoint(deltaTime, _missColor);
                break;
        }
    }

    public void UpdatePosition()
    {
        _position = ExactPosition;
    }

    public void AddPoint(float deltaTime, Color color)
    {
        _errorPoints[currentIndex] = new ErrorPoint(
            -deltaTime,
            color
        );
    }

    public void Draw(Godot.Node2D node)
    {
        node.DrawLine(
            new Vector2(
                _position.x, 
                _position.y - 20
            ),
            new Vector2(
                _position.x,
                _position.y + 40
            ),
            new Color(1, 1, 1, 1),
            3
        );

        foreach(ErrorPoint point in _errorPoints)
        {
            node.DrawLine(
                new Vector2(
                    _position.x + point.Delta, 
                    _position.y
                ),
                new Vector2(
                    _position.x + point.Delta,
                    _position.y + 20
                ),
                point.Color,
                3
            );
        }
    }
}