using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveByDottween
{
    public Transform target;
    public float length;

    public MoveByDottween()
    {

    }

    void DoMove()
    {
        target.DOMoveX(target.transform.position.x + length, 1f);
    }
}
