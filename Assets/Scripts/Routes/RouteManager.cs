using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using System.Collections;
using UnityEngine.Events;
using static Assets.Scripts.Data;
using System;
using Random = UnityEngine.Random;

public class RouteManager : MonoBehaviour
{
	// public var
	public GameObject XCrashPrefab;
	public UnityEvent resetEvents;
	public EventSystem m_EventSystem;
	public Point dotToReach;
	public Transform dotsList;
	public Button resetLineButton;
	public Text flyValueText;
	public LineController[] linesController;
	public Image[] startingPoints;
	public Image[] startingLines;
	public GameObject[] buttonsAirplanes;
	public Texture2D[] spriteAirplanes;
	public Texture2D[] spriteAirplanesCrashes;

    // score var
    [HideInInspector] public int countTotalCrash = 0;
    [HideInInspector] public int countTotalDestinationRight = 0;
    [HideInInspector] public int countTotalDestinationWrong = 0;
    [HideInInspector] public int countTotalAutonomy = 0;
    [HideInInspector] public int countTotalAutonomyUsed = 0;
	public static bool monitorInteractable = false;
	public static float timeBetweenEachMovement;
	
	// private var
	GraphicRaycaster m_Raycaster;
	PointerEventData m_PointerEventData;
	LineController currentLine;
	Dictionary<RouteColor, LineController> lines;
	Point currentPoint = null;
	readonly int maxGroups = 6;
	readonly int maxAirplanes = 6;
	RectTransform rectTransform;
	List<Point> destinations;
	bool gatesOpen = false;
    bool audioOneShot = true;
    bool connectingRoutePhase = true;
	bool isFirstPopUpActive = false;
	bool isSecondPopUpActive = false;

	// new system spawn based on manual
	string flyValue = "";
	List<RouteColor> currentAirships = new();
	List<GameObject> instantiatedPrefabs = new();

	// const var
    const int MULTIPLIERCRASH = -4;
	const int MULTIPLIERDESTINATIONRIGHT = 7;
	const int MULTIPLIERDESTINATIONWRONG = -2;
	const int MULTIPLIERAUTONOMY = 2;

    public bool IsFirstPopUpActive { get => isFirstPopUpActive; set => isFirstPopUpActive = value; }
    public bool IsSecondPopUpActive { get => isSecondPopUpActive; set => isSecondPopUpActive = value; }

    private void Awake()
	{
		Random.InitState((int)DateTime.Now.Ticks);
        lines = new Dictionary<RouteColor, LineController>()
		{
			{RouteColor.RED, linesController[0]},
			{RouteColor.ORANGE, linesController[1]},
			{RouteColor.YELLOW, linesController[2]},
			{RouteColor.SKYBLUE, linesController[3]},
			{RouteColor.BLUE, linesController[4]},
			{RouteColor.VIOLET, linesController[5]}
		};
		destinations = new List<Point>();
		rectTransform = GetComponent<RectTransform>();
        m_Raycaster = GetComponent<GraphicRaycaster>();
        AddGroupsTag();
		NewFlyCode();
	}

	// random generate a new fly code between range
	public void NewFlyCode() {
		flyValue = "00" + Random.Range(1, 5).ToString();
		flyValueText.text = "Fly Value: " + flyValue;
	}

	// add group's tag 
	private void AddGroupsTag()
	{

        int maxRowElements = 7;
        int tagStart = 1;
		int tag;
		// loop through the dotlist children
		for(int i=0; i<dotsList.childCount; i++)
		{
			tag = tagStart;
			Transform child = dotsList.GetChild(i);
			if (i < maxGroups)
			{	// first half part of the dots: above midline
				
				//  add tag uqual to the tagstart for the the first max-i points
				for (int j = maxRowElements - 1-i; j>=0; j--)
					child.GetChild(j).gameObject.tag = "Group" + tag;
				
				//  then add tag decresing each time by one for the remaining points
				for (int j = maxRowElements - i; j < maxRowElements; j++)
				{
					tag--;
					child.GetChild(j).gameObject.tag = "Group" + tag;
				}
				tagStart++;
			}
			else if(i>=maxGroups && i <= maxGroups + 2)
			{
				tagStart = 6;
				tag = tagStart;
				int index = 5;
                //  add tag uqual to the tagstart for the the first max-i points
                for (int j = maxRowElements - 1 - index; j >= 0; j--)
                    child.GetChild(j).gameObject.tag = "Group" + tag;

                //  then add tag decresing each time by one for the remaining points
                for (int j = maxRowElements - index; j < maxRowElements; j++)
                {
                    tag--;
                    child.GetChild(j).gameObject.tag = "Group" + tag;
                }
            }
			else
            {	// second half part of the dots: under midline
                tagStart--;
                tag = tagStart;
                // inverse as before
				for (int j = 0; j < maxRowElements - tagStart + 1; j++)
                    child.GetChild(j).gameObject.tag = "Group" + tag;
                
                for (int j = maxRowElements - tagStart + 1; j < maxRowElements; j++)
                {
                    tag--;
                    child.GetChild(j).gameObject.tag = "Group" + tag;
                }
            }
        }
	}
	
