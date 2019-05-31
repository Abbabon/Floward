using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public enum ShipSpeed
{
    Stop=0,
    Normal=50,
    Fast=100,
    SuperBoost=200
}

public class ShipStateManager : MonoBehaviour
{
    private static ShipStateManager _instance;
    public static ShipStateManager Instance { get { return _instance; } }
    private static readonly object padlock = new object();

    private ShipSpeed speed;
    public ShipSpeed Speed { get => speed; set => speed = value; }

    private float greenLightTimerDuration = 3f;
    private float shipStopBuffer = 5f;

    //TODO: consider moving to a seperate component for clarity
    public GameObject flag;
    private Animator flagAnimator;

    private Animator shipAnimator;
    public ParticleSystem sailsGreenLight;

    public GameObject mainCamera;

    private void Awake()
    {
        Debug.Log("AWAKE");
        lock (padlock)
        {
            if (_instance != null && _instance != this)
            {
                Debug.Log("DESTROY");
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
                //Here any additional initialization should occur:
                InitializeCachedVariables();
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }

    private void InitializeCachedVariables()
    {
        flagAnimator = flag.GetComponent<Animator>();
        shipAnimator = GetComponent<Animator>();
        speed = ShipSpeed.Stop;
    }

    // Start is called before the first frame update
    void Start()
    {

        //TODO: remove and switch / control from WorldStateManager
    }

    // Update is called once per frame
    void Update()
    {
        ManageFlag();

        bool startStoppingShip = ManageSailsInteractions(); //make an 'or' between this and other interactions

        ManageSpeed(startStoppingShip);
        ManageLights();
    }

    private void ManageFlag()
    {
        flagAnimator.SetBool("FlagFaceRight", WindManager.Instance.State == WindState.BackWind);
        flagAnimator.SetBool("FlagFaceLeft", WindManager.Instance.State == WindState.FrontWind);
    }

    private bool turnOnSailsGreenLight = false;
    private bool ManageSailsInteractions()
    {
        bool startStoppingShip = false;
        if (WindManager.Instance.State == WindState.FrontWind)
        {
            if (SailsController.Instance.State == SailsState.SailsUp)
            {
                turnOnSailsGreenLight = false;
                startStoppingShip = true;
            }
            else if (SailsController.Instance.State == SailsState.SailsDown)
            {
                turnOnSailsGreenLight = true;
            }
        }
        else if (WindManager.Instance.State == WindState.BackWind)
        {
            if (SailsController.Instance.State == SailsState.SailsUp)
            {
                turnOnSailsGreenLight = true;
            }
            else if (SailsController.Instance.State == SailsState.SailsDown)
            {
                turnOnSailsGreenLight = false;
            }
        }
        return startStoppingShip;
    }

    //TODO: how to expose booster lights? There's clearly one for the update loop, and one exposed for the boost controller
    private float sailLightTimer = 0f;
    private void ManageLights()
    {

        //Sails Light Logic: 
        if (turnOnSailsGreenLight && speed == ShipSpeed.Normal && SailsController.Instance.State == SailsState.SailsUp)
        {
            sailLightTimer += Time.deltaTime;

            if (sailLightTimer >= greenLightTimerDuration && sailsGreenLight.isStopped)
            {
                Debug.Log("Turn on the green light!");
                sailsGreenLight.Play();
                //TODO: sound
            }
        
        } else {
            if (sailsGreenLight.isPlaying)
                sailsGreenLight.Stop();

            sailLightTimer = 0f;
            //TODO: sound
        }
    }

    private float shipStopTimer = 0f;
    private void ManageSpeed(bool startStoppingShip)
    {
        //can't stop when already stopped
        if (Speed != ShipSpeed.Stop){
            if (startStoppingShip)
            {
                shipStopTimer += Time.deltaTime;

                if (shipStopTimer >= shipStopBuffer)
                {
                    shipStopTimer = 0;
                    SetShipSpeed(ShipSpeed.Stop);
                }
            }
        }
        else
        {
            shipStopTimer = 0;
        }
    }

    public void SetShipSpeed(ShipSpeed shipSpeed)
    {
        speed = shipSpeed;
        Speedometer.Instance.SetTargetSpeed(shipSpeed);

        float rotationSpeed;
        switch (shipSpeed)
        {
            case ShipSpeed.Normal:
                rotationSpeed = 5f;
                break;
            default:
                rotationSpeed = 0.1f;
                break;
        }
        RenderSettings.skybox.SetFloat("_RotationSpeed", rotationSpeed * -1);

        shipAnimator.SetFloat("Speed", (int)shipSpeed);

        //move camera
        switch (shipSpeed)
        {
            case ShipSpeed.Stop:
                mainCamera.transform.DOMove(new Vector3(0, 0, 2), 2f);
                break;
            case ShipSpeed.Normal:
                mainCamera.transform.DOMove(new Vector3(0, 0, 0), 2f);
                break;
            default:
                rotationSpeed = 0.1f;
                break;
        }

        //TODO: stop ship animation, sound
    }
}
