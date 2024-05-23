using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KeySetting : SingletonMonoBehaviour<KeySetting>
{
    public Dictionary<string, KeyCode> keyDictionary = new Dictionary<string, KeyCode>();

    public TextMeshProUGUI Left, Right, BothSongSong, BothSole;

    public GameObject currentKey;
    private void Start()
    {
        keyDictionary.Add("BothSongSong", KeyCode.B);
        keyDictionary.Add("BothSole", KeyCode.V);
        keyDictionary.Add("Left", KeyCode.M);
        keyDictionary.Add("Right", KeyCode.N);

        Left.text = keyDictionary["Left"].ToString();
        Right.text = keyDictionary["Right"].ToString();
        BothSongSong.text = keyDictionary["BothSongSong"].ToString();
        BothSole.text = keyDictionary["BothSole"].ToString();
    }
    private void OnGUI()
    {
        if (currentKey != null)
        {
            Event e = Event.current;
            if (e.isKey)
            {
                keyDictionary[currentKey.name] = e.keyCode;
                currentKey.GetComponentInChildren<TextMeshProUGUI>().text = e.keyCode.ToString();
                currentKey = null;
            }
        }
    }
    public void SellectKey(GameObject clicked)
    {
        currentKey = clicked;
    }
}
