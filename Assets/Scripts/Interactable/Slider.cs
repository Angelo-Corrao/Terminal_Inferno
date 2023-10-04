using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Slider : MonoBehaviour
{
	public enum SliderState {
		High,
		Middle,
		Low
	}

	public SliderState state = SliderState.Middle;
	Camera cam;
	public float sliderSensitivity = 0.05f;
	float zPosition = 0f;
	float currentPosition = 0f;
	int currentState = 0;
	float extremeUP = -16f;
	float extremeDOWN = 16f;


	void Awake() {
		cam = Camera.main;
	}

	private void OnMouseUp() {
		SoundManager.Instance.PlaySliderPosition();
		zPosition = currentPosition;
		transform.localPosition = new Vector3(0, 0, zPosition);
		cam.GetComponent<CameraMovement>().isDragging = false;
		GameManager.instance.pointer.sprite = GameManager.instance.iconDefault;
	}

	// When the player drags the slider he is able to move it based on the Mouse Y axis
	private void OnMouseDrag() {
		float mouseY = Input.GetAxis("Mouse Y");
		mouseY *= sliderSensitivity;
		zPosition -= mouseY;
		zPosition = Mathf.Clamp(zPosition, extremeUP, extremeDOWN);
		transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, zPosition);

		// Every time the slider reach a state it will change the enum value so the EventsManager knows the slider's state
		switch (zPosition) {
			case float i when i > -1f && i < 1f:
				if (currentState != 1) {
					currentPosition = 0f;
					state = SliderState.Middle;
					currentState = 1;
				}
				break;

			case 16:
				if (currentState != 2) {
					currentPosition = extremeDOWN;
					state = SliderState.Low;
					currentState = 2;
				}
				break;

			case -16:
				if (currentState != 3) {
					currentPosition = extremeUP;
					state = SliderState.High;
					currentState = 3;
				}
				break;
		}

		cam.GetComponent<CameraMovement>().isDragging = true;
		GameManager.instance.pointer.sprite = GameManager.instance.iconDragging;
	}

	private void OnMouseEnter() {
		GameManager.instance.pointer.sprite = GameManager.instance.iconDragging;
	}

	private void OnMouseExit() {
		GameManager.instance.pointer.sprite = GameManager.instance.iconDefault;
	}
}
