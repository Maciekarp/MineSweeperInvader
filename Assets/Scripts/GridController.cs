using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;

public class GameTile {
    public bool isMine = false;
    public bool isLand = false;
    public GameObject resource = null;
    public bool resourceTaken = false;
    public bool isCloud = false;
    public bool isExploded = false;
    public GameObject building = null;
    public GameObject flag = null;
    public GameObject number = null;
    public bool hasHighlight = false;
    public bool onFire = false;
}

public class GridController : MonoBehaviour {
    public int cursorTier = 0;

    private Grid grid;
    [SerializeField] private ResourceManager resources;
    [SerializeField] private InventoryControl invCont;
    [SerializeField] private Tilemap mineMap = null;
    [SerializeField] private Tilemap waterMap = null;
    [SerializeField] private Tilemap landMap = null;
    [SerializeField] private Tilemap cloudMap = null;
    [SerializeField] private Tilemap cursorMap = null;
    [SerializeField] private AnimatedTile waterTile = null;
    [SerializeField] private RuleTile landTile = null;
    [SerializeField] private GameObject resourceTreesPrefab;
    [SerializeField] private GameObject resourceRocksPrefab;
    [SerializeField] private RuleTile cloudTile = null;
    [SerializeField] private AnimatedTile cursorTile = null;
    [SerializeField] private Tile[] bombTiles = null;
    [SerializeField] private Tile explosionTile = null;
    [SerializeField] private AnimatedTile fireTile = null;
    [SerializeField] private Tile craterTile;
    [SerializeField] private GameObject botPrefab;
    [SerializeField] private GameObject baseBuilding;
    [SerializeField] private BaseController baseController;

    [SerializeField] private Canvas tileUICanvas;
    [SerializeField] private RectTransform tileUITransform;
    [SerializeField] private ModularUIManager modularUIManager;
    [SerializeField] private GameObject extenderBuildUI;
    [SerializeField] private GameObject scannerBuildUI;
    [SerializeField] private GameObject launchPadBuildUI;
    [SerializeField] private GameObject waterCollectorBuildUI;
    [SerializeField] private GameObject putOutFireUI;
    [SerializeField] private GameObject repairUI;
    [SerializeField] private GameObject removeUI;

    [SerializeField] private GameObject extenderPrefab;
    [SerializeField] private GameObject scannerPrefab;
    [SerializeField] private GameObject launchPadPrefab;
    [SerializeField] private GameObject waterCollectorPrefab;
    [SerializeField] private GameUIManager gameUIManager;
    [SerializeField] private GameObject highlightPrefab;
    [SerializeField] private GameObject flagPrefab;
    [SerializeField] private GameObject[] numPrefabList;
    private GameTile[,] gameBoard;
    private bool numShown = false;
    private bool isBigNum = true;
    public int boardX = 10, boardY = 10; 
    [Range(0f, 1f)] public float chanceMines = 0.5f;
    private Vector3Int previousMousePos = new Vector3Int();
    [Range(0f, 1f)] public float chanceLand = 0.1f;
    [Range(0, 10)] public int landIterations = 2;
    [Range(1, 7)] public int minNeighbors = 3;
    [Range(0, 5)] public int neighborNeeded = 0;
    [Range(0f, 1f)] public float chanceEdgeDead = 0.05f;
    [Range(0f, 1f)] public float resourceChance = 0.25f;
    private int resourceTreesWeight = 5;
    private int resourceRocksWeight = 5;
    private List<BuildingController> buildingList = new List<BuildingController>();
    private int currTileUIX, currTileUIY;
    private float tileUIDispX = 0.5f, tileUIDispY = 3f;

