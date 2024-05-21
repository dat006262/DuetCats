using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CatMoveController : MonoBehaviour
{
    public BoxCollider2D catGameObject;
    public Camera _camera;
    Vector2 mousePositionOffset;
    float campX;
    private void Start()
    {
        if (this.transform.position.x < 0)
        {
            campX = _camera.ViewportToWorldPoint(new Vector3(0, 1, Camera.main.nearClipPlane)).x + catGameObject.size.x / 2;
        }
        else
        {
            campX = _camera.ViewportToWorldPoint(new Vector3(1, 1, Camera.main.nearClipPlane)).x - catGameObject.size.x / 2;
        }
        Debug.Log(campX);
    }

    private Vector2 GetMouseWordPosition()
    {
        return _camera.ScreenToWorldPoint(Input.mousePosition);
    }


    public void OnMouseDown()
    {
        mousePositionOffset = GetMouseWordPosition();


    }
    public void OnMouseDrag()
    {
        float posX = catGameObject.transform.position.x + (GetMouseWordPosition().x - mousePositionOffset.x);

        catGameObject.transform.position = new Vector3(ClampX(posX), -5, 0);


        mousePositionOffset = GetMouseWordPosition();
    }
    float ClampX(float input)
    {
        if (campX < 0)
        {
            return Mathf.Clamp(input, campX, 0 - catGameObject.size.x / 2);
        }
        else
        {
            return Mathf.Clamp(input, 0 + catGameObject.size.x / 2, campX);
        }
    }
}
