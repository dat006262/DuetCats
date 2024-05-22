using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "SongDataSplit")]
public class SongDataSplit : ScriptableObject
{

    public List<SongElement> songData = new List<SongElement>();


}
[System.Serializable]
public class SongElement
{
    public float from;
    public float to;
    public List<Note> data = new List<Note>();

    public SongElement(float from, float to, List<Note> data)
    {
        this.from = from;
        this.to = to;
        this.data = data;
    }
}