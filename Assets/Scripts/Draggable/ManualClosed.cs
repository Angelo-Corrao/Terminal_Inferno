using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ManualClosed : TriggerDependent
{
    public UnityEvent poseManualEvent;


    protected override void Awake()
    {
        // set correct position inside the trigger 
        triggersPositionRelation.Add(
            "Desk",
            new Tuple<Vector3, Quaternion>
            (
                Vector3.zero,
                Quaternion.Euler(0, 0, 180)
            )
        );
        triggersPositionRelation.Add(
            "Drawer",
            new Tuple<Vector3, Quaternion>
            (
                Vector3.zero,
                Quaternion.Euler(0, 0, 180)
            )
        );
        base.Awake();
        resetLocation = "Drawer";
	}

    protected override void OnTriggerStay(Collider other)
    {
        isTriggering = true;
        if (canDrop)
        {   // chek for put in the trigger
            base.OnTriggerStay(other);
            if (other.CompareTag("DeskTrigger"))
            {
                if (audioOneShot)
                {
				    SoundManager.Instance.PlayManualOpening();
                    audioOneShot = false;
                }
				poseManualEvent.Invoke();
            }
        }
    }

}
