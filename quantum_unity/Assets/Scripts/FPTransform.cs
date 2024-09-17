using Photon.Deterministic;
using UnityEngine;

public static class FPTransform
{
	public static FPVector3 TOPosition(this Transform transform)
	{
		var position = transform.position;
		var trans = new FPVector3(FP.FromFloat_UNSAFE(position.x), FP.FromFloat_UNSAFE(position.y), FP.FromFloat_UNSAFE(position.z));
		return trans;
	}
	
	public static FPQuaternion TORotation(this Transform transform)
	{
		var quaternion = transform.rotation;
		var rotations = FPQuaternion.Euler(FP.FromFloat_UNSAFE(quaternion.x), FP.FromFloat_UNSAFE(quaternion.y), FP.FromFloat_UNSAFE(quaternion.z)); 
		return rotations;
	}
}
