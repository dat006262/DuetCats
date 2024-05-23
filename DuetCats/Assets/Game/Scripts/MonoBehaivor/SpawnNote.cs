using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnNote : SingletonMonoBehaviour<SpawnNote>
{

    public Camera camera;
    //  List<GameObject> lstNotes = new List<GameObject>();
    List<NoteMusic> lstNotes = new List<NoteMusic>();
    //Quy đổi  1s => 1đơn vị

    public GameObject notePrefabs;

    private void Start()
    {

    }
    public NoteMusic SpawnANote(int line, float length, float time)
    {
        length = (float)length / 1000f;
        time = time / 1000f;
        Vector3 pos = new Vector3(0, 0, 0);
        Vector3 scale = new Vector3(1 * length, 1, 1);
        if (line == 1)
        {
            pos += Vector3.up * 1f;
        }
        else if (line == 2)
        {

        }
        else if (line == 3)
        {
            pos += Vector3.down * 1f;
        }
        else if (line == 4)
        {
            pos += Vector3.down * 2f;
        }
        pos += Vector3.right * time;

        GameObject note = Instantiate(notePrefabs, pos, Quaternion.identity);
        note.transform.localScale = scale;
        NoteMusic x = note.GetComponentInChildren<NoteMusic>();
        x._camera = camera;
        x.length = length;
        x.time = time;
        x.line = line;
        lstNotes.Add(x);
        return x;
    }

    public void ClearAll()
    {
        foreach (NoteMusic x in lstNotes)
        {

            Destroy(x.transform.parent.gameObject);
        }
        lstNotes.Clear();
    }
}
