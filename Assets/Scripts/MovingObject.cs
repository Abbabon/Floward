using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    protected Transform _transform;
    [SerializeField] protected float _baseSpeed = 0f;
    protected Vector2 _direction = (Vector2.right * -1);
    [SerializeField] protected Animator _animator;
    [SerializeField] protected bool _isMoving = true;

    protected virtual void Start()
    {
        //the animator could be on a child object ATM so we'll set it in the inspector for now.
        //_animator = GetComponent<Animator>();
        _transform = GetComponent<Transform>();
    }

    protected virtual void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsRunning && _isMoving) {
            _transform.Translate(_direction * Time.deltaTime * (VisualSpeedController.BGVisualSpeed + _baseSpeed));
        }
    }
}
