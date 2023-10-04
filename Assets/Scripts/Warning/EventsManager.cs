using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Assets.Scripts.Data;

public class EventsManager : MonoBehaviour
{
	public float eventTimer = 5f;
	float eventCounterTimer = 5f;

	public static bool areGatesOpened = false;
	public List<GameObject> eventsUI;
	public Text[] eventIds;
	public Text[] colors;
	List<Event> activeEvents = new List<Event>();
	public Error01 error01;
	public Error02 error02;
	public Error03 error03;
	public static event Action noErrorOnScreen;
	bool speedRoutes = true;
	int errorLimits;
	int generatedEventCounter = 0;
    [HideInInspector] public bool firstEventSpawn = true;
	bool isPopUpActive = false;

	private void Start() {
		errorLimits = planeWaves[GameManager.currentWave].Item2;
	}

	private List<string> unexpectedEvents = new List<string>()
	{
		"401",
		"402",
		"403",
		"404",
		"601",
		"602",
		"603",
		"604",
		"800",
		"800"
	};

    public bool IsPopUpActive { get => isPopUpActive; set => isPopUpActive = value; }

    void Update() {
		if (areGatesOpened) {
			if (eventCounterTimer <= 0) {
				if (activeEvents.Count < errorLimits) {
					if(activeEvents.Count == 0)
						error01.errorClicked(1,0,1);
					GenerateEvent();
					eventCounterTimer = eventTimer;
					generatedEventCounter++;
				}
			}
			else {
				if (generatedEventCounter < errorLimits)
					eventCounterTimer -= Time.deltaTime;
				else if(activeEvents.Count == 0 && speedRoutes)
				{
					RouteManager.timeBetweenEachMovement = 1f;
					speedRoutes = false;
				}
			}
		}
		
	}

	public void GenerateEvent() {
		int index = UnityEngine.Random.Range(0, unexpectedEvents.Count);
		string id = unexpectedEvents[index];
		Event unexpectedEvent = new Event(id);
		activeEvents.Add(unexpectedEvent);

		SetUpError(true, activeEvents.Count - 1);
		SoundManager.Instance.PlayEvent();

		if (GameManager.instance.tutorial && firstEventSpawn) {
			GameManager.instance.tutorialPopUps[4].SetActive(true);
			firstEventSpawn = false;
			IsPopUpActive = true;
			GameManager.instance.PauseTutorial();
            GameManager.instance.AnyPopUpactive = true;
        }
	}

	public void RemoveEvent(int errorToRemove, bool solved) {
		switch (errorToRemove) {
			case 1:
				activeEvents.Remove(activeEvents[0]);
				SetUpError(false, 0);
				break;

			case 2:
				activeEvents.Remove(activeEvents[1]);
				error02.errorClicked(1, 0, 1);
				SetUpError(false, 1);
				break;

			case 3:
				activeEvents.Remove(activeEvents[2]);
				error03.errorClicked(2, 2, 3);
				SetUpError(false, 2);
				break;
		}

		if (!error01.isActiveAndEnabled && !error02.isActiveAndEnabled && !error03.isActiveAndEnabled) {
			noErrorOnScreen.Invoke();
			Error.errorSelected = 0;
		}
	}

	public void SetUpError(bool isGenerating, int errorToManage) {
		for (int i = 0; i < eventsUI.Count; i++) {
			if (i < activeEvents.Count) {
				eventIds[i].text = activeEvents[i].id;
				colors[i].text = activeEvents[i].description.ToString().ToUpper();

				
				if (isGenerating && i == errorToManage) {
					if (i == activeEvents.Count - 1) {
						AssociateEventToError(i, activeEvents[i], true, errorToManage);
					}
					else
						AssociateEventToError(i, activeEvents[i], false, errorToManage);
				}
				else
					AssociateEventToError(i, activeEvents[i], false, errorToManage);

				eventsUI[i].gameObject.SetActive(true);
			}
			else {
				eventsUI[i].gameObject.SetActive(false);
			}
		}

		switch (Error.errorSelected) {
			case 1:
				error01.SetLights(error01.color);
				break;

			case 2:
				error02.SetLights(error02.color);
				break;

			case 3:
				error03.SetLights(error03.color);
				break;
		}
	}

	// After the random generation of an event it will be associated to a new error on the screen
	public void AssociateEventToError(int errorNum, Event activeEvent, bool isGenerating, int errorToManage) {
		switch (errorNum) {
			case 0:
				error01.id = activeEvent.id;
				error01.sequence = activeEvent.sequence;
				error01.color = activeEvent.lightColor;
				if (isGenerating) {
					if (errorNum == errorToManage)
						error01.duration = activeEvent.duration;
				}
				else {
					if (errorNum >= errorToManage)
						error01.duration = error02.duration;
				}
				break;

			case 1:
				error02.id = activeEvent.id;
				error02.sequence = activeEvent.sequence;
				error02.color = activeEvent.lightColor;
				if (isGenerating) {
					if (errorNum == errorToManage)
						error02.duration = activeEvent.duration;
				}
				else {
					if (errorNum >= errorToManage)
						error02.duration = error03.duration;
				}
				break;

			case 2:
				error03.id = activeEvent.id;
				error03.sequence = activeEvent.sequence;
				error03.color = activeEvent.lightColor;
				if (isGenerating) {
					if (errorNum == errorToManage)
						error03.duration = activeEvent.duration;
				}
				break;
		}
	}

	// Each wave if there are errors on the screen they will be resetted
	public void ResetAllEvents()
	{
		areGatesOpened = false;
		int unresolvedEvents = activeEvents.Count;
		GameManager.instance.UpdateScore(-4 * unresolvedEvents);
		Error.totErrorsUnsolved += unresolvedEvents;
		GameManager.instance.UpdateStatistics();
		for (int i=0; i < activeEvents.Count; i++) {
			eventsUI[i].gameObject.SetActive(false);
		}
		activeEvents.Clear();
		eventCounterTimer = eventTimer;
		errorLimits = planeWaves[GameManager.currentWave].Item2;
		generatedEventCounter = 0;
		noErrorOnScreen.Invoke();
		error01.sequenceText.text = "";
		speedRoutes = true;
	}
}
