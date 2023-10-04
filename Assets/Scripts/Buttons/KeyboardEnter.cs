using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is the button that send the code written with the others keyboard buttons to the EventsManager
public class KeyboardEnter : ButtonAnimation
{
    public string insertedCode = string.Empty;
	public KeyboardPower keyboardPower;

	void Update() {
		base.Animation(minScale, maxScale, scalePerUnit, axesToScale);

		if (isPressed) {
			if (isNegativeAnimation) {
				if (transform.localScale.y >= maxScale && keyboardPower.isKeyboardOn) {
					pressed.Invoke(insertedCode);
					// After the code is sent it is resetted
					insertedCode = string.Empty;
                    SoundManager.Instance.PlayButtonPress();
                }
			}
		}
	}
}
