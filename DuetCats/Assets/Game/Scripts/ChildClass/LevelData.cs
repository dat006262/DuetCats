using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData
{
    public string SongName;
    public List<Note> lstNode;
    public LevelData()
    {
        lstNode = new List<Note>();
    }
    public LevelData(List<Note> p_lstTimeSpawn)
    {
        lstNode = new List<Note>();

        foreach (var p in p_lstTimeSpawn)
        {
            lstNode.Add(p);
        }
    }
}
[System.Serializable]
public class Note
{
    public float time;
    public int length;
    public IcecreamType types;
    public int line;

    public Note(float time, int line, IcecreamType icecreamType, int length = 0)
    {
        this.time = time;
        this.line = line;
        types = icecreamType;
        this.length = length;
    }
}
public enum IcecreamType
{
    Full,
    NgonKem
}