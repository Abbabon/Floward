using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnedCloud : MovingObject
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (GameManager.Instance.IsRunning)
        {
            if (_animator){
                _animator.SetFloat("Wind", WindController.Instance.State);
            }
        }
    }

}
