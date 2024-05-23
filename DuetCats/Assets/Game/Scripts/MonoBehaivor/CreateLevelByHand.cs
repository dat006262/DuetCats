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
    private float clipLength = 1;
    private float timeSong => stopwatch.IsRunning ? _startTime + (float)stopwatch.ElapsedMilliseconds / 1000f : _startTime;
    public Slider sliderSong;
    private float _startTime;
    public float startTime
    {
        get { return _startTime; }
        set { _startTime = value; startTimeTxt.text = $"StartTime: {_startTime}"; }
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
    Stopwatch stopwatch;

    public KeyCode KeyCodeLeft = KeyCode.N;
    public KeyCode KeyCodeRight = KeyCode.M;
    public KeyCode KeyCodeBothSongSong = KeyCode.Space;
    public KeyCode KeyCodeBothTuongPhan = KeyCode.B;
    //===================================================

    public AudioSource audioSource;
    bool isSpawnNoteLeft = false;
    bool isSpawnNoteRight = false;
    bool isSpawnBoth;

    public static MidiFile midiFile;

    List<Note> list = new List<Note>();
    List<NoteMusic> listNoteMusic = new List<NoteMusic>();
    public List<NoteMusic> listNoteMusicFinal = new List<NoteMusic>();
    int nodeLeftLength;
    int nodeRightLength;
    int nodeBothLength;
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

        int x = UnityEngine.Random.Range(0, 2);


        //====================================================================
        if (Input.GetKeyDown(KeyCode.S) && state == StateTool.CREATE && timeSong < clipLength)
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

        if (Input.GetKeyDown(KeyCode.P) && timeSong < clipLength)
        {
            Presssstop();

        }
        if (Input.GetKeyDown(KeyCode.E) && stopwatch.ElapsedMilliseconds < (endTime - startTime) * 1000 && timeSong < clipLength)
        {
            PressEnd();
        }
        if (!stopwatch.IsRunning) return;
        //===========================================================
        if (isSpawnNoteLeft)
        {
            nodeLeftLength += (int)(Time.deltaTime * 1000);
        }
        if (isSpawnNoteRight)
        {
            nodeRightLength += (int)(Time.deltaTime * 1000);
        }
        if (isSpawnBoth)
        {
            nodeBothLength += (int)(Time.deltaTime * 1000);
        }
        //====================================================
        if (Input.GetKeyDown(KeySetting.Instance.keyDictionary["Left"]))
        {
            isSpawnNoteLeft = true;
            nodeLeftLength = 0;
        }
        if (Input.GetKeyDown(KeySetting.Instance.keyDictionary["Right"]))
        {
            isSpawnNoteRight = true;
            nodeRightLength = 0;
        }
        if (Input.GetKeyDown(KeySetting.Instance.keyDictionary["BothSongSong"]))
        {
            isSpawnBoth = true;
            nodeBothLength = 0;
        }
        if (Input.GetKeyDown(KeySetting.Instance.keyDictionary["BothSole"]))
        {
            isSpawnBoth = true;
            nodeBothLength = 0;
        }
        //===============================================

        if (Input.GetKeyUp(KeySetting.Instance.keyDictionary["Left"]))
        {
            isSpawnNoteLeft = false;

            list.Add(new Note(stopwatch.ElapsedMilliseconds + startTime * 1000 - nodeLeftLength, 1 + x, IcecreamType.Full, nodeLeftLength));
            NoteMusic noteMusic = SpawnNote.Instance.SpawnANote(1 + x, nodeLeftLength, stopwatch.ElapsedMilliseconds + startTime * 1000 - nodeLeftLength);
            listNoteMusic.Add(noteMusic);

        }
        if (Input.GetKeyUp(KeySetting.Instance.keyDictionary["Right"]))
        {
            isSpawnNoteRight = false;
            list.Add(new Note(stopwatch.ElapsedMilliseconds + startTime * 1000 - nodeRightLength, 3 + x, IcecreamType.Full, nodeRightLength));
            NoteMusic noteMusic = SpawnNote.Instance.SpawnANote(3 + x, nodeRightLength, stopwatch.ElapsedMilliseconds + startTime * 1000 - nodeRightLength);
            listNoteMusic.Add(noteMusic);
        }
        if (Input.GetKeyUp(KeySetting.Instance.keyDictionary["BothSongSong"]))
        {
            isSpawnBoth = false;
            list.Add(new Note(stopwatch.ElapsedMilliseconds + startTime * 1000 - nodeBothLength, 1 + x, IcecreamType.Full, nodeBothLength));
            list.Add(new Note(stopwatch.ElapsedMilliseconds + startTime * 1000 - nodeBothLength, 3 + x, IcecreamType.Full, nodeBothLength));
            NoteMusic noteMusic1 = SpawnNote.Instance.SpawnANote(1 + x, nodeBothLength, stopwatch.ElapsedMilliseconds + startTime * 1000 - nodeBothLength);
            NoteMusic noteMusic2 = SpawnNote.Instance.SpawnANote(3 + x, nodeBothLength, stopwatch.ElapsedMilliseconds + startTime * 1000 - nodeBothLength);
            listNoteMusic.Add(noteMusic1);
            listNoteMusic.Add(noteMusic2);
        }
        if (Input.GetKeyUp(KeySetting.Instance.keyDictionary["BothSole"]))
        {
            isSpawnBoth = false;
            list.Add(new Note(stopwatch.ElapsedMilliseconds + startTime * 1000 - nodeBothLength, 2 - x, IcecreamType.Full, nodeBothLength));
            list.Add(new Note(stopwatch.ElapsedMilliseconds + startTime * 1000 - nodeBothLength, 3 + x, IcecreamType.Full, nodeBothLength));
            NoteMusic noteMusic1 = SpawnNote.Instance.SpawnANote(2 - x, nodeBothLength, stopwatch.ElapsedMilliseconds + startTime * 1000 - nodeBothLength);
            NoteMusic noteMusic2 = SpawnNote.Instance.SpawnANote(3 + x, nodeBothLength, stopwatch.ElapsedMilliseconds + startTime * 1000 - nodeBothLength);
            listNoteMusic.Add(noteMusic1);
            listNoteMusic.Add(noteMusic2);
        }
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
        lstNote = SpawnNgonKemAndSort(lstNote);
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
    public void ImportJson()
    {

        string dataAsJson = File.ReadAllText(PathFileRead);


        LevelData levelData = JsonConvert.DeserializeObject<LevelData>(dataAsJson);
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


