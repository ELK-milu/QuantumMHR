using System;
using Photon.Deterministic;
using Quantum;
using UnityEngine;

public class LocalInput : MonoBehaviour {
  
	public InputReader ThisInputReader;
    
	private bool _isScroll = false;
	private bool _isDash = false;
	private Vector2 _inputDirection = Vector2.zero;
	
	private bool _isWireCombine = false;
	private bool _isWireUpCombine = false;
	private bool _isWireDownCombine = false;
	private bool _isWireLeftCombine = false;
	private bool _isWireRightCombine = false;
	private bool _isWireForwardCombine = false;

	
	private bool _isTEST = false;

	private void OnEnable() {
		QuantumCallback.Subscribe(this, (CallbackPollInput callback) => PollInput(callback));
		ThisInputReader.OnMoveHandler += GetInputDirection;
		ThisInputReader.OnScrollHandler += GetScroll;
		ThisInputReader.OnDashHandler += GetDash;
		
		ThisInputReader.OnWireCombineHandler += GetWireCombine;
		ThisInputReader.OnWireUpHandler += GetWireUp;
		ThisInputReader.OnWireDownHandler += GetWireDown;
		ThisInputReader.OnWireLeftHandler += GetWireLeft;
		ThisInputReader.OnWireRightHandler += GetWireRight;
		ThisInputReader.OnWireForwardHandler += GetWireForward;


		ThisInputReader.OnTESTCombineHandler += GetTEST;
	}

	private void GetTEST (bool obj)
	{
		_isTEST = obj;
	}
	private void GetWireForward (bool flag)
	{
		_isWireForwardCombine = flag;
	}
	private void GetWireRight (bool flag)
	{
		_isWireRightCombine = flag;
	}

	private void GetWireLeft (bool flag)
	{
		_isWireLeftCombine = flag;
	}

	private void GetWireDown (bool flag)
	{
		_isWireDownCombine = flag;
	}

	private void GetWireUp (bool flag)
	{
		_isWireUpCombine = flag;
	}

	private void GetWireCombine (bool flag)
	{
		_isWireCombine = flag;
	}

	private void GetScroll(bool flag)
	{
		_isScroll = flag;
	}
	private void GetDash(bool flag)
	{
		_isDash = flag;
	}
	
	private void GetInputDirection(Vector2 inputVec)
	{
		_inputDirection = inputVec;
	}
	private void OnDisable()
	{
		ThisInputReader.OnScrollHandler -= GetScroll;
		ThisInputReader.OnMoveHandler -= GetInputDirection;
		ThisInputReader.OnDashHandler -= GetDash;
		
		ThisInputReader.OnWireCombineHandler -= GetWireCombine;
		ThisInputReader.OnWireUpHandler -= GetWireUp;
		ThisInputReader.OnWireDownHandler -= GetWireDown;
		ThisInputReader.OnWireLeftHandler -= GetWireLeft;
		ThisInputReader.OnWireRightHandler -= GetWireRight;
		ThisInputReader.OnWireForwardHandler -= GetWireForward;
		
		ThisInputReader.OnTESTCombineHandler -= GetTEST;
	}
	
	public void PollInput(CallbackPollInput callback) {
		Quantum.Input i = new Quantum.Input(); 
		
		i.Scroll = _isScroll;
		i.Dash = _isDash;
		i.DirctionX = (short)(_inputDirection.x * 10);
		i.DirctionY = (short)(_inputDirection.y * 10);
	 	
		i.WireCombine = _isWireCombine;
		i.WireUp = _isWireUpCombine;
		i.WireLeft = _isWireLeftCombine;
		i.WireDown = _isWireDownCombine;
		i.WireRight = _isWireRightCombine;
		i.WireForward = _isWireForwardCombine;


		i.TEST = _isTEST;
		
		callback.SetInput(i,DeterministicInputFlags.Repeatable);
	}

}
