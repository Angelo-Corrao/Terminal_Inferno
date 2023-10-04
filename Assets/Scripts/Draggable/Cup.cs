using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Cup : TriggerDependent
{
	Vector3 localDrawerPos;
	Vector3 localDrawerScale;
	public float throwForce = 10f;
	bool isThrown = false;

	protected override void Awake() {
		cam = Camera.main;
		rb = GetComponent<Rigidbody>();
		isCup = true;
		localDrawerPos = new Vector3(0, 0.05f, -0.15f);
		localDrawerScale = new Vector3(0.25f, 0.25f, 0.25f);
	}

	private void Update() {
		base.Update();

		if (isDragging && Input.GetKeyDown(KeyCode.Mouse1)) {
			canDrag = false;
			UnDragging();
			isThrown = true;
		}
	}

	private void FixedUpdate() {
		if (isThrown) {
			rb.AddForce(cam.transform.forward * throwForce, ForceMode.Impulse);
			isThrown = false;
		}
	}

	protected override void OnTriggerStay(Collider other) {
		if (canDrop) {
			if (other.gameObject.CompareTag("Drawer")) {
				transform.SetParent(other.transform, false);
				transform.localPosition = localDrawerPos;
				transform.localRotation = Quaternion.identity;
				transform.localScale = localDrawerScale;
			}
		}
	}
}
