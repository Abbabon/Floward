using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindVertex : MonoBehaviour
{
    private Cloth clothReference;
    public float searchLength = 0.15f;

    void Start()
    {
        
        clothReference = GetComponent<Cloth>();
    }
        
        void Update()
    {
        for (int i = 0; i < clothReference.coefficients.Length; i++)
        {
            if (clothReference.coefficients[i].maxDistance == searchLength)
            {
                print(i);
            } 
        }
    }
}
