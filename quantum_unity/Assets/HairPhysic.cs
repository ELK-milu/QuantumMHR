using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HairPhysic : MonoBehaviour
{
    public Vector3 RiginPos;
    public Rigidbody RB;

    private void OnTriggerStay (Collider other)
    {
        RB.isKinematic = false;
    }

    private void OnTriggerExit (Collider other)
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, RiginPos, 1f);
        RB.isKinematic = true;
    }

}
