using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KeySetting : SingletonMonoBehaviour<KeySetting>
{
    public Dictionary<string, KeyCode> keyDictionary = new Dictionary<string, KeyCode>();

    public TextMeshProUGUI Line1, Line2, Line3, Line4, Starttxt, End;

    public GameObject currentKey;
    private void Start()
    {
        keyDictionary.Add("Line1", KeyCode.Alpha1);
        keyDictionary.Add("Line2", KeyCode.Alpha2);
        keyDictionary.Add("Line3", KeyCode.Alpha3);
        keyDictionary.Add("Line4", KeyCode.Alpha4);
        keyDictionary.Add("Start", KeyCode.S);
        keyDictionary.Add("End", KeyCode.E);

        Line1.text = keyDictionary["Line1"].ToString();
        Line2.text = keyDictionary["Line2"].ToString();
        Line3.text = keyDictionary["Line3"].ToString();
        Line4.text = keyDictionary["Line4"].ToString();
        Starttxt.text = keyDictionary["Start"].ToString();
        End.text = keyDictionary["End"].ToString();
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
