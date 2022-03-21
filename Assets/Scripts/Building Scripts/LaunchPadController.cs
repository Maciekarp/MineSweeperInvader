using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchPadController : MonoBehaviour
{
    
    [SerializeField] private GameObject tower;
    [SerializeField] private GameObject rocket;
    [SerializeField] private GameObject deadSprites;
    [SerializeField] private GameObject rocketDead;
    [SerializeField] private GameObject wires;
    [SerializeField] private Animator towerAnimator;
    [SerializeField] private GameObject fireSprites;
    [SerializeField] private GameObject rocketFire;

    public void KillLaunchPad() {
        tower.SetActive(false);
        rocket.SetActive(false);
        deadSprites.SetActive(true);
        rocketDead.SetActive(true);
        fireSprites.SetActive(true);
        rocketFire.SetActive(true);
        //wires.SetActive(true);
    }

    // changes the sprites to the no power ones
    public void LosePower() {
        towerAnimator.enabled = false;
    }

    // changes the sprites to the alive ones and turns off no power ones
    public void GainPower() {
        towerAnimator.enabled = true;
    }

    // changes sprites to the ones when the scanner is dead and not on fire
    public void ClearFire() {
        fireSprites.SetActive(false);
        rocketFire.SetActive(false);
    }

    // changes sprites to the ones where the scanner is alive
    public void RepairLaunchPad() {
        tower.SetActive(true);
        rocket.SetActive(true);
        deadSprites.SetActive(false);
        rocketDead.SetActive(false);
        fireSprites.SetActive(false);
    }
}
