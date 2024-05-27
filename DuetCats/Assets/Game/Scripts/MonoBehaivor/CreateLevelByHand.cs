using AnotherFileBrowser.Windows;
using BulletHell;
using JetBrains.Annotations;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.MusicTheory;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
#if UNITY_EDITOR
[CustomEditor(typeof(CreateLevelByHand))]
public class CreateLevelByHandEditor : Editor
{

    public override void OnInspectorGUI()
    {
        CreateLevelByHand tool = (CreateLevelByHand)target;
        if (GUILayout.Button("MergeSongElementToLevelData"))
        {

            tool.LevelsDataSO.LevelDataArray[tool.songElement] = tool.MergeSongElement(tool.songDataSplit.SongElements);
            if (EditorUtility.DisplayDialog("RebuildLevelData?", $"Thay doi Data cua bai{tool.songElement}", "Save", "No"))
            {
                EditorUtility.SetDirty(tool.LevelsDataSO);
                AssetDatabase.SaveAssets();
            }
        }
        if (GUILayout.Button("ImportData"))
        {

            tool.ImportJson();
            if (EditorUtility.DisplayDialog("RebuildLevelData?", $"Thay doi Data cua bai{tool.ImportToLevelDataElement}", "Save", "No"))
            {
                EditorUtility.SetDirty(tool.LevelsDataSO);
                AssetDatabase.SaveAssets();
            }
        }
        //if (GUILayout.Button(" JsonConvertLevelManager"))
        //{
        //    tool.ReadFromFile();
        //    if (EditorUtility.DisplayDialog("RebuildLevelData?", $"Thay doi Data cua bai{tool.songElement}", "Save", "No"))
        //    {
        //        EditorUtility.SetDirty(tool.LevelsDataSO);
        //        AssetDatabase.SaveAssets();
        //    }
        //}
        //if (GUILayout.Button($" Fix Level Data{tool.songElement}"))
        //{
        //    tool.FixData();
        //    if (EditorUtility.DisplayDialog("RebuildLevelData?", $"Thay doi Data cua bai so {tool.songElement}", "Save", "No"))
        //    {
        //        EditorUtility.SetDirty(tool.LevelsDataSO);
        //        AssetDatabase.SaveAssets();
        //    }
        //}
        GUILayout.Space(50);
        base.OnInspectorGUI();
    }

}
#endif
public class CreateLevelByHand : SingletonMonoBehaviour<CreateLevelByHand>
{
    public LineStartAt LineStartAt;
    public TextMeshProUGUI noteDataTxt;
    public GameObject timeLine;
    LevelData levelData;
    public string PathFileRead;
    public int ImportToLevelDataElement = 0;
    public enum StateTool
    {
        CREATE,
    }
    public StateTool state;
    public float clipLength = 1;
    private float timeSong => stopwatch.IsRunning ? _startTime + (float)stopwatch.ElapsedMilliseconds / 1000f : _startTime;
    public Slider sliderSong;
    private float _startTime;
    public float startTime
    {
        get { return _startTime; }
        set { _startTime = value; startTimeTxt.text = $"StartTime: {_startTime}"; LineStartAt.transform.position = new Vector3(_startTime, 0, 0); }
    }
    public TextMeshProUGUI startTimeTxt;
    public float endTime = 0;
    public NoteMusic currentNote;
    AudioClip audioClip;

    public LevelsDataSO LevelsDataSO;
    public SongClipSO SongClipSO;
    public SongDataSplit songDataSplit;
    public List<SongElement> songElements;
    public int songElement;
    public Stopwatch stopwatch;

    public KeyCode KeyCodeLeft = KeyCode.N;
    public KeyCode KeyCodeRight = KeyCode.M;
    public KeyCode KeyCodeBothSongSong = KeyCode.Space;
    public KeyCode KeyCodeBothTuongPhan = KeyCode.B;
    //===================================================

    public AudioSource audioSource;
    bool isSpawnLine1 = false;
    bool isSpawmLine2 = false;
    bool isSpawnLine3;
    bool isSpawnLine4;

    public static MidiFile midiFile;

