using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashboardBoostHandle : MonoBehaviour
{
    private Animator _animator;

    //TODO: Move to global object
    private bool isOperating = false;
    private float operatingTimerTimeout = 2f;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void CanOperateAgain()
    {
        isOperating = false;
    }

    public void Operate()
    {
        if (!isOperating)
        {
            _animator.SetTrigger("Operate");
            isOperating = true;
            Invoke("CanOperateAgain", operatingTimerTimeout);
            BoostController.Instance.BoostHandlePulled();
        }
    }
}