	public void StartRoutes() =>
		StartCoroutine(SetUpRoutes()); 

	IEnumerator SetUpRoutes()
	{
		SpawnDepartures();
		yield return null;
        SpawnDestinations();
		yield return null;
        ComputeAirplanesAutonomy();
        resetLineButton.interactable = true;
		connectingRoutePhase = true;

		// tutorial staff
		if (GameManager.instance.tutorial && GameManager.currentWave == 4) {
			GameManager.instance.tutorialPopUps[7].SetActive(true);
			IsFirstPopUpActive = true;
			GameManager.instance.PauseTutorial();
            GameManager.instance.AnyPopUpactive = true;
        }

		if (GameManager.instance.tutorial && GameManager.currentWave == 5) {
			GameManager.instance.tutorialPopUps[8].SetActive(true);
			IsFirstPopUpActive = false;
			IsSecondPopUpActive = true;
			GameManager.instance.PauseTutorial();
            GameManager.instance.AnyPopUpactive = true;
        }
	}

	// select random airship given maximun numbers available
	private void SelectAirships(int numMax)
	{
		List<RouteColor> availableAirships = new()
		{
			RouteColor.RED,
			RouteColor.ORANGE,
			RouteColor.YELLOW,
			RouteColor.SKYBLUE,
			RouteColor.BLUE,
			RouteColor.VIOLET
		};

		for (int i = 0; i < numMax; i++)
		{
			int selectedIndex = Random.Range(0, availableAirships.Count);
			currentAirships.Add(availableAirships[selectedIndex]);
			availableAirships.RemoveAt(selectedIndex);
		}
	}

    // spawn starting points and available buttons
    private void SpawnDepartures()
    {
        bool first = true;
		int numberSpawnAirship = planeWaves[GameManager.currentWave].Item1;
		SelectAirships(numberSpawnAirship);
		// loop through each route
        for (int i = 0; i < currentAirships.Count; i++)
        {
            // get corresponding index into [0,5] form
            int index = ((int)currentAirships[i]) - 1;
            if (index >= 0 && index <= maxAirplanes)
            {
                linesController[index].active = true;
                linesController[index].canIncrease = true;
                RawImage imageBackButton = buttonsAirplanes[index].GetComponent<RawImage>();
                Button button = buttonsAirplanes[index].GetComponent<Button>();
                if (first)
                {   // select first active line and button
                    currentLine = linesController[index];
                    buttonsAirplanes[index].GetComponent<Outline>().enabled = true;
					startingPoints[index].color = routeColorAssociation[(RouteColor)index + 1];
                    first = false;
                }
				startingPoints[index].GetComponent<Button>().interactable = true;
                startingLines[index].color = routeColorAssociation[(RouteColor)index + 1];
				imageBackButton.color = new Color(1, 1, 1, 1);//routeColorAssociation[(RouteColor)index + 1];
                button.interactable = true;
            }
        }
    }

	// auxiliary function to compute possibilities for destination spawn into correct group
    private int ComputePossibilities(int group)
    {
        if (group >= maxGroups)
            return 1;
        else // (x < 6)
            return 2 + ComputePossibilities(group + 1);
    }
    
