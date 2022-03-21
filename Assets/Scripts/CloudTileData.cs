using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class CloudTileData : MonoBehaviour{
    public Tilemap tilemap;

    public int boardX, boardY = 10;

    // Update is called once per frame
    void Update(){
        // On mouse button press removes a tile at mouse position
        if(Input.GetMouseButtonDown(0)){
            Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            tilemap.SetTile(tilemap.WorldToCell(position), null);
        }
    }
}
