using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryControl : MonoBehaviour
{
    [SerializeField] private ResourceManager resources;
    [SerializeField] private TMPro.TextMeshProUGUI woodTxt;
    [SerializeField] private TMPro.TextMeshProUGUI stoneTxt;
    [SerializeField] private RectTransform invTransform;
    [SerializeField] private RectTransform arrowTransform;
    private Vector3 invPos;
    private bool inventoryShown = false;
    

    public void Awake() {
        invPos = invTransform.anchoredPosition;
        updateWood();
        updateStone();
    }

    public void Update() {
        updateWood();
        updateStone();
    }

    public void updateWood() {
        woodTxt.text = resources.GetWood().ToString();
    }

    public void updateStone() {
        stoneTxt.text = resources.GetStone().ToString();
    }

    // Toggles visibility of the inventory on the UI
    public void toggleInventory() {
        if(inventoryShown) {
            StartCoroutine(InvUp());
            inventoryShown = !inventoryShown;
        } else {
            StartCoroutine(InvDown());
            inventoryShown = !inventoryShown;
        }
    }

    IEnumerator InvDown() {
        for(int i = (int)invTransform.anchoredPosition.y; i > 0; i -= 20){
            yield return new WaitForSeconds(0.01f);
            invTransform.anchoredPosition = new Vector3(0, i, 0);
        }
        invTransform.anchoredPosition = new Vector3(0, 0, 0);
        arrowTransform.eulerAngles = new Vector3(0, 0, 180);
    }

    IEnumerator InvUp() {
        for(int i = 0; i < invPos.y; i += 20){
            yield return new WaitForSeconds(0.01f);
            invTransform.anchoredPosition = new Vector3(0, i, 0);
        }
        invTransform.anchoredPosition = invPos;
        arrowTransform.eulerAngles = new Vector3(0, 0, 0);
    }

}
