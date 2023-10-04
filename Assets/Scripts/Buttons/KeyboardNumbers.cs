using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class KeyboardNumbers : ButtonAnimation
{
	public KeyboardPower keyboardPower;
	public UnityEvent<string> digitPressed;

	void Update() {
		base.Animation(minScale, maxScale, scalePerUnit, axesToScale);

		if (isPressed) {
			if (isNegativeAnimation) {
				if (transform.localScale.y >= maxScale && keyboardPower.isKeyboardOn) {
					// Every time a keyboardButton is pressed it will be added to the insertedCode until the keaboardEnter is not pressed
					digitPressed.Invoke(buttonNum);
                    SoundManager.Instance.PlayButtonPress();
                }
			}
		}
	}
}
