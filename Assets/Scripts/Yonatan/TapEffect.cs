using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapEffect : MonoBehaviour
{
    public GameObject tapEffectObject;

    public Transform tapPos;
    


    public bool tap = false;

    void Start()
    {

    }

    
    void Update()
    {
        if (tap)
        {
            Instantiate(tapEffectObject, tapPos.position, Quaternion.identity, transform);
            tap = false;
            //print("done tap effect");
        }
    }

    public void InstantiateEffect()
    {
        Instantiate(tapEffectObject, tapPos.position, Quaternion.identity, transform);
    }
}
