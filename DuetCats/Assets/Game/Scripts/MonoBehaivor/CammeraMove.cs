using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class CammeraMove : SingletonMonoBehaviour<CammeraMove>
{
    public bool canMoveCam = true;
    [SerializeField] private Camera cam;

    [SerializeField] public Transform camlookat;
    //MoveCam--------------//
    private Vector3 StartTouch;
    public override void Awake()
    {
        base.Awake();
        canMoveCam = true;
    }

    private void Update()
    {
        if (!canMoveCam) { return; }

        if (Input.GetMouseButtonDown(0))
        {
            StartTouch = cam.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 direction = StartTouch - cam.ScreenToWorldPoint(Input.mousePosition);
            camlookat.transform.position = camlookat.transform.position + Vector3.right * direction.x;

        }
        if (Input.GetMouseButtonUp(0))
        {

            StartTouch = Input.mousePosition;
        }

    }
}
