using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinToSail : MonoBehaviour
{

    Cloth sail;
    public GameObject pin;

    private void Start()
    {

        sail = GetComponent<Cloth>();
    }

    void Update()
    {
        Vector3 pinPos = transform.TransformPoint(sail.vertices[58]);
        //print(pinPos);
        pin.transform.position = pinPos;
    }
}
