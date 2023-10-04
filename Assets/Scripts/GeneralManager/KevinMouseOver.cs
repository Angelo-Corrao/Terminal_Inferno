using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KevinMouseOver : MonoBehaviour
{
    public EventSystem m_EventSystem;
    public RawImage img;
    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;

    private void Awake()
    {
        m_Raycaster = GetComponent<GraphicRaycaster>();
    }

    private void Update()
    {
        //Set up the new Pointer Event
        m_PointerEventData = new PointerEventData(m_EventSystem);
        //Set the Pointer Event Position to that of the mouse position
        m_PointerEventData.position = Input.mousePosition;

        //Create a list of Raycast Results
        List<RaycastResult> results = new();

        //Raycast using the Graphics Raycaster and mouse click position
        m_Raycaster.Raycast(m_PointerEventData, results);

        // make transparent kevin on over
        if (results.Count > 0)
        {
            if(results[0].gameObject.CompareTag("Kevin"))
                img.color = new Color(1f, 1f, 1f, 120f / 255f);
            else
                img.color = new Color(1, 1, 1, 1);
        }
        else
            img.color = new Color(1, 1, 1, 1);
    }
}
