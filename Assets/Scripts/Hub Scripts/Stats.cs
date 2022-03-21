// Code that stores the important data between scenes

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour {
    
    // Tier of cloud bot
    // 0 is dot 1 is cross 2 is 3x3
    public static int cloudBotTier = 1;
    public static int reBotTier = 0;

    public static int maxCloudBots = 20;
    public static int maxREBots = 10;
    public static int numWood = 0;
    public static int numStone = 0;



    void Awake() {
        
    }
}
