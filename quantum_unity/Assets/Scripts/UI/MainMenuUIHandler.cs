using ExitGames.Client.Photon;
using JetBrains.Annotations;
using Photon.Realtime;
using Quantum;
using Quantum.Demo;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;
using Button = UnityEngine.UI.Button;

public class MainMenuUIHandler : MonoBehaviour,IConnectionCallbacks,IMatchmakingCallbacks,IOnEventCallback
{
	[Header("Connection Handler")]
	[SerializeField]
	private ConnectionHandler _connectionHandler;
	[SerializeField]
	Canvas _mainMenuCanvas;
	[SerializeField]
	private GameObject _roomMemberPerfab;
	private ObjectPool<GameObject> _roomMemberPool;
	[SerializeField]
	private Transform _flexibleGridTransform;
	[SerializeField]
	private Transform _roomPanel;

	public Button TestConnectBtn;
	public Button TestPlayBtn;
	private QuantumLoadBalancingClient _client;

	private long _mapGuid = 0L;
	private string _playerName = "123";

	private void Awake()
	{
		Initiate();
	}

	void Initiate()
	{
		InitiateBtn();
		InitiatePool();
	}
	void InitiateBtn()
	{
		TestConnectBtn.onClick.AddListener(OnQuickPlayClicked);
		TestPlayBtn.onClick.AddListener(OnStartGameClicked);
	}
	void InitiatePool()
	{
		_roomMemberPool = new ObjectPool<GameObject>(() => { return Instantiate(_roomMemberPerfab, _flexibleGridTransform);});
	}

	private void Update()
	{
		_client?.Service();
	}

	#region UI Logic
	void UpdateRoomDetails()
	{
		ClearRoomMemberList();
		if (!_client.InRoom)
		{
			GameLogger.LogError("Client no longer in room ,cannot update room details");
			return;
		}
		foreach (var player in _client.CurrentRoom.Players)
		{
			GameLogger.Log($"_client.CurrentRoom.Players.Count : {_client.CurrentRoom.Players.Count}");
			var roomMember =_roomMemberPool.Get();
			roomMember.GetComponent<IInitiate<Player>>()?.Initiate(player.Value);
		}
	}

	IEnumerator UpdateRoomDetailsCoroutine()
	{
		while (_roomPanel.gameObject.activeInHierarchy)
		{
			UpdateRoomDetails();
			yield return new WaitForSeconds(0.2f);
		}
	}
	
	private void ClearRoomMemberList()
	{
		for (int i = 0; i < _flexibleGridTransform.transform.childCount; i++)
		{
			_roomMemberPool.Release(_flexibleGridTransform.GetChild(i).gameObject);
		}
	}
	#endregion
	#region Button Event
	private void OnStartGameClicked()
	{
		StartGame();
	}
	private void OnQuickPlayClicked()
	{
		if (_client != null && _client.IsConnected)
		{
			JoinRandomOrCreateRoom();
		}
		else
		{
			ConnectToMaster();
		}
	}
	#endregion
	#region Multiplayer connect and join code
	bool ConnectToMaster()
	{
		var appSettings = PhotonServerSettings.CloneAppSettings((PhotonServerSettings.Instance.AppSettings));
		_client = new QuantumLoadBalancingClient(PhotonServerSettings.Instance.AppSettings.Protocol);
		_client.AddCallbackTarget(this);
		if (string.IsNullOrEmpty(appSettings.AppIdRealtime.Trim()))
		{
			Debug.LogError("AppIdRealtime is not set in the PhotonServerSettings. Please set it to your AppId and try again.");
			return false;
		}
		if (!_client.ConnectUsingSettings(appSettings, _playerName))
		{
			Debug.LogError("Could not connect to master server. Please check your internet connection and try again.");
			return false;
		}
		
		Debug.Log($"Attempting to connect to region {appSettings.FixedRegion}");
		return true;
	}



	EnterRoomParams CreateEnterRoomParams(string roomName)
	{
		EnterRoomParams enterRoomParams = new EnterRoomParams();
		enterRoomParams.RoomOptions = new RoomOptions();
		enterRoomParams.RoomName = roomName;
		enterRoomParams.RoomOptions.IsVisible = true;
		enterRoomParams.RoomOptions.MaxPlayers = 0;
		enterRoomParams.RoomOptions.Plugins = new string[] { "QuantumPlugin" };
		enterRoomParams.RoomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable
		{
			{ "MAP-GUID", _mapGuid },
		};
		enterRoomParams.RoomOptions.PlayerTtl = PhotonServerSettings.Instance.PlayerTtlInSeconds * 1000;
		enterRoomParams.RoomOptions.EmptyRoomTtl = PhotonServerSettings.Instance.EmptyRoomTtlInSeconds * 1000;
		return enterRoomParams;
	}
	
