﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;


// "Borrowed" from https://forum.unity.com/threads/touchscreen-steering-wheel-rotation-example-mouse-supported.196741/
public class DashboardSailsWheel : MonoBehaviour
{
    Graphic UI_Element;

    RectTransform rectT;
    Vector2 centerPoint;

    //TODO: Move to the global settings area
    private float maximumSteeringAngle = 720f;
    private float wheelReleasedSpeed = 300f;

    public float wheelAngle = 0f;
    float wheelPrevAngle = 0f;

    bool wheelBeingHeld = false;

    public float GetClampedValue()
    {
        // returns a value in range [-1,1] similar to GetAxis("Horizontal")
        return wheelAngle / maximumSteeringAngle;
    }

    public float GetAngle()
    {
        // returns the wheel angle itself without clamp operation
        return wheelAngle;
    }

    private void Awake()
    {
        UI_Element = GetComponent<Image>();
    }

    void Start()
    {
        rectT = UI_Element.rectTransform;
        InitEventsSystem();
    }

    private float EPSILON = 0.01f;
    void Update()
    {
        // If the wheel is released, reset the rotation
        // to initial (zero) rotation by wheelReleasedSpeed degrees per second
        if (!wheelBeingHeld && !SailsController.Instance.Locked && !Mathf.Approximately(0f, wheelAngle))
        {
            float deltaAngle = wheelReleasedSpeed * Time.deltaTime;
            if (Mathf.Abs(deltaAngle) > Mathf.Abs(wheelAngle))
                wheelAngle = 0f;
            else if (wheelAngle > 0f)
                wheelAngle -= deltaAngle;
            else
                wheelAngle += deltaAngle;

            SoundManager.Instance.PlaySoundEffect(SoundManager.SoundEffect.Dashboard_Sails, true);
        }
        else
        {
            if (Mathf.Abs(GetClampedValue() - 1) > EPSILON && SailsController.Instance.Locked)
            {
                SailsController.Instance.SetState(SailsState.SailsDown);
                SailsController.Instance.Locked = false;
            }
        }

        // Rotate the wheel image
        rectT.localEulerAngles = Vector3.back * wheelAngle;

        if ((System.Math.Abs(GetClampedValue() - 1) < EPSILON && !SailsController.Instance.Locked))
        {
            SailsController.Instance.SetState(SailsState.SailsUp);
            SailsController.Instance.Locked = true;
        }
    }

    void InitEventsSystem()
    {
        // Warning: Be ready to see some extremely boring code here :-/
        // You are warned!
        EventTrigger events = UI_Element.gameObject.GetComponent<EventTrigger>();

        if (events == null)
            events = UI_Element.gameObject.AddComponent<EventTrigger>();

        if (events.triggers == null)
            events.triggers = new System.Collections.Generic.List<EventTrigger.Entry>();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        EventTrigger.TriggerEvent callback = new EventTrigger.TriggerEvent();
        UnityAction<BaseEventData> functionCall = new UnityAction<BaseEventData>(PressEvent);
        callback.AddListener(functionCall);
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback = callback;

        events.triggers.Add(entry);

        entry = new EventTrigger.Entry();
        callback = new EventTrigger.TriggerEvent();
        functionCall = new UnityAction<BaseEventData>(DragEvent);
        callback.AddListener(functionCall);
        entry.eventID = EventTriggerType.Drag;
        entry.callback = callback;

        events.triggers.Add(entry);

        entry = new EventTrigger.Entry();
        callback = new EventTrigger.TriggerEvent();
        functionCall = new UnityAction<BaseEventData>(ReleaseEvent);//
        callback.AddListener(functionCall);
        entry.eventID = EventTriggerType.PointerUp;
        entry.callback = callback;

        events.triggers.Add(entry);
    }

    public void PressEvent(BaseEventData eventData)
    {
        // Executed when mouse/finger starts touching the steering wheel
        Vector2 pointerPos = ((PointerEventData)eventData).position;

        wheelBeingHeld = true;
        centerPoint = RectTransformUtility.WorldToScreenPoint(((PointerEventData)eventData).pressEventCamera, rectT.position);
        wheelPrevAngle = Vector2.Angle(Vector2.up, pointerPos - centerPoint);
    }

    public void DragEvent(BaseEventData eventData)
    {
        // Executed when mouse/finger is dragged over the steering wheel
        Vector2 pointerPos = ((PointerEventData)eventData).position;

        float wheelNewAngle = Vector2.Angle(Vector2.up, pointerPos - centerPoint);
        // Do nothing if the pointer is too close to the center of the wheel
        if (Vector2.Distance(pointerPos, centerPoint) > 20f)
        {
            if (pointerPos.x > centerPoint.x)
                wheelAngle += wheelNewAngle - wheelPrevAngle;
            else
                wheelAngle -= wheelNewAngle - wheelPrevAngle;
        }
        // Make sure wheel angle never exceeds maximumSteeringAngle
        //wheelAngle = Mathf.Clamp(wheelAngle, -maximumSteeringAngle, maximumSteeringAngle);
        wheelAngle = Mathf.Clamp(wheelAngle, 0, maximumSteeringAngle);
        wheelPrevAngle = wheelNewAngle;

        if (!SailsController.Instance.Locked)
            SoundManager.Instance.PlaySoundEffect(SoundManager.SoundEffect.Dashboard_Sails, true);
    }

    public void ReleaseEvent(BaseEventData eventData)
    {
        // Executed when mouse/finger stops touching the steering wheel
        // Performs one last DragEvent, just in case
        DragEvent(eventData);

        wheelBeingHeld = false;
    }

}