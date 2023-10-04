using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Error : MonoBehaviour
{
	public int errorNum;
    public static int errorSelected = 0;
	public List<string> sequence;
	public Text sequenceText;
	protected static int stepCounter = 0;
	public Image[] borders;
	public float duration;
	public UnityEvent<int, bool> errorOutcome;
	public string color = null;
	public GameObject redLight;
	public GameObject greenLight;
	public GameObject blueLight;
	public string id;
	public Slider slider;
	public bool isSliderOK;
	public bool isMagneticCardNedeed = false;
	public bool isFirst600Input = true;
	public MagneticCard magneticCard;
	public static string lastInput;
	public static int totErrorsSolved = 0;
	public static int totErrorsUnsolved = 0;
	public KeyboardEnter keyboardEnter;
	public Key key;
	public bool isSwitchOK = false;
	public Animation[] animationLight;
	public Light[] lights;

	private void Awake() {
		EventsManager.noErrorOnScreen += TurnLightsOff;
		gameObject.SetActive(false);
		foreach (Animation animation in animationLight)
			animation.wrapMode = WrapMode.Loop;
		
	}

	// Check if the interactable object pressed is the one nedeed to resolve the error selected in the errors monitor
	public virtual void validateSequence(string lastInput) {
		if (stepCounter < sequence.Count) {
			if (lastInput == sequence[stepCounter]) {
				if (stepCounter == sequence.Count - 1) {
					if (!isMagneticCardNedeed) {
						sequenceText.text = "";
						stepCounter = 0;
						isFirst600Input = true;
						GameManager.instance.UpdateScore(6);
						totErrorsSolved++;
						GameManager.instance.UpdateStatistics();
						errorOutcome.Invoke(errorSelected, true);
					}
					else {
						sequenceText.text = lastInput + ", Magnetic card is nedeed";
						magneticCard.isNedeed = true;
					}
				}
				else {
					sequenceText.text += ", " + lastInput;
					if (stepCounter == 0)
						sequenceText.text = sequenceText.text.Substring(2);
					stepCounter++;
				}
			}
			else {
				sequenceText.text = "";
				stepCounter = 0;
				Error.lastInput = "";
				magneticCard.isNedeed = false;

				GameManager.instance.UpdateScore(-0.25f);
				GameManager.instance.UpdateStatistics();
			}
		}
	}

	public void SetLights(string color) {
		switch (color) {
			case "RED":
				redLight.GetComponent<Renderer>().material.color = Color.red;
				greenLight.GetComponent<Renderer>().material.color = Color.white;
				blueLight.GetComponent<Renderer>().material.color = Color.white;
				for (int i = 0; i < animationLight.Length; i++) {
					if (i == 0) {
						animationLight[i].Play();
					}
					else {
						animationLight[i].Stop();
						lights[i].color = Color.black;
					}
				}
				break;

			case "BLUE":
				redLight.GetComponent<Renderer>().material.color = Color.white;
				greenLight.GetComponent<Renderer>().material.color = Color.white;
				blueLight.GetComponent<Renderer>().material.color = Color.blue;
				for (int i = 0; i < animationLight.Length; i++) {
					if (i == 1) {
						animationLight[i].Play();
					}
					else { 
						animationLight[i].Stop();
						lights[i].color = Color.black;
					}
				}
				break;

			case "GREEN":
				redLight.GetComponent<Renderer>().material.color = Color.white;
				greenLight.GetComponent<Renderer>().material.color = Color.green;
				blueLight.GetComponent<Renderer>().material.color = Color.white;
				for (int i = 0; i < animationLight.Length; i++) {
					if (i == 2) {
						animationLight[i].Play();
					}
					else {
						animationLight[i].Stop();
						lights[i].color = Color.black;
					}
				}
				break;
		}
	}

	public void TurnLightsOff() {
		redLight.GetComponent<Renderer>().material.color = Color.white;
		greenLight.GetComponent<Renderer>().material.color = Color.white;
		blueLight.GetComponent<Renderer>().material.color = Color.white;
		for (int i = 0; i < animationLight.Length; i++) {
			animationLight[i].Stop();
			lights[i].color = Color.black;
		}
	}

	// Check if slider is in the right position to resolve 400's errors
	public void SliderCheck() {
		switch (color) {
			case "RED":
				if (slider.state == Slider.SliderState.Low)
					isSliderOK = true;
				else
					isSliderOK = false;
				break;

			case "GREEN":
				if (slider.state == Slider.SliderState.Middle)
					isSliderOK = true;
				else
					isSliderOK = false;
				break;

			case "BLUE":
				if (slider.state == Slider.SliderState.High)
					isSliderOK = true;
				else
					isSliderOK = false;
				break;
		}
	}

	// Check if the key is inserted in the correct switch to resolve 800's errors
	public void SwitchCheck() {
		switch (color) {
			case "RED":
				if (key.currentSwitch == "Red Switch")
					isSwitchOK = true;
				else
					isSwitchOK = false;
				break;

			case "GREEN":
				if (key.currentSwitch == "Green Switch")
					isSwitchOK = true;
				else
					isSwitchOK = false;
				break;

			case "BLUE":
				if (key.currentSwitch == "Blue Switch")
					isSwitchOK = true;
				else
					isSwitchOK = false;
				break;
		}
	}

	// Change the error selected by clicking so the sequence to control change
	public void errorClicked(int errorNum, int border1, int border2) {
		Error.errorSelected = errorNum;

		// Reset all error's sequences
		sequenceText.text = "";
		stepCounter = 0;
		Error.lastInput = "";
		magneticCard.isNedeed = false;
		keyboardEnter.insertedCode = "";

		for (int i = 0; i < borders.Length; i++) {
			if (i == border1 || i == border2)
				borders[i].color = Color.white;
			else
				borders[i].color = Color.black;
		}

		if (color != null) {
			SetLights(color);
		}
	}
}
