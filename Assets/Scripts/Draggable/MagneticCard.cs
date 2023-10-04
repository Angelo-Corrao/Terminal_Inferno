using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MagneticCard : TriggerDependent
{
	public Collider cardReader;
	public Error01 error01;
	public Error02 error02;
	public Error03 error03;
	public bool isNedeed = false;
	public UnityEvent<string> pressed;

    protected override void Awake()
    {
		// set correct position inside the trigger
		triggersPositionRelation.Add(
            "Desk",
            new Tuple<Vector3, Quaternion>
            (
                new Vector3(0.3f, 0.028f, -0.01f),
                Quaternion.Euler(-90f, 0, -90f)
            )
        );
        triggersPositionRelation.Add(
            "Drawer",
            new Tuple<Vector3, Quaternion>
            (
                new Vector3(0.2f,0,0),
                Quaternion.Euler(-90, 90, 0)
            )
        );
        base.Awake();
        resetLocation = "Desk";
    }

    protected override void OnTriggerStay(Collider other) {
        isTriggering = true;

		// check for resolve reoutes event error
		if (isNedeed) {
			if (other == cardReader) {
				switch (Error.errorSelected) {
					case 1:
						error01.isMagneticCardNedeed = false;
						break;

					case 2:
						error02.isMagneticCardNedeed = false;
						break;

					case 3:
						error03.isMagneticCardNedeed = false;
						break;
				}
				pressed.Invoke(Error.lastInput);
				isNedeed = false;
				SoundManager.Instance.PlayCardReaderConfirm();
			}
		}
		if (canDrop)
		{	// check for put in the trigger
			if(audioOneShot)
			{
				SoundManager.Instance.PlayMagnetic_Card_Grounded();
				audioOneShot = false;
			}

			base.OnTriggerStay(other);
		}
	}

}
