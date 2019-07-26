using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TouchZone
{
    Engine,
    Sails,
    Boost
}

public class TouchController : MonoBehaviour
{
    private bool startTap, tap, swipeLeft, swipeRight, swipeUp, swipeDown;
    private bool currentlyHeld = false;
    [SerializeField] private bool noSwipeWhileHolding = true;
    private bool isDragging = false;
    private Vector2 startTouch, swipeDelta;

    [SerializeField] private Camera mainCamera;

    [SerializeField] private float swipeMagnitude = 75f;

    [SerializeField] private RectTransform SailsArea;
    [SerializeField] private RectTransform EngineArea;
    [SerializeField] private RectTransform BoostArea;

    void Update()
    {
        if (GameManager.Instance.TouchEnabled)
        {
            tap = swipeLeft = swipeRight = swipeUp = swipeDown = false;

            #region Standalone Inputs

            if (Input.GetMouseButtonDown(0))
            {
                isDragging = true;
                startTouch = Input.mousePosition;
                startTap = true;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (startTap && noSwipeWhileHolding)
                    tap = true;

                Reset();
                noSwipeWhileHolding = true;
            }

            #endregion

            #region Mobile Inputs

            if (Input.touchCount > 0)
            {
                if (Input.touches[0].phase == TouchPhase.Began)
                {
                    startTap = true;
                    isDragging = true;
                    startTouch = Input.touches[0].position;
                }
                else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
                {
                    if (startTap && noSwipeWhileHolding)
                        tap = true;

                    Reset();
                    noSwipeWhileHolding = true;
                }

                currentlyHeld = (Input.touches[0].phase == TouchPhase.Moved || Input.touches[0].phase == TouchPhase.Stationary);
            }
            else
            {
                currentlyHeld = Input.GetMouseButton(0);
            }

            #endregion

            swipeDelta = Vector2.zero;

            if (isDragging)
            {
                if (Input.touchCount > 0)
                    swipeDelta = Input.touches[0].position - startTouch;
                else if (Input.GetMouseButton(0))
                    swipeDelta = (Vector2)Input.mousePosition - startTouch;
            }

            if (swipeDelta.magnitude > swipeMagnitude)
            {
                float x = swipeDelta.x;
                float y = swipeDelta.y;

                if (Mathf.Abs(x) > Mathf.Abs(y))
                {
                    swipeLeft |= x < 0;
                    swipeRight |= x > 0;
                }
                else
                {
                    swipeDown |= y < 0;
                    swipeUp |= y > 0;
                }

                if (swipeDown || swipeUp || swipeLeft || swipeRight)
                {
                    noSwipeWhileHolding = false;
                }

                Reset();
            }
        }
    }

    public void Reset()
    {
        startTouch = swipeDelta = Vector2.zero;
        isDragging = false;
        startTap = false;
    }

    // got a tip for this from here: https://forum.unity.com/threads/detect-when-mouseposition-in-a-recttransform.500263/
    public bool TouchInZone(TouchZone touchZone, int touchCount=1)
    {
        if (Input.touchCount >= touchCount || Input.anyKey)
        {
            RectTransform rect = null;
            switch (touchZone)
            {
                case TouchZone.Engine:
                    rect = EngineArea;
                    break;
                case TouchZone.Sails:
                    rect = SailsArea;
                    break;
                case TouchZone.Boost:
                    rect = BoostArea;
                    break;
                default:
                    break;
            }

            switch (touchCount)
            {
                case 1:
                    //hacky way of enabling mouse testing:
                    Vector2 posOne = (Input.touchCount == 1 ? Input.touches[0].position : (Vector2)rect.InverseTransformPoint(Input.mousePosition));
                    //Vector2 posOne = (Input.touchCount == 1 ? Input.touches[0].position : (Vector2)Input.mousePosition);
                    return rect.rect.Contains(posOne);
                case 2:
                    return rect.rect.Contains(rect.InverseTransformPoint(Input.touches[0].position)) &&
                            rect.rect.Contains(rect.InverseTransformPoint(Input.touches[1].position));
                default:
                    return false;
            }
        }

        return false;
    }

    public Vector2 SwipeDelta { get { return swipeDelta; } }
    public Vector2 LastTapLocation { get { return startTouch; } }

    public bool Tap { get { return tap; } }

    public bool CurrentlyHeld { get { return currentlyHeld; } }

    public bool SwipeLeft { get { return swipeLeft; } }
    public bool SwipeRight { get { return swipeRight; } }
    public bool SwipeUp { get { return swipeUp; } }
    public bool SwipeDown { get { return swipeDown; } }


}
