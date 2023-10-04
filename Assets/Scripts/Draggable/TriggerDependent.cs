using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

// class to make objects depend from some triggers
public class TriggerDependent : Draggable
{
    // public var
    public Transform deskTrigger;
    public Transform[] drawers;
    public bool isCup = false;
    // protected var
    protected bool isTriggering = true;
    protected Dictionary<string, Tuple<Vector3, Quaternion>> triggersPositionRelation = new();
    protected string resetLocation;
    protected bool audioOneShot = true;
    // private var
    int waitBeforeReset = 3;
    bool toCheck = true;

    protected override void Awake()
    {
        base.Awake();
        RandomSpawnInDrawers(Random.value < 0.5f);
    }

    // spawn the object into one random drawer 
    public virtual void RandomSpawnInDrawers(bool left)
    {
		audioOneShot = true;
		isTriggering = true;
        // Spawn manual to one random drawer
        int randomIndex;
        if (left)
            randomIndex = Random.Range(0, drawers.Length / 2);
        else
            randomIndex = Random.Range(drawers.Length / 2, drawers.Length);
        transform.SetParent(drawers[randomIndex]);
        SetTransform("Drawer");
    }

    protected virtual void SetTransform(string trigger)
    {
        transform.localPosition = triggersPositionRelation[trigger].Item1;
        transform.localRotation = triggersPositionRelation[trigger].Item2;
    }

    protected virtual void OnTriggerStay(Collider other)
    {   // set the object in the correct position inside the trigger
        if (other.gameObject.layer == 2) {
            transform.SetParent(other.gameObject.transform);
            if (other.gameObject.CompareTag("DeskTrigger"))
                SetTransform("Desk");
            else
                SetTransform("Drawer");
            canDrop = false;
        }
        else
            isTriggering = false;
    }

    private void OnTriggerExit(Collider other)
    {
        isTriggering = false;
        audioOneShot = true;
    }

    protected override void Update()
    {
        base.Update();
        if(!isTriggering && !isDragging && toCheck && !isCup)
        {
            toCheck = false;
            StartCoroutine(CheckTimerForReset());
        }
    }

    // check if an object is not in the correct position
    IEnumerator CheckTimerForReset()
    {
        int i = 0;
        for (; i < waitBeforeReset && !isTriggering && !isDragging; i++)
            yield return new WaitForSeconds(1);
        if (i == waitBeforeReset && !isDragging && !isTriggering)
            ResetObjectInTrigger();
        yield return null;
        toCheck = true;
    }

    // reset object after check is outside correct position
    void ResetObjectInTrigger()
    {
        if(resetLocation == "Desk")
        {
            transform.SetParent(deskTrigger);
            SetTransform(resetLocation);
        }
        else
            RandomSpawnInDrawers(Random.value < 0.5f);
    }

}
