using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    private float currentShipSpeed;
    private float currentBackgroundSpeed;

	private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void ChangeFogDensity(float density)
    {
        RenderSettings.fogDensity = density;
    }

    private void Update()
    {
        currentBackgroundSpeed = VisualSpeedController.BGVisualSpeed;
        currentShipSpeed = ShipSpeedController.Instance.CurrentSpeed;
    }

    public void MoveIn()
    {
        _animator.SetTrigger("MoveIn");
    }

    public void OpenSky()
    {
        _animator.SetTrigger("OpenSky");
    }

    public void Skip()
    {
        _animator.Play("Open_Sequence", 0, 0.9f);
    }
}
