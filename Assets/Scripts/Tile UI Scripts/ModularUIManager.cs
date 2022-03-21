using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModularUIManager : MonoBehaviour {
    [SerializeField] private GameObject defaultUITile;
    [SerializeField] private RectTransform topUITransform;
    private List<GameObject> modularUITileList = new List<GameObject>();
    private List<TileUIController> modularTileUIList = new List<TileUIController>();
    private Vector2 defaultTopPos;
    private float modularTileHeight = 87.5f;

    //sucks dick
    void Start() { 
        modularTileHeight = defaultUITile.GetComponent<RectTransform>().sizeDelta.y;
        defaultTopPos = topUITransform.anchoredPosition;
    }

    // moves object height up the size of one tile
    private void MoveObjectUp(RectTransform rectTrans) {
        Vector2 currPos = rectTrans.anchoredPosition;
        currPos.y += modularTileHeight;
        rectTrans.anchoredPosition = currPos;
    }

    // adds the tile to the TileUI used for testing
    public void AddTile() {
        defaultUITile.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -92f);
        MoveObjectUp(topUITransform);
        foreach(GameObject tile in modularUITileList) {
            MoveObjectUp(tile.GetComponent<RectTransform>());
        }
        modularUITileList.Add(defaultUITile);
    }

    // adds the tile to the TileUI
    public void AddTile(GameObject newTile) {
        /*
        newTile.SetActive(true);
        newTile.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -92f);
        MoveObjectUp(topUITransform);
        foreach(GameObject tile in modularUITileList) {
            MoveObjectUp(tile.GetComponent<RectTransform>());
        }
        modularUITileList.Add(newTile);
        */
        MoveObjectUp(topUITransform);
        foreach(TileUIController curr in modularTileUIList) {
            curr.MoveUp(modularTileHeight);
        }
        TileUIController cont = newTile.GetComponent<TileUIController>();
        cont.SetTile(new Vector2(0f, -92f));
        modularTileUIList.Add(cont);
    }

    // Resets the UI to the empty TileUI
    public void ResetUI() {
        foreach(TileUIController curr in modularTileUIList) {
            curr.ClearTile();
        }
        modularTileUIList.Clear();
        topUITransform.anchoredPosition = defaultTopPos;
    }
}
