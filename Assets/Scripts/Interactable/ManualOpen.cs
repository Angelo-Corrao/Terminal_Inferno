using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class ManualOpen: MonoBehaviour
{
    private bool interactable = false;
    private bool reading = false;
    [HideInInspector] public static bool over = false;
    public UnityEvent readingEvent;

    private void OnMouseEnter()
    {
		GameManager.instance.pointer.sprite = GameManager.instance.iconOver;

		if (!reading)
        {
            over = true;
            interactable = true;
        }
    }

    private void OnMouseExit()
    {
		GameManager.instance.pointer.sprite = GameManager.instance.iconDefault;

		if (!reading)
        {
            over = false;
            interactable = false;
        }
    }

    public void ResetInteraction()
    {
        reading = false;
        interactable = false;
        over = false;
    }

    protected void Update()
    {
        if(interactable && Input.GetMouseButtonDown(0))
        {
            interactable = false;
            reading = true;
            // call event
			readingEvent.Invoke();
        }
        // reset with right click
        if (Input.GetKeyDown(KeyCode.Mouse1) && reading)
        {
            interactable = true;
            reading = false;
        }
    }
}
