// Class used to control the wires prefab

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireController : MonoBehaviour
{
    [SerializeField] private Renderer wireNW, wireN, wireNE, wireW, wireE, wireSW, wireS, wireSE;

    // Updates the state of the wires based off the neighbors it is told it has
    public void UpdateWires(bool hasNW, bool hasN, bool hasNE, bool hasW, bool hasE, bool hasSW, bool hasS, bool hasSE){
        wireNW.enabled = hasNW;
        wireN.enabled = hasN;
        wireNE.enabled = hasNE;
        wireW.enabled = hasW;
        wireE.enabled = hasE;
        wireSW.enabled = hasSW;
        wireS.enabled = hasS;
        wireSE.enabled = hasSE;
    }

    // Set the wire to the desired state
    public void SetNW(bool hasWire) {
        wireNW.enabled = hasWire;
    }

    // Set the wire to the desired state
    public void SetN(bool hasWire) {
        wireN.enabled = hasWire;
    }

    // Set the wire to the desired state
    public void SetNE(bool hasWire) {
        wireNE.enabled = hasWire;
    }

    // Set the wire to the desired state
    public void SetW(bool hasWire) {
        wireW.enabled = hasWire;
    }

    // Set the wire to the desired state
    public void SetE(bool hasWire) {
        wireE.enabled = hasWire;
    }

    // Set the wire to the desired state
    public void SetSW(bool hasWire) {
        wireSW.enabled = hasWire;
    }

    // Set the wire to the desired state
    public void SetS(bool hasWire) {
        wireS.enabled = hasWire;
    }

    // Set the wire to the desired state
    public void SetSE(bool hasWire) {
        wireSE.enabled = hasWire;
    }
}
