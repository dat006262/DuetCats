using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteMusic : MonoBehaviour
{
    public Camera _camera;
    Vector2 mousePositionOffset;


    public int line;
    public float length;
    public float time;
    private Vector2 GetMouseWordPosition()
    {
        return _camera.ScreenToWorldPoint(Input.mousePosition);
    }
    private void OnMouseDown()
    {
        CammeraMove.Instance.canMoveCam = false;
        CreateLevelByHand.Instance.ShowDataNoteMussic(this);
        mousePositionOffset = GetMouseWordPosition();
        CreateLevelByHand.Instance.currentNote = this;
    }
    public void OnMouseDrag()
    {

        float posX = (GetMouseWordPosition().x - mousePositionOffset.x);

        this.transform.parent.position += Vector3.right * posX;
        this.time = this.transform.parent.position.x;
        CreateLevelByHand.Instance.ShowDataNoteMussic(this);
        mousePositionOffset = GetMouseWordPosition();
    }


    public void OnMouseUp()
    {
        CammeraMove.Instance.canMoveCam = true;
    }

}
