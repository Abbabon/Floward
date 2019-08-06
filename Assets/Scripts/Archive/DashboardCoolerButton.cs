using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DashboardCoolerButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    public void OnPointerDown(PointerEventData eventData)
    {
        EngineController.Instance.EngineCooling = true;

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        EngineController.Instance.EngineCooling = false;
    }
}
