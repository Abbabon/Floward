using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashboardManager : MonoBehaviour
{
    private static DashboardManager _instance;
    public static DashboardManager Instance { get { return _instance; } }

    private static readonly object padlock = new object();

    [SerializeField] private Animator dashboardAnimator;
    [SerializeField] private Animator pullieAnimator;
    [SerializeField] private Animator pullieAnimatorGlow;

    [SerializeField] private Image fuelStationIndicator;

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

            }
        }
        //DontDestroyOnLoad(this.gameObject);
    }

    private bool _dashboardOn = false;
    public void TurnOnDashboard()
    {
        if (!_dashboardOn)
        {
            _dashboardOn = true;
            dashboardAnimator.SetBool("DashboardOn", true);
        }
    }

    public void TurnOffDashboard()
    {
        if (_dashboardOn)
        {
            _dashboardOn = false;
            dashboardAnimator.SetBool("DashboardOn", false);
        }
    }

    private bool PullieOn = false;
    public void TurnOnBoostPullie()
    {
        if (!PullieOn)
        {
            pullieAnimator.SetBool("BoostAvailable", true);
            pullieAnimatorGlow.SetBool("BoostAvailable", true);
			PullieOn = true;
        }
    }

    public void TurnOffBoostPullie()
    {
        if (PullieOn)
        {
            pullieAnimator.SetBool("BoostAvailable", false);
            pullieAnimatorGlow.SetBool("BoostAvailable", false);
            PullieOn = false;
        }
    }


    private bool _fuelIndicatorOn = true;
    public void TurnOnFuelStationIndicator()
    {
        if (!_fuelIndicatorOn)
        {
            _fuelIndicatorOn = true;
            fuelStationIndicator.color = new Color(fuelStationIndicator.color.r,
                                                    fuelStationIndicator.color.g,
                                                    fuelStationIndicator.color.b,
                                                    1);
        }
    }

    public void TurnOffFuelStationIndicator()
    {
        if (_fuelIndicatorOn)
        {
            _fuelIndicatorOn = false;
            fuelStationIndicator.color = new Color(fuelStationIndicator.color.r,
                                                    fuelStationIndicator.color.g,
                                                    fuelStationIndicator.color.b,
                                                    0);
        }
    }

    private bool shaking;
    public void TurnOffShaking()
    {
        if (shaking)
        {
            dashboardAnimator.SetBool("SpeedCapping", false);
            shaking = false;
        }
    }

    public void TurnOnShaking()
    {
        if (!shaking)
        {
            dashboardAnimator.SetBool("SpeedCapping", true);
            shaking = true;
        }
    }

    private bool arriveAtStation;
    public void TurnOffArriveAtStation()
    {
        if (arriveAtStation)
        {
            dashboardAnimator.SetBool("ArriveAtStation", false);
            arriveAtStation = false;
        }
    }

    public void TurnOnArriveAtStation()
    {
        if (!arriveAtStation)
        {
            dashboardAnimator.SetBool("ArriveAtStation", true);
            arriveAtStation = true;
        }
    }
}