	// spawn the ending point based on inspector values
    private void SpawnDestinations()
	{
		for(int i = 0; i<currentAirships.Count; i++)
		{
			int index = ((int)currentAirships[i]) - 1;
			if (index >= 0 && index <= maxAirplanes)
			{
				int currentGroup = airshipsDestination[flyValue][currentAirships[i]];
				int numPossibilities;
				if(currentGroup == maxGroups)
					numPossibilities = 4;
				else
	                numPossibilities = ComputePossibilities(currentGroup);
				// spawn into a random point between all possibilities for that group
				int randomIndex = Random.Range(0, numPossibilities);
				int currentIndex = 0;
				int verse;
				// check if corrisponding starting point is above/under midline to set the verse
				if (linesController[index].points[0].gameObject.transform.position.y < rectTransform.position.y)
					verse = 1;	// spawn above midline
				else
					verse = -1; // spawn under miline

                for (int j = 0; j < dotsList.childCount; j++)
                {
                    Transform hlgChild = dotsList.GetChild(j);
                    foreach (Transform point in hlgChild)
                    {	// check point with same group and with position based on verse
                        if (point.gameObject.CompareTag("Group" + currentGroup) &&
                            point.position.y * verse > rectTransform.position.y * verse)
                        {
                            if (currentIndex == randomIndex)
                            {
                                // spawn
                                Point p = point.GetComponent<Point>();
                                p.pointType = PointType.END;
                                p.color = currentAirships[i];
                                p.image.color = Color.white;
                                destinations.Add(p);
                            }
                            currentIndex++;
                        }
                    }
                }
			}
		
		}
	}

	// compute autonomy for each line
    private void ComputeAirplanesAutonomy()
    {
		// loop through current airships
		for (int i=0; i< currentAirships.Count; i++)
		{
			int randomAdder = Random.Range(1, 4); // add a random value [1,3]
			// compute the y difference (row difference)
			int difference = Mathf.Abs(destinations[i].indexRow - lines[destinations[i].color].points[0].indexRow);
			// if rowDiff < columnDiff then columnDiff else rowDiff
			if (difference <= destinations[i].indexColumn)
                lines[destinations[i].color].maxAutonomy = destinations[i].indexColumn;
			else
                lines[destinations[i].color].maxAutonomy = difference;
            lines[destinations[i].color].maxAutonomy += randomAdder;
			// add to UI
			Text[] texts = buttonsAirplanes[(int)currentAirships[i]-1].GetComponentsInChildren<Text>();
			for (int j=0; j<texts.Length; j++)
				if (texts[j].gameObject.CompareTag("Autonomy"))
                    lines[destinations[i].color].SetUpAutonomy(texts[j]);
		}
    }

	// end tutorial and start game
	public void TutorialEnd()
	{
        GameManager.instance.tutorial = false;
        GameManager.instance.tutorialPopUps[8].SetActive(false);
        GameManager.instance.ResumeGame();
        GameManager.instance.GameEnd();
        GameManager.instance.GameRestart();
        GameManager.instance.startMenu.SetActive(true);
        SoundManager.Instance.sourceBkg.Stop();
        SoundManager.Instance.sourceMenu.Play();
    }

    void Update()
	{
		if (!GameManager.startingMenu && monitorInteractable && connectingRoutePhase)
		{
			//Check if the left Mouse button is clicked
			if (Input.GetKey(KeyCode.Mouse0))
			{
				//Set up the new Pointer Event
				m_PointerEventData = new PointerEventData(m_EventSystem);
				//Set the Pointer Event Position to that of the mouse position
				m_PointerEventData.position = Input.mousePosition;

				//Create a list of Raycast Results
				List<RaycastResult> results = new();

				//Raycast using the Graphics Raycaster and mouse click position
				m_Raycaster.Raycast(m_PointerEventData, results);


				if (results.Count > 0)
				{
					currentPoint = results[0].gameObject.GetComponent<Point>();

					if (currentLine.canIncrease)
					{
						// first check if is the background plane 
						if (results.Count == 1 && results[0].gameObject.CompareTag("Interactable"))
						{
							// remove last point (since add a point on keeping press the mouse button)
							currentLine.ResetLastPoint();
							// get the world space point and add an empty object (since need a transform at least for now)
							dotToReach.transform.position = results[0].worldPosition;
							currentLine.AddPointAround(dotToReach);
						}
						// otherwise check if is a point to connect
						else if (results.Count == 2 && currentPoint != null && currentPoint.pointType != PointType.START && currentPoint.connectable)
						{
							// same as before
							currentLine.ResetLastPoint();
							// this time get directly the object transform to add
							currentLine.CheckBeforAddPoint(currentPoint);
						}
					}

				}
			}
			// adjust saved point at click realise
			else if (Input.GetMouseButtonUp(0) && connectingRoutePhase)
				currentLine.UpdateLineAfterClickRealese();

		}
		
    }

