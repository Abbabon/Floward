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

    [Range(0, 100)]
    public float overHeat;

    public bool tap = false;

    public bool fuelDrop = false;

    public bool boost = false;

    public bool shutDown = false;

    public bool overDrive = false;

    public bool tutorial;

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
        shipAnim.SetBool("OverDrive", overDrive);
        shipAnim.SetFloat("OverHeat", overHeat);
        shipAnim.SetBool("Tutorial", tutorial);

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

    private void SetWeight(string name, float value)
    {
        shipAnim.SetLayerWeight(shipAnim.GetLayerIndex(name), value);
    }
}
