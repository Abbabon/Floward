using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShipSpeed
{
    Stop=0,
    Normal=1,
    Fast=2,
    SuperBoost=3
}

public class ShipStateManager : MonoBehaviour
{
    private static ShipStateManager _instance;
    public static ShipStateManager Instance { get { return _instance; } }
    private static readonly object padlock = new object();

    private ShipSpeed speed;
    public ShipSpeed Speed { get => speed; set => speed = value; }

    private float greenLightTimerDuration = 3f;
    private float shipStopBuffer = 2f;

    //TODO: consider moving to a seperate component for clarity
    public GameObject flag;
    private Animator flagAnimator;

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
                //Do Nothing!
            }
        }
        return startStoppingShip;
    }

    //TODO: how to expose booster lights? There's clearly one for the update loop, and one exposed for the boost controller
    private float sailLightTimer = 0f;
    private void ManageLights()
    {
        if (turnOnSailsGreenLight)
        {
            sailLightTimer += Time.deltaTime;

            if (sailLightTimer >= greenLightTimerDuration)
            {
                //TODO: turn on green light, sound
            }
        }
        else 
        {
            //TODO: turn off green light, sound
            sailLightTimer = 0f;
        }
    }

    private float shipStopTimer = 0f;
    private void ManageSpeed(bool startStoppingShip)
    {
        if (startStoppingShip)
        {
            shipStopTimer += Time.deltaTime;

            if (shipStopTimer >= shipStopBuffer)
            {
                speed = ShipSpeed.Stop;
                //TODO: stop the ship animation, sound
            }
        }
        else
        {
            shipStopTimer = 0f;
        }
    }
}
