using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateEffect : MonoBehaviour
{
    public GameObject effectObject;

    public Transform effectPos;

    public Transform effectParent;
    

    public void InstantiateThisEffect()
    {
        Instantiate(effectObject, effectPos.position, Quaternion.identity, effectParent);
    }
}
