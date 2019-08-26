using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelingStation : MovingObject
{
    public float EPSILON = 0.1f;
    public bool _fueledOnce;

    protected override void Update()
    {
        base.Update();

        if (!_fueledOnce &&
            !ShipSpeedController.Instance.InStation &&
            System.Math.Abs(_transform.position.x - ShipSpeedController.Instance.ShipTransform.position.x) < EPSILON)
        {
            Debug.Log("Fueling!");
            ShipSpeedController.Instance.EnteringStation();
            FuelController.Instance.AddFuel();
            _isMoving = false;
            _fueledOnce = true; 
        }
    }

    //called from fuelstationscontroller
    public void FuelingDone(){
        //TODO: add plant
        _isMoving = true;
    }
}
