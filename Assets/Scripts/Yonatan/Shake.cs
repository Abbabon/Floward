using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Shake : MonoBehaviour
{
    
    public Transform trns;

    // How long the object should shake for.
    //public float shakeDuration = 0f;

    // Amplitude of the shake. A larger value shakes the camera harder.
    [Range(0,1)]
    public float shakeAmount;


    //public float decreaseFactor = 1.0f;

    Vector3 originalPos;

    public ShakerCtrl shakerCtrl;

    void Awake()
    {
        if (trns == null)
        {
            trns = GetComponent(typeof(Transform)) as Transform;
        }
    }

    void OnEnable()
    {
        originalPos = trns.localPosition;
    }

    void Update()
    {
        shakeAmount = shakerCtrl.shake;

        if (shakeAmount > 0)
        {
            trns.localPosition = originalPos + Random.insideUnitSphere * 0.1f * shakeAmount;

            //shakeDuration -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            //shakeDuration = 0f;
            trns.localPosition = originalPos;
        }
    }
}
