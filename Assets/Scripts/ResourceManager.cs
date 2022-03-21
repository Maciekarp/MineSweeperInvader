using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    private int numWood = 0;
    private int numStone = 0;

    public int GetWood() {
        return numWood;
    }
    
    public int GetStone() {
        return numStone;
    }

    public int GetResource(string resource) {
        if(resource == "wood") {
            return GetWood();
        }
        if(resource == "stone") {
            return GetStone();
        }

        // if the resource string is incorrect display that
        print("!!! \"" + resource + "\" does not exist !!!");
        return -1;
    }

    public void AddWood(int amount) {
        numWood += amount;
    }
    
    public void AddStone(int amount) {
        numStone += amount;
    }

    public bool RemoveWood(int amount) {
        numWood -= amount;
        if(numWood < 0) {
            print("!!! Wood amount went below zero !!!");
            return false;
        }
        return true;
    }
    
    public bool RemoveStone(int amount) {
        numStone -= amount;
        if(numStone < 0) {
            print("!!! Stone amount went below zero !!!");
            return false;
        }
        return true;
    }

    public bool RemoveResource(string resource, int amount) {
        if(resource == "wood") {
            return RemoveWood(amount);
        }
        if(resource == "stone") {
            return RemoveStone(amount);
        }

        // if the resource string is incorrect display that
        print("!!! \"" + resource + "\" does not exist !!!");
        return false;
    }

    public void AddResource(string resource, int amount) {
        if(resource == "wood") {
            AddWood(amount);
            return;
        }
        if(resource == "stone") {
            AddStone(amount);
            return;
        }

        // if the resource string is incorrect display that
        print("!!! \"" + resource + "\" does not exist !!!");
        return;
    }

}