    List<Note> list = new List<Note>();
    List<NoteMusic> listNoteMusic = new List<NoteMusic>();
    public List<NoteMusic> listNoteMusicFinal = new List<NoteMusic>();
    int nodeLine1Length;
    int nodeLine2Length;
    int nodeLine3Length;
    int nodeLine4Length;
    private void Start()
    {
        stopwatch = new Stopwatch();
    }
    private void Update()
    {
        sliderSong.value = timeSong / clipLength;
        timeLine.transform.position = new Vector3(timeSong, 0, 0);
        if (stopwatch.IsRunning)
        {
            CammeraMove.Instance.camlookat.position = new Vector3(timeSong - 16, 2, 0);
        }




        //====================================================================
        if (Input.GetKeyDown(KeySetting.Instance.keyDictionary["Start"]) && state == StateTool.CREATE && timeSong < clipLength)
        {
            pressStart();

        }
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            if (currentNote != null)
            {
                if (listNoteMusicFinal.Contains(currentNote))
                {
                    listNoteMusicFinal.Remove(currentNote);
                }
                if (listNoteMusic.Contains(currentNote))
                {
                    listNoteMusic.Remove(currentNote);
                }
                Destroy(currentNote.gameObject);
            }
        }

        //if (Input.GetKeyDown(KeyCode.P) && timeSong < clipLength)
        //{
        //    Presssstop();

        //}
        if (Input.GetKeyDown(KeySetting.Instance.keyDictionary["End"]) || stopwatch.ElapsedMilliseconds > (endTime - startTime) * 1000 || timeSong > clipLength)
        {
            PressEnd();
        }
        if (!stopwatch.IsRunning) return;
        //===========================================================
        if (isSpawnLine1)
        {
            nodeLine1Length += (int)(Time.deltaTime * 1000);
        }
        if (isSpawmLine2)
        {
            nodeLine2Length += (int)(Time.deltaTime * 1000);
        }
        if (isSpawnLine3)
        {
            nodeLine3Length += (int)(Time.deltaTime * 1000);
        }
        if (isSpawnLine4)
        {
            nodeLine4Length += (int)(Time.deltaTime * 1000);
        }
        //====================================================


        #region Spawn Line
        if (Input.GetKeyDown(KeySetting.Instance.keyDictionary["Line1"]))
        {
            isSpawnLine1 = true;
            nodeLine1Length = 0;
        }
        if (Input.GetKeyDown(KeySetting.Instance.keyDictionary["Line2"]))
        {
            isSpawmLine2 = true;
            nodeLine2Length = 0;
        }
        if (Input.GetKeyDown(KeySetting.Instance.keyDictionary["Line3"]))
        {
            isSpawnLine3 = true;
            nodeLine3Length = 0;
        }
        if (Input.GetKeyDown(KeySetting.Instance.keyDictionary["Line4"]))
        {
            isSpawnLine4 = true;
            nodeLine4Length = 0;
        }
        //===============================================