    // Start is called before the first frame Update
    void Awake() {
        // initializes variables and fills the gameboard
        grid = gameObject.GetComponent<Grid>();
        gameBoard = new GameTile[boardX, boardY];
        for(int x = 0; x < boardX; x++){
            for(int y = 0; y < boardY; y++){
                gameBoard[x,y] = new GameTile(); 

                // Draws clouds and water based off board size
                Vector3Int pos = new Vector3Int(x - (boardX /2),y - (boardY / 2), 0);
                waterMap.SetTile(pos, waterTile);
                cloudMap.SetTile(pos, cloudTile);
                gameBoard[x,y].isCloud = true;
                // Spawns mines
                if(Random.value < chanceMines && !(Mathf.Abs(x - (boardX / 2)) < 2 && Mathf.Abs(y - (boardY / 2)) < 2)) {
                    //mineMap.SetTile(pos, mineTile);
                    gameBoard[x, y].isMine = true;
                }
            }
        }

        // Assigns number of mines around tile
        for (int x = 0; x < boardX; x++) {
            for (int y = 0; y < boardY; y++) {
                gameBoard[x, y].number = Instantiate(numPrefabList[NumMines(x, y)]);
                gameBoard[x, y].number.transform.position = new Vector3(x - boardX / 2 + 0.5f, y - boardY / 2 + 0.5f, 0f);
                gameBoard[x, y].number.GetComponent<SpriteSizeController>().ShowBig();
                gameBoard[x, y].number.SetActive(false);
            }
        }

        drawLand();

        spawnResources();

        gameBoard[boardX / 2, boardY / 2].building = baseBuilding;
        buildingList.Add(baseBuilding.GetComponent<BuildingController>());
        BuildingController baseBuildingController = baseBuilding.GetComponent<BuildingController>();
        baseBuildingController.buildingType = "base";
        baseBuildingController.boardX = boardX / 2;
        baseBuildingController.boardY = boardY / 2;
        
        baseController.StartLanding();
    }

    // Update is called once per frame
    void Update() {
        if(gameUIManager.isPaused) {
            return;
        }
        // draws cursor if mouse moves enough
        Vector3Int mousePos = GetMousePosition();
        if(!mousePos.Equals(previousMousePos)) {
            drawCursors(mousePos);
            previousMousePos = mousePos;
        }

        // Debuging tool 
        if(Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftControl)) {
            int x = (int) mousePos.x + boardX / 2;
            int y = (int) mousePos.y + boardY / 2;
            print(gameBoard[x, y].resource.GetComponent<ResourceController>().type);
        }
        
        // On click toggles UI for that tile
        if(!EventSystem.current.IsPointerOverGameObject()) {
            if(Input.GetMouseButtonDown(0)) {
                TileUIManager(mousePos.x, mousePos.y);
            }
        }

        if(Input.GetMouseButtonDown(1)) {
            ToggleFlag(mousePos.x + (boardX / 2), mousePos.y + (boardY / 2));
        }

        /*
        // used to test mine triggering
        if(Input.GetMouseButtonDown(1)) {
            StartCoroutine(Explode(mousePos.x + (boardX / 2), mousePos.y + (boardY / 2)));
        }
        */

