using BulletHell;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using TMPro;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using static Unity.VisualScripting.Member;
using Debug = UnityEngine.Debug;
public class GameManager : SingletonMonoBehaviour<GameManager>
{
    [ReadOnly] public LevelsDataSO LevelsDataSO;
    [ReadOnly] public SongClipSO SongClipSO;
    public float delayTime = 2f;
    public int songElement;
    [Foldout("Reffernet-DontTouch", true)]
    private bool startGame = false;
    public ProjectileManager projectileManager;
    public TextMeshProUGUI endGame;
    public Button btnRestart;
    public Stopwatch gameTimer = new Stopwatch();
    public AudioSource audioSource;

    // public SongClipSO._SongClipEnum _SongClipEnum;
    public LevelData levelData;
    private int currenNode;
    public IceCreamEmmiter[] iceCreamEmmiters;
    public IceCreamEmmiter[] ngonKemEmmiters;
    public GameObject hackGO;
    public Toggle toggle;
    private void Start()
    {
        StartCoroutine(NewGame());
    }
    IEnumerator NewGame()
    {
        yield return new WaitForSeconds(2);
        startGame = true;
        // levelData = DataManager.Instance.levelDataDictionary.dictionary[fileLocation];
        levelData = LevelsDataSO.LevelDataArray[(int)songElement];
        StartStopWatch();
    }
    private void Update()
    {
        if (!startGame) { return; }
        if (currenNode >= levelData.lstNode.Count)
        {
            gameTimer.Stop();

        }
        if (gameTimer.IsRunning)
        {

            if (gameTimer.ElapsedMilliseconds > levelData.lstNode[currenNode].time)
            {
                switch (levelData.lstNode[currenNode].types)
                {
                    case IcecreamType.Full: iceCreamEmmiters[levelData.lstNode[currenNode].line - 1].SpawnOnePrijectTile(); break;
                    case IcecreamType.NgonKem: ngonKemEmmiters[levelData.lstNode[currenNode].line - 1].SpawnOnePrijectTile(); break;
                }

                currenNode++;

            }

        }
        else
        {

            if (ProjectileManager.Instance.totalProjectTile <= 0)
            {
                EndGame();
            }
        }
    }


    public void StartStopWatch()
    {
        currenNode = 0;
        gameTimer.Start();

        StartCoroutine(PlaySound());
    }
    public void ReStart()
    {
        btnRestart.gameObject.SetActive(false);
        StartCoroutine(restart());
    }
    IEnumerator restart()
    {
        if (startGame) yield break;
        endGame.gameObject.SetActive(false);
        yield return new WaitForSeconds(2f);
        startGame = true;
        foreach (var x in iceCreamEmmiters)
        {
            x.ClearAllProjectiles();
        }
        foreach (var x in ngonKemEmmiters)
        {
            x.ClearAllProjectiles();
        }
        gameTimer.Stop();
        audioSource.Stop();
        projectileManager.Stop();

        gameTimer.Restart();
        StartStopWatch();

        projectileManager.Play();
    }
    IEnumerator PlaySound()
    {
        yield return new WaitForSeconds(delayTime);
        audioSource.clip = SongClipSO.GetSfx(songElement);
        audioSource.Play();
    }
    public void EndGame()
    {
        btnRestart.gameObject.SetActive(true);

        foreach (var x in iceCreamEmmiters)
        {
            x.ClearAllProjectiles();
        }
        foreach (var x in ngonKemEmmiters)
        {
            x.ClearAllProjectiles();
        }
        ProjectileManager.Instance.Refress();
        endGame.gameObject.SetActive(true);
        audioSource.Stop();
        projectileManager.Stop();
        startGame = false;

    }
    public void HackMode()
    {
        hackGO.SetActive(toggle.isOn);
    }
}
