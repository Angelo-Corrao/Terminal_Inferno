using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

[DisallowMultipleComponent]
[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public AudioMixer mixer;
    public AudioSource source;
    public AudioSource runTimeSource;
    public AudioSource sourceMenu;
    public AudioSource sourceMenuPause;
    public AudioSource sourceBkg;
    public AudioClip ManualOpening;
    public AudioClip ManualFlipping;
    public AudioClip ButtonPress;
    public AudioClip SliderPosition;
    public AudioClip HellGatesOpen;
    public AudioClip HellGatesSiren;
    public AudioClip ClickUI;
    public AudioClip Siren;
    public AudioClip DrawerOpening;
    public AudioClip DrawerClosing;
    public AudioClip EventWarning;
    public AudioClip On_Off;
    public AudioClip Magnetic_Card_Grounded;
    public AudioClip Key_Grounded;
    public AudioClip Object_Picking;
    public AudioClip Lever1;
    public AudioClip Lever2;
    public AudioClip AirshipCrash;
    public AudioClip CardReaderConfirm;
    public AudioClip KevinSiren;
    public AudioClip KevinHandbook;
    public AudioClip KevinKey;
    public AudioClip KevinKeyboard;
    public AudioClip KevinMagneticCard;
    public AudioClip KevinMonitor;

    private static SoundManager _instance = null;

    public static SoundManager Instance { get => _instance; }

    void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        sourceMenu.volume = 0.4f;
        sourceBkg.volume = 0.3f;
    }

    void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }

    // play correct audio
    #region Audios
    public void PlayMenuSoundtrack() =>
        sourceMenu.Play();
    public void PlaySoundtrack() =>
        sourceBkg.Play();

    public void PlayButtonPress() => source.PlayOneShot(ButtonPress);
    public void PlaySliderPosition() => source.PlayOneShot(SliderPosition);
    public void PlayHellGates()
    {
        source.PlayOneShot(HellGatesOpen);
        source.PlayOneShot(HellGatesSiren, 0.7f);
    }
    public void PlayHellGatesOpen() => source.PlayOneShot(HellGatesOpen);
    public void PlayHellGatesSiren() => source.PlayOneShot(HellGatesSiren);
    public void PlayClickUI() => source.PlayOneShot(ClickUI);
    public void PlaySiren() => source.PlayOneShot(Siren);
    public void PlaySirenLoop()
    {
        runTimeSource.clip = Siren;
        runTimeSource.Play();
    }
    public void PlayDrawerOpening() => source.PlayOneShot(DrawerOpening);
    public void PlayDrawerClosing(){
        source.clip = DrawerClosing;
        source.PlayDelayed(0.5f);
    }//source.PlayOneShot(DrawerClosing);
    public void PlayEvent() => source.PlayOneShot(EventWarning);
    public void PlayOn_Off() => source.PlayOneShot(On_Off);
    public void PlayMagnetic_Card_Grounded() => source.PlayOneShot(Magnetic_Card_Grounded);
    public void PlayKey_Grounded() => source.PlayOneShot(Key_Grounded);

    public void PlayObejctPicking() => source.PlayOneShot(Object_Picking);
	public void PlayKevinSiren() => source.PlayOneShot(KevinSiren);
	public void PlayKevinHandbook() => source.PlayOneShot(KevinHandbook);
	public void PlayKevinKey() => source.PlayOneShot(KevinKey);
	public void PlayKevinKeyboard() => source.PlayOneShot(KevinKeyboard);
	public void PlayKevinMagneticCard() => source.PlayOneShot(KevinMagneticCard);
	public void PlayKevinMonitor() => source.PlayOneShot(KevinMonitor);
	public void PlayLever1() => source.PlayOneShot(Lever1);
	public void PlayLever2() => source.PlayOneShot(Lever2);
	public void PlayManualOpening() => source.PlayOneShot(ManualOpening);
	public void PlayManualFlip() => source.PlayOneShot(ManualFlipping);
	public void PlayAirshipCrash() => source.PlayOneShot(AirshipCrash);
	public void PlayCardReaderConfirm() => source.PlayOneShot(CardReaderConfirm);

    #endregion

    private void Start()
    {
        PlayMenuSoundtrack();
    }

    private void Update()
    {
        if(!source.isPlaying)
            source.clip = null;
    }

}