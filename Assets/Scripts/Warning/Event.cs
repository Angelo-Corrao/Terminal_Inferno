using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Event
{
	public string id;
	public float duration;
	public List<string> sequence;
	public string lightColor;
	public string letter;
	public string description;
	public UnityEvent<int, bool> errorOutcome;
	public string keyboardCode = "";
	public ButtonAnimation keyboardEnter;

	private List<string> lightColors = new List<string>()
	{
		"RED",
		"BLUE",
		"GREEN"
	};

	private List<string> errorDescriptions = new List<string>()
	{
		"Weather issues",
		"Security check",
		"Communication problems",
		"Navigation problems",
		"Excessive air traffic",
		"Infrastructure problems"
	};

	public Event(string id) {
		this.id = id;

		int index;
		index = Random.Range(0, errorDescriptions.Count);
		description = errorDescriptions[index];
		index = Random.Range(0, lightColors.Count);
		lightColor = lightColors[index];
		letterGeneration(id, lightColor);

		switch (id) {
			case "401":
			case "402":
			case "403":
			case "404":
				duration = 1000f;

				sequence = new List<string>()
				{
					LetterAction400(letter)
				};
				break;

			case "601":
			case "602":
			case "603":
			case "604":
				duration = 1000f;

				sequence = new List<string>()
				{
					LetterAction600(letter)
				};
				break;

			case "800":
				duration = 1000f;
				description = letter;

				sequence = new List<string>()
				{
					LetterAction800(letter)
				};
				break;
		}
	}

	private string LetterAction400(string letter) {
		switch (letter) {
			case "A":
				return "Red Button";

			case "B":
				return "Blue Button";

			case "C":
				return "Lever 1 Up";

			case "D":
				return "Lever 2 Down";

			default:
				return "";
		}
	}

	private string LetterAction600(string letter) {
		switch (letter) {
			case "A":
				return "Lever 2 Up";

			case "B":
				return "Lever 1 Down";

			case "C":
				return "Yellow Button";

			case "D":
				return "Green Button";

			default:
				return "";
		}
	}

	private string LetterAction800(string letter) {
		char[] letters = letter.ToCharArray();
		for (int i = 0; i < letters.Length; i++) {
			switch (letters[i].ToString()) {
				case "A":
					keyboardCode += "1";
					break;

				case "B":
					keyboardCode += "2";
					break;

				case "C":
					keyboardCode += "3";
					break;

				case "D":
					keyboardCode += "4";
					break;

				case "E":
					keyboardCode += "5";
					break;

				case "F":
					keyboardCode += "6";
					break;

				case "G":
					keyboardCode += "7";
					break;

				case "H":
					keyboardCode += "8";
					break;

				case "I":
					keyboardCode += "9";
					break;

				default:
					return "";
			}
		}
		return keyboardCode;
	}

	private void letterGeneration(string id, string lightColor) {
		if (id == "401") {
			switch (lightColor) {
				case "RED":
					letter = "A";
					break;

				case "GREEN":
					letter = "D";
					break;

				case "BLUE":
					letter = "C";
					break;
			}
		}
		else if (id == "402") {
			switch (lightColor) {
				case "RED":
					letter = "B";
					break;

				case "GREEN":
					letter = "A";
					break;

				case "BLUE":
					letter = "D";
					break;
			}
		}
		else if (id == "403") {
			switch (lightColor) {
				case "RED":
					letter = "C";
					break;

				case "GREEN":
					letter = "B";
					break;

				case "BLUE":
					letter = "A";
					break;
			}
		}
		else if (id == "404") {
			switch (lightColor) {
				case "RED":
					letter = "D";
					break;

				case "GREEN":
					letter = "C";
					break;

				case "BLUE":
					letter = "B";
					break;
			}
		}
		else if (id == "601") {
			switch (lightColor) {
				case "RED":
					letter = "A";
					break;

				case "GREEN":
					letter = "D";
					break;

				case "BLUE":
					letter = "C";
					break;
			}
		}
		else if (id == "602") {
			switch (lightColor) {
				case "RED":
					letter = "B";
					break;

				case "GREEN":
					letter = "A";
					break;

				case "BLUE":
					letter = "D";
					break;
			}
		}
		else if (id == "603") {
			switch (lightColor) {
				case "RED":
					letter = "C";
					break;

				case "GREEN":
					letter = "B";
					break;

				case "BLUE":
					letter = "A";
					break;
			}
		}
		else if (id == "604") {
			switch (lightColor) {
				case "RED":
					letter = "D";
					break;

				case "GREEN":
					letter = "C";
					break;

				case "BLUE":
					letter = "B";
					break;
			}
		}
		else if (id == "800") {
			List<string> letters = new List<string>()
			{
				"A",
				"B",
				"C",
				"D",
				"E",
				"F",
				"G",
				"H",
				"I"
			};

			for (int i = 0; i < 4; i++) {
				int index = Random.Range(0, letters.Count);
				letter += letters[index];
			}
		}
	}
}
