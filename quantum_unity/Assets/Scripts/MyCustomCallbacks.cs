using Quantum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCustomCallbacks : QuantumCallbacks
{
	public RuntimePlayer _runtimePlayer;

	public override void OnGameStart (QuantumGame game)
	{
		if(game.Session.IsPaused) return;

		foreach (var localPlayer in game.GetLocalPlayers())
		{
			GameLogger.Log("CustomCallbacks - sending player:" + localPlayer);
			game.SendPlayerData(localPlayer,_runtimePlayer);
		}
	}

	public override void OnGameResync (QuantumGame game)
	{
		GameLogger.Log("Detected Resync,Verified tick :" + game.Frames.Verified.Number);
	}
}
