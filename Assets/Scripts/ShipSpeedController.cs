using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ShipSpeedController : MonoBehaviour
{
    private static ShipSpeedController _instance;
    public static ShipSpeedController Instance { get { return _instance; } }
    private static readonly object padlock = new object();

    public bool IsBoosting = false;
    public float Speed = 0f; // in KMph, between 0 and 100

    public CinemachineVirtualCamera virtualCamera;

    public AudioSource audioSource;

    public Ship_ctrl ship_Ctrl;

    public float miles;
    public TextMeshProUGUI milesDisplay;

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
                //Here any additional initialization should occur:
                InitializeCachedVariables();
            }
        }
        //DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        SoundManager.Instance.RegisterSoundEffectsAudioSource(audioSource);
    }

    private void InitializeCachedVariables()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
		if (GameManager.Instance.IsRunning)
		{
		    ManageSpeed();
            ManageClouds();
            ManageCamera();
            ManageAnimations();
            ManageMileage();
		}
    }

    /* 
     * manage ship's speed (according to engine and sail): Formula:
     * (EngineGear * EngineWeight) +- (if sails up ? ) (direction) * (power)  : 0 
    */ 
    private void ManageSpeed()
    {
        float newSpeedValue = (int)EngineController.Instance.CurrentGear * GlobalGameplayVariables.Instance.SpeedEngineWeight + 
            (int)(SailsController.Instance.State) * (int)(WindController.Instance.Direction()) * (int)(WindController.Instance.Strength()) * GlobalGameplayVariables.Instance.SpeedWindWeight;

        Speed = Mathf.Clamp(newSpeedValue, 0f, GlobalGameplayVariables.Instance.MaxSpeedWithoutBoost);

        if (IsBoosting){
            Speed = GlobalGameplayVariables.Instance.MaxSpeed;
        }
    }

    //set background scrolling speed (if needed - the clouds query it on their own)
    private void ManageClouds()
    {

    }

    // change camera position if needed (if boosting)
    private void ManageCamera()
    {
        if (Speed < 2)
        {
            DOTween.To(() => virtualCamera.m_Lens.OrthographicSize, x => virtualCamera.m_Lens.OrthographicSize = x, 6.6f, 1f);
        }
        else if (Speed < 30)
        {
            DOTween.To(() => virtualCamera.m_Lens.OrthographicSize, x => virtualCamera.m_Lens.OrthographicSize = x, 7f, 1f);
        }
        else if (Speed < 60)
        {
            DOTween.To(() => virtualCamera.m_Lens.OrthographicSize, x => virtualCamera.m_Lens.OrthographicSize = x, 7.3f, 1f);

        }
        else if (Speed < 80)
        {
            DOTween.To(() => virtualCamera.m_Lens.OrthographicSize, x => virtualCamera.m_Lens.OrthographicSize = x, 7.7f, 1f);

        }
        else if (Speed > 100)//boosting
        {
            DOTween.To(() => virtualCamera.m_Lens.OrthographicSize, x => virtualCamera.m_Lens.OrthographicSize = x, 8.5f, 1f);
        }
    }

    //TODO: change to Km, according to the specs>?
    private void ManageMileage()
    {
        miles += (Speed/50) * Time.deltaTime;
        milesDisplay.text = ((int)miles).ToString().PadLeft(7, '0');
    }

    internal void StartBoosting()
    {
        IsBoosting = true;
        //?
    }

    internal void StopBoosting()
    {
        IsBoosting = false;
        //??
    }

    //TODO: consider moving ot its own script
    private void ManageAnimations()
    {
		float gear = 0f;
        if (EngineController.Instance.HeatLevel > 0f)
		{
            if (EngineController.Instance.HeatLevel > EngineController.Instance.OverheatLevel)//in overheat
            {
                gear = Mathf.Clamp(EngineController.Instance.HeatLevel / 100f, 0.26f, 1f);
            }
            else
            {
                gear = Mathf.Clamp(EngineController.Instance.HeatLevel / 100f, 0.26f, 0.79f);
            }
		}

        ship_Ctrl.gearS = gear;
        ship_Ctrl.boost = IsBoosting;
    }

    public float GetSpeedFactor()
    {
        return Mathf.Max((Speed / 10f), 1f);
    }
}
