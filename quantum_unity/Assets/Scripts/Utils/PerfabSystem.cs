using Photon.Deterministic;
using Quantum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameObjectDic
{
	public static Dictionary<EntityRef,GameObject> EntityRefGameObjectDic = new Dictionary<EntityRef,GameObject>();
}
public class PerfabSystem : MonoBehaviour
{
	public static void CreateWireEntity(string path, FPVector3 position,FPQuaternion rotation,FPVector3 destination,Transform parent, out EntityRef entityRef)
	{
		var prototype = UnityDB.FindAsset<EntityPrototypeAsset>(path + "|EntityPrototype");
		CommandRespawnEntity command = new CommandRespawnEntity()
		{
			EnemyPrototypeGUID = prototype.Settings.Guid.Value,
			Position = position,
		};
		QuantumRunner.Default.Game.SendCommand(command);
		entityRef = command.Entity;
	}
	
	public static void DestroyEntity(EntityRef entity)
	{
		
	}
	
	public static void DestroyPerfab(GameObject obj)
	{
		Destroy(obj);
	}
}
