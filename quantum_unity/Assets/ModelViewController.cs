using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelViewController : QuantumCallbacks
{
	override protected void OnEnable()
	{
		base.OnEnable();
		RegisterQuantumEvent();
	}

	public virtual void RegisterQuantumEvent()
	{
		
	}

	override protected void OnDisable()
	{
		base.OnDisable();
		UnRegisterQuantumEvent();
	}
	
	public virtual void UnRegisterQuantumEvent()
	{
		
	}
}
