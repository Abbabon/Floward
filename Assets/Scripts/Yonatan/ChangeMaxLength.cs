using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMaxLength : MonoBehaviour
{

    private Cloth sailCloth;

    private ClothSkinningCoefficient[] coefficients;

    [Range(0,2)]
    public float distance;

    void Start()
    {

        sailCloth = GetComponent<Cloth>();
        coefficients = sailCloth.coefficients;
        //var coefficients = new ClothSkinningCoefficient[sailCloth.coefficients.Length];
    }

    
    void Update()
    {
        //var coefficients = new ClothSkinningCoefficient[sailCloth.coefficients.Length];
        coefficients[58].maxDistance = distance;
        coefficients[66].maxDistance = distance;

        sailCloth.coefficients = coefficients;

        //print(sailCloth.coefficients[66].maxDistance);
    }
}
