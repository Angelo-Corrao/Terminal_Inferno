using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using static Assets.Scripts.Data;
using System.Linq;

public class KevinEvents : MonoBehaviour
{
    // public var
    public GameObject kevinUI;
    [Tooltip("Min time in second of the random range of an event spawn")]
    [SerializeField] float minTimeSpawnEvent = 90;
    [Tooltip("Max time in second of the random range of an event spawn")]
    [SerializeField] float maxTimeSpawnEvent = 120;
    [Tooltip("Seconds kevin event remain visible")]
    [SerializeField] int numSecondKevin = 5;
    public Texture[] sprites; // happy, neutral, angry
    public GameObject[] monitorsUI;
    public MonitorZoom[] monitorsZoom;
    public Power[] powersButton;
    public GameObject lines;
    public Ack siren;
    public ManualClosed manual;
    public ManualOpen manualOpen;
    public ManualReading manualReading;
    public Key key;
    public MagneticCard magneticCard;
    public KeyboardPower keyboard;
    public Text kevinPhraseTextUI;
    public RawImage kevinImgUI;
    [HideInInspector] public int countTotalKevinEvents = 0;
    [HideInInspector] public bool firstKevinEvents = true;

    // private var
    private bool left = true;
    private float startTime = 0;
    private float randomWaitTime;
    private delegate void KevinEvent();
    private Dictionary<string, Tuple<KevinState, string, KevinEvent>> kevinEvents = new();
    private Dictionary<KevinState, Texture> spriteCorrelation = new();    
	private bool isPopUpActive = false;

    public bool IsPopUpActive { get => isPopUpActive; set => isPopUpActive = value; }

    private void DictInitialization()
    {
        kevinEvents.Add(
            "Monitor",
            new Tuple<KevinState, string, KevinEvent>
            (
                KevinState.Happy,
                $"Oops, I tripped over the <b>monitor</b> cable, I'll reattach it for you! You're welcome!",
                MonitorEvent
            )
        );
        kevinEvents.Add(
            "Keyboard",
            new Tuple<KevinState, string, KevinEvent>
            (
                KevinState.Angry,
                $"I turned off your <b>keyboard</b> so it doesn't waste battery power! The planet thanks",
                KeyboardEvent
            )
        );
        kevinEvents.Add(
            "Alarm",
            new Tuple<KevinState, string, KevinEvent>
            (
                KevinState.Neutral,
                "Ah sorry, I touched something back there.",
                AlarmEvent
            )
        );
		kevinEvents.Add(
			"Handbook",
			new Tuple<KevinState, string, KevinEvent>
			(
				KevinState.Happy,
				$"I put your <b>handbook</b> back in the <b>@side drawer</b>, you’re welcome",
				HandbookEvent
			)
		);
		kevinEvents.Add(
			"Key",
			new Tuple<KevinState, string, KevinEvent>
			(
				KevinState.Neutral,
				$"I found a misplaced <b>key</b>, I put it back in the <b>@side drawer</b>",
				KeyEvent
			)
		);
		kevinEvents.Add(
			"Magnetic Card",
			new Tuple<KevinState, string, KevinEvent>
			(
				KevinState.Angry,
				$"What is this <b>magnetic card</b>? Are you kidding me? This place is a total mess!",
				MagneticCardEvent
			)
		);

        spriteCorrelation.Add(KevinState.Happy, sprites[0]);
        spriteCorrelation.Add(KevinState.Neutral, sprites[1]);
        spriteCorrelation.Add(KevinState.Angry, sprites[2]);

    }

    private void Awake() =>
        DictInitialization();
    

    // activate kevin timer
    public void StartKevin()
    {
        startTime = Time.time;
        randomWaitTime = Random.Range(minTimeSpawnEvent, maxTimeSpawnEvent);
    }


