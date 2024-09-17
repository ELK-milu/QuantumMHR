using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomMember : MonoBehaviour,IInitiate<Player>
{

	[SerializeField]
	private TextMeshProUGUI NameText;
	public void Initiate()
	{
		
	}

	public void Initiate (Player t)
	{
		NameText.text = t.NickName;
	}
}
