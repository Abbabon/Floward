using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnedCloud : MovingObject
{
    protected override void Start()
    {
        base.Start();
        StartCoroutine(CloudAnimations());
    }
    
    private IEnumerator CloudAnimations()
    {
        while(true) 
        { 
            if (GameManager.Instance.IsRunning)
            {
                if (_animator != null){
                    _animator.SetFloat("Wind", WindController.Instance.State);
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }


    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

}