	private void LateUpdate() =>
		currentPoint = null;
	
	// Set correct line at mouse click on corrisponding button
    public void SetLine(int index)
	{
        RouteColor color = (RouteColor)index;
		currentLine = lines[color];
		foreach (LineController line in linesController)
			line.UpdateLineAfterClickRealese();
		for (int i=0; i< startingPoints.Length; i++)
		{
			if (i + 1 == index)
				startingPoints[i].color = routeColorAssociation[color];
			else
			{
				if (linesController[i].points.Count <= 1)
					startingPoints[i].color = standardPointColor;//Color.gray;
            }
		}
	}

	public void ResetCurrentLine() =>
		currentLine.ResetLine();
	

	public void OpenGates()
	{
		if(!gatesOpen)
			StartCoroutine(OpenGatesCoroutine());
	}

	IEnumerator OpenGatesCoroutine()
	{
		// check if all lines reach the end
        // count how many points has the longer line
        int maxPoints = 0;
        bool linesEnd = true;
        for (int i = 0; i < linesController.Length; i++)
        {
			if (linesController[i].active)
			{
				if (linesController[i].points.Last().pointType != Data.PointType.END)
					linesEnd = false;
				if (linesController[i].points.Count > maxPoints)
					maxPoints = linesController[i].points.Count;
			}
        }

        if(linesEnd)
        {
            // if all lines end then open the gates
            gatesOpen = true;
            if (audioOneShot)
            {
                SoundManager.Instance.PlayHellGates();
                audioOneShot = false;
            }
            connectingRoutePhase = false;
            EventsManager.areGatesOpened = true;
			timeBetweenEachMovement = (float)GameManager.routeTime / (float)maxPoints;
            monitorInteractable = false;
            resetLineButton.interactable = false;
            yield return new WaitForSeconds(timeBetweenEachMovement);
            // Loop for each points of each line till the longest one
            for (int indexPoint = 1; indexPoint < maxPoints; indexPoint++)
            {	// loop for each active line
                for (int i = 0; i < linesController.Length; i++)
                {
					// consider only if it has enough points
					if (linesController[i].active && !linesController[i].crash && linesController[i].points.Count > indexPoint)
					{	
						// loop for each consecutive active line
						for (int j = i + 1; j < linesController.Length; j++)
						{	// consider only if it has enough points
							if (linesController[j].active && linesController[j].points.Count > indexPoint)
							{
								// check for line intersection to cause crash
								Point presentPoint1 = linesController[i].points[indexPoint];
								Point oldPoint1 = linesController[i].points[indexPoint-1];
								Point presentPoint2 = linesController[j].points[indexPoint];
								Point oldPoint2 = linesController[j].points[indexPoint-1];
								// check also same position to cause crash
								if ( presentPoint1 == presentPoint2 || 
										(
										oldPoint1.indexColumn == oldPoint2.indexColumn && Mathf.Abs(oldPoint2.indexRow - oldPoint1.indexRow) == 1 &&
										presentPoint2.indexRow == oldPoint1.indexRow && presentPoint1.indexRow == oldPoint2.indexRow &&
										presentPoint1.indexColumn == presentPoint2.indexColumn
										)
									)
								{	// crash
									linesController[i].crash = true;
									linesController[j].crash = true;
									linesController[i].points[indexPoint-1].image.color = standardPointColor;
									linesController[i].points[indexPoint].image.color = standardPointColor;
									linesController[j].points[indexPoint-1].image.color = standardPointColor;
									linesController[j].points[indexPoint].image.color = standardPointColor;
									instantiatedPrefabs.Add(Instantiate(XCrashPrefab, linesController[i].points[indexPoint].transform));
									instantiatedPrefabs.Last().transform.position += Vector3.forward * -0.005f;	
									instantiatedPrefabs.Add(Instantiate(XCrashPrefab, linesController[j].points[indexPoint].transform));
									instantiatedPrefabs.Last().transform.position += Vector3.forward * -0.005f;
									buttonsAirplanes[i].GetComponent<RawImage>().texture = spriteAirplanesCrashes[i];
									buttonsAirplanes[j].GetComponent<RawImage>().texture = spriteAirplanesCrashes[j];
									ResetWaitTimeCheck();
									SoundManager.Instance.PlayAirshipCrash();
								}
							}
						}
						if (!linesController[i].crash)
						{
                            // change color
                            linesController[i].points[indexPoint].image.color = routeColorAssociation[linesController[i].color];
                            // reset precedent color
                            if (linesController[i].points.Count > indexPoint)
                                linesController[i].points[indexPoint - 1].image.color = standardPointColor;
                        }
					}
                }
                yield return new WaitForSeconds(timeBetweenEachMovement);
            }

			ChangeScore();
			EventsManager.areGatesOpened = false;

			// wait and reset route
            yield return new WaitForSeconds(2.5f);
			// increase wave
			GameManager.instance.NextWave();
			ResetRoutes();
			resetEvents.Invoke();
			yield return new WaitForSeconds(2.5f);
			// route restart
			StartRoutes();
			audioOneShot = true;
		}
        yield return null;
	}

