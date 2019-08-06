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

    [FMODUnity.EventRef]
    public string _HummingSound;
    FMOD.Studio.EventInstance _hummingEvent;

    [SerializeField] private Ship_ctrl _ship_ctrl;
    private int _numberOfLights = 7;
    private int _numberOfLightsOn = 0;

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
                _hummingEvent = FMODUnity.RuntimeManager.CreateInstance(_HummingSound);
            }
        }
        //DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        _boostPerLight = 100f / _numberOfLights;
        Restart();
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
        if (Mathf.FloorToInt(_ship_ctrl.boostLightsOn) > _numberOfLightsOn){
            _numberOfLightsOn++;
            SoundManager.Instance.PlayOneshotound(String.Format("Light Bulb {0}", _numberOfLightsOn));

            if (_numberOfLightsOn == _numberOfLights)
            {
                _numberOfLightsOn++;
                _hummingEvent.start();
            }
        }
    }

    internal void Restart()
    {
        _numberOfLightsOn = 0;
        _hummingEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}
