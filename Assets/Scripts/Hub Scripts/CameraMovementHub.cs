using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovementHub : MonoBehaviour
{
    [SerializeField] private Camera cam;

    private Vector3 dragOrigin;

    public GridControllerHub gc;

    private float gameMinX, gameMaxX, gameMinY, gameMaxY;
    // Initializes the camera pan boundaries 
    private void Awake(){
        gameMinX = -(gc.boardX / 2);// * 2;
        gameMaxX = (gc.boardX / 2);// * 2;
        gameMinY = -(gc.boardY / 2);// * 2;
        gameMaxY = (gc.boardY / 2);// * 2;
    }

    // Update is called once per frame
    void Update()
    {
        PanCamera();
    }

    private void PanCamera()
    {
        // Save mouse position when drag starts
        if(Input.GetMouseButtonDown(2))
        {
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
        }

        // Calculate Distance betweeen drag origin and new position if held down
        if(Input.GetMouseButton(2))
        {
            Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);

            // Moe the Camera that distance
            cam.transform.position = ClampCamera(cam.transform.position + difference);
        }
    }

    private Vector3 ClampCamera(Vector3 targetPosition){
        float camHeight = cam.orthographicSize;
        float camWidth = cam.orthographicSize * cam.aspect;

        float minX = gameMinX + camWidth;
        float maxX = gameMaxX - camWidth;
        float minY = gameMinY + camHeight;
        float maxY = gameMaxY - camHeight;

        float newX = Mathf.Clamp(targetPosition.x, minX, maxX);
        float newY = Mathf.Clamp(targetPosition.y, minY, maxY);
        //print("NewX " + newX + " NewY " + newY);
        return new Vector3(newX, newY, targetPosition.z);
        
    }
}
