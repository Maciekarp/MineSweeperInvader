using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NewResourceController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private TextMeshProUGUI amountText;

    private const float MAX_HEIGHT = 1f;
    private const int STEPS = 100;
    private const float TIME_ANIM = 1f;

    // wrapper for DrawNewResource

    public void StartDraw(Vector3 startPos, int num) {
        StartCoroutine(DrawNewResource(startPos, num));
    }

    // animates the apearance and fade out of a new resource 
    IEnumerator DrawNewResource(Vector3 startPos, int num) {
        amountText.text = "+" + num.ToString();
        transform.position = startPos;
        Vector3 curr = startPos;
        Color currColorSprite = sprite.color;
        Color currColorText = amountText.color;
        for(int i = 0; i < STEPS; i++) {
            curr.y += MAX_HEIGHT / STEPS;
            transform.position = curr;
            currColorSprite.a -= 1f / STEPS;
            currColorText.a -= 1f / STEPS;
            sprite.color = currColorSprite;
            amountText.color = currColorText;
            yield return new WaitForSeconds(TIME_ANIM / STEPS);
        }
        Destroy(this);
    }
}
