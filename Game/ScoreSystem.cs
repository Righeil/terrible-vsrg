using Godot;
using System;
using System.Collections.Generic;

public interface IScoreSystem
{
    void ProcessHit(float deltaTime);
    void ProcessMiss();
}

public class ScoreSystem
{
    private Info _info; // Ради одного метода тащить сюда это гавно

    public Dictionary<string, IScoreSystem> Container;

    public ScoreSystem(Info info)
    {
        Container = new Dictionary<string, IScoreSystem>
        {
            {"Base", new BaseScoreSystem()},
            {"Osu", new OsuScoreSystem(8)},
            {"Wife", new WifeScoreSystem(1)},
            {"Hp", new HpScoreSystem(1, 0.05f, 0.1f)}
        };

        _info = info;
    }

    public void ProcessHit(float deltaTime)
    {
        foreach(var scoreSystem in Container)
        {
            scoreSystem.Value.ProcessHit(deltaTime);
        }

        _info.UpdateValues();
    }

    public void ProcessMiss()
    {
        foreach(var scoreSystem in Container)
        {
            scoreSystem.Value.ProcessMiss();
        }

        _info.UpdateValues();
    }
}