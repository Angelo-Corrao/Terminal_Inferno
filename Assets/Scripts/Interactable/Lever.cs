using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Lever : MonoBehaviour
{
	enum LeverState
	{
		Default,
		Up,
		Down
	}

	LeverState state = LeverState.Default;
	Camera cam;
	public float leverSensitivity = 5f;
	float xRotation = 0f;
	public string leverNumber;
	public UnityEvent<string> pressed;

	void Awake() {
		cam = Camera.main;
	}

	// When released after 1 second the lever will turn in the middle position
	private void OnMouseUp() {
		if (xRotation <= -45f) {
			SoundManager.Instance.PlayLever1();
			transform.localRotation = Quaternion.Euler(-45f, 0, 0);
			state = LeverState.Up;
			pressed.Invoke($"Lever {leverNumber} Up");
			StartCoroutine(ResetPosition());
		}
		else if (xRotation >= 45f) {
			SoundManager.Instance.PlayLever2();
			transform.localRotation = Quaternion.Euler(45f, 0, 0);
			state = LeverState.Down;
			pressed.Invoke($"Lever {leverNumber} Down");
			StartCoroutine(ResetPosition());
		}
		else
			transform.localRotation = Quaternion.Euler(0, 0, 0);

		xRotation = 0;
		cam.GetComponent<CameraMovement>().isDragging = false;
		GameManager.instance.pointer.sprite = GameManager.instance.iconDefault;
	}

	// When the player drags the lever he is able to move it based on the Mouse Y axis
	private void OnMouseDrag() {
		float mouseY = Input.GetAxis("Mouse Y");
		mouseY = mouseY * leverSensitivity;
		xRotation -= mouseY;
		transform.localRotation = Quaternion.Euler(Mathf.Clamp(xRotation, -45f, 45f), 0, 0);

		cam.GetComponent<CameraMovement>().isDragging = true;
		GameManager.instance.pointer.sprite = GameManager.instance.iconDragging;
	}

	private IEnumerator ResetPosition() {
		yield return new WaitForSeconds(1f);
		transform.localRotation = Quaternion.Euler(0, 0, 0);
		state = LeverState.Default;
	}

	private void OnMouseEnter() {
		GameManager.instance.pointer.sprite = GameManager.instance.iconDragging;
	}

	private void OnMouseExit() {
		GameManager.instance.pointer.sprite = GameManager.instance.iconDefault;
	}
}
