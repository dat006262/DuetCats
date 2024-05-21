using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchMove
{
    public int touchID;
    public GameObject controllGameObject;
    public Vector2 posTouch;
    public bool isleft;
    private SpriteRenderer spriteRenderer;
    public TouchMove(int p_touchID, GameObject p_test, Vector2 _posTouch, bool isleft)
    {
        this.touchID = p_touchID;
        this.controllGameObject = p_test;
        this.posTouch = _posTouch;
        this.isleft = isleft;
        spriteRenderer = controllGameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
    }
    public void SetFlip(bool isMoveleft)
    {
        spriteRenderer.flipX = isMoveleft;
    }
}
