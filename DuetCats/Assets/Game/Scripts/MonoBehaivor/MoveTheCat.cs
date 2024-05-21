using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTheCat : MonoBehaviour
{
    public Camera _camera;
    public BoxCollider2D catGameObjectLeft;
    public BoxCollider2D catGameObjectRight;
    public Animator catleft;
    bool isLeft = true;
    bool isTouch0Left;
    bool isTouch1Left;
    private Vector2 GetMouseWordPosition()
    {
        return _camera.ScreenToWorldPoint(Input.mousePosition);
    }

    public Vector2 posTouch0;
    public Vector2 posTouch1;
    private Transform theCatTouch0Cotroller;

    private Transform theCatTouch1Cotroller;
    public float PosScreemLeft;
    public float PosScreemRight;



    public List<TouchMove> touchMoves = new List<TouchMove>();
    private void Start()
    {
        Input.multiTouchEnabled = true;
        PosScreemLeft = _camera.ViewportToWorldPoint(new Vector3(0, 1, Camera.main.nearClipPlane)).x + catGameObjectLeft.size.x / 2;
        PosScreemRight = _camera.ViewportToWorldPoint(new Vector3(1, 1, Camera.main.nearClipPlane)).x - catGameObjectRight.size.x / 2;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Play");
            catleft.Play("Base Layer.test", 0, 0);
        }
#if !UNITY_EDITOR
        int i = 0;
        while (i < Input.touchCount)
        {
            Touch t = Input.GetTouch(i);
            switch (t.phase)
            {
                case TouchPhase.Began:
                    if (t.position.x < Screen.width / 2)
                    {
                        touchMoves.Add(new TouchMove(t.fingerId, catGameObjectLeft.gameObject, t.position, true));
                    }
                    else
                    {
                        touchMoves.Add(new TouchMove(t.fingerId, catGameObjectRight.gameObject, t.position, false));
                    }

                    break;
                case TouchPhase.Moved:
                    TouchMove touchMove2 = touchMoves.Find(touchLocation => touchLocation.touchID == t.fingerId);
                    float posX = touchMove2.controllGameObject.transform.position.x
                       + (_camera.ScreenToWorldPoint(t.position).x - _camera.ScreenToWorldPoint(touchMove2.posTouch).x);
                    touchMove2.controllGameObject.transform.position = new Vector3(ClampX(posX, touchMove2.isleft), -5, 0);
                    touchMove2.posTouch = t.position;
                    if (_camera.ScreenToWorldPoint(t.position).x - _camera.ScreenToWorldPoint(touchMove2.posTouch).x != 0)
                    {
                        touchMove2.SetFlip(_camera.ScreenToWorldPoint(t.position).x - _camera.ScreenToWorldPoint(touchMove2.posTouch).x > 0);
                    }

                    break;
                case TouchPhase.Ended:
                    TouchMove touchEnd = touchMoves.Find(touchLocation => touchLocation.touchID == t.fingerId);
                    touchMoves.Remove(touchEnd);
                    break;
            }
            i++;
        }


#else

        if (Input.GetMouseButtonDown(0))
        {
            isLeft = Input.mousePosition.x < Screen.width / 2;
            //Debug.Log(isLeft);
            if (isLeft)
            {
                theCatTouch0Cotroller = catGameObjectLeft.transform;

            }
            else
            {
                theCatTouch0Cotroller = catGameObjectRight.transform;
            }
            posTouch0 = Input.mousePosition;

        }

        if (Input.GetMouseButton(0))
        {
            float posX = theCatTouch0Cotroller.transform.position.x
                + (_camera.ScreenToWorldPoint(Input.mousePosition).x - _camera.ScreenToWorldPoint(posTouch0).x);
            theCatTouch0Cotroller.transform.position = new Vector3(ClampX(posX, isLeft), -5, 0);
            if ((_camera.ScreenToWorldPoint(Input.mousePosition).x - _camera.ScreenToWorldPoint(posTouch0).x) != 0)
            {
                theCatTouch0Cotroller.transform.GetChild(0).GetComponent<SpriteRenderer>().flipX = (_camera.ScreenToWorldPoint(Input.mousePosition).x - _camera.ScreenToWorldPoint(posTouch0).x) > 0;
            }

            posTouch0 = Input.mousePosition;


        }
#endif
    }

    float ClampX(float input, bool isleft)
    {
        if (isleft)
        {
            return Mathf.Clamp(input, PosScreemLeft, 0 - catGameObjectLeft.size.x / 2);
        }
        else
        {
            return Mathf.Clamp(input, 0 + catGameObjectRight.size.x / 2, PosScreemRight);
        }
    }
}
