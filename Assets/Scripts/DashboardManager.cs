using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DashboardManager : MonoBehaviour
{
    private static DashboardManager _instance;
    public static DashboardManager Instance { get { return _instance; } }

    private static readonly object padlock = new object();

    [SerializeField] private Animator dashboardAnimator;
    [SerializeField] private Animator pullieAnimator;
    [SerializeField] private Animator pullieAnimatorGlow;

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

    public void TurnOnDashboard()
    {
        dashboardAnimator.SetBool("DashboardOn", true);
    }

    public void TurnOffDashboard()
    {
        dashboardAnimator.SetBool("DashboardOn", false);
    }

    public void TurnOnBoostPullie()
    {
        pullieAnimator.SetBool("BoostAvailable", true);
        pullieAnimatorGlow.SetBool("BoostAvailable", true);
    }

    public void TurnOffBoostPullie()
    {
        pullieAnimator.SetBool("BoostAvailable", false);
        pullieAnimatorGlow.SetBool("BoostAvailable", false);
    }
}
