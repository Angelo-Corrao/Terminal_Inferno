using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public RouteManager routeManager;
    public EventsManager eventsManager;
    public Ack siren;
    public KevinEvents kevin;
    public ManualReading manualReading;
    public TriggerDependent[] draggableObjects;
    public GameObject[] tutorialPopUps;
    public MonitorZoom[] monitors;
    public Power[] powerButtons;
    public KeyboardPower keyboardPower;
    public CameraMovement cameraMov;
    public Canvas menu;
    public GameObject startMenu;
    public GameObject settingMenu;
    public GameObject pauseMenu;
    public GameObject endMenu;
    public GameObject canvasPointer;
    public GameObject timer_wave;
    public Image pointer;
	public Sprite iconDefault;
	public Sprite iconOver;
	public Sprite iconDragging;
	public Text waveText;
    public Text timerText;
    public Text timerTextUIMonitor;
    public Text[] statisticsText;
    [Tooltip("Game time in seconds")]
    public static int routeTime = 60;
    public static bool zoom = false;
    public GameObject invisibleWall;
    public bool AnyPopUpactive { get => anyPopUpactive; set => anyPopUpactive = value; }
    public Cup cup;
    public static GameManager instance { get; private set; } = null;
    int totalGameTime = 900; // 15 minutes
	float highScore = 0;
	float currentScore = 0;
    public static int currentWave = 1;

    const int STARTDAYTIME = 8;
    const int ENDDAYTIME = 18;

    public static bool gamePause = false;
    public static bool startingMenu = true;
    private bool anyPopUpactive = false;
    bool endingMenu = false;
    public bool tutorial = false;
    bool isFirstPopUp = false;
    int popUpCounter = 1;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

		pointer = canvasPointer.GetComponentInChildren<Image>();
	}

    // set up tutorial
	public void StartTutorial() {
		tutorial = true;
        isFirstPopUp = true;
        popUpCounter = 1;
    }

    // set up game to start
    public void StartGame()
    {
        if (tutorial)
        {
            currentWave = 3;
			siren.enabled = true;
			siren.StartSirenTimer();
			kevin.enabled = true;
			kevin.StartKevin();
		}
        invisibleWall.SetActive(false);
        Time.timeScale = 1;
        if (PlayerPrefs.HasKey("HighScore"))
			highScore = PlayerPrefs.GetFloat("HighScore");
        UpdateStatistics();
		startingMenu = false;
        endingMenu = false;
        timer_wave.SetActive(true);
        cameraMov.enabled = true;
        cameraMov.StartOrientation();
        Cursor.lockState = CursorLockMode.Locked;
		routeManager.StartRoutes();
		eventsManager.ResetAllEvents();
		foreach (MonitorZoom mz in monitors)
        {
            mz.Enable = true;
            mz.Zoom = false;
        }
        StartCoroutine(Timer());
    }

    // increase wave
    public void NextWave()
    {
        if (currentWave < 15)
            currentWave++;
        if(currentWave == 2)
        {
            siren.enabled = true;
            siren.StartSirenTimer();
        }
        else if( currentWave == 3)
        {
            kevin.enabled = true;
            kevin.StartKevin();
        }
    }

    // game timer
    IEnumerator Timer()
    {
        for (int i = 0; i < totalGameTime; i++)
        {
            for(int j=0; j<10; j++)
            {
                float newTimeVal = Data.ReMap(i + j * 0.1f, 0, totalGameTime, STARTDAYTIME, ENDDAYTIME);
                string[] components = string.Format("{0:00.00}",newTimeVal).Split(',');
                float newsecondVal = Data.ReMap(newTimeVal % 1, 0f, 1f, 0f, 0.6f);
                timerText.text = components[0] + ":" + string.Format("{0:0.00}", newsecondVal).Split(',')[1];
                timerTextUIMonitor.text = components[0] + ":" + string.Format("{0:0.00}", newsecondVal).Split(',')[1];
                yield return new WaitForSeconds(0.1f);
            }
        }
        // game end
		if (currentScore > highScore) {
            highScore = currentScore;
            PlayerPrefs.SetFloat("HighScore", currentScore);
        }
        //GameQuit();
        GameEnd();
    }

    // end game by stop all and move camera into monitor statistics
    public void GameEnd()
    {
        // reset focus
        foreach (MonitorZoom mzoom in monitors)
        {
            mzoom.Enable = false;
            mzoom.ZoomOutCam();
            mzoom.firstMainZoom = true;
        }
        if (manualReading.NowReading)
            manualReading.ResetReading();
		// statistics show:
        if (!tutorial && !gamePause) {
            monitors[2].ZoomIN();
            monitors[2].Enable = false;
            endMenu.SetActive(true);
            endingMenu = true;
        }

        foreach (Power pw in powerButtons)
            pw.setMonitorOn();
        keyboardPower.setKeyboardOn();
        menu.gameObject.SetActive(true);
        kevin.enabled = false;
        kevin.kevinUI.SetActive(false);
        kevin.firstKevinEvents = true;
		siren.SirenStop();
        siren.enabled = false;
        siren.firstTimeSiren = true;
        timer_wave.SetActive(false);
        routeManager.StopAllCoroutines();
        routeManager.ResetRoutes();
        eventsManager.ResetAllEvents();
        eventsManager.firstEventSpawn = true;
        tutorial = false;
        if (manualReading.openManual.activeSelf)
            manualReading.SwitchManuals();
        foreach (TriggerDependent td in draggableObjects)
            td.RandomSpawnInDrawers(Random.value < 0.5f);
        foreach (Transform drawer in draggableObjects[0].drawers)
            drawer.gameObject.GetComponent<DrawerTranslate>().ResetPosition();
        cup.transform.position = new Vector3(-1.26f, -2.03f, -3.82f);
        cup.transform.localRotation = Quaternion.identity;
        if (gamePause)
            GameRestart();
    }

    // reset statistics and come back to main menu
    public void GameRestart()
    {
        currentWave = 1;
        currentScore = 0;
        routeManager.countTotalCrash = 0;
        routeManager.countTotalAutonomy = 0;
        routeManager.countTotalAutonomyUsed = 0;
        routeManager.countTotalDestinationRight = 0;
        routeManager.countTotalDestinationWrong = 0;
		kevin.countTotalKevinEvents = 0;
		siren.countDamageSecondSiren = 0;
		Error.totErrorsSolved = 0;
        Error.totErrorsUnsolved = 0;
        UpdateStatistics();
        monitors[1].ZoomOutCam();
		cameraMov.transform.localRotation = Quaternion.Euler(13.3f, 180f, 0);
		cameraMov.enabled = false;
        routeManager.NewFlyCode();
        gamePause = false;
        startingMenu = true;
        endingMenu = false;
        invisibleWall.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        canvasPointer.SetActive(false);
        StopAllCoroutines();
    }

    // update score
    public void UpdateScore(float points)
    {
        if (currentScore + points <= 0)
            currentScore = 0;
        else
            currentScore += points;
	}
    
    // update ui statistics on monitor
    public void UpdateStatistics()
    {
		for (int i=0; i<statisticsText.Length; i++)
        {
            switch (statisticsText[i].gameObject.tag)
            {
                case "Score":
                    statisticsText[i].text = (currentScore*100).ToString();
                    break;
                case "DestinationRight":
                    statisticsText[i].text = routeManager.countTotalDestinationRight.ToString();
                    break;
                case "DestinationWrong":
                    statisticsText[i].text = routeManager.countTotalDestinationWrong.ToString();
                    break;
                case "Crash":
                    statisticsText[i].text = routeManager.countTotalCrash.ToString();
                    break;
                case "AutonomyScore":
                    statisticsText[i].text = routeManager.countTotalAutonomyUsed.ToString() + "/" + routeManager.countTotalAutonomy.ToString();
                    break;
				case "EventsSolved":
					statisticsText[i].text = Error.totErrorsSolved.ToString();
					break;
				case "EventsUnsolved":
					statisticsText[i].text = Error.totErrorsUnsolved.ToString();
					break;
				case "KevinEvents":
					statisticsText[i].text = kevin.countTotalKevinEvents.ToString();
					break;
				case "SirenSeconds":
					statisticsText[i].text = siren.countDamageSecondSiren.ToString();
					break;
				case "BestScore":
                    statisticsText[i].text = (highScore*100).ToString();
                    break;
			}
        }
    }

    // quit game
    public void GameQuit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    
    void PauseGame()
    {
        invisibleWall.SetActive(true);
        gamePause = true;
        Time.timeScale = 0;
        SoundManager.Instance.sourceBkg.Pause();
        SoundManager.Instance.source.Stop();
        SoundManager.Instance.runTimeSource.Stop();
        menu.gameObject.SetActive(true);
        pauseMenu.SetActive(true);
        SoundManager.Instance.sourceMenuPause.Play();
        routeManager.GetComponent<GraphicRaycaster>().enabled = false;
        eventsManager.GetComponent<GraphicRaycaster>().enabled = false;
		kevin.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        canvasPointer.gameObject.SetActive(false);
		foreach (MonitorZoom mz in monitors)
            mz.Enable = false;
        cameraMov.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // change menu after closing settings
    public void SettingsBack()
    {
        if(startingMenu)
            startMenu.SetActive(true);
        else
            pauseMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        if (settingMenu.activeSelf)
            settingMenu.SetActive(false);
        invisibleWall.SetActive(false);
        gamePause = false;
        SoundManager.Instance.sourceMenuPause.Stop();
        SoundManager.Instance.sourceBkg.UnPause();
        Time.timeScale = 1;
        cameraMov.enabled = true;
        pauseMenu.SetActive(false);
        routeManager.GetComponent<GraphicRaycaster>().enabled = true;
        eventsManager.GetComponent<GraphicRaycaster>().enabled = true;
        kevin.gameObject.transform.GetChild(0).gameObject.SetActive(true);
        foreach (MonitorZoom mz in monitors)
            mz.Enable = true;
        if (!zoom && !manualReading.NowReading)
        {
            canvasPointer.SetActive(true);
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !startingMenu && !endingMenu && !AnyPopUpactive)
            if (!gamePause)
                PauseGame();
            else
                ResumeGame();

        // Update UI wave
		string[] wavesTextParts = waveText.text.Split(":");
		waveText.text = wavesTextParts[0] + ": " + currentWave.ToString();
	}

    public void ContinueAfterDrawerPopUP()
    {
        ResumeGame();
        isFirstPopUp = false;
        StartGame();
    }

	public void PauseTutorial() {
        gamePause = true;
        invisibleWall.SetActive(true);
        SoundManager.Instance.sourceBkg.Pause();
        SoundManager.Instance.sourceMenu.Stop();
        SoundManager.Instance.sourceMenuPause.Play();
        Time.timeScale = 0;
		routeManager.GetComponent<GraphicRaycaster>().enabled = false;
		eventsManager.GetComponent<GraphicRaycaster>().enabled = false;
		kevin.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        canvasPointer.gameObject.SetActive(false);
        foreach (MonitorZoom mz in monitors)
			mz.Enable = false;
		cameraMov.enabled = false;
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}
}
