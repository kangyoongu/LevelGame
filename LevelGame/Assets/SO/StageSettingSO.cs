using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ClearTarget
{
    public int nodeLevel;
    public int count;

}
[Serializable]
public struct SpawnNode
{
    public int turnCount;
    public int spawnLevel;
}
[Serializable]
public struct StartFormat
{
    public int blockNum;
    public int spawnLevel;
    public bool bomb;
}

[Serializable]
public enum Game : short
{
    Normal = 0,
    BlockBlock = 1,
    MultiMerge = 2
}

[CreateAssetMenu(fileName = "StageSettingData", menuName = "SO/StageSettingData")]
public class StageSettingSO : ScriptableObject
{
    public Game gameMode = Game.Normal;
    public List<ClearTarget> clearTarget = new List<ClearTarget>();
    public List<StartFormat> startFormat = new List<StartFormat>();
    public int startNodeCount;
    public List<SpawnNode> spawnNode = new List<SpawnNode>();
}
