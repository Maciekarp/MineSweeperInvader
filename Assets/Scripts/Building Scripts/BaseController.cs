using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour {
    [SerializeField] private GameObject radarDish;
    [SerializeField] private Renderer baseRend;
    [SerializeField] private GameObject lander;
    [SerializeField] private GridController gc;

    [HideInInspector] private Vector3 homePos;

    // localX and localY represent the amound of pixels off center
    [HideInInspector] private int localX = 0;
    [HideInInspector] private int localY = 0;
    void Awake() {
        homePos = transform.position;
    }

    // function that updates the position of the sprite
    private void Draw(){
        transform.position = new Vector3(localX / 32f + homePos.x, localY / 32f + homePos.y, 0);
    }

    // Wrapper that starts the landing coroutine
    public void StartLanding() {
        StartCoroutine(Landing());
    }

    // animates the landing, crash, and starts idle animations
    IEnumerator Landing() { 
        // Flying down animation
        for(int i = 0; i < (homePos.y - 0.5) * 32; i++) {
            yield return new WaitForSeconds(0.001f);
            localY--;
            Draw();
        }
        // Clears clouds and replaces lander with base
        homePos = transform.position;
        gc.ClearCloud(gc.boardX / 2, gc.boardY / 2);
        lander.SetActive(false);
        baseRend.enabled = true;
        yield return new WaitForSeconds(0.1f);
        for(int x = -1; x < 2; x++) {
            for(int y = -1; y < 2; y++) {
                //gc.AllowNum(gc.boardX / 2 + x, gc.boardY / 2  + y);
                gc.ClearCloud(gc.boardX / 2 + x, gc.boardY / 2  + y);
            }
        }

        // Moves Radar Dish up
        Vector3 radarPos;
        for(int i = 0; i < 10; i++) { 
            radarPos = radarDish.transform.position;
            radarPos.y += 1/32f;
            radarDish.transform.position = radarPos;
            yield return new WaitForSeconds(0.2f);
        }
    }
}