	private void JoinRandomOrCreateRoom()
	{
		_connectionHandler.Client = _client;
		// 向服务端发送Ack包确认
		_connectionHandler.StartFallbackSendAckThread();

		// 获取所有地图
		var allMapsInResources = UnityEngine.Resources.LoadAll<MapAsset>(QuantumEditorSettings.Instance.DatabasePathInResources);
		_mapGuid = allMapsInResources[0].AssetObject.Guid.Value;
		GameLogger.Log($"Using Map long GUID {_mapGuid},GUID {allMapsInResources[0].AssetObject.Guid}");
		
		// 创建房间参数
		EnterRoomParams enterRoomParams = CreateEnterRoomParams(_playerName);
		OpJoinRandomRoomParams joinRandomRoomParams = new OpJoinRandomRoomParams();

		if (!_client.OpJoinRandomOrCreateRoom(joinRandomRoomParams, enterRoomParams))
		{
			GameLogger.LogError("Could not join random room. Please check your internet connection and try again.");
			return;
		}
		GameLogger.Log("Attempting to join or Create random room");
	}

	private void StartGame()
	{
		if (!_client.OpRaiseEvent((byte)PhotonEventCode.StartGame, null,new RaiseEventOptions{Receivers = ReceiverGroup.All},SendOptions.SendReliable))
		{
			GameLogger.LogError("Could not start game. Please check your internet connection and try again.");
			return;
		}
		GameLogger.Log("Start Game");
	}

	/// <summary>
	/// 创建Game
	/// </summary>
	private void StartQuantumGame()
	{
		if (QuantumRunner.Default != null)
		{
			GameLogger.LogWarning($"Another QuantumRunner is running :{QuantumRunner.Default.name}");
			return;
		}
		RuntimeConfig runtimeConfig = new RuntimeConfig();
		runtimeConfig.Map.Id = _mapGuid;
		var param = new QuantumRunner.StartParameters
		{
			RuntimeConfig = runtimeConfig,
			DeterministicConfig = DeterministicSessionConfigAsset.Instance.Config,
			GameMode = Photon.Deterministic.DeterministicGameMode.Multiplayer,
			FrameData = null,
			InitialFrame = 0,
			PlayerCount = _client.CurrentRoom.MaxPlayers,
			LocalPlayerCount = 1,
			RecordingFlags = RecordingFlags.None,
			NetworkClient = _client,
			StartGameTimeoutInSeconds = 10f,
		};
		var clientID = ClientIdProvider.CreateClientId(ClientIdProvider.Type.PhotonUserId, _client);
		GameLogger.Log($"Start QuantumGame:{clientID},map GUID:{_mapGuid},Local Player Count:{param.LocalPlayerCount}");
		_mainMenuCanvas.gameObject.SetActive(false);
		QuantumRunner.StartGame(clientID, param);
	}
	#endregion

	#region Connection Events
	public void OnConnected()
	{
		GameLogger.Log($"OnConnected UserID:{_client.UserId}");
	}

	public void OnConnectedToMaster()
	{
		GameLogger.Log(($"connected to master server in region {_client.CloudRegion}"));
		JoinRandomOrCreateRoom();
	}

	public void OnDisconnected (DisconnectCause cause)
	{
		GameLogger.Log(($"OnDisconnected，Cause {cause}"));
	}

	public void OnRegionListReceived (RegionHandler regionHandler)
	{
		GameLogger.Log(($"OnRegionListReceived，RegionHandler {regionHandler}"));
	}

	public void OnCustomAuthenticationResponse (Dictionary<string, object> data)
	{
		GameLogger.Log($"OnCustomAuthenticationResponse，Data {data}");
		
	}

	public void OnCustomAuthenticationFailed (string debugMessage)
	{
		GameLogger.Log($"OnCustomAuthenticationFailed，DebugMessage {debugMessage}");
	}
	#endregion

	#region Room Events
	public void OnFriendListUpdate (List<FriendInfo> friendList)
	{
		GameLogger.Log($"OnFriendListUpdate");
	}

	public void OnCreatedRoom()
	{
		GameLogger.Log($"OnCreateRoom");
	}

	public void OnCreateRoomFailed (short returnCode, string message)
	{
		GameLogger.LogError($"OnCreateRoomFailed {returnCode}, {message}");
	}

	public void OnJoinedRoom()
	{
		GameLogger.Log($"OnJoinedRoom");
		StartCoroutine(UpdateRoomDetailsCoroutine());
	}

	public void OnJoinRoomFailed (short returnCode, string message)
	{
		GameLogger.LogError($"OnJoinRoomFailed {returnCode}, {message}");
	}

	public void OnJoinRandomFailed (short returnCode, string message)
	{
		GameLogger.LogError($"OnJoinRandomFailed {returnCode}, {message}");
	}

	public void OnLeftRoom()
	{
		GameLogger.Log($"OnLeftRoom");
	}
	#endregion

	#region Photon Events
	public void OnEvent (EventData photonEvent)
	{
		GameLogger.Log($"OnEvent {photonEvent.Code}");
		switch (photonEvent.Code)
		{
			case (byte)PhotonEventCode.StartGame:
				_client.CurrentRoom.CustomProperties.TryGetValue("MAP-GUID", out object mapGuidValue);
				if (mapGuidValue == null)
				{
					GameLogger.LogError("Failed to get map GUID");
					_client.Disconnect();
					return;
				}
				if (_client.LocalPlayer.IsMasterClient)
				{
					_client.CurrentRoom.IsVisible = false;
					_client.CurrentRoom.IsOpen = false;
				}
				StartQuantumGame();
				break;
		}
	}
	#endregion

}
