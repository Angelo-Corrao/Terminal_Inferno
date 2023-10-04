using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Key : TriggerDependent
{
	public Collider[] keySwitches;
	public UnityEvent<string> pressed;
	public string currentSwitch = "";

	private List<string> inputs= new List<string>() 
	{
		"Red Switch",
		"Green Switch",
		"Blue Switch"
	};

    protected override void Awake()
    {
		// set correct position inside the trigger
		triggersPositionRelation.Add(
            "Desk",
            new Tuple<Vector3, Quaternion>
            (
                new Vector3(-0.29f, 0.035f, -0.04f),
                Quaternion.Euler(180, -90, -90)
            )
        );
        triggersPositionRelation.Add(
            "Drawer",
            new Tuple<Vector3, Quaternion>
            (
                new Vector3(-0.17f, 0, 0),
                Quaternion.Euler(0, 90, 90)
            )
        );
        base.Awake();
		resetLocation = "Desk";
    }


    protected override void OnTriggerStay(Collider other)
	{
        isTriggering = true;
		if (canDrop)
		{
			if(audioOneShot)
			{
				SoundManager.Instance.PlayKey_Grounded();
				audioOneShot = false;
			}
			// check for resolve routes event error
			for (int i = 0; i < keySwitches.Count(); i++) {
				if (other == keySwitches[i]) {
					transform.parent = keySwitches[i].transform;
					transform.localPosition = Vector3.zero;
					transform.localRotation = Quaternion.Euler(0, 0, 0);
					rb.useGravity = false;
					rb.drag = 100;
					rb.angularDrag = 100;

					currentSwitch = other.gameObject.name;
					return;
				}
				else
					currentSwitch = string.Empty;
			}
			// check for put into the trigger
			base.OnTriggerStay(other);		
		}
	}

	public override void RandomSpawnInDrawers(bool left)
	{
		base.RandomSpawnInDrawers(left);
		currentSwitch = string.Empty;
		audioOneShot = true;
	}

}
