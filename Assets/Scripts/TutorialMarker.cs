using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMarker : MonoBehaviour
{
    private Animator _animator;

    public enum MarkerAnimation
    {
        Tap,
        SwipeDown,
        SwipeLeft,
        SwipeRight,
    }

    public void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void Animate(MarkerAnimation animation)
    {
        _animator.SetBool("Tap", animation == MarkerAnimation.Tap);
        _animator.SetBool("SwipeDown", animation == MarkerAnimation.SwipeDown);
        _animator.SetBool("SwipeLeft", animation == MarkerAnimation.SwipeLeft);
        _animator.SetBool("SwipeRight", animation == MarkerAnimation.SwipeRight);

    }
}
