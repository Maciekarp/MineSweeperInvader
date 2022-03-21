using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class REBotDrag : BotDrag
{
    public override void Awake() {
        maxBots = Stats.maxREBots;
        base.Awake();
    }

    public override void OnMouseDown() {
        gridCont.cursorTier = Stats.reBotTier;
    }

    public override void OnMouseUp() {
        // Dont end drag if mouse button is still down
        if(Input.GetMouseButton(0)) {
            return;
        }
        Vector3 currMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // if the mouse is not over UI drop saucer
        if(NumUIObjectsOverPointer() < 2) {
            if(!gridCont.isCloud(currMousePos)) {
                if(gridCont.takeResource(currMousePos, this)) {
                    currBots -= 1;
                    amountText.text = currBots.ToString();
                }
            }
        }
        // If not all bots are used return bot
        if(currBots != 0) {
            rectTransform.anchoredPosition = cardPos;
        } else {
            rectTransform.anchoredPosition = -cardPos;
        }
        gridCont.cursorTier = 0;
        gridCont.drawCursors(gridCont.GetMousePosition());
    }
}
