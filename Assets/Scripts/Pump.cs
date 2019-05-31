using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pump : MonoBehaviour
{
    public void StartEngine()
    {
        if (ShipStateManager.Instance.Speed == ShipSpeed.Stop){
            //TODO: PUMP IT (LOUDER!
            ShipStateManager.Instance.SetShipSpeed(ShipSpeed.Normal);
        }
    }
}
