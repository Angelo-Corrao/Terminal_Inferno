using UnityEngine;
using UnityEngine.Events;

// This is the button that open the gates after the routes have been traced
public class HellGatesButton : ButtonAnimation
{
	public UnityEvent openGatesEvent;
	
	void Update()
	{
		base.Animation(minScale, maxScale, scalePerUnit, axesToScale);

		if (isPressed)
			openGatesEvent.Invoke();
		
	}

}
