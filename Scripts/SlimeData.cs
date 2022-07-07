using Godot;
using System;

public class SlimeData
{
    public SlimeData()
    {

    }

    public Difficulty AssociatedDifficulty;
    public int MaxLevel;
    public int MinLevel;
    public Slime Slime;

    public static System.Collections.Generic.List<SlimeData> GetEnemyList()
    {
        var enemyList = new System.Collections.Generic.List<SlimeData>();

        enemyList.Add(CreateSlime(Slime.BasicSlime, 0, 9, Difficulty.Easy));
        enemyList.Add(CreateSlime(Slime.BunnySlime, 10, 19, Difficulty.Easy));
        enemyList.Add(CreateSlime(Slime.WaterSlime, 20, 29, Difficulty.Easy));
        enemyList.Add(CreateSlime(Slime.PandaSlime, 30, 39, Difficulty.Normal));
        enemyList.Add(CreateSlime(Slime.PufferSlime, 40, 49, Difficulty.Normal));
        enemyList.Add(CreateSlime(Slime.LavaSlime, 50, 59, Difficulty.Hard));
        enemyList.Add(CreateSlime(Slime.FoxSlime, 50, 59, Difficulty.Hard));

        return enemyList;
    }

    private static SlimeData CreateSlime(Slime name, int minLevel, int maxLevel, Difficulty difficulty)
    {
        return new SlimeData()
                {
                    Slime = name,
                    MinLevel = minLevel,
                    MaxLevel = maxLevel,
                    AssociatedDifficulty = difficulty
                };
    }
}
