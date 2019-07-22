using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostLightsController : MonoBehaviour
{
    private static BoostLightsController _instance;
    public static BoostLightsController Instance { get { return _instance; } }
    private static readonly object padlock = new object();

    private float _boostPerLight;

    [SerializeField] private Ship_ctrl _ship_ctrl;
    private float _numberOfLights = 7f;

    private void Awake()
    {
        lock (padlock)
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
        }
        //DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        _boostPerLight = 100f / _numberOfLights;
        StartCoroutine(nameof(ManageBoostLightsCoruotine)); //no need to run more than every 0.25 second or so.
    }

    private IEnumerator ManageBoostLightsCoruotine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.25f);
            ManageBoostLights();
        }
    }

    private void ManageBoostLights()
    {
        _ship_ctrl.boostLightsOn = (BoostController.Instance.boostPercentage / _boostPerLight);
    }
}
