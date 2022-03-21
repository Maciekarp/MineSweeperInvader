// General bot drag script that specific bots inherit from

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class BotDrag : MonoBehaviour {
    
    [SerializeField] protected Canvas canvas;
    [SerializeField] protected GridController gridCont;
    [SerializeField] protected TMPro.TextMeshProUGUI amountText;
    [SerializeField] protected TMPro.TextMeshProUGUI maxText;

    [HideInInspector] protected int maxBots;
    [HideInInspector] protected RectTransform rectTransform;
    [HideInInspector] protected Vector3 cardPos;
    [HideInInspector] protected int currBots;

    public virtual void Awake() {
        rectTransform = GetComponent<RectTransform>();
        cardPos = rectTransform.anchoredPosition;
        currBots = maxBots;
        maxText.text = maxBots.ToString();
        amountText.text = currBots.ToString();
    }

    // Used to add bot if currBots was 0 returns bot to cardPos
    public void AddBot() {
        if(currBots == 0) {
            rectTransform.anchoredPosition = cardPos;
        }
        currBots++;
        amountText.text = currBots.ToString();
    }

    public virtual void OnMouseDown() {
        // Virtual
    }

    // moves the bot with the mouse position when dragged
    public void OnMouseDrag() {
        if(!Input.GetMouseButton(0)) {
            return;
        }
        rectTransform.anchoredPosition = Input.mousePosition;
    }

    // When the bot is dropped
    public virtual void OnMouseUp() {
        // Virtual 
    }

    // Determines how many UI elements are over the cursor 
    // used to make sure dropped object is not over UI
    protected int NumUIObjectsOverPointer() {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count;
    }
}
