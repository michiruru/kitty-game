using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public Camera camera;

    //ZOOM LEVELS
    private float fOriginalSize;
    private float fCurrentSize;
    private float fNewSize;

    public float fZoomDuration;
    private float fZoomTimer;


    //FOLLOWING
    public Vector2 fOriginalPos;
    public Vector2 fCurrentPos;
    public Vector2 fNewPos;

    public float fPosDurationHoriz;
    public float fPosDurationVert;
    public float fPosTimerHoriz;
    public float fPosTimerVert;

    // GAMEOBJECTS
    public GameObject player;
     public bool isFollowing;

    public float xOffset;
     public float yOffset;

     // Use this for initialization
     void Start () {
         isFollowing = true;

        fOriginalSize = camera.orthographicSize;
        fCurrentSize = camera.orthographicSize;
        fNewSize = camera.orthographicSize;

        fOriginalPos = new Vector2(player.transform.position.x, player.transform.position.y);
        fCurrentPos = new Vector2(player.transform.position.x, player.transform.position.y);
        fNewPos = new Vector2(player.transform.position.x, player.transform.position.y);
     }

     
     void Update () {
        //Set currents
        fCurrentSize = camera.orthographicSize;
        fNewPos = new Vector2(player.transform.position.x, player.transform.position.y);
        fCurrentPos = new Vector2(camera.transform.position.x, camera.transform.position.y);

         if (isFollowing) 
        {
            //transform.position = new Vector3(player.transform.position.x + xOffset, player.transform.position.y + yOffset, transform.position.z); 
         }

        //TIMERS
        if (fZoomTimer > 0)
        {
            fZoomTimer -= Time.deltaTime;
        }

        if (fPosTimerHoriz > 0)
        {
            fPosTimerHoriz -= Time.deltaTime;
        }

        if (fPosTimerVert > 0)
        {
            fPosTimerVert -= Time.deltaTime;
        }


        //LERPING
        //Zoom
        if (fCurrentSize != fNewSize)
        {
            camera.orthographicSize = Mathf.Lerp(fNewSize, fOriginalSize, fZoomTimer / fZoomDuration);  //we lerp the wrong way because the timer is counting DOWN (meaning that the ratio goes from 1 TO 0 (not 0 to 1))
       }
        else
        {
            fOriginalSize = fNewSize;
        }

        //Position
        //if ((fCurrentPos.x != fNewPos.x || fCurrentPos.y != fNewPos.y) && isFollowing)
        //{
        //    //float inputx = Mathf.Lerp(fNewPos.x, fOriginalPos.x, fPosTimerHoriz / fPosDurationHoriz);
        //    //float inputy = Mathf.Lerp(fNewPos.y, fOriginalPos.y, fPosTimerVert / fPosDurationVert);

        //    float inputx = fCurrentPos.x + 1f * (fNewPos.x - fCurrentPos.x);
        //    float inputy = fCurrentPos.y + 1f * (fNewPos.y - fCurrentPos.y);

        //    transform.position = new Vector3(inputx + xOffset, inputy + yOffset, transform.position.z);
        //}
        //else
        //{
        //    fOriginalPos.x = fNewPos.x;
        //    fOriginalPos.y = fNewPos.y;
        //}




     }

    //ENTER CAMERA ZOOM ZONE
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "CameraZone"){
            //Debug.Log("enter zone of size " + other.gameObject.GetComponent<CameraTriggerClass>().size);

            fNewSize = other.gameObject.GetComponent<CameraTriggerClass>().size;

            fZoomTimer = fZoomDuration;

        }
    }

}
