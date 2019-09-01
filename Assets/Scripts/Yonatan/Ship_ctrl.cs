using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship_ctrl : MonoBehaviour
{
    private Animator shipAnim;

    [Range(0, 3)]
    public float gearS;

    [Range(0, 1)]
    public float compressS;

    [Range(0, 7)]
    public float boostLightsOn;

    [Range(0, 6)]
    public float fuelUsage;

    [Range(-2, 2)]
    public float wind;

    [Range(0, 1)]
    public float alertLight;

    [Range(0, 100)]
    public float speed;

    [Range(0, 100)]
    public float overHeat;

    public bool tap = false;

    public bool fuelDrop = false;

    public bool lowFuel = false;

    public bool boost = false;

    public bool shutDown = false;

    public bool overDrive = false;

    public bool hlFlag = false;

    public bool hlEngine = false;

    public bool hlFire = false;

    public bool hlBoost = false;

    public bool hlTank = false;

    public bool tutorial;

    public bool end = false;

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
        shipAnim.SetBool("LowFuel", lowFuel);

        //Tutorial highlights

        shipAnim.SetBool("HLflag", hlFlag);
        shipAnim.SetBool("HLtank", hlTank);
        shipAnim.SetBool("HLfire", hlFire);
        shipAnim.SetBool("HLengine", hlEngine);
        shipAnim.SetBool("HLboost", hlBoost);

        if (fuelDrop)
        {
            shipAnim.SetTrigger("FuelDrop");
            fuelDrop = false;
        }

        if (tap)
        {
            shipAnim.SetTrigger("Tap");
            tap = false;
        }

        if (end)
        {
            shipAnim.SetTrigger("End");
            end = false;
        }

       
    }

    // Animation events
    public void BoostStartTime()
    {
        shipAnim.SetTrigger("BoostStartTime");
        ShipSpeedController.Instance.IsBoostingAnimation = true;
    }

    public void Endboost() 
    {
        boost = false;
        shipAnim.ResetTrigger("BoostStartTime");
        ShipSpeedController.Instance.IsBoostingAnimation = false;
    }


    private void SetWeight(string name, float value)
    {
        shipAnim.SetLayerWeight(shipAnim.GetLayerIndex(name), value);
    }

    public void EndGame()
    {
		GameManager.Instance.StartGameOverSequence();
	}
}
