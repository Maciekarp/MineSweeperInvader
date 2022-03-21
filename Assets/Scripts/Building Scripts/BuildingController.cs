// Parent class for the buildings

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BuildingController : MonoBehaviour {
    [HideInInspector] public BuildingController buildingNW = null, buildingN = null, buildingNE = null, buildingW = null, buildingE = null, buildingSW = null, buildingS = null, buildingSE = null;

    [SerializeField] private WireController wireCont;
    //[SerializeField] private ;
    [SerializeField] private UnityEvent killBuildingEvent;
    [SerializeField] private UnityEvent clearFireEvent;
    [SerializeField] private UnityEvent repairEvent;
    [SerializeField] private UnityEvent losePowerEvent;
    [SerializeField] private UnityEvent gainPowerEvent;
    
    [SerializeField] private GameObject noPower;
    [HideInInspector] public bool isAlive = true;
    
    // string that represents what type of building it is
    // base scanner launchpad extender watercollector
    [HideInInspector] public string buildingType;

    // string that represents what state the building is in
    // alive onfire dead nopower
    [HideInInspector] public string buildingState = "alive";

    [HideInInspector] public bool hasPower = true;
    [HideInInspector] public int boardX;
    [HideInInspector] public int boardY;
    [HideInInspector] public bool isDead = false;
    [HideInInspector] public bool willHavePower = false;

    // used in the recursive function that checks which buildings have power 
    [HideInInspector] public bool isChecked = false;
    

    // kills the building
    public void KillBuilding() {
        buildingState = "onfire";
        if(killBuildingEvent.GetPersistentEventCount() == 0) {
            Debug.Log("!!!!! No way to kill " + buildingType + " Exists !!!!!");
        } else {
            killBuildingEvent.Invoke();
        }
        isAlive = false;
    }

    // clears fire from building
    public void ClearFire() {
        buildingState = "dead";
        if(clearFireEvent.GetPersistentEventCount() == 0) {
            Debug.Log("!!!!! No way to clear fire on " + buildingType + " Exists !!!!!");
        } else {
            clearFireEvent.Invoke();
        }
    }

    // repairs building
    public void RepairBuilding() {
        buildingState = "alive";
        if(repairEvent.GetPersistentEventCount() == 0) {
            Debug.Log("!!!!! No way to repair " + buildingType + " Exists !!!!!");
        } else {
            repairEvent.Invoke();
        }
        isAlive = true;
    }

    // removes building and removes the cables of neighbors
    public void RemoveBuilding() {
        if(buildingNW != null) {
            buildingNW.SetSE(null);
        }
        if(buildingN != null) {
            buildingN.SetS(null);
        }
        if(buildingNE != null) {
            buildingNE.SetSW(null);
        }
        if(buildingW != null) {
            buildingW.SetE(null);
        }
        if(buildingE != null) {
            buildingE.SetW(null);
        }
        if(buildingSW != null) {
            buildingSW.SetNE(null);
        }
        if(buildingS != null) {
            buildingS.SetN(null);
        }
        if(buildingSE != null) {
            buildingSE.SetNW(null);
        }
    }

    // removes power from building relies on script that calls the function to determine what neighbors are removed
    public void LosePower() {
        buildingState = "nopower";
        hasPower = false;
        if(buildingType != "base") {
            if(losePowerEvent.GetPersistentEventCount() == 0) {
                Debug.Log("!!!!! No way to Lose Power on " + buildingType + " Exists !!!!!");
            } else {
                losePowerEvent.Invoke();
            }
        }
        StartCoroutine(NoPowerAnim());
    }

    // if in unpowered state gives power to the building and starts recursive function to give power to connected neighbors
    public void GainPower() {
        // Base case if the building is not in the "nopower" state do not continue to recursive function
        if(buildingState != "nopower") {
            return;
        }
        if(buildingType != "base") {
            if(gainPowerEvent.GetPersistentEventCount() == 0) {
                Debug.Log("!!!!! No way to Gain Power on " + buildingType + " Exists !!!!!");
            } else {
                gainPowerEvent.Invoke();
            }
        }
        // Recursive case is called if neighbors are in the "nopower" state
        buildingState = "alive";
        hasPower = true;
        if(buildingNW != null) {
            buildingNW.GainPower();
        }
        if(buildingN != null) {
            buildingN.GainPower();
        }
        if(buildingNE != null) {
            buildingNE.GainPower();
        }
        if(buildingW != null) {
            buildingW.GainPower();
        }
        if(buildingE != null) {
            buildingE.GainPower();
        }
        if(buildingSW != null) {
            buildingSW.GainPower();
        }
        if(buildingS != null) {
            buildingS.GainPower();
        }
        if(buildingSE != null) {
            buildingSE.GainPower();
        }
    }

    // coroutine that animates the no power symbol while in the nopower state 
    IEnumerator NoPowerAnim() {
        yield return new WaitForSeconds(0.5f);
        while(buildingState == "nopower") {
            noPower.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            noPower.SetActive(false);
            yield return new WaitForSeconds(0.5f);
        }
    }

    // Set the wire to the desired state
    public void SetNW(BuildingController neighborBC) {
        buildingNW = neighborBC;
        wireCont.SetNW(neighborBC != null);
    }

    // Set the wire to the desired state
    public void SetN(BuildingController neighborBC) {
        buildingN = neighborBC;
        wireCont.SetN(neighborBC != null);
    }

    // Set the wire to the desired state
    public void SetNE(BuildingController neighborBC) {
        buildingNE = neighborBC;
        wireCont.SetNE(neighborBC != null);
    }

    // Set the wire to the desired state
    public void SetW(BuildingController neighborBC) {
        buildingW = neighborBC;
        wireCont.SetW(neighborBC != null);
    }

    // Set the wire to the desired state
    public void SetE(BuildingController neighborBC) {
        buildingE = neighborBC;
        wireCont.SetE(neighborBC != null);
    }

    // Set the wire to the desired state
    public void SetSW(BuildingController neighborBC) {
        buildingSW = neighborBC;
        wireCont.SetSW(neighborBC != null);
    }

    // Set the wire to the desired state
    public void SetS(BuildingController neighborBC) {
        buildingS = neighborBC;
        wireCont.SetS(neighborBC != null);
    }

    // Set the wire to the desired state
    public void SetSE(BuildingController neighborBC) {
        buildingSE = neighborBC;
        wireCont.SetSE(neighborBC != null);
    }
}
