using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawerTranslate : MonoBehaviour
{
	const float vel = 2.5f;
	bool isClosed = true;
	bool move = false;
	Vector3 startPos = Vector3.zero;
	float closedPos;
	public float openPos = 0.46f;
	bool audioOneShot = true;


	void Start()
	{
		closedPos = transform.localPosition.z;
		startPos = transform.localPosition;
	}

	void Update()
	{
		if (move)
			Animation();
	}

	public void Animation()
	{
		float axes = transform.localPosition.z;
		Vector3 direction = Vector3.forward;
		

		if(isClosed)
		{
			if (axes < openPos)
			{
				if(audioOneShot)
					SoundManager.Instance.PlayDrawerOpening();
				transform.Translate(direction * Time.deltaTime * vel);
				audioOneShot = false;
			}
			else
			{
				isClosed = false;
				move = false;
				audioOneShot = true;
			}
		}
		else
		{
			if (axes > closedPos)
			{
                if (audioOneShot)
				    SoundManager.Instance.PlayDrawerClosing();
				
                transform.Translate(-direction * Time.deltaTime * vel);
				audioOneShot = false;
			}
			else
			{
                isClosed = true;
				move = false;
				audioOneShot = true;
			}
		}
	}

	public void ResetPosition()
	{
		transform.localPosition = startPos;
        isClosed = true;
    }

	private void OnMouseDown()
	{
		move = true;
	}

	private void OnMouseEnter() {
		GameManager.instance.pointer.sprite = GameManager.instance.iconOver;
	}

	private void OnMouseExit() {
		GameManager.instance.pointer.sprite = GameManager.instance.iconDefault;
	}
}