        if (Input.GetKeyUp(KeySetting.Instance.keyDictionary["Line1"]))
        {
            isSpawnLine1 = false;

            list.Add(new Note(stopwatch.ElapsedMilliseconds + startTime * 1000 - nodeLine1Length, 1, IcecreamType.Full, nodeLine1Length));
            NoteMusic noteMusic = SpawnNote.Instance.SpawnANote(1, nodeLine1Length, stopwatch.ElapsedMilliseconds + startTime * 1000 - nodeLine1Length);
            listNoteMusic.Add(noteMusic);

        }
        if (Input.GetKeyUp(KeySetting.Instance.keyDictionary["Line2"]))
        {
            isSpawmLine2 = false;
            list.Add(new Note(stopwatch.ElapsedMilliseconds + startTime * 1000 - nodeLine2Length, 2, IcecreamType.Full, nodeLine2Length));
            NoteMusic noteMusic = SpawnNote.Instance.SpawnANote(2, nodeLine2Length, stopwatch.ElapsedMilliseconds + startTime * 1000 - nodeLine2Length);
            listNoteMusic.Add(noteMusic);
        }
        if (Input.GetKeyUp(KeySetting.Instance.keyDictionary["Line3"]))
        {
            isSpawnLine3 = false;
            list.Add(new Note(stopwatch.ElapsedMilliseconds + startTime * 1000 - nodeLine3Length, 3, IcecreamType.Full, nodeLine3Length));

            NoteMusic noteMusic1 = SpawnNote.Instance.SpawnANote(3, nodeLine3Length, stopwatch.ElapsedMilliseconds + startTime * 1000 - nodeLine3Length);

            listNoteMusic.Add(noteMusic1);
        }
        if (Input.GetKeyUp(KeySetting.Instance.keyDictionary["Line4"]))
        {
            isSpawnLine4 = false;
            list.Add(new Note(stopwatch.ElapsedMilliseconds + startTime * 1000 - nodeLine4Length, 4, IcecreamType.Full, nodeLine4Length));
            NoteMusic noteMusic1 = SpawnNote.Instance.SpawnANote(4, nodeLine4Length, stopwatch.ElapsedMilliseconds + startTime * 1000 - nodeLine4Length);
            listNoteMusic.Add(noteMusic1);
        }
        #endregion
        //==============================================================
        if (timeSong >= clipLength)
        {
            PressEnd();
        }

    }


    public List<Note> SpawnNgonKemAndSort(List<Note> lstNote)
    {
        int x = lstNote.Count;
        List<Note> P_list = new List<Note>();


        foreach (Note note in lstNote)
        {
            P_list.Add(new Note(note.time, note.line, IcecreamType.Full));
            if (note.length > 150)
            {
                for (int i = 50 / 50; i < (note.length / 50) - 2; i++)
                {
                    P_list.Add(new Note(note.time + 50 * i, note.line, IcecreamType.NgonKem));
                }
            }
        }
        P_list.Sort(Compare);
        return P_list;


    }


    public void ReadFromFile()
    {
        midiFile = MidiFile.Read(Path.Combine(Application.dataPath, "Resources/Midi", songElement.ToString() + ".mid"));
        GetDataFromMidi();
    }

    public void GetDataFromMidi()
    {
        var notes = midiFile.GetNotes();
        var array = new Melanchall.DryWetMidi.Interaction.Note[notes.Count];
        notes.CopyTo(array, 0);
        List<Note> list = new List<Note>();
        int notephu = 0;
        foreach (var note in array)
        {
            int lane = 0;
            var metricTimeSpan = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, midiFile.GetTempoMap());
            long time = metricTimeSpan.Minutes * 60 * 1000 + metricTimeSpan.Seconds * 1000 + metricTimeSpan.Milliseconds;
            switch ((int)note.NoteName)
            {

                case < 0:
                    lane = 1;

                    break;
                case < 6: lane = 2; break;
                case < 8: lane = 3; break;
                case < 12: lane = 4; break;
                default: break;
            }
            list.Add(new Note(time, lane, IcecreamType.Full));
            if (note.Length > 150)
            {
                for (int i = 50 / 50; i < (note.Length / 50) - 2; i++)
                {
                    notephu++;
                    list.Add(new Note(time + 50 * i, lane, IcecreamType.NgonKem));
                }
            }

        }
        list.Sort(Compare);
        Debug.Log($"NodePhu{notephu}");
        Debug.Log("Thanh cong doc Note" + list.Count);
        LevelData levelData = new LevelData(list);
        LevelsDataSO.LevelDataArray[songElement] = levelData;
    }
    void pressStart()
    {
        if (audioClip == null) return;

        Debug.Log("Start");
        //if (songElement >= SongClipSO.GetLength)
        //{
        //    Debug.LogError($"Nhap lai songElement. {songElement}>{SongClipSO.GetLength}");
        //    return;
        //}
        stopwatch = new Stopwatch();

        stopwatch.Start();
        foreach (NoteMusic note in listNoteMusic)
        {
            Destroy(note.transform.parent.gameObject);
        }
        List<NoteMusic> listDelete = new List<NoteMusic>();
        for (int i = listNoteMusicFinal.Count - 1; i >= 0; i--)
        {
            if (listNoteMusicFinal[i].time > startTime)
            {
                listDelete.Add(listNoteMusicFinal[i]);
            }
        }
        foreach (var x in listDelete)
        {
            listNoteMusicFinal.Remove(x);
            Destroy(x.transform.parent.gameObject);
        }
        list = new List<Note>();
        listNoteMusic = new List<NoteMusic>();


        audioSource.clip = audioClip;


        audioSource.time = startTime;
        audioSource.Play();
    }

    void Presssstop()
    {
        if (audioClip == null) return;

        if (stopwatch.IsRunning)
        {
            stopwatch.Stop();
            audioSource.Stop();
        }
    }

    void PressEnd()
    {
        if (!stopwatch.IsRunning)
        {
            return;
        }
        endTime = (float)stopwatch.ElapsedMilliseconds / (float)1000 + startTime;
        stopwatch.Stop();
        audioSource.Stop();
        List<Note> P_list = SpawnNgonKemAndSort(list);
        songElements.Add(new SongElement(startTime, endTime, P_list));
        startTime = endTime;
        Debug.Log($"Luu bai hat tai {startTime}");
        endTime = 10000;

        listNoteMusicFinal.AddRange(listNoteMusic);
        listNoteMusic = new List<NoteMusic>();
    }
    public LevelData MergeSongElement(List<SongElement> songElements)
    {
        List<Note> list = new List<Note>();
        for (int i = 0; i < songElements.Count; i++)
        {
            list.AddRange(songElements[i].lstNote);
        }
        LevelData P_levelData = new LevelData(list);
        return P_levelData;

    }

    public LevelData CreateLevelData(List<NoteMusic> listNoteMusicFinal)
    {
        List<Note> lstNote = new List<Note>();
        foreach (var noteMusic in listNoteMusicFinal)
        {
            lstNote.Add(new Note(noteMusic.time * 1000, noteMusic.line, IcecreamType.Full, (int)(noteMusic.length * 1000)));
        }
        //  lstNote = SpawnNgonKemAndSort(lstNote);
        LevelData P_levelData = new LevelData(lstNote);
        return P_levelData;

    }
    public int Compare(Note a, Note b)
    {
        return a.time.CompareTo(b.time);
    }



    public void ExPort()
    {
        if (stopwatch.IsRunning)
        {
            PressEnd();
        }

        levelData = CreateLevelData(listNoteMusicFinal);
        var bp = new BrowserProperties();
        bp.filter = "Folder ( *.txt, *.json) | *.txt; *.json";
        bp.filterIndex = 0;
        new FileBrowser().OpenFileBrowser(bp, path =>
        {
            string x = JsonConvert.SerializeObject(levelData);
            File.WriteAllText(path, x);
            //Load image from local path with UWR
        });

        Debug.Log("Export thanh cong");

    }

    public void DoAgain()
    {
        if (stopwatch.IsRunning)
        {
            PressEnd();
        }
        foreach (var note in listNoteMusicFinal)
        {
            Destroy(note.gameObject);
        }
        listNoteMusicFinal = new List<NoteMusic>();

        songElements = new List<SongElement>();
        startTime = 0;
        clipLength = audioClip.length;

    }

    public void TESTSONG()
    {
        stopwatch = new Stopwatch();

        stopwatch.Start();
        startTime = 0;
        audioSource.time = 0;
        audioSource.Play();

    }
    public void ImportJson()
    {

        string dataAsJson = File.ReadAllText(PathFileRead);


        LevelData levelData = JsonConvert.DeserializeObject<LevelData>(dataAsJson);
        levelData.lstNode = SpawnNgonKemAndSort(levelData.lstNode);
        LevelsDataSO.LevelDataArray[ImportToLevelDataElement] = levelData;
    }
    public void LoadFileOGG()
    {
        if (stopwatch.IsRunning)
        {
            PressEnd();
        }
        foreach (var note in listNoteMusicFinal)
        {
            Destroy(note.gameObject);
        }
        listNoteMusicFinal = new List<NoteMusic>();

        songElements = new List<SongElement>();
        var bp = new BrowserProperties();
        bp.filter = "Song File ( *.ogg ) | *.ogg";
        bp.filterIndex = 0;


        new FileBrowser().OpenFileBrowser(bp, path =>
        {
            //Load image from local path with UWR
            StartCoroutine(LoadOGG(path));
        });
    }
    IEnumerator LoadOGG(string path)
    {
        using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.OGGVORBIS))
        {
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                audioClip = DownloadHandlerAudioClip.GetContent(uwr);

            }
        }
        Debug.Log("Load OGG thanh cong  ");
        startTime = 0;
        clipLength = audioClip.length;
    }

    public void ShowDataNoteMussic(NoteMusic noteMusic)
    {
        noteDataTxt.text = $" Time{noteMusic.time}\n Length:{noteMusic.length} \n Line:{noteMusic.line}";
    }
    public void ClearNoteProperty()
    {
        noteDataTxt.text = $" Time\n Length: \n Line:";
    }
}


