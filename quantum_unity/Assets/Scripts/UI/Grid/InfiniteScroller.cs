using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class InfiniteScroller : FlexibleGridLayout,IDragHandler,IBeginDragHandler,IEndDragHandler
{
    // ScrollRect仅用于查看进度，滑动逻辑使用IDrag接口重写
    public ScrollRect ScrollRect;
    private  RectTransform _selfRect;
    private float _sidePos;
    private float _halfSidePos;
    [HideInInspector]
    public List<ItemData> ItemDataList = new List<ItemData>();
    [HideInInspector]
    public List<ScrollerItem> _scrollerItemList = new List<ScrollerItem>();
    private int _startDataIndex = 0;
    private int _endDataIndex = 0;
    void Awake()
    {
        base.Awake();
        Initiate();
        InitiateData();
    }

    private void InitiateData()
    {
        for (int i = MidItemIndex; i < rectChildren.Count + MidItemIndex; i++)
        {
            var thisItem = rectChildren[(i - MidItemIndex) % rectChildren.Count].GetComponent<ScrollerItem>();
            _scrollerItemList.Add(thisItem);
            thisItem.ClearData();
            thisItem.SetData(ItemDataList[(_startDataIndex + i - MidItemIndex) % ItemDataList.Count]);
        }
    }

    private void Initiate()
    {
        ScrollRect = ParentRect.GetComponent<ScrollRect>();
        _selfRect = GetComponent<RectTransform>();
        _sidePos = (cellSize.x + spacing.x + Decay.PositionDecay.x);
        _halfSidePos = _sidePos / 2;
        ItemDataList = new List<ItemData>();
        _scrollerItemList = new List<ScrollerItem>();
        for (int i = 0; i < 10; i++)
        {
            ItemData thisData = new ItemData();
            thisData.index = i;
            ItemDataList.Add(thisData);
        }
        _startDataIndex = ItemDataList.Count - MidItemIndex;
        _endDataIndex = MidItemIndex;
    }

    public void GetData(List<ItemData> dataList)
    {
        ItemDataList = dataList;
        for (int i = 0; i < rectChildren.Count; i++)
        {
            var thisItem = rectChildren[i].GetComponent<ScrollerItem>();
            _scrollerItemList.Add(thisItem);
            thisItem.SetData(ItemDataList[i]);
        }
    }
    

    private float lastValueX = 0;
    public override void Update()
    {
        base.Update();
        // 滑动超出判断
        if(!_selfRect) return;
        PosLoop();
    }

    private void PosLoop()
    {
        Vector3 anchoredPosition = _selfRect.anchoredPosition;
        if (anchoredPosition.x >= _sidePos)
        {
            _selfRect.anchoredPosition = new Vector3(0, anchoredPosition.y, anchoredPosition.z);
            ScrollRect.horizontalScrollbar.value = 0.5f;
            UpdateSideItem(false);
        }
        else if (anchoredPosition.x <= -_sidePos)
        {
            _selfRect.anchoredPosition = new Vector3(0, anchoredPosition.y, anchoredPosition.z);
            ScrollRect.horizontalScrollbar.value = 0.5f;
            UpdateSideItem(true);
        }
    }

    private void UpdateSideItem(bool flag)
    {
        ScrollerItem data;
        switch (flag)
        {
            // T左F右
            case true:
                _startDataIndex = (_startDataIndex + 1) % ItemDataList.Count;
                _endDataIndex = (_endDataIndex + 1) % ItemDataList.Count;
                data = _scrollerItemList[0];
                _scrollerItemList.Remove(data); 
                data.SetData(ItemDataList[_endDataIndex]);
                _scrollerItemList.Add(data);
                rectChildren[0].SetAsLastSibling();
                break;
            case false:
                _startDataIndex = (_startDataIndex - 1 + ItemDataList.Count) % ItemDataList.Count;
                _endDataIndex = (_endDataIndex - 1 + ItemDataList.Count) % ItemDataList.Count;
                data = _scrollerItemList.Last();
                _scrollerItemList.Remove(data);
                data.SetData(ItemDataList[_startDataIndex]);
                _scrollerItemList.Insert(0,data);
                rectChildren[rectChildren.Count - 1].SetAsFirstSibling();
                break;
        }
    }

    public float dragThreshold = 0.1f; // 拖动速度阈值，低于此速度不会触发惯性滑动
    public float friction = 0.95f; // 摩擦系数，控制惯性滑动的减速程度
    public float maxTime = 2.0f; // 最大惯性滑动时间
    public float ResetSpeed = 1f; // 位置修正速度

    private Vector2 velocity;
    private bool isDragging;
    private Coroutine inertiaCoroutine;
    public void OnDrag(PointerEventData eventData)
    {
        // 更新速度
        velocity = new Vector2(eventData.delta.x, 0);
        rectTransform.anchoredPosition += velocity;
        isDragging = true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 开始拖动时重置状态
        StopInertia();
        isDragging = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 结束拖动时，如果速度超过阈值则启动惯性滑动
        if (isDragging && velocity.sqrMagnitude > dragThreshold * dragThreshold)
        {
            inertiaCoroutine = StartCoroutine(InertiaSlide());
        }
        isDragging = false;
    }

    private void StopInertia()
    {
        // 停止惯性滑动
        if (inertiaCoroutine != null)
        {
            StopCoroutine(inertiaCoroutine);
            inertiaCoroutine = null;
        }
    }

    private IEnumerator InertiaSlide()
    {
        WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
        float startTime = Time.time;
        while (Time.time - startTime < maxTime && velocity.sqrMagnitude > dragThreshold * dragThreshold)
        {
            // 应用摩擦力减慢速度
            velocity *= friction;
            _selfRect.anchoredPosition += velocity;
            yield return waitForEndOfFrame;
        }
        velocity = Vector2.zero;
        // 停止后自动修正定位
        Vector3 anchoredPosition = _selfRect.anchoredPosition;
        if (Mathf.Abs(anchoredPosition.x) >= _halfSidePos)
        {
            Vector2 finalPos = new Vector2(anchoredPosition.x>0?_sidePos:-_sidePos, _selfRect.anchoredPosition.y);
            while (Mathf.Abs(_selfRect.anchoredPosition.x) >= 0.1f)
            {
                _selfRect.anchoredPosition = Vector2.Lerp(_selfRect.anchoredPosition,finalPos , ResetSpeed);
                yield return waitForEndOfFrame;
            }
        }
        else
        {
            while (Mathf.Abs(_selfRect.anchoredPosition.x) >= 1f)
            {
                _selfRect.anchoredPosition = Vector2.Lerp(_selfRect.anchoredPosition, MidItemPositon, ResetSpeed);
                yield return waitForEndOfFrame;
            }
        }
    }

}
