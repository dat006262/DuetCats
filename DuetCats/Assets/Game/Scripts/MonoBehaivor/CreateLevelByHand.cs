using BulletHell;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.MusicTheory;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
#if UNITY_EDITOR
[CustomEditor(typeof(CreateLevelByHand))]
public class CreateLevelByHandEditor : Editor
{

    public override void OnInspectorGUI()
    {
        CreateLevelByHand tool = (CreateLevelByHand)target;
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
public class CreateLevelByHand : MonoBehaviour
{

    public LevelsDataSO LevelsDataSO;
    public SongClipSO SongClipSO;
    public int songElement;
    Stopwatch stopwatch;

    public KeyCode KeyCodeLeft = KeyCode.N;
    public KeyCode KeyCodeRight = KeyCode.M;
    public KeyCode KeyCodeBoth = KeyCode.Space;
    //===================================================

    public AudioSource audioSource;
    bool isSpawnNoteLeft = false;
    bool isSpawnNoteRight = false;
    bool isSpawnBoth;

    public static MidiFile midiFile;

    List<Note> list = new List<Note>();


    int nodeLeftLength;
    int nodeRightLength;
    int nodeBothLength;
    private void Update()
    {
        int x = UnityEngine.Random.Range(0, 2);
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("Start");
            if (songElement >= SongClipSO.GetLength)
            {
                Debug.LogError($"Nhap lai songElement. {songElement}>{SongClipSO.GetLength}");
                return;
            }
            stopwatch = new Stopwatch();

            stopwatch.Start();
            list = new List<Note>();
            audioSource.clip = SongClipSO.GetSfx(songElement);
            audioSource.Play();
        }
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
        if (Input.GetKeyDown(KeyCodeLeft))
        {
            isSpawnNoteLeft = true;
            nodeLeftLength = 0;
        }
        if (Input.GetKeyDown(KeyCodeRight))
        {
            isSpawnNoteRight = true;
            nodeRightLength = 0;
        }
        if (Input.GetKeyDown(KeyCodeBoth))
        {
            isSpawnBoth = true;
            nodeBothLength = 0;
        }
        //===============================================

        if (Input.GetKeyUp(KeyCodeLeft))
        {
            isSpawnNoteLeft = false;

            list.Add(new Note(stopwatch.ElapsedMilliseconds, 1 + x, IcecreamType.Full, nodeLeftLength));
        }
        if (Input.GetKeyUp(KeyCodeRight))
        {
            isSpawnNoteRight = false;
            list.Add(new Note(stopwatch.ElapsedMilliseconds, 3 + x, IcecreamType.Full, nodeRightLength));
        }
        if (Input.GetKeyUp(KeyCodeBoth))
        {
            isSpawnBoth = false;
            list.Add(new Note(stopwatch.ElapsedMilliseconds, 1 + x, IcecreamType.Full, nodeBothLength));
            list.Add(new Note(stopwatch.ElapsedMilliseconds, 3 + x, IcecreamType.Full, nodeBothLength));
        }
        //==============================================================
        if (Input.GetKeyDown(KeyCode.E))
        {
            stopwatch.Stop();
            audioSource.Stop();

            LevelData levelData = new LevelData(list);
            LevelsDataSO.LevelDataArray[songElement] = levelData;
            Debug.Log($"End And SaveTOLevelDataArray[{songElement}]");

            FixData();

            EditorUtility.SetDirty(LevelsDataSO);
            AssetDatabase.SaveAssets();
        }
#endif
    }

    public void FixData()
    {
        int x = LevelsDataSO.LevelDataArray[songElement].lstNode.Count;
        List<Note> P_list = new List<Note>();


        foreach (Note note in LevelsDataSO.LevelDataArray[songElement].lstNode)
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

        LevelData P_levelData = new LevelData(P_list);
        LevelsDataSO.LevelDataArray[songElement] = P_levelData;

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

    public int Compare(Note a, Note b)
    {
        return a.time.CompareTo(b.time);
    }

}


