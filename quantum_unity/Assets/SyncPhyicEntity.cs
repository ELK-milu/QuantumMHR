using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncPhyicEntity : MonoBehaviour
{
	private Transform _bindEntity;
	public bool IsMaster = false;
	public void Initiate(Transform transform)
	{
		_bindEntity = transform;
	}

	public void Update()
	{
		if (!IsMaster)
		{
			transform.position = _bindEntity.transform.position;
			transform.rotation = _bindEntity.transform.rotation;
		}
	}
}
