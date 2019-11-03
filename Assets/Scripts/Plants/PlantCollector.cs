using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantCollector : MonoBehaviour
{
    private readonly string PLANT_LAYER_NAME = "Plants";

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer != LayerMask.NameToLayer(PLANT_LAYER_NAME))
        {
            return;
        }
        other.gameObject.SetActive(false);
        var plantIndex = (int) Char.GetNumericValue(other.gameObject.name[7]) - 1;
        PlantsController.Instance.CollectPlantAtIndex(plantIndex);
    }
}
