using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Draggable : MonoBehaviour
{
	public bool canDrag = true;
	public float sensitivity = 0.2f;
	public float upperThresholdValue = 4f;
	public float lowerThresholdValue = 3f;
    protected Camera cam;
	protected Rigidbody rb;
	protected bool isDragging = false;
	protected bool canDrop = false;
	bool firstPick = true;

	protected virtual void Awake() {
		rb = GetComponent<Rigidbody>();
		cam = Camera.main;
	}

	private void OnMouseEnter() {
		GameManager.instance.pointer.sprite = GameManager.instance.iconDragging;
	}

	private void OnMouseExit() {
		GameManager.instance.pointer.sprite = GameManager.instance.iconDefault;
	}

	private void OnMouseDrag() {
		if (canDrag)
		{
			// on drag reset rigidbody gravity and set object as child of camera
			isDragging = true;
			rb.useGravity = false;
			rb.drag = 100;
			rb.angularDrag = 100;
			transform.SetParent(cam.transform);
			canDrop = false;
			GameManager.instance.pointer.sprite = GameManager.instance.iconDragging;
		}
	}

	private void OnMouseUp() {
		if (canDrag)
			UnDragging();
		else
			canDrag = true;
	}

	public void UnDragging()
	{
		isDragging = false;
		rb.useGravity = true;
		rb.drag = 1;
		rb.angularDrag = 0.01f;
		transform.SetParent(null);
		canDrop = true;
		firstPick = true;
		GameManager.instance.pointer.sprite = GameManager.instance.iconDefault;
	}

	private void OnMouseDown()
	{
		canDrag = true;
		SoundManager.Instance.PlayObejctPicking();
	}

	protected virtual void Update()
	{
		if (isDragging)
		{
			// by using mouse scroll can move the object in the forward direction
            float upperThreshold = upperThresholdValue;
            transform.forward = cam.transform.forward;
			transform.localRotation = Quaternion.Euler(0, 180, 0);
			transform.position += Input.mouseScrollDelta.y * sensitivity * cam.transform.forward;
			if (firstPick)
			{
				upperThreshold = lowerThresholdValue;
				firstPick = false;
			}
			// value of moving are clamp into a range
			float z = Mathf.Clamp(transform.localPosition.z, lowerThresholdValue, upperThreshold);
			transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, z);
		}
	}

}
