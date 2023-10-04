using UnityEngine;

// This is the monitor's power button
public class Power : ButtonAnimation
{
	public GameObject monitor;
	public GameObject lines;
	public bool isPcOn = true;
	public bool main;
	public Material materialsOn;
	public Material materialsOff;

	private MeshRenderer mesh;

	private void Awake()
	{
		mesh = GetComponent<MeshRenderer>();
	}


	void Update() {
		base.Animation(minScale, maxScale, scalePerUnit, axesToScale);

		if (isPressed) {
			if (isNegativeAnimation) {
				if (transform.localScale.z >= maxScale) {
					ChangeState();
					SoundManager.Instance.PlayOn_Off();
				}
			}
		}
	}

	public void ChangeState() {
		if (isPcOn) {
			monitor.gameObject.SetActive(false);
			if (main)
				lines.SetActive(false);
			isPcOn = false;
			mesh.material = materialsOff;
		}
		else {
			monitor.gameObject.SetActive(true);
			if (main)
				lines.SetActive(true);
			isPcOn = true;
			mesh.material = materialsOn;
		}
	}

	// This is nedeed when the game restart, so if a game end with the monitor turned off the next game it will be on again
	public void setMonitorOn() {
		monitor.gameObject.SetActive(true);
		if (main)
			lines.SetActive(true);
		isPcOn = true;
		mesh.material = materialsOn;
	}
}
