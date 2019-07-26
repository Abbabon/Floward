using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship_ctrl : MonoBehaviour
{
    private Animator shipAnim;

    [Range(0, 1)]
    public float gearS;

    [Range(0, 1)]
    public float compressS;

    [Range(0, 7)]
    public float boostLightsOn;

    [Range(0, 6)]
    public float fuelUsage;

    [Range(-3, 3)]
    public float wind;

    [Range(0, 1)]
    public float alertLight;

    [Range(0, 100)]
    public float speed;

    public bool tap = false;

    public bool fuelDrop = false;

    public bool boost = false;

    public bool shutDown = false;

    void Awake()
    {
        shipAnim = GetComponent<Animator>();
    }

    
    void Update()
    {
        shipAnim.SetFloat("Gear", gearS);
        shipAnim.SetFloat("Compress", compressS);
        shipAnim.SetFloat("LightsOn", boostLightsOn);
        shipAnim.SetBool("Boost", boost);
        shipAnim.SetFloat("Wind", wind);
        shipAnim.SetFloat("FuelUsage", fuelUsage);
        shipAnim.SetFloat("AlertLight", alertLight);
        shipAnim.SetFloat("Speed", speed);
        shipAnim.SetBool("ShutDown", shutDown);

        if (fuelDrop)
        {
            shipAnim.SetTrigger("FuelDrop");
            fuelDrop = false;
        }

        if (tap)
        {
            shipAnim.SetBool("Tap", tap);
            tap = false;
        }

       
    }

    // Animation events

    public void Endboost() 
    {
        boost = false;
        shipAnim.ResetTrigger("BoostStartTime");
    }

    public void BoostStartTime()
    {
        shipAnim.SetTrigger("BoostStartTime");
    }


}
