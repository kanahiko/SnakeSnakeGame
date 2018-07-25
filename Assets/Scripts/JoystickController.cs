using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class JoystickController : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler {

    Vector2 joysticDirection = new Vector2(0, 1);
    bool isPressed = false;
    Transform joystick;

    void Start()
    {
        joystick = transform.GetChild(0);
    }

    public void OnDrag(PointerEventData eventData)    {
        //gets vector for direction by moving joystic really far and then clampining it
        joystick.position =eventData.position;
        joystick.localPosition = Vector2.ClampMagnitude(joystick.localPosition*100, 30);
        joysticDirection = joystick.localPosition.normalized;

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = false;
    }

    public Vector2 GetJoysticDirection()
    {
        return joysticDirection;
    }

    public bool GetIsPressed()
    {
        return isPressed;
    }
}
