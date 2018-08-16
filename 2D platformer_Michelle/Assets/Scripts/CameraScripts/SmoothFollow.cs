using UnityEngine;
using System.Collections;

public class SmoothFollow : MonoBehaviour
{
    // Camera 2D smoothing
    public Camera thisCamera;
    public Transform target; // The target we are following
    public bool bIsfollowing;
   private float distance = 10.0f;  // The distance in the x-z plane to the target (AP no idea what this does. Leave it as 10?)


    public float fOffsetY = 4.0f; // the height we want the camera to be above the target
    public float fOffsetX = 3.5f; // the height we want the camera to be rightwards of the target

    public float heightTightness = 2.0f;
    public float translationalTightness = 2.0f;

    float wantedHeight;
    float currentHeight;
    float wantedPos;
    float currentPos;


    // Camera zoom areas
    public float fWantedSize;
    public float fCurrentSize;

    public float fZoomTightness = 2.0f;


    private void Start()
    {
        fWantedSize = 10.0f;
    }


    void LateUpdate()
    {
        if (target)
        {
            wantedHeight = target.position.y + fOffsetY;
            currentHeight = transform.position.y;

            wantedPos = target.position.x + fOffsetX;
            currentPos = transform.position.x;

            fCurrentSize = thisCamera.orthographicSize;

            // Damp the height
            currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightTightness * Time.deltaTime);

            // Damp the translation
            currentPos = Mathf.Lerp(currentPos, wantedPos, translationalTightness * Time.deltaTime);

            // Damp the zoom
            //Debug.Log("change from :" + fCurrentSize + " to " + fWantedSize);
            fCurrentSize = Mathf.Lerp(fCurrentSize, fWantedSize, fZoomTightness * Time.deltaTime);

            // Set the position of the camera on the x-z plane to:
            // distance meters behind the target
            transform.position = target.position;
            transform.position -= Vector3.forward * distance;

            // Set the camera
            transform.position = new Vector3(currentPos, currentHeight, transform.position.z);
            thisCamera.orthographicSize = fCurrentSize;
         }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "CameraZone")
        {
            fWantedSize = other.gameObject.GetComponent<CameraTriggerClass>().size;
        }
    }
}