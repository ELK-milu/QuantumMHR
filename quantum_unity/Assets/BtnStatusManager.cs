using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class BtnStatusManager : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    private Button _btn;
    private Tweener _animationTweener;

    private void Awake()
    {
        _btn = GetComponent<Button>();
    }
    
    public void OnPointerEnter (PointerEventData eventData)
    {
        if(!_btn.interactable) return;
        _animationTweener = _btn.image.DOFade(0f, 1f).SetEase(Ease.Linear).SetLoops(-1,LoopType.Yoyo);
    }

    public void OnPointerExit (PointerEventData eventData)
    {
        _animationTweener.Kill();
        _btn.image.DOFade(1f, 0.02f);
    }
}