	// compute all routes scores and update statistics
	private void ChangeScore()
	{
        int countCrash = 0;
        int countDestinationRight = 0;
        int countDestinationWrong = 0;
        int countAutonomyLeft = 0;
        int countCurrentTotalAutonomy = 0;
		bool oneCrash = false;
		for(int i=0; i < currentAirships.Count; i++)
		{
			LineController line = lines[currentAirships[i]];

			countCurrentTotalAutonomy += line.maxAutonomy;
			if (line.crash && !oneCrash)
			{
				countCrash++;
				oneCrash = true;
			}
			
			if (line.CurrentAutonomy > 0)
				countAutonomyLeft += line.CurrentAutonomy;
			
			if (line.points.Last().color == currentAirships[i])
				countDestinationRight++;
			else
				countDestinationWrong++;
        }

		countTotalCrash += countCrash;
		countTotalDestinationRight += countDestinationRight;
		countTotalDestinationWrong += countDestinationWrong;
		countTotalAutonomy += countCurrentTotalAutonomy;
		countTotalAutonomyUsed += countCurrentTotalAutonomy - countAutonomyLeft;


        int result = countCrash * MULTIPLIERCRASH + countAutonomyLeft* MULTIPLIERAUTONOMY 
					+ countDestinationRight * MULTIPLIERDESTINATIONRIGHT
					+ countDestinationWrong * MULTIPLIERDESTINATIONWRONG;
		GameManager.instance.UpdateScore(result);
		GameManager.instance.UpdateStatistics();
    }

	// check for a crash to reduce waiting time routes
	private void ResetWaitTimeCheck()
	{
		int counter = 0;
		for(int i = 0; i<currentAirships.Count; i++)
		{
			if (lines[currentAirships[i]].crash)
				counter++;
		}
		if (counter == currentAirships.Count)
			timeBetweenEachMovement = 0;
	}


	// reset all reference for routes
	public void ResetRoutes()
	{
		IsFirstPopUpActive = false;
		IsSecondPopUpActive = false;
        GameManager.instance.AnyPopUpactive = false;
        connectingRoutePhase = false;
		gatesOpen = false;
		currentPoint = null;
		currentLine = null;
		audioOneShot = true;
		destinations.Clear();
		for (int i = 0; i < currentAirships.Count; i++)
		{
			int index = ((int)currentAirships[i]) - 1;
			buttonsAirplanes[index].GetComponent<Outline>().enabled = false;
			if (index >= 0 && index <= maxAirplanes)
			{
				// Reset lines
				linesController[index].TotalResetLine();
				linesController[index].active = false;
				linesController[index].crash = false;
				// Reset departures
				startingPoints[index].color = standardPointColor;//routeColorAssociation[RouteColor.NULL];
                startingPoints[index].GetComponent<Button>().interactable = false;
				startingLines[index].color = standardPointColor;// routeColorAssociation[RouteColor.NULL];
                buttonsAirplanes[index].GetComponent<RawImage>().color = routeColorAssociation[RouteColor.NULL];
                buttonsAirplanes[index].GetComponent<RawImage>().texture = spriteAirplanes[index];
                buttonsAirplanes[index].GetComponent<Button>().interactable = false;
            }
				
		}

		for (int i = 0; i < instantiatedPrefabs.Count; i++)
			Destroy(instantiatedPrefabs[i]);

		instantiatedPrefabs.Clear();
		currentAirships.Clear();

		// Reset destinations
		for (int j = 0; j < dotsList.childCount; j++)
		{
			Transform hlgChild = dotsList.GetChild(j);
			foreach (Transform point in hlgChild)
				point.GetComponent<Point>().ResetPoint();
        }

    }


}