using Photon.Deterministic;
using Quantum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSpawner : MonoBehaviour
{
    public EntityPrototypeAsset enemyPrototype = null;
    public void CreateWireEntity(string path)
    {
        var prototype = UnityDB.FindAsset<EntityPrototypeAsset>(path);
        Debug.Log("CreateWireEntity:" + prototype.name);
        CommandRespawnEntity command = new CommandRespawnEntity()
        {
            EnemyPrototypeGUID = prototype.Settings.Guid.Value,
            Position = FPVector3.Zero,
        };
        QuantumRunner.Default.Game.SendCommand(command);
        Debug.Log("CreateWireEntity:" + command.Entity.Index);
    }
}