        // toggles showing and hiding numbers
        if(Input.GetKeyDown(KeyCode.Space)) {
            NumToggle();
        }
    }

    // recursive function that explodes the tile and triggers the neighbor mines
    IEnumerator Explode(int x, int y) {
        // Base case if it is outside the board or already exploded
        if(x < 0 || y < 0 || x >= boardX || y >= boardY) yield break;
        if(tileUICanvas.enabled && (currTileUIX == x - boardX / 2 && currTileUIY == y - boardY / 2)) {
            TileUIManager(x - boardX / 2, y - boardY / 2);
        }
        if(!gameBoard[x, y].isMine || gameBoard[x, y].isExploded) {
            if(gameBoard[x, y].isLand) {
                if(gameBoard[x, y].building == null) {
                    mineMap.SetTile(new Vector3Int(x - (boardX / 2), y - (boardY / 2), 0), fireTile);
                    gameBoard[x, y].onFire = true;
                }
                //resourceMap.SetTile(new Vector3Int(x - (boardX / 2), y - (boardY / 2), 0), null);
                if(gameBoard[x, y].resource != null) {
                    Destroy(gameBoard[x, y].resource);
                    gameBoard[x, y].resource = null;
                }
                // if bot is on tile kill it  NEEDS TO BE INPLEMENTED
                
                // if Tile is exploded set to crater
                if(gameBoard[x, y].isExploded) {
                    mineMap.SetTile(new Vector3Int(x - (boardX / 2), y - (boardY / 2), 0), craterTile);
                }

                // if has building that is alive kill it
                if(gameBoard[x, y].building != null && gameBoard[x, y].building.GetComponent<BuildingController>().buildingState != "onfire") {
                    BuildingController currBC = gameBoard[x, y].building.GetComponent<BuildingController>();
                    currBC.KillBuilding();
                    gameBoard[x, y].onFire = true;
                    mineMap.SetTile(new Vector3Int(x - (boardX / 2), y - (boardY / 2), 0), null);
                    
                    ResetPower();
                }
            }
            yield break;
        } 

        gameBoard[x, y].isExploded = true;
        // recursive case that starts the explosion animation and calls tiles around it
        for(int i = 0; i < bombTiles.Length; i++) {
            mineMap.SetTile(new Vector3Int(x - (boardX / 2), y - (boardY / 2), 0), bombTiles[i]);
            yield return new WaitForSeconds(0.1f);
        }
        mineMap.SetTile(new Vector3Int(x - (boardX / 2), y - (boardY / 2), 0), explosionTile);
        yield return new WaitForSeconds(0.1f);
        mineMap.SetTile(new Vector3Int(x - (boardX / 2), y - (boardY / 2), 0), null);
        for(int i = -1; i < 2; i++) {
            for(int j = -1; j < 2; j++) {
                StartCoroutine(Explode(x + i, y + j));
            }
        }
    }

    // Builds the tile specified
    public void BuildOnTile(string buildingType) { 
        int currX = currTileUIX + boardX / 2;
        int currY = currTileUIY + boardY / 2;

        // if the tile being built on is a mine start explosion and dont build building
        if(gameBoard[currX, currY].isMine) {
            StartCoroutine(Explode(currX, currY));
            TileUIManager(currTileUIX, currTileUIY);
            return;
        }

        // Builds extender
        if(buildingType == "extender") {
            gameBoard[currX, currY].building = Instantiate(extenderPrefab);
        
        // Builds Scanner
        } else if(buildingType == "scanner") {
            gameBoard[currX, currY].building = Instantiate(scannerPrefab);
        
        // Builds Launch Pad
        } else if(buildingType == "launchPad") {
            gameBoard[currX, currY].building = Instantiate(launchPadPrefab);
        
        // Builds Water Collector
        } else if(buildingType == "waterCollector") {
            gameBoard[currX, currY].building = Instantiate(waterCollectorPrefab);
            // Sets active pipes
            gameBoard[currX, currY].building.GetComponent<WaterController>().SetPipes(
                currY != boardY - 1 && !gameBoard[currX , currY + 1].isLand, // North
                currY != 0 && !gameBoard[currX , currY - 1].isLand, // South
                currX != boardX - 1 && !gameBoard[currX + 1, currY].isLand, // East
                currX != 0 && !gameBoard[currX - 1, currY].isLand // West
            );
        // In case of incorrect input
        } else {
            print("\"" + buildingType + "\" is the incorect string");
            return;
        }

        gameBoard[currX, currY].building.transform.position = new Vector3(currTileUIX + 0.5f, currTileUIY + 0.5f, 0f);
        TileUIManager(currTileUIX, currTileUIY);
        BuildingController currBC = gameBoard[currX, currY].building.GetComponent<BuildingController>();
        currBC.buildingType = buildingType;

        currBC.boardX = currX;
        currBC.boardY = currY;
        buildingList.Add(currBC);

        // manages the wires of current and neighboring tiles
        // North West
        if(currX != 0 && currY != boardY - 1 && gameBoard[currX - 1, currY + 1].building != null) {
            BuildingController otherBC = gameBoard[currX - 1, currY + 1].building.GetComponent<BuildingController>();
            otherBC.SetSE(currBC);
            currBC.SetNW(otherBC);
        }
        // North
        if(currY != boardY - 1 && gameBoard[currX, currY + 1].building != null) {
            BuildingController otherBC = gameBoard[currX, currY + 1].building.GetComponent<BuildingController>();
            otherBC.SetS(currBC);
            currBC.SetN(otherBC);
        }
        // North East
        if(currX != boardX - 1 && currY != boardY - 1 && gameBoard[currX + 1, currY + 1].building != null) {
            BuildingController otherBC = gameBoard[currX + 1, currY + 1].building.GetComponent<BuildingController>();
            otherBC.SetSW(currBC);
            currBC.SetNE(otherBC);
        }
        // West
        if(currX != 0 && gameBoard[currX - 1, currY].building != null) {
            BuildingController otherBC = gameBoard[currX - 1, currY].building.GetComponent<BuildingController>();
            otherBC.SetE(currBC);
            currBC.SetW(otherBC);
        }
        // East
        if(currX != boardX - 1 && gameBoard[currX + 1, currY].building != null) {
            BuildingController otherBC = gameBoard[currX + 1, currY].building.GetComponent<BuildingController>();
            otherBC.SetW(currBC);
            currBC.SetE(otherBC);
        }
        // South West
        if(currX != 0 && currY != 0 && gameBoard[currX - 1, currY - 1].building != null) {
            BuildingController otherBC = gameBoard[currX - 1, currY - 1].building.GetComponent<BuildingController>();
            otherBC.SetNE(currBC);
            currBC.SetSW(otherBC);
        }
        // South
        if(currY != 0 && gameBoard[currX, currY - 1].building != null) {
            BuildingController otherBC = gameBoard[currX, currY - 1].building.GetComponent<BuildingController>();
            otherBC.SetN(currBC);
            currBC.SetS(otherBC);
        }
        // South East
        if(currX != boardX - 1 && currY != 0 && gameBoard[currX + 1, currY - 1].building != null) {
            BuildingController otherBC = gameBoard[currX + 1, currY - 1].building.GetComponent<BuildingController>();
            otherBC.SetNW(currBC);
            currBC.SetSE(otherBC);
        }
        ResetPower();
    }

    // Clears fire on the tile where the ui is currently open
    public void ClearFire() {
        int currX = currTileUIX + boardX / 2;
        int currY = currTileUIY + boardY / 2;
        if(gameBoard[currX, currY].building == null) {
            mineMap.SetTile(new Vector3Int(currTileUIX, currTileUIY, 0), null);
            gameBoard[currX, currY].onFire = false;
            TileUIManager(currTileUIX, currTileUIY);
            return;
        }
        
        BuildingController currCont = gameBoard[currX, currY].building.GetComponent<BuildingController>();
        if(currCont.buildingState == "onfire") {
            currCont.ClearFire();
            TileUIManager(currTileUIX, currTileUIY);
            gameBoard[currX, currY].onFire = false;
            return;
        }

        print("Something is wrong clear fire was called on a non fire tile!!!!!!!");
    }

    // Clears fire on the tile specified by the mouse position
    public void ClearFire(Vector3 position) {
        int currX = cloudMap.WorldToCell(position).x + (boardX / 2); 
        int currY = cloudMap.WorldToCell(position).y + (boardY / 2);
        if(gameBoard[currX, currY].building == null) {
            mineMap.SetTile(new Vector3Int(currX - (boardX / 2), currY - (boardY / 2), 0), null);
            gameBoard[currX, currY].onFire = false;
            return;
        }
        
        BuildingController currCont = gameBoard[currX, currY].building.GetComponent<BuildingController>();
        if(currCont.buildingState == "onfire") {
            currCont.ClearFire();
            gameBoard[currX, currY].onFire = false;
            return;
        }

        print("Something is wrong clear fire was called on a non fire tile!!!!!!!");
    }

    // Repairs building 
    public void RepairBuilding() {
        int currX = currTileUIX + boardX / 2;
        int currY = currTileUIY + boardY / 2;
        BuildingController currCont = gameBoard[currX, currY].building.GetComponent<BuildingController>();
        currCont.RepairBuilding();
        TileUIManager(currTileUIX, currTileUIY);
        ResetPower();
    }

    // Removes building
    public void RemoveBuilding() {
        int currX = currTileUIX + boardX / 2;
        int currY = currTileUIY + boardY / 2;
        BuildingController currCont = gameBoard[currX, currY].building.GetComponent<BuildingController>();
        currCont.RemoveBuilding();
        buildingList.Remove(currCont);
        Destroy(gameBoard[currX, currY].building);
        gameBoard[currX, currY].building = null;
        TileUIManager(currTileUIX, currTileUIY);
        ResetPower();
    }

    // Draws the land using Cellular automata stuff
    private void drawLand() {
        bool[,] nextLand = new bool[boardX, boardY];
        // Places random land tiles
        for (int x = 0; x < boardX; x++) {
            for (int y = 0; y < boardY; y++) {
                Vector3Int pos = new Vector3Int(x - (boardX /2),y - (boardY / 2),0);
                if(Random.value < chanceLand) {
                    landMap.SetTile(pos, landTile);
                    gameBoard[x, y].isLand = true;
                }
            }
        }

        // Makes the center have land 
        for(int x = boardX / 2 - 1; x <= boardX / 2 + 1; x++) {
            for(int y = boardY / 2 - 1; y <= boardY / 2 + 1; y++) {
                Vector3Int pos = new Vector3Int(x - (boardX /2),y - (boardY / 2),0);
                landMap.SetTile(pos, landTile);
                gameBoard[x, y].isLand = true;
            }
        }

        // each iteration is one round of cellular automata
        for(int i = 0; i < landIterations; i++) {
            for (int x = 0; x < boardX; x++) {
                for (int y = 0; y < boardY; y++) {
                    // if a tile doesnt have neighbors in the 4 cardinal directions has a chance to die
                    if(x > 0 && x < boardX - 1 && y > 0 && y < boardY - 1 && gameBoard[x, y].isLand) {
                        if(!(gameBoard[x - 1, y].isLand &&
                            gameBoard[x + 1, y].isLand &&
                            gameBoard[x, y - 1].isLand &&
                            gameBoard[x, y + 1].isLand)) {
                            if(Random.value < chanceEdgeDead) {
                                Vector3Int pos = new Vector3Int(x - (boardX / 2), y - (boardY / 2), 0);
                                landMap.SetTile(pos, null);
                                gameBoard[x, y].isLand = false;
                            }
                        }
                    }
                    // gets the new land map in nextLand
                    if(NumLand(x, y) >= minNeighbors) {
                        nextLand[x, y] = true;
                    }
                    // kill if below neighbor amount
                    if(NumLand(x,y) <= neighborNeeded) {
                        nextLand[x, y] = false;
                    }
                }
            }
            // sets the tiles to the next land ones and updates the map
            for (int x = 0; x < boardX; x++) {
                for (int y = 0; y < boardY; y++) {
                    Vector3Int pos = new Vector3Int(x - (boardX /2),y - (boardY / 2),0);
                    gameBoard[x, y].isLand = nextLand[x, y];
                    if(gameBoard[x, y].isLand) {
                        landMap.SetTile(pos, landTile);
                    } else {
                        landMap.SetTile(pos, null);
                    }
                }
            }
        }
    }

    // Used to update which building have or dont have power by using a recursive function that regains power from the base
    private void ResetPower() {
        foreach(BuildingController buildCont in buildingList) {
            if(buildCont.buildingState == "alive") {
                buildCont.LosePower();
            }
        }
        buildingList[0].GainPower();

        // resets the numbers visible
        NumToggle();
        NumToggle();
    }

    // gets the number of land around a tile
    private int NumLand(int currX, int currY) {
        int numLand = 0;
        for (int x = currX - 1; x <= currX + 1; x++) {
            for (int y = currY - 1; y <= currY + 1; y++) {
                if(x >= 0 && x < boardX && y >= 0 && y < boardY) {
                    if(gameBoard[x, y].isLand) {
                        numLand++;
                    }
                }
            }
        }
        if(gameBoard[currX, currY].isLand){
            numLand--;
        }
        return numLand;
    }

    // Spawns and draws resources
    private void spawnResources() {
        float[] probs = new float[2];
        // uses the weight of each resource to calculate probabilities that it will spawn
        int totalWeight = resourceTreesWeight + resourceRocksWeight;
        probs[0] = (float)resourceTreesWeight / totalWeight * resourceChance;
        
        probs[1] = (float)resourceRocksWeight / totalWeight * resourceChance + probs[0];

        // spawns the resource tiles
        for (int x = 0; x < boardX; x++) {
            for (int y = 0; y < boardY; y++) {
                // will not spawn on the center tiles
                if(gameBoard[x, y].isLand && !(Mathf.Abs(x - (boardX / 2)) < 2 && Mathf.Abs(y - (boardY / 2)) < 2)) {
                    Vector3 pos = new Vector3(x - (boardX / 2) + 0.5f, y - (boardY / 2) + 0.5f, 0f);
                    float rand = Random.value;
                    if(rand < probs[0]){
                        gameBoard[x, y].resource = Instantiate(resourceTreesPrefab);
                        gameBoard[x, y].resource.GetComponent<ResourceController>().type = "wood";
                        gameBoard[x, y].resource.transform.position = pos;
                    } else {
                        if(rand < probs[1]) {
                            gameBoard[x, y].resource = Instantiate(resourceRocksPrefab);
                            gameBoard[x, y].resource.GetComponent<ResourceController>().type = "stone";
                            gameBoard[x, y].resource.transform.position = pos;
                        }
                    }
                }
            }
        }
    }

    // Opens tile UI based off the tile clicked on
    public void TileUIManager(int x, int y) {
        // if the clicked tile is a cloud you can only close tile UI not open
        GameTile currGameTile = gameBoard[x + boardX / 2, y + boardY / 2];
        if(currGameTile.isCloud) {
            modularUIManager.ResetUI();
            tileUICanvas.enabled = false;
            return;
        }

        // turns on tile if tile has a UI and is clicked
        if(!tileUICanvas.enabled || !(currTileUIX == x && currTileUIY == y)) {
            // if the ui is already open but another tile is clicked reste the ui
            if(tileUICanvas.enabled) {
                modularUIManager.ResetUI();
            }

            

            BuildingController buildCont = null;
            if(currGameTile.building != null) {
                buildCont = currGameTile.building.GetComponent<BuildingController>();
            }
            if(gameBoard[x + boardX / 2, y + boardY / 2].onFire && !gameBoard[x + boardX / 2, y + boardY / 2].isMine) {
                modularUIManager.AddTile(putOutFireUI);
            // Different UI is built for different tiles
            // if tile has fire give option to clear it
            // if it has a building that building needs to be interactable to show UI
            } else if(currGameTile.building != null) {
                if(buildCont.buildingType == "base"){

                }
                if(buildCont.buildingState == "onfire") {
                    modularUIManager.AddTile(putOutFireUI);
                }
                if(buildCont.buildingState == "dead") {
                    modularUIManager.AddTile(repairUI);
                }
                if((buildCont.buildingState == "alive" || buildCont.buildingState == "dead") && buildCont.buildingType != "base"){
                    modularUIManager.AddTile(removeUI);
                }
            // if the tile has no building it must be buildable to be interacted with
            } else {
                if(!CanBuildOn(x + boardX / 2, y + boardY / 2)) {
                    modularUIManager.ResetUI();
                    tileUICanvas.enabled = false;
                    return;
                }
                // if the tile has a resource 
                if(gameBoard[x + boardX / 2, y + boardY / 2].resource != null) {

                } else {
                    modularUIManager.AddTile(extenderBuildUI);
                    modularUIManager.AddTile(scannerBuildUI);
                    modularUIManager.AddTile(launchPadBuildUI);
                    if(IsNearWater(x + boardX / 2, y + boardY / 2)) {
                        modularUIManager.AddTile(waterCollectorBuildUI);
                    }
                }
            }
            tileUICanvas.enabled = true;
            tileUITransform.anchoredPosition = new Vector3(x + tileUIDispX, y + tileUIDispY, 0);
            currTileUIX = x;
            currTileUIY = y;
            
        } else {
            modularUIManager.ResetUI();
            tileUICanvas.enabled = false;
            
        }
    }

    // Returns true if the tile given is buildable
    public bool CanBuildOn(int x, int y) {
        if(gameBoard[x, y].isLand && !gameBoard[x, y].onFire && !gameBoard[x, y].isExploded) {
            // goes through the neighbors of the tile checking if they are buildings and alive
            for (int currX = Mathf.Max(0, x - 1); currX <= Mathf.Min(boardX - 1, x + 1); currX++) {
                for (int currY = Mathf.Max(0, y - 1); currY <= Mathf.Min(boardY - 1, y + 1); currY++) {
                    if(gameBoard[currX, currY].building != null) {
                        if(gameBoard[currX, currY].building.GetComponent<BuildingController>().buildingState == "alive") {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    // Draws curor tiles 
    public void drawCursors(Vector3Int mousePos) {
        int prevX = cursorMap.WorldToCell(previousMousePos).x + (boardX / 2);
        int prevY = cursorMap.WorldToCell(previousMousePos).y + (boardY / 2);
        int x = cursorMap.WorldToCell(mousePos).x + (boardX / 2);
        int y = cursorMap.WorldToCell(mousePos).y + (boardY / 2);
        
        // removes possible cursor tiles
        for(int i = -1; i < 2; i++) {
            for(int j = -1; j < 2; j++) {
                drawTile(prevX + i, prevY + j, null, cursorMap);
            }
        }
        
        // Decides where to put the cursor tiles based off the tier
        switch(cursorTier) {
            // Tier 0
            case 0:
                drawTile(x, y, cursorTile, cursorMap);
                break;
            // Tier 1
            case 1:
                drawTile(x, y, cursorTile, cursorMap);
                drawTile(x - 1, y, cursorTile, cursorMap);
                drawTile(x + 1, y, cursorTile, cursorMap);
                drawTile(x, y - 1, cursorTile, cursorMap);
                drawTile(x, y + 1, cursorTile, cursorMap);
                break;
            // Tier 2
            case 2:
                for(int i = -1; i < 2; i++) {
                    for(int j = -1; j < 2; j++) {
                        drawTile(x + i, y + j, cursorTile, cursorMap);
                    }
                }
                break;

        }
        
        
    }

    //
    private void drawTile(int x, int y, AnimatedTile tile, Tilemap tilemap) {
        if(x < 0 || y < 0 || x >= boardX || y >= boardY){
            return;
        }
        tilemap.SetTile(new Vector3Int(x - (boardX / 2), y - (boardY / 2), 0), tile);
    }

    // returnes true if tile is a cloud at position
    public bool isCloud(Vector3 position) {
        return gameBoard[cloudMap.WorldToCell(position).x + (boardX / 2), 
                         cloudMap.WorldToCell(position).y + (boardY / 2)].isCloud;
    }

    // Acts as the controller for the cloud clearing bot
    public bool ClearCloudController(Vector3 position) {
        int x = cloudMap.WorldToCell(position).x + (boardX / 2);
        int y = cloudMap.WorldToCell(position).y + (boardY / 2);
        bool clearedCloud = false;
        switch(Stats.cloudBotTier) {
            // Tier 0
            case 0:
                return ClearCloud(x, y);
            // Tier 1
            case 1:
                if(ClearCloud(x, y)) clearedCloud = true;
                if(ClearCloud(x + 1, y)) clearedCloud = true;
                if(ClearCloud(x - 1, y)) clearedCloud = true;
                if(ClearCloud(x, y + 1)) clearedCloud = true;
                if(ClearCloud(x, y - 1)) clearedCloud = true;
                break;
            // Tier 2
            case 2:
                for(int currX = x - 1; currX <= x + 1; currX++) {
                    for(int currY = y - 1; currY <= y + 1; currY++) {
                        if(ClearCloud(currX, currY)) {
                            clearedCloud = true;
                        }
                    }
                }
                return clearedCloud;
        }
        return clearedCloud;
    }

    // Clears cloud at board coordinate returns true if successful
    public bool ClearCloud(int x, int y) {
        if(x < 0 || y < 0 || x >= boardX || y >= boardY){
            return false;
        }
        if(gameBoard[x, y].isCloud) {
            cloudMap.SetTile(new Vector3Int(x - (boardX / 2), y - (boardY / 2), 0), null);
            gameBoard[x, y].isCloud = false;
            return true;
        }
        return false;
    }

    // Places saucer at coordinate returns true if robot is used
    public bool takeResource(Vector3 position, REBotDrag reBotDrag) {
        int x = (int) (position.x + (boardX / 2));
        int y = (int) (position.y + (boardY / 2));
        // places a bot
        if(gameBoard[x, y].resource != null) {
            gameBoard[x, y].resourceTaken = true;
            GameObject saucer = Instantiate(botPrefab);
            SaucerController saucerCont = saucer.GetComponent<SaucerController>();
            saucerCont.SetResource(x - (boardX / 2), y - (boardY / 2), resources, gameBoard[x, y].resource, reBotDrag);
            return true;
        }
        return false;
    }

    // used to place or remove flag
    public void ToggleFlag(int x, int y) {
        if(gameBoard[x, y].isCloud || gameBoard[x, y].building != null) {
            return;
        }
        if(gameBoard[x, y].flag == null) {
            gameBoard[x, y].flag = Instantiate(flagPrefab);
            gameBoard[x, y].flag.transform.position = new Vector3(x - boardX / 2 + 0.5f, y - boardY / 2 + 0.5f, 0f);
            if(isBigNum) {
                gameBoard[x, y].flag.GetComponent<SpriteSizeController>().ShowSmall();
            } else {
                gameBoard[x, y].flag.GetComponent<SpriteSizeController>().ShowBig();
            }
            gameBoard[x, y].flag.SetActive(numShown);
        } else {
            Destroy(gameBoard[x, y].flag);
        }
    }

    // Draws numbers or removes them
    public void NumToggle() {
        // if number is shown hide numbers
        if(numShown) {
            for (int x = 0; x < boardX; x++) {
                for (int y = 0; y < boardY; y++) {
                    gameBoard[x, y].number.SetActive(false);
                    if(gameBoard[x, y].flag != null) {
                        gameBoard[x, y].flag.SetActive(false);
                    }
                }
            }
            numShown = false;
            return;
        }

        // otherwise show them 
        foreach(BuildingController buildCont in buildingList){
            if((buildCont.buildingType == "scanner" || buildCont.buildingType == "base") && buildCont.buildingState == "alive"){
                for (int x = Mathf.Max(0, buildCont.boardX - 1); x <= Mathf.Min(boardX - 1, buildCont.boardX + 1); x++) {
                    for (int y = Mathf.Max(0, buildCont.boardY - 1); y <= Mathf.Min(boardY - 1, buildCont.boardY + 1); y++) {
                        gameBoard[x, y].number.SetActive(true);
                    }
                }
            }
        }
        for (int x = 0; x < boardX; x++) {
                for (int y = 0; y < boardY; y++) {
                    if(gameBoard[x, y].flag != null) {
                        gameBoard[x, y].flag.SetActive(true);
                    }
                }
            }
        numShown = true;
    }

    public void SweeperUIToggle() {
        // if Big number and small flag is shown make the numbers small and flag big
        if(isBigNum) {
            for (int x = 0; x < boardX; x++) {
                for (int y = 0; y < boardY; y++) {
                    gameBoard[x, y].number.GetComponent<SpriteSizeController>().ShowSmall();
                    if(gameBoard[x, y].flag != null) {
                        gameBoard[x, y].flag.GetComponent<SpriteSizeController>().ShowBig();
                    }
                }
            }
            isBigNum = false;
            return;
        }

        // if small number and big flag is shown make the numbers big and flag small
        for (int x = 0; x < boardX; x++) {
            for (int y = 0; y < boardY; y++) {
                gameBoard[x, y].number.GetComponent<SpriteSizeController>().ShowBig();
                if(gameBoard[x, y].flag != null) {
                    gameBoard[x, y].flag.GetComponent<SpriteSizeController>().ShowSmall();
                }
            }
        }
        isBigNum = true;
    }

    //8===D

    // Gets the amount of mines around a tile
    private int NumMines(int currX, int currY) {
        int numMines = 0;
        for (int x = currX - 1; x <= currX + 1; x++) {
            for (int y = currY - 1; y <= currY + 1; y++) {
                if(x >= 0 && x < boardX && y >= 0 && y < boardY) {
                    if(gameBoard[x, y].isMine) {
                        numMines++;
                    }
                }
            }
        }
        if(gameBoard[currX, currY].isMine){
            numMines--;
        }
        return numMines;
    }

    // returns true of the tile is near water
    private bool IsNearWater(int currX, int currY) {
        if(currX != 0 && !gameBoard[currX - 1, currY].isLand) {
            return true;
        }
        if(currY != 0 && !gameBoard[currX , currY - 1].isLand) {
            return true;
        }
        if(currX != boardX - 1 && !gameBoard[currX + 1, currY].isLand) {
            return true;
        }
        if(currY != boardY - 1 && !gameBoard[currX , currY + 1].isLand) {
            return true;
        }
        return false;
    }

    // Shows what tiles can be built on
    public void ShowBuildable() {
        foreach(BuildingController buildCont in buildingList) {
            for (int x = Mathf.Max(0, buildCont.boardX - 1); x <= Mathf.Min(boardX - 1, buildCont.boardX + 1); x++) {
                for (int y = Mathf.Max(0, buildCont.boardY - 1); y <= Mathf.Min(boardY - 1, buildCont.boardY + 1); y++) {
                    if(!gameBoard[x, y].hasHighlight && gameBoard[x, y].building == null && CanBuildOn(x, y)) {
                        gameBoard[x, y].hasHighlight = true;
                        StartCoroutine(HighlightAnimate(x, y));
                    }
                }
            }
        }
    }

    // manages the animation of the hightlight 
    IEnumerator HighlightAnimate(int x, int y) {
        int maxAlpha = 80;
        int steps = 10;
        GameObject highlightObject = Instantiate(highlightPrefab);
        SpriteRenderer highlightRenderer = highlightObject.GetComponent<SpriteRenderer>();
        highlightObject.transform.position = new Vector3(x - (boardX / 2) + 0.5f, y - (boardY / 2) + 0.5f, 0f);
        Color color = highlightRenderer.color;
        for(int i = 0; i < maxAlpha / steps; i++) {
            color.a = (i * (maxAlpha / steps) / 100f);
            highlightRenderer.color = color;
            yield return new WaitForSeconds(0.04f);
        }
        for(int i = maxAlpha / steps; i > 0; i--) {
            yield return new WaitForSeconds(0.04f);
            color.a = (i * (maxAlpha / steps) / 100f);
            highlightRenderer.color = color;
        }
        gameBoard[x, y].hasHighlight = false;
        Destroy(highlightObject);
        
    }

    public Vector3Int GetMousePosition() {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return grid.WorldToCell(mouseWorldPos);
    }

    // returns whether or not there is a fire at that tile and it is not a mine
    public bool IsOnFire(Vector3 position) {
        return gameBoard[cloudMap.WorldToCell(position).x + (boardX / 2), 
                         cloudMap.WorldToCell(position).y + (boardY / 2)].onFire && 
               !gameBoard[cloudMap.WorldToCell(position).x + (boardX / 2), 
                          cloudMap.WorldToCell(position).y + (boardY / 2)].isMine;        
    } 
}
