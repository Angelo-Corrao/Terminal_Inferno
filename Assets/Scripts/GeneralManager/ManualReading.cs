using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManualReading : MonoBehaviour
{
    private int currentPageIndex = 0;
    private bool reading = false;
    public Canvas pointer;
    public CameraMovement camMov;
    public GameObject leftArrow;
    public GameObject rigthArrow;
    public GameObject rootPages;
    public GameObject[] pages;
    public MonitorZoom monitorZoom;
    public GameObject openManual;
    public GameObject closedManual;

    public bool NowReading { get => reading; }

    // switch between manual models: open-close
    public void SwitchManuals()
    {
        if (closedManual.activeSelf)
        {
            closedManual.SetActive(false);
            openManual.SetActive(true);
        }
        else
        {
            closedManual.SetActive(true);
            openManual.SetActive(false);
        }
    }

    // open manual UI
    public void Reading()
    {
        reading = true;
        monitorZoom.Enable = false;
        pointer.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        camMov.locked = true;
        rootPages.SetActive(true);
    }

    // change page after clicking an arrow
    public void MovePage(bool next)
    {
        pages[currentPageIndex].SetActive(false);
        if(next)
            currentPageIndex++;
        else
            currentPageIndex--;
        pages[currentPageIndex].SetActive(true);
    }

    private void Update()
    {
        // check page index to show arrow correctly
        if(currentPageIndex == 0)
        {
            leftArrow.SetActive(false);
            rigthArrow.SetActive(true);
        }
        else if(currentPageIndex == pages.Length - 1)
        {
            leftArrow.SetActive(true);
            rigthArrow.SetActive(false);
        }
        else
        {
            leftArrow.SetActive(true);
            rigthArrow.SetActive(true);
        }

        // check for reset UI
        if (Input.GetKeyDown(KeyCode.Mouse1) && reading && !GameManager.gamePause)
        {
            ResetReading();
        }
    }

    // reset manual UI
    public void ResetReading()
    {
        pointer.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        camMov.locked = false;
        rootPages.SetActive(false);
        monitorZoom.Enable = true;
        reading = false;
    }

}
