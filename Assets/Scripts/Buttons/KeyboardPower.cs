using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class KeyboardPower : ButtonAnimation
{
	public bool isKeyboardOn = true;

	void Update() {
		base.Animation(minScale, maxScale, scalePerUnit, axesToScale);

		if (isPressed) {
			if (isNegativeAnimation) {
				if (transform.localScale.y >= maxScale) {
					ChangeState();
					SoundManager.Instance.PlayOn_Off();
				}
			}
		}
	}

	// This manage the keyboardState and if it's off the keyboard will not save the inserted inputs and the color of the keyboard power will be red
	public void ChangeState() {
		if (isKeyboardOn) {
			isKeyboardOn = false;
			gameObject.GetComponent<Renderer>().material.color = Color.red;
		}
		else {
			isKeyboardOn = true;
			gameObject.GetComponent<Renderer>().material.color = Color.green;
		}
	}

	// This is nedeed when the game restart, so if a game end with the keyboard turned off the next game it will be on again
	public void setKeyboardOn() {
		isKeyboardOn = true;
		gameObject.GetComponent<Renderer>().material.color = Color.green;
	}
}
