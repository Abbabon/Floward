using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    public string TagToDestroy;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<MovingObject>() != null){
            Destroy(other.gameObject);
        }
    }
}
