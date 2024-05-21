using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class DataManager : SingletonMonoBehaviour<DataManager>
{
    public LevelDataDictionary levelDataDictionary;
    public override void Awake()
    {
        base.Awake();

        //   levelDataDictionary = new LevelDataDictionary();
    }
    private void Start()
    {
        levelDataDictionary = JsonConvert.DeserializeObject<LevelDataDictionary>(Const.DICTIONARY_LEVELDATA);
    }
}
