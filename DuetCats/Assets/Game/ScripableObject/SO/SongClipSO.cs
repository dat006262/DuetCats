using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SoundDataSO;
[CreateAssetMenu(fileName = "SongClipSO")]
public class SongClipSO : ScriptableObject
{
    public enum _SongClipEnum
    {
        WiiChannels = 0,
        Lost_Memories = 1,
        Badguy = 4

    }
    [SerializeField] private AudioClip[] _lstSong;



    private int GetIndexSfx(_SongClipEnum sound)
    {
        return (int)sound;
    }

    public AudioClip GetSfx(_SongClipEnum sound) => _lstSong[GetIndexSfx(sound)];
    public AudioClip GetSfx(int soundElement) => _lstSong[soundElement];
    public int GetLength => _lstSong.Length;
}
