using Godot;

public class Playfield : Node2D
{
    private Main _main;
    private Chart _chart;
    private Skin _skin;

    private Conveyor _conveyor;
    private Info _info;
    private Audio _audio;

    private GameLogic _gameLogic;
    private TimeLogic _timeLogic;
    private ScoreSystem _scoreSystem;
    private NoteLogic _noteLogic;
    private InputLogic _inputLogic;

    private float _timeRate;

    public Playfield() {}
    public Playfield(Main main, Skin skin, Chart chart, Audio audio, Settings settings, float timeRate)
    {
        _main = main;
        _chart = chart;
        _timeRate = timeRate;
        _skin = skin;
        _audio = audio;
        
        int[] inputMap = settings.InputMap[chart.InputMode];

        _scoreSystem = new ScoreSystem();
        _noteLogic = new NoteLogic(
            _scoreSystem,
            lateMiss: 155,
            timeRate: timeRate
        );

        _timeLogic = new TimeLogic(
            audio: _audio,
            prepareTime: -_chart.Notes[0].Time + settings.PrepareTime,
            timeRate: _timeRate,
            afterPauseTimeDecrease: 750,
            pauseCooldown: 5000
        );
        
        _inputLogic = new EarlyInputLogic(
            inputMode: _chart.InputMode, 
            scoreSystem: _scoreSystem, 
            hitWindow: 155f,
            timeRate: _timeRate,
            inputMap: inputMap
        );

        _gameLogic = new GameLogic(
            notes: ref _chart.Notes,
            playfield: this,
            timeLogic: _timeLogic,
            noteLogic: _noteLogic,
            inputLogic: _inputLogic
        );

        _conveyor = new RealTimeConveyor(
            notes: ref _chart.Notes,
            skin: _skin,
            timeLogic: _timeLogic,
            gameLogic: _gameLogic,
            scrollSpeed: settings.ScrollSpeed,
            timeRate: _timeRate
        );
        AddChild(_conveyor);

        _info = new Info(
            _skin,
            _scoreSystem
        );
        AddChild(_info);
        _scoreSystem.After(_info); // yes

        AddChild(_audio);
    }

    public override void _Process(float delta)
    {  
        _timeLogic.Process(delta);
        _gameLogic.Process();
    }

    public override void _PhysicsProcess(float delta)
    {
        _gameLogic.FixedProcess();
    }

    public override void _Input(InputEvent input)
    {
        _gameLogic.Input(input);

        if (input is InputEventKey keyEvent)
        {
            if (Input.IsActionJustPressed("restart_chart"))
                GetTree().ReloadCurrentScene();

            if (Input.IsActionJustPressed("pause"))
                _timeLogic.SetPause();

            if (Input.IsActionJustPressed("return_to_song_select"))
                _main.SetToSongSelect();
        }
    }

    public void SizeChanged()
    {
        _skin.Update();
    }

    public void ChartEnded()
    {
        _main.SetToResultScreen(_chart, _scoreSystem);
    }
}