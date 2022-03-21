using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterController : MonoBehaviour
{
    [SerializeField] GameObject northPipes, southPipes, eastPipes, westPipes;

    

    // sets the active pipes 
    public void SetPipes(bool isNorth, bool isSouth, bool isEast, bool isWest) {
        northPipes.SetActive(isNorth);
        southPipes.SetActive(isSouth);
        eastPipes.SetActive(isEast);
        westPipes.SetActive(isWest);
    }
}
