using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class JoystickController : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler {

    Vector2 joysticDirection = new Vector2(0, 1);
    bool isPressed = false;

    public void OnDrag(PointerEventData eventData)
    {
        //gets vector for direction by moving joystic really far and then clampining it
        transform.GetChild(0).position =eventData.position;
        transform.GetChild(0).localPosition = Vector2.ClampMagnitude(transform.GetChild(0).localPosition*100, 30);
        joysticDirection = new Vector2(transform.GetChild(0).localPosition.x, transform.GetChild(0).localPosition.y).normalized;

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
