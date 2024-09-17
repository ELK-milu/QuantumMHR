using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothRootMotion : MonoBehaviour
{
    private Animator animator;
    private Vector3 lastPosition;
    private Quaternion lastRotation;

    void Awake()
    {
        animator = GetComponent<Animator>();
        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }

    void OnAnimatorMove()
    {
        // Apply root motion manually
        Vector3 deltaPosition = animator.deltaPosition;
        Quaternion deltaRotation = animator.deltaRotation;

        // 手动应用根运动
        transform.position += deltaPosition;
        transform.rotation *= deltaRotation;

        // 更新最后的位置和旋转
        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }
}
