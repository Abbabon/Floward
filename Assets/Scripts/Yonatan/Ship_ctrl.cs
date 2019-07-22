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

    public bool boost = false;

    void Awake()
    {
        shipAnim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        shipAnim.SetFloat("Gear", gearS);
        shipAnim.SetFloat("Compress", compressS);
        shipAnim.SetFloat("LightsOn", boostLightsOn);
        shipAnim.SetBool("Boost", boost);
        shipAnim.SetFloat("Wind", wind);
        shipAnim.SetFloat("FuelUsage", fuelUsage);

        //if (boost)
        //{
        //    boostLightsOn = 0;
        //}
    }

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
