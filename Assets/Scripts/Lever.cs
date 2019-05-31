using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour
{
    public Wheel wheel;

    public void ReleaseSail()
    {
        if (SailsController.Instance.State == SailsState.SailsUp){
            SailsController.Instance.SetState(SailsState.SailsDown);
        }
    }
}
