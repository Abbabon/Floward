/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Speedometer : MonoBehaviour {

    private const float MAX_SPEED_ANGLE = -89.9f;
    private const float ZERO_SPEED_ANGLE = 89.9f;
    private float totalAngleSize = ZERO_SPEED_ANGLE - MAX_SPEED_ANGLE;

    [SerializeField] private Transform needleTranform;
    [SerializeField] private float speedMax;
    [SerializeField] private float targetSpeed;
    
    private void Awake() {
        targetSpeed = 0f;
        speedMax = GlobalGameplayVariables.Instance.MaxSpeedWithoutBoost;
    }

    private void Update() {
        float newTargetSpeed = ShipSpeedController.Instance.CurrentSpeed;

        if (newTargetSpeed != targetSpeed)
        {
            if (newTargetSpeed <= speedMax)
            {
                targetSpeed = newTargetSpeed;
            }
            else
            {
                targetSpeed = speedMax;
                //TODO: do the animation?
            }
            needleTranform.DOLocalRotate(new Vector3(0, 0, GetSpeedRotation()), 1f);

            //DOTween.To(() => needleTranform.eulerAngles, x => needleTranform.eulerAngles = x, new Vector3(0, 0, GetSpeedRotation()), 1f);
        }
    }

    private float GetSpeedRotation() {
        float speedNormalized = targetSpeed / speedMax;

        return ZERO_SPEED_ANGLE - speedNormalized * totalAngleSize;
    }
}
