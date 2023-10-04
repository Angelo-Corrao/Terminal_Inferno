public class SequenceButtons : ButtonAnimation
{
	void Update() {
		base.Animation(minScale, maxScale, scalePerUnit, axesToScale);

		if (isPressed) {
			if (isNegativeAnimation) {
				if (transform.localScale.y >= maxScale) {
					// When pressed the sequence button the event manager will check if it is the input nedeed to the sequence that resolve the error selected in the errors monitor
					pressed.Invoke(buttonNum);
					SoundManager.Instance.PlayButtonPress();
				}
			}
		}
	}
}