    private void Update()
    {
		if (!GameManager.startingMenu)
			if (Time.time > startTime + randomWaitTime)
            {   // after timer end spawn kevin pop up and activate a random event
                StartCoroutine(SpawnKevin());
                countTotalKevinEvents++;
                int rndIndex;
                if (GameManager.instance.tutorial && firstKevinEvents)
                    rndIndex = 0;
                else
                    rndIndex = Random.Range(0, kevinEvents.Count);
                string phrase = kevinEvents.ElementAt(rndIndex).Value.Item2;
                if (kevinEvents.ElementAt(rndIndex).Key == "Handbook" ||
                    kevinEvents.ElementAt(rndIndex).Key == "Key")
                {
                    left = Random.value < 0.5f;
                    if(left)
                        phrase = kevinEvents.ElementAt(rndIndex).Value.Item2.Replace("@side", "left");
                    else
                        phrase = kevinEvents.ElementAt(rndIndex).Value.Item2.Replace("@side", "right");
                }
                kevinPhraseTextUI.text = phrase;
                kevinImgUI.texture = spriteCorrelation[kevinEvents.ElementAt(rndIndex).Value.Item1];
                kevinEvents.ElementAt(rndIndex).Value.Item3();
                startTime = Time.time;
                randomWaitTime = Random.Range(minTimeSpawnEvent, maxTimeSpawnEvent);

                // tutorial case
			    if (GameManager.instance.tutorial && firstKevinEvents) {
					GameManager.instance.tutorialPopUps[6].SetActive(true);
					firstKevinEvents = false;
				    IsPopUpActive = true;
				    GameManager.instance.PauseTutorial();
                    GameManager.instance.AnyPopUpactive = true;
                }
		    }
        
    }

    IEnumerator SpawnKevin()
    {
        kevinUI.SetActive(true);
        for(int i=0; i<numSecondKevin; i++)
            yield return new WaitForSeconds(1);
        kevinUI.SetActive(false);
    }

    // Disable all non current focus monitors
    private void MonitorEvent()
    {
        SoundManager.Instance.PlayKevinMonitor();
        
        if (!monitorsZoom[1].Zoom)
            lines.SetActive(false);
        for(int i=0; i < monitorsUI.Length; i++)
        {
            if (!monitorsZoom[i].Zoom)
                monitorsUI[i].SetActive(false);
        }
        for (int i = 0; i < powersButton.Length; i++)
        {
            powersButton[i].isPcOn = false;
            powersButton[i].GetComponent<MeshRenderer>().material = powersButton[i].materialsOff;
        }
    }

    // Disable keyboard
    private void KeyboardEvent()
    {
		SoundManager.Instance.PlayKevinKeyboard();
		keyboard.isKeyboardOn = false;
		keyboard.gameObject.GetComponent<Renderer>().material.color = Color.red;
	}

    // activate Siren Alarm
    private void AlarmEvent()
    {
        siren.KevinSirenEvent();
        SoundManager.Instance.PlayKevinSiren();

	}

    // Handbook move to a random drawer
	private void HandbookEvent()
	{
        manual.canDrag = false;
        manual.UnDragging();
        SoundManager.Instance.PlayKevinHandbook();
		if (manualReading.openManual.activeSelf)
            manualReading.SwitchManuals();
        manual.RandomSpawnInDrawers(left);
        if(manualReading.NowReading)
            manualReading.ResetReading();
        manualOpen.ResetInteraction();
	}

	// Key move to a random drawer 
	private void KeyEvent()
	{
		key.canDrag = false;
		key.UnDragging();
		SoundManager.Instance.PlayKevinKey();
		key.RandomSpawnInDrawers(left);
	}

	// Magnetic Card move to a random drawer 
	private void MagneticCardEvent()
	{
		magneticCard.canDrag = false;
		magneticCard.UnDragging();
		SoundManager.Instance.PlayKevinMagneticCard();
		magneticCard.RandomSpawnInDrawers(left);
	}

}
