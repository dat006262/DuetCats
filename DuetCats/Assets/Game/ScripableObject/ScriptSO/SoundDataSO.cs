using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Core/Sound")]
public class SoundDataSO : ScriptableObject
{
    public enum _SoundEnum
    {
        KnifeDown,
        CutTrue

    }

    [SerializeField] private AudioClip[] _arrMusic;
    [SerializeField] private AudioClip[] _arrSfx;

    private int _idMusic;

    public AudioClip GetMusic
    {
        get
        {
            if (_arrMusic.Length <= 0) return null;
            var result = _arrMusic[_idMusic];
            _idMusic++;
            if (_idMusic >= _arrMusic.Length) _idMusic = 0;
            return result;
        }
    }

    private int GetIndexSfx(_SoundEnum sound) => sound switch
    {
        _SoundEnum.KnifeDown => 0,
        _SoundEnum.CutTrue => 1,
        _ => 0
    };

    public AudioClip GetSfx(_SoundEnum sound) => _arrSfx[GetIndexSfx(sound)];

}
