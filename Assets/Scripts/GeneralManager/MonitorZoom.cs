using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class MonitorZoom : MonoBehaviour
{
    public bool main;
    public bool warning;
    public GraphicRaycaster raycaster;
    public Canvas pointer;
    public float posUPOffset;

    Camera cam;
    bool enable = false;
    bool zoom = false;
    [HideInInspector] public bool firstMainZoom = true;
	bool isPopUpActive = false;
	Vector3 standardPos;
    Quaternion standardRot;

	public bool Enable { get => enable; set => enable = value; }
    public bool Zoom { get => zoom; set => zoom = value; }
    public bool IsPopUpActive { get => isPopUpActive; set => isPopUpActive = value; }

    void Awake()
    {
        cam = Camera.main;
        standardPos = cam.transform.position;
        standardRot = cam.transform.rotation;
	}

    private void OnMouseDown()
    {
        if (!Enable) return;
        //Need to zoom in
        if (!zoom)
        {
            ZoomIN();
            StartCoroutine(EnableAfterZoom());
        }
    }

    public void ZoomIN()
    {
        if (GameManager.instance.tutorial && main && firstMainZoom) {
			GameManager.instance.tutorialPopUps[3].SetActive(true);
			firstMainZoom = false;
            IsPopUpActive = true;
            GameManager.instance.PauseTutorial();
            GameManager.instance.AnyPopUpactive = true;
        }

        // change camera into ortographics, change camera position and rotation based on monitor
        if (main)
            RouteManager.monitorInteractable = true;
        zoom = true;
        GameManager.zoom = true;
        standardPos = cam.transform.position;
        standardRot = cam.transform.rotation;
        cam.GetComponent<CameraMovement>().locked = true;

        cam.transform.SetParent(transform);
        cam.transform.localPosition = Vector3.zero + Vector3.up * posUPOffset;
        cam.transform.localPosition += Vector3.forward;
        cam.transform.localRotation = Quaternion.Euler(transform.localRotation.x, 180, transform.localRotation.z);

        if (warning)
            cam.fieldOfView = 40;
        else
        {
            cam.orthographic = true;
            cam.orthographicSize = 0.6f;
        }
        pointer.gameObject.SetActive(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    IEnumerator EnableAfterZoom()
    {   // enable interaction with monitor after zoom
        yield return new WaitForSeconds(0.3f);
        raycaster.enabled = true;
    }

    public void ZoomOutCam()
    {   // reset camera with previous position
        zoom = false;
        GameManager.zoom = false;
		raycaster.enabled = false;
		cam.orthographic = false;
        cam.transform.SetParent(null);
        cam.transform.position = standardPos;
        cam.transform.rotation = standardRot;
		cam.GetComponent<CameraMovement>().locked = false;
        if (main)
            RouteManager.monitorInteractable = false;
        if (warning)
            cam.fieldOfView = 60;
    }

    public void ZoomOut()
    {
        ZoomOutCam();
        // reset pointer and cursor
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		pointer.gameObject.SetActive(true);
    }

    void Update()
    {
		if (Input.GetKeyDown(KeyCode.Mouse1) && zoom && enable && !GameManager.gamePause)
			ZoomOut();
    }

	private void OnMouseEnter() {
		GameManager.instance.pointer.sprite = GameManager.instance.iconOver;
	}

	private void OnMouseExit() {
		GameManager.instance.pointer.sprite = GameManager.instance.iconDefault;
	}
}