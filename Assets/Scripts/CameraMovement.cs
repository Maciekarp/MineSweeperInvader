using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private GameUIManager gameUIManager;
    [SerializeField] private int moveSpeed = 10;

    private Vector3 dragOrigin;

    public GridController gc;

    private float gameMinX, gameMaxX, gameMinY, gameMaxY;
    // Initializes the camera pan boundaries 
    private void Awake(){
        gameMinX = -(gc.boardX / 2);// * 2;
        gameMaxX = (gc.boardX / 2);// * 2;
        gameMinY = -(gc.boardY / 2);// * 2;
        gameMaxY = (gc.boardY / 2);// * 2;
    }

    // Update is called once per frame
    void Update() {
        if(!gameUIManager.isPaused) {
            PanCamera();
        }
    }

    // Pans the camera around based off mouse drag and directional input
    private void PanCamera() {
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
        // Is camera is not being dragged move camera based on wasd or arrow keys
        } else {
            // print("camera horizontal = " + Input.GetAxis("Horizontal"));
            cam.transform.position = ClampCamera(cam.transform.position + (new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0) * moveSpeed * Time.deltaTime));
        }

    }

    // Clamps camera to the maximum boundary 
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
