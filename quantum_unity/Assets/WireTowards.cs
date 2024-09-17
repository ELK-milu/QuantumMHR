using DG.Tweening;
using Photon.Deterministic;
using Quantum;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireTowards : Hovl_Laser
{
    public Vector3 EndPosition = Vector3.forward;
    public Vector3 StartPosition = Vector3.zero;
    public Vector3 NowPosition = Vector3.zero;
    [Header("Dotween设置")]
    public float OpenSpeed = 1f;
    public Ease OpenEase = Ease.InQuad;
    public float CloseSpeed = 1f;
    public Ease CloseEase = Ease.InQuad;
    public static float WireAnimEnd = 0.7f;
    public static float WireUpEnd = 0.3f;

    private Tweener _openTweener;
    private Tweener _closeTweener;
    
    private Vector3 zeroPos = FPVector3.Zero.ToUnityVector3();
    private Quaternion identity = FPQuaternion.Identity.ToUnityQuaternion();
    public Transform EndFlash;
    public Transform StartFlash;
    private Vector3 StartFlashPos;
    public override void Start()
    {
        Laser = GetComponent<LineRenderer>();
        Effects = GetComponentsInChildren<ParticleSystem>();
        Hit = HitEffect.GetComponentsInChildren<ParticleSystem>();
    }
    private bool IsClose = false;
    public void DoOpen()
    {
        /*
        IsClose = false;
        if (_openTweener != null)
        {
            _openTweener.Kill();
            _openTweener = null;
        }
        if(_closeTweener != null)
        {
            _closeTweener.Kill();
            _closeTweener = null;
        }
        HitEffect.transform.position = StartPosition;
        _openTweener = DOTween.To(()=> NowPosition, x=> NowPosition = x, EndPosition, OpenSpeed).SetEase(OpenEase).OnComplete(() =>
        {
            _openTweener = null;
        });
        */
        IsClose = false;
    }


    public void DoClose()
    {
        StartFlashPos = StartFlash.position;
        IsClose = true;
    }

    public void SetEndPosition (Vector3 offset)
    {
        EndPosition = transform.TransformPoint(offset);
        Debug.Log("Updated EndPosition: " + EndPosition);
    }

   public virtual void Update()
    {
        if (IsClose)
        {
            NowPosition = Vector3.Lerp(NowPosition, StartFlash.position, Time.deltaTime * CloseSpeed);
            if (Vector3.Distance(NowPosition, StartFlash.position) <= 0.5f)
            {
                IsClose = false;
                gameObject.SetActive(false);
            }
        } 
        else
        {
            NowPosition = Vector3.Lerp(NowPosition, EndPosition, Time.deltaTime * OpenSpeed);
        }
        
        //transform.localPosition = zeroPos;
        //transform.localRotation = identity;
        Laser.material.SetTextureScale("_MainTex", new Vector2(Length[0], Length[1]));
        Laser.material.SetTextureScale("_Noise", new Vector2(Length[2], Length[3]));
        
        if (Laser != null && UpdateSaver == false)
        {
            Laser.SetPosition(0, transform.position);
            RaycastHit hit;
            Vector3 direction = (transform.InverseTransformPoint(NowPosition) - Vector3.zero);
            if (Physics.Raycast(transform.position, direction, out hit, MaxLength))
            {
                if(hit.transform.gameObject.layer == 2)
                {
                    return;
                }
                //Debug.Log("Raycast hit at: " + hit.point);
                Laser.SetPosition(1, hit.point);
                HitEffect.transform.position = hit.point + hit.normal * HitOffset;
                if (useLaserRotation)
                {
                    HitEffect.transform.rotation = transform.rotation;
                }
                else
                    HitEffect.transform.LookAt(hit.point + hit.normal);
                EndFlash.transform.position = HitEffect.transform.position;

                foreach (var AllPs in Effects)
                {
                    if (!AllPs.isPlaying) AllPs.Play();
                }
                Length[0] = MainTextureLength * (Vector3.Distance(transform.position, hit.point));
                Length[2] = NoiseTextureLength * (Vector3.Distance(transform.position, hit.point));
            }
            else
            {
                //Debug.Log("Raycast did not hit any object.");
                Laser.SetPosition(1, NowPosition);
                HitEffect.transform.position = NowPosition;
                EndFlash.transform.position = HitEffect.transform.position;
                foreach (var AllPs in Hit)
                {
                    if (AllPs.isPlaying) AllPs.Stop();
                }
                Length[0] = MainTextureLength * (Vector3.Distance(transform.position, NowPosition));
                Length[2] = NoiseTextureLength * (Vector3.Distance(transform.position, NowPosition));
            }
            if (Laser.enabled == false && LaserSaver == false)
            {
                LaserSaver = true;
                Laser.enabled = true;
            }
        }
    }
}