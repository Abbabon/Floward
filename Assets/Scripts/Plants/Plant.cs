using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    //this will be serialized to player prefs if the player collected this plant, so the ending scene could know it.
    public int PlantId;
    public bool Collected;
}
