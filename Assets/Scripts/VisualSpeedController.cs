using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualSpeedController : MonoBehaviour
{
    //this is the actual speed all moving objects needs to query
	public static float BGVisualSpeed;

    // these are changed from the animator
	public float BGSpeedMultiplier = 1f;
	public float BGSpeedAdditive = 0f;
	public float ShipAccelerationAdditive = 0f;

    void Update(){
        //BGVisualSpeed = GameManager.Instance.IsRunning ? ((ShipSpeedController.Instance.CurrentSpeed * BGSpeedMultiplier) + BGSpeedAdditive) : 0;
        BGVisualSpeed = GameManager.Instance.IsRunning ? ((ShipSpeedController.Instance.CurrentSpeed * BGSpeedMultiplier) ) : 0; // without addative untill bug fixed
        ShipSpeedController.Instance.ShipAcceleration = GlobalGameplayVariables.Instance.AccelerationRate + ShipAccelerationAdditive;
    }
}
