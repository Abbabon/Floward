using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashboardFuelBar : MonoBehaviour {

    //TODO: animate 

    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    [SerializeField] private float minY;
    [SerializeField] private float maxY;

    [SerializeField] private float XUnit;
    [SerializeField] private float YUnit;

    private float originalZ;

    [SerializeField] private GameObject fuelPump;

    private void Awake()
    {
        XUnit = (maxX - minX );
        YUnit = (maxY - minY);
        originalZ = fuelPump.transform.localPosition.z;
    }

    // percentage runs from 0 to 1
    public void Set(float percentage) {
        fuelPump.transform.localPosition = new Vector3(minX + XUnit * percentage, minY + YUnit * percentage, originalZ);
    }
}
