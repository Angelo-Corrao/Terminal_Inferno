using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    public float sensitivity;
    Camera cam;
    Vector2 turn;
    public bool isDragging = false;
    public bool locked = false;
    [Range(0f, 180f)]
    public int xP = 90;
    [Range(0f, -180f)]
    public int xN = -90;
    [Range(0f, 180f)]
    public int yP = 90;
    [Range(0f, -180f)]
    public int yN = -90;


    private void Awake()
    {
        cam = GetComponent<Camera>();
    }
    
    public void StartOrientation() =>
        turn = new Vector2(180f, -13.3f);

    void Update()
    {

        if (!locked)
        {   // rotate the camera based on mouse movement
            turn.y += Input.GetAxis("Mouse Y") * sensitivity;
            turn.y = Mathf.Clamp(turn.y,yN,yP);
            if(!isDragging)
            {
                turn.x += Input.GetAxis("Mouse X") * sensitivity;
            }
            transform.localRotation = Quaternion.Euler(-turn.y, turn.x, 0);
        }
	}
}
