using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SoundDataSO;
[CreateAssetMenu(menuName = "Core/Paritical")]
public class ParticalDataSO : ScriptableObject
{
    public enum _PariticalEnum
    {
        YellowExplo,
        RedExplo

    }
    [SerializeField] private ParticleSystem[] _pariticalSys;


    private int GetIndexParitical(_PariticalEnum paritical) => paritical switch
    {
        _PariticalEnum.YellowExplo => 0,
        _PariticalEnum.RedExplo => 1,
        _ => 0
    };

    public ParticleSystem GetParitical(_PariticalEnum paritical) => _pariticalSys[GetIndexParitical(paritical)];
}
