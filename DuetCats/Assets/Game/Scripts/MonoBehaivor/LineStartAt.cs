using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineStartAt : MonoBehaviour
{
    public Camera _camera;
    Vector2 mousePositionOffset;
    private Vector2 GetMouseWordPosition()
    {
        return _camera.ScreenToWorldPoint(Input.mousePosition);
    }
    private void OnMouseDown()
    {
        if (CreateLevelByHand.Instance.stopwatch.IsRunning)
        {
            return;
        }
        CammeraMove.Instance.canMoveCam = false;
        mousePositionOffset = GetMouseWordPosition();
    }
    public void OnMouseDrag()
    {
        if (CreateLevelByHand.Instance.stopwatch.IsRunning)
        {
            return;
        }
        float posX = (GetMouseWordPosition().x - mousePositionOffset.x);

        this.transform.position += Vector3.right * posX;
        CreateLevelByHand.Instance.startTime = this.transform.position.x;
        mousePositionOffset = GetMouseWordPosition();
    }


    public void OnMouseUp()
    {
        if (CreateLevelByHand.Instance.stopwatch.IsRunning)
        {
            return;
        }
        CammeraMove.Instance.canMoveCam = true;
    }
}
