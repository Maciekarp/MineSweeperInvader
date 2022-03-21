// This class manages the sprites of the scanner object and is used to show
// which state the building is in
// requires BuildingController class to call functions and manage the state changes

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScannerController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer scanner;
    [SerializeField] private GameObject deadSprites;
    [SerializeField] private GameObject deadDishRight;
    [SerializeField] private GameObject deadDishLeft;
    [SerializeField] private GameObject fire;
    [SerializeField] private GameObject wires;

    [SerializeField] private GameObject[] noPowerSprites;

    // changes the sprites to the on fire and dead ones
    public void KillScanner() {
        scanner.enabled = false;
        deadSprites.SetActive(true);
        fire.SetActive(true);
        for(int i = 0; i < noPowerSprites.Length; i++) {
            noPowerSprites[i].SetActive(false);
        }
        if(Random.value < 0.5f) {
            deadDishLeft.SetActive(true);
            deadDishRight.SetActive(false);
        } else {
            deadDishRight.SetActive(true);
            deadDishLeft.SetActive(false);
        }
        //wires.SetActive(false);
    }

    // changes the sprites to the no power ones
    public void LosePower() {
        scanner.enabled = false;
        noPowerSprites[(int) (Random.value * 4)].SetActive(true);
    }

    // changes the sprites to the alive ones and turns off no power ones
    public void GainPower() {
        for(int i = 0; i < noPowerSprites.Length; i++) {
            noPowerSprites[i].SetActive(false);
        }
        scanner.enabled = true;
    }

    // changes sprites to the ones when the scanner is dead and not on fire
    public void ClearFire() {
        fire.SetActive(false);
    }

    // changes sprites to the ones where the scanner is alive
    public void RepairScanner() {
        deadSprites.SetActive(false);
        scanner.enabled = true;
    }
}
