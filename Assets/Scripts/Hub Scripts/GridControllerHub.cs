using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;

public class HubTile {
    public bool isShip = false;
    public bool canBeShip = false;
}

public class GridControllerHub : MonoBehaviour {

    [SerializeField] private Tilemap spaceMap;
    [SerializeField] private Tilemap shipMap;
    [SerializeField] private Tilemap canShipMap;
    [SerializeField] private Canvas tileUICanvas;
    [SerializeField] private RectTransform tileUITransform;

    [SerializeField] private AnimatedTile[] spaceTiles;
    [SerializeField] private RuleTile shipTile;
    [SerializeField] private Tile nextShipTile;


    private static HubTile[,] hubBoard;
    private static bool isLoaded  = false;

    public int boardX = 20, boardY = 20;
    private bool nextVisible = false;
    private int currTileX, currTileY;
    private float tileUIDispX = 0.5f, tileUIDispY = 3f;

    void Awake() {
        currTileX = boardX / 2;
        currTileY = boardY / 2;
        // initializes hub tiles if they have not been loaded yet
        if(!isLoaded) {
            hubBoard = new HubTile[boardX, boardY];
            for(int x = 0; x < boardX; x++){
                for(int y = 0; y < boardY; y++){
                    hubBoard[x,y] = new HubTile(); 
                }
            }
            // creats first ship tile and allows tiles around it to become ship tiles
            Vector3Int centerPos = new Vector3Int(0, 0, 0);
            hubBoard[boardX / 2, boardY / 2].isShip = true;
            shipMap.SetTile(centerPos, shipTile);
            hubBoard[boardX / 2 - 1, boardY / 2].canBeShip = true;
            hubBoard[boardX / 2 + 1, boardY / 2].canBeShip = true;
            hubBoard[boardX / 2, boardY / 2 - 1].canBeShip = true;
            hubBoard[boardX / 2, boardY / 2 + 1].canBeShip = true;

            isLoaded = true;
        // If it is already loaded draws the tiles
        } else {
            for(int x = 0; x < boardX; x++){
                for(int y = 0; y < boardY; y++){
                    if(hubBoard[x, y].isShip) {
                        Vector3Int pos = new Vector3Int(x - (boardX / 2), y - (boardY / 2), 0);
                        shipMap.SetTile(pos, shipTile);
                    }
                }
            }
        }

        //grid = gameObject.GetComponent<Grid>();
        int randNum;
        // Draws space
        for (int x = 0; x < boardX; x++) {
            for (int y = 0; y < boardY; y++) {
                Vector3Int pos = new Vector3Int(x - (boardX / 2), y - (boardY / 2), 0);
                randNum = Random.Range(0, spaceTiles.Length);
                spaceMap.SetTile(pos, spaceTiles[randNum]);
            }
        }
    }


    void Update() {
        // if the mouse is not over a UI elemnt
        if(!EventSystem.current.IsPointerOverGameObject()) {
            if(Input.GetMouseButtonDown(0)) {
                Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                // toggles tile UI if possible
                tileUIManager((int)(position.x + boardX / 2), (int)(position.y + boardY / 2));
                
                

                // builds new tile
                toggleNextShip();
                buildShip(position);
                toggleNextShip();
            }
        }

        if(Input.GetKeyDown(KeyCode.Space)) {
            toggleNextShip();
        }

        
    }

    // Based off the tile clicked either opens menu or closes it
    public void tileUIManager(int x, int y) {
        
        // if the clicked tile is a ship tile turn on the ui for it
        if(hubBoard[x,y].isShip) {
            if(!tileUICanvas.enabled || !(currTileX == x && currTileY == y)) {
                tileUICanvas.enabled = true;
                tileUITransform.anchoredPosition = new Vector3(x - boardX / 2 + tileUIDispX, y - boardY / 2 + tileUIDispY, 0);
                currTileX = x;
                currTileY = y;
            } else {
                tileUICanvas.enabled = false;
            }
        } else {
            tileUICanvas.enabled = false;
        }
    }

    // Toggles visibility of where next ship can be placed
    public void toggleNextShip() {
        for (int x = 0; x < boardX; x++) {
            for (int y = 0; y < boardY; y++) {
                Vector3Int pos = new Vector3Int(x - (boardX / 2), y - (boardY / 2), 0);
                if(nextVisible) {
                    canShipMap.SetTile(pos, null);
                }
                if(!nextVisible && hubBoard[x, y].canBeShip) {
                    canShipMap.SetTile(pos, nextShipTile);
                }
            }
        }
        nextVisible = !nextVisible;
    }

    // Builds the ship tile and updates the surounding ones to be able to be built
    public void buildShip(Vector3 position) {
        int x = shipMap.WorldToCell(position).x + (boardX / 2);
        int y = shipMap.WorldToCell(position).y + (boardY / 2);
        if(hubBoard[x, y].canBeShip) {
            shipMap.SetTile(shipMap.WorldToCell(position), shipTile);
            hubBoard[x, y].canBeShip = false;
            hubBoard[x, y].isShip = true;
            canShipMap.SetTile(canShipMap.WorldToCell(position), null);
            if(x > 0 && !hubBoard[x - 1, y].isShip) {
                hubBoard[x - 1, y].canBeShip = true;
            }
            if(x < boardX - 1 && !hubBoard[x + 1, y].isShip) {
                hubBoard[x + 1, y].canBeShip = true;
            }
            if(y > 0 && !hubBoard[x, y - 1].isShip) {
                hubBoard[x, y - 1].canBeShip = true;
            }
            if(y < boardY - 1 && !hubBoard[x, y + 1].isShip) {
                hubBoard[x, y + 1].canBeShip = true;
            }
        }
    }
}
