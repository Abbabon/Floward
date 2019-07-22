using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnedCloud : MonoBehaviour
{
    private Transform myTransform;
    public Vector2 direction = (Vector2.right * -1);

    [SerializeField] private Animator _animator;

    private float myCoreSpeed;

    // Start is called before the first frame update
    void Start()
    {
        myTransform = GetComponent<Transform>();
        myCoreSpeed = RandomizeSpeed();
    }

    // Update is called once per frame
    void Update()
    {
		if (GameManager.Instance.IsRunning)
		{
		    myTransform.Translate(direction * Time.deltaTime * myCoreSpeed * ShipSpeedController.Instance.GetSpeedFactor());
            if (_animator){
                _animator.SetFloat("Wind", WindController.Instance.State);
            }
		}
    }

    float RandomizeSpeed()
    {
        return Random.Range(GlobalGameplayVariables.Instance.MinimumCloudSpeed, GlobalGameplayVariables.Instance.MaxCloudSpeed);
    }
}
