using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Error03 : Error, IPointerDownHandler {
	private void Update() {
		duration -= Time.deltaTime;

		if (duration <= 0) {
			errorOutcome.Invoke(errorNum, false);
		}
	}

	public override void validateSequence(string lastInput) {
		Error.lastInput = lastInput;

		if (id == "401" || id == "402" || id == "403" || id == "404") {
			if (Error.errorSelected == 3) {
				SliderCheck();
				if (isSliderOK)
					base.validateSequence(lastInput);
				else {
					GameManager.instance.UpdateScore(-0.25f);
					GameManager.instance.UpdateStatistics();
				}
			}
		}
		else if (id == "601" || id == "602" || id == "603" || id == "604") {
			if (Error.errorSelected == 3) {
				if (isFirst600Input) {
					isMagneticCardNedeed = true;
					isFirst600Input = false;
				}
				base.validateSequence(lastInput);
			}
		}
		else if (id == "800") {
			if (Error.errorSelected == 3) {
				SwitchCheck();
				if (isSwitchOK)
					base.validateSequence(lastInput);
				else {
					keyboardEnter.insertedCode = string.Empty;
					sequenceText.text = string.Empty;
					GameManager.instance.UpdateScore(-0.25f);
					GameManager.instance.UpdateStatistics();
				}
			}
		}
	}

	public void OnPointerDown(PointerEventData eventData) {
		errorClicked(3,4,5);
	}

	public void WriteDigitedNumbers(string buttonNum) {
		if (id == "800") {
			if (Error.errorSelected == 3) {
				char[] codeNumbers = keyboardEnter.insertedCode.ToCharArray();
				if (codeNumbers.Length < 4)
					keyboardEnter.insertedCode += buttonNum;
				sequenceText.text = keyboardEnter.insertedCode;
			}
		}
	}
}
