using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearNoteProperty : MonoBehaviour
{
    private void OnMouseDown()
    {
        CreateLevelByHand.Instance.ClearNoteProperty();
    }
}
