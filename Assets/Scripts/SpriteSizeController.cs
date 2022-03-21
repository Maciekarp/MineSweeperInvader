using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSizeController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer largeSprite;
    [SerializeField] private SpriteRenderer smallSprite;

    public void ShowBig() {
        largeSprite.enabled = true;
        smallSprite.enabled = false;
    }

    public void ShowSmall() {
        largeSprite.enabled = false;
        smallSprite.enabled = true;
    }
        
}
