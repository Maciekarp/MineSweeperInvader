using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class TileUIController : MonoBehaviour
{
    [SerializeField] private Button tileButton;
    [SerializeField] private GameObject redPanel1;
    [SerializeField] private GameObject redPanel2;
    [SerializeField] private Sprite woodSprite;
    [SerializeField] private Sprite stoneSprite;
    [SerializeField] private ResourceManager resources;
    [SerializeField] private RectTransform rectTrans;
    [SerializeField] private GameObject tile;
    [SerializeField] private GameObject resourceIcon1;
    [SerializeField] private GameObject resourceIcon2;
    [SerializeField] private Image resource1Renderer;
    [SerializeField] private Image resource2Renderer;
    [SerializeField] private GameObject plusIcon;
    [SerializeField] private TextMeshProUGUI resource1Text;
    [SerializeField] private TextMeshProUGUI resource2Text;
    [SerializeField] private UnityEvent buttonEvent;
    [SerializeField] private string resource1;
    [SerializeField] private string resource2;
    [SerializeField] private int resource1Cost;
    [SerializeField] private int resource2Cost;

    void Awake() {
        // if there is no first resouce hide all elements
        if(resource1Cost <= 0) {
            resourceIcon1.SetActive(false);
        } else {
            resource1Text.text = resource1Cost.ToString();
            ApplySprite(resource1Renderer, resource1);
        }
        // if there is no second resource hide the ui for it 
        if(resource2Cost <= 0) {
            resourceIcon2.SetActive(false);
            plusIcon.SetActive(false);
        } else {
            resource2Text.text = resource2Cost.ToString();
            ApplySprite(resource2Renderer, resource2);
        }
    }

    // helper function that applies the correct sprite to the Icon
    private bool ApplySprite(Image currRend, string resource) {
        if(resource == "wood") {
            currRend.sprite = woodSprite;
            return true;
        }
        if(resource == "stone") {
            currRend.sprite = stoneSprite;
            return true;
        }
        print("!!! \"" + resource + "\" does not exist !!!");
        return false;
    }

    // Sets the tile to the position given
    public void SetTile(Vector2 pos) {
        tile.SetActive(true);
        rectTrans.anchoredPosition = pos;
    }

    // Moves the tile up a certain height
    public void MoveUp(float height) {
        Vector2 currPos = rectTrans.anchoredPosition;
        currPos.y += height;
        rectTrans.anchoredPosition = currPos;
    }

    // Deactivates Tile
    public void ClearTile() {
        tile.SetActive(false);

    }

    // This is called when the tile is pressed if there are enough resources it will use those resources
    // and call the required functione if not resource amount will flash saying not enough resources
    public void ButtonEvent() {
        // has resources
        if((resource1 == "" && resource2 == "") ||
           (resource2 == "" && resources.GetResource(resource1) >= resource1Cost) ||
           (resources.GetResource(resource1) >= resource1Cost && resources.GetResource(resource2) >= resource2Cost)) {
            resources.RemoveResource(resource1, resource1Cost);
            if(resource2 != "") {
                resources.RemoveResource(resource2, resource2Cost);
            }
            buttonEvent.Invoke();
        // doesnt have resources
        } else {
            StartCoroutine(FlashRed(resources.GetResource(resource1) >= resource1Cost, 
                                    resource2 == "" || resources.GetResource(resource2) >= resource2Cost));
            //print("Not enough resources");
        }
    }

    IEnumerator FlashRed(bool hasFirst, bool hasSecond) {
        tileButton.interactable = false;
        if(!hasFirst) {
            redPanel1.SetActive(true);
        }
        if(!hasSecond) {
            redPanel2.SetActive(true);
        }
        yield return new WaitForSeconds(0.1f);
        if(!hasFirst) {
            redPanel1.SetActive(false);
        }
        if(!hasSecond) {
            redPanel2.SetActive(false);
        }
        yield return new WaitForSeconds(0.1f);
        if(!hasFirst) {
            redPanel1.SetActive(true);
        }
        if(!hasSecond) {
            redPanel2.SetActive(true);
        }
        yield return new WaitForSeconds(0.1f);
        if(!hasFirst) {
            redPanel1.SetActive(false);
        }
        if(!hasSecond) {
            redPanel2.SetActive(false);
        }
        tileButton.interactable = true;
    }
}
