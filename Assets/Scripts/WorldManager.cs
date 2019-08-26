using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    private float currentShipSpeed;
    private float currentBackgroundSpeed;

    void ChangeFogDensity(float density)
    {
        RenderSettings.fogDensity = density;
    }

    private void Update()
    {
        currentBackgroundSpeed = VisualSpeedController.BGVisualSpeed;
        currentShipSpeed = ShipSpeedController.Instance.CurrentSpeed;
    }
}
