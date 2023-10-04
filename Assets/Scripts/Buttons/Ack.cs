using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Ack : ButtonAnimation
{
    public Light siren;
    public bool isSirenOn = false;
    public int rMin = 85;
    public int rMax = 100;
    private float timeToStart=0;
    private float startTime;
    private Animation sirenAnimation;
    private int sirenScoreCounter = 0;
    private const int SIRENSCOREMULTIPLIWER = -1;
    [HideInInspector] public bool firstTimeSiren = true;
    [HideInInspector] public int countDamageSecondSiren = 0;
	bool isPopUpActive = false;

    public bool IsPopUpActive { get => isPopUpActive; set => isPopUpActive = value; }

    private void Awake()
    {
        //Get the animation and  set the mode to Loop
        sirenAnimation = siren.GetComponent<Animation>();
        sirenAnimation.wrapMode = WrapMode.Loop;
    }

    public void StartSirenTimer()
    {
        //Range in which to start the animation
        timeToStart = Random.Range(rMin, rMax);
        startTime = Time.time;
    }

    public void SirenStop()
    {
        //Stop the animation and forcing color to black because animation doesn't reset from start
        sirenAnimation.Stop();
        siren.color = Color.black;
        isSirenOn = false;
        sirenScoreCounter = 0;
        SoundManager.Instance.runTimeSource.Stop();
		GameManager.instance.UpdateScore(countDamageSecondSiren * SIRENSCOREMULTIPLIWER);
		GameManager.instance.UpdateStatistics();
	}

    void Update()
    {
		base.Animation(minScale, maxScale, scalePerUnit, axesToScale);

        Siren();

        if (isPressed)
        {
            if (isNegativeAnimation)
            {
                if (transform.localScale.y >= maxScale)
                {
                    if (isSirenOn)
                    {
                        SirenStop();
                        sirenScoreCounter = 0;
						StartSirenTimer();
                        countDamageSecondSiren = 0;
                    }

                    SoundManager.Instance.PlayButtonPress();
                }
            }
        }
        
    }

    // After the game is started the siren will play after a random time between a min and max values
    void Siren()
    {
        if (!GameManager.startingMenu) {
            if (Time.time > startTime + timeToStart)
            {
                if (!isSirenOn)
                {
                    sirenAnimation.Play();
                    isSirenOn = true;
                    StartCoroutine(StartScoreCounter());
                    //Set a new time for next siren
                    timeToStart = Random.Range(rMin, rMax);
                    SoundManager.Instance.PlaySirenLoop();
                    // In the tutorial if this is the first time siren spawns the relative pop-up will be activated
                    if (GameManager.instance.tutorial && firstTimeSiren) {
						GameManager.instance.tutorialPopUps[5].SetActive(true);
						firstTimeSiren = false;
						IsPopUpActive = true;
						GameManager.instance.PauseTutorial();
                        GameManager.instance.AnyPopUpactive = true;
                    }
                }
            }
        }
    }

    IEnumerator StartScoreCounter()
    {
        while (isSirenOn)
        {
            yield return new WaitForSeconds(1);
            sirenScoreCounter++;
            // After 5 seconds the siren will give a negative score to the player each second
			if (sirenScoreCounter > 5)
				countDamageSecondSiren++;
			
		}
    }

    public void KevinSirenEvent()
    {
        startTime = -timeToStart;
    }
}