using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public CameraMovement cam;
    float inizialSensitivity;

	private void Start() {
        inizialSensitivity = cam.sensitivity;
	}

	public void SetScreenMode(int optionSelected) {
        switch (optionSelected) {
            case 0:
                Screen.fullScreen = true;
                break;

            case 1:
                Screen.fullScreen = false;
                break;
        }
    }

	public void SetAudioVolume(float volume) {
        if (volume == 0)
			SoundManager.Instance.mixer.SetFloat("MasterVol", -80f);
        else
            SoundManager.Instance.mixer.SetFloat("MasterVol",Mathf.Lerp(-60f,10f,volume));
	}

	public void SetSensitivity(float volume) {
		cam.sensitivity = inizialSensitivity / 100 * ((volume + 0.5f) * 100);
	}
}
