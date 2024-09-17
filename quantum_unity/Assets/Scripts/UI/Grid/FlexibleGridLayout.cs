/* FlexibleGridLayout.cs
* From: Game Dev Guide - Fixing Grid Layouts in Unity With a Flexible Grid Component
* Created: June 2020, NowWeWake
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.EventSystems;

public class  FlexibleGridLayout : LayoutGroup
{
    public enum FitType
    {
        Uniform,
        Width,
        Height,
        FixedRows,
        FixedColumns,
    }
    public RectTransform ParentRect;
    [Header("Flexible Grid,FixedColumns代表按列展开,FixedRows代表按行展开")]
    public FitType fitType = FitType.Uniform;

    public int rows;
    public int columns;
    public int Pages;
    public Vector2 cellSize;
    public Vector3 spacing;
    public bool fitX;
    public bool fitY;
    [Header("父实体Rect自适应")]
    public ParentFitProperties ParentAdaption;
    [Header("生成预制体选项，可为空")]
    public GameObject Prefab;
    public int TotalNum = 0;
    [Header("设置中心元素序号")]
    public int MidItemIndex;
    public bool IsDecay;
    [Header("边际衰减")]
    [Tooltip("以中间元素为核心，向两边以相同值逐渐衰减")]
    public PropertiesDecay Decay;
    protected int _itemCount = 0;
    protected Vector3 MidItemPositon = Vector3.back;
    protected Vector3 LSidePosition = Vector3.back;  
    protected Vector3 RSidePosition = Vector3.back;  

    protected Quaternion SideRotation = Quaternion.identity;    
    protected Vector3 SideScale = Vector3.back;  
    protected Quaternion Quaternion_ZERO = Quaternion.Euler(0,0,0);


    [System.Serializable]
    public class ParentFitProperties
    {
        [Header("开启自适应")]
        public bool ParentFit;
        [Header("按照页数自适应,仅在Fixed模式下生效")]
        public bool PageFit;
        [Header("每页行(列)数")]
        [Tooltip("这个数字决定了网格的子实体的增加模式,仅在Fixed模式下生效," +
                 "\n它代表了在与设定Fixed每增加多少个子实体进行一次换行(列)" +
                 "\n当为Fixed Columns且Columns=N,则每Column增加NumPerPage个子实体后会切换到下一Column" +
                 "\n当为Fixed Rows且Rows=N,则每Row增加NumPerPage个子实体后会切换到下一Row" +
                 "\n直到增加完一个N行NumPerPage列为止")]
        public int NumPerPage = 1;
    }
    [Serializable]
    public class PropertiesDecay
    {
        public Vector3 ScaleDecay;
        public Vector3 PositionDecay;
        public Vector3 RotationDecay;
    }
    override protected void Awake()
    {
        base.Awake();
        if (!ParentRect)
        {
            ParentRect = GetComponent<RectTransform>();
        }
        if (Prefab)
        {
            MidItemIndex = TotalNum / 2;
        }
    }

    override protected void Start()
    {
        base.Start();
        StartCoroutine(DelayInit());
    }

    IEnumerator DelayInit()
    {
        yield return null;
        OnValidate();
    }

    protected bool IsCaculate = true;
    protected void OnValidate()
    {
        Initiate();
        CalculateLayoutInputHorizontal();
    }

    private void ResetVector()
    {
        MidItemPositon = Vector3.back;
        LSidePosition = Vector3.back;
        SideRotation = Quaternion.identity;
        SideScale = Vector3.back;
    }

    private void Initiate()
    {
        IsCaculate = false;
        ResetVector();
    }

    /// <summary>
    /// 绘制时调用
    /// </summary>
    public override void CalculateLayoutInputHorizontal()
    {
        if(IsCaculate)return;
        if (!ParentAdaption.ParentFit)
        {
            ParentAdaption.NumPerPage = 1;
        }
        // 调用基类方法以确保计算水平布局输入
        base.CalculateLayoutInputHorizontal();

        if (fitType == FitType.Width || fitType == FitType.Height || fitType == FitType.Uniform)
        {
            // 计算子对象数量的平方根
            float squareRoot = Mathf.Sqrt(transform.childCount);
            // 向上取整得到行数和列数
            rows = columns = Mathf.CeilToInt(squareRoot);
            // 根据fitType选择布局方式
            switch (fitType)
            {
                case FitType.Width:
                    fitX = true;
                    fitY = false;
                    break;
                case FitType.Height:
                    fitX = false;
                    fitY = true;
                    break;
                case FitType.Uniform:
                    fitX = fitY = true;
                    break;
                case FitType.FixedColumns:
                    if (ParentAdaption.ParentFit)
                    {
                        fitX = true;
                        fitY = false;
                    }
                    break;
                case FitType.FixedRows:
                    if (ParentAdaption.ParentFit)
                    {
                        fitX = false;
                        fitY = true;
                    }
                    break;
            }
        }

        // 列展开
        if (fitType == FitType.Width || fitType == FitType.FixedColumns)
        {
            // 根据列数计算行数
            rows = Mathf.CeilToInt(transform.childCount / (float)columns);
            // 当前的屏幕能够显示的item数量
            _itemCount = Mathf.Min((int)(ParentRect.sizeDelta.y / (cellSize.y + spacing.y)+0.99),columns);
        }
        // 行展开
        if (fitType == FitType.Height || fitType == FitType.FixedRows)
        {
            // 根据行数计算列数
            columns = Mathf.CeilToInt(transform.childCount / (float)rows);
            // 当前的屏幕能够显示的item数量
            _itemCount = Mathf.Min((int)(ParentRect.sizeDelta.x / (cellSize.x + spacing.x)+0.99),columns);
        }
        float parentWidth = rectTransform.rect.width;
        float parentHeight = rectTransform.rect.height;
        float cellWidth = parentWidth / (float)columns - ((spacing.x / (float)columns) * (columns - 1))
            - (padding.left / (float)columns) - (padding.right / (float)columns);
        float cellHeight = parentHeight / (float)rows - ((spacing.y / (float)rows) * (rows - 1))
            - (padding.top / (float)rows) - (padding.bottom / (float)rows);
        // 如果fitX为true，则使用计算的单元格宽度，否则使用cellSize.x
        // cellSize.x = fitX ? cellWidth : cellSize.x;
        // 如果fitY为true，则使用计算的单元格高度，否则使用cellSize.y
        // cellSize.y = fitY ? cellHeight : cellSize.y;
        // 当前列的索引
        int columnCount = 0;
        // 当前行的索引
        int rowCount = 0;
        if (ParentAdaption.PageFit)
        {
            if (fitType == FitType.FixedRows)
            {
                Pages = (columns + ParentAdaption.NumPerPage - 1) / ParentAdaption.NumPerPage;
            }
            else if (fitType == FitType.FixedColumns)
            {
                Pages = (rows + ParentAdaption.NumPerPage - 1) / ParentAdaption.NumPerPage;
            }

        }
        // 每页子实体数量
        int ChildPerPage = ParentAdaption.NumPerPage * rows;
        for (int i = 0; i < rectChildren.Count; i++)
        {
            // 根据fitType决定计算行索引和列索引的方式
            if (fitType == FitType.FixedRows)
            {
                // 页面增量
                int pageAddition = i / ChildPerPage * ParentAdaption.NumPerPage;
                // 行索引基值
                int baseColumn = i % ParentAdaption.NumPerPage;
                // 列索引基值
                int baseRow = i / ParentAdaption.NumPerPage;
                // 计算当前子对象所在的行索引
                rowCount = baseRow % rows;
                // 计算当前子对象所在的列索引
                columnCount = baseColumn + pageAddition;
            }
            else if (fitType == FitType.FixedColumns)
            {
                // 计算当前子对象所在的行索引
                rowCount = i / columns;
                // 计算当前子对象所在的列索引
                columnCount = i % columns;
            }
            // 获取子对象
            RectTransform item = rectChildren[i];
            if (MidItemPositon == Vector3.back)
            {
                MidItemPositon = rectChildren[MidItemIndex].position;
            }
            if (LSidePosition == Vector3.back)
            {
                LSidePosition = rectChildren[0].position;
                RSidePosition = rectChildren[0 + MidItemIndex + MidItemIndex].position;
            }
            var xPos = (cellSize.x * columnCount) + (spacing.x * columnCount) + padding.left;
            var yPos = (cellSize.y * rowCount) + (spacing.y * rowCount) + padding.top;
            var zPos = spacing.z;
             CaculateDecay(i, columnCount, rowCount, ref item, ref xPos, ref yPos, ref zPos);
            // 设置子对象在x轴上的位置和大小(LayoutZGroup方法默认设置左上角为锚点，需要重置一下)
            SetChildAlongAxis(item, 0, xPos, cellSize.x);
            // 根据ChlidAlignment重置
            RectTransform.Edge horizon = RectTransform.Edge.Left;
            RectTransform.Edge vertical = RectTransform.Edge.Top;
            int typeIndex = (int)m_ChildAlignment;
            if (typeIndex >= 6)
            {
                horizon = RectTransform.Edge.Right;
                vertical = RectTransform.Edge.Bottom; 
            }
            item.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(horizon, xPos,  cellSize.x);
            // 设置子对象在y轴上的位置和大小
            SetChildAlongAxis(item, 1, yPos, cellSize.y);
            item.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(vertical, yPos,  cellSize.y);
            // 设置子对象在Z轴上的位置和大小
            var location = item.GetComponent<RectTransform>().localPosition;
            item.GetComponent<RectTransform>().localPosition = new Vector3(location.x, location.y, zPos);
            // 随着子对象的增加，父对象也应当自适应增加宽高
            if (ParentAdaption.ParentFit)
            {
                // 父对象的宽度和高度
                parentWidth = (cellSize.x * columns) + (spacing.x * (columns - 1)) + padding.left + padding.right;
                parentHeight = (cellSize.y * rows) + (spacing.y * (rows - 1)) + padding.top + padding.bottom;
                // 设置父对象的大小
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, parentWidth);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, parentHeight);
            }
        }
        if (ParentAdaption.ParentFit)
        {
            // 如果开启页自适应模式,会以页数来进行父对象的尺寸增加
            if (ParentAdaption.PageFit)
            {
                if (fitType == FitType.FixedRows)
                {
                    // 父对象的宽度和高度
                    parentWidth = (cellSize.x * (Pages * ParentAdaption.NumPerPage)) + (spacing.x * ((Pages * ParentAdaption.NumPerPage) - 1)) + padding.left + padding.right;
                    parentHeight = (cellSize.y * rows) + (spacing.y * (rows - 1)) + padding.top + padding.bottom;
                }
                else if (fitType == FitType.FixedColumns)
                {
                    // 父对象的宽度和高度
                    parentWidth = (cellSize.x * columns) + (spacing.x * (columns - 1)) + padding.left + padding.right;
                    parentHeight = (cellSize.y * (Pages * ParentAdaption.NumPerPage)) + (spacing.y * ((Pages * ParentAdaption.NumPerPage) - 1)) + padding.top + padding.bottom;
                }
                // 设置父对象的大小
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, parentWidth);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, parentHeight);
            }
        }
        IsCaculate = false;
    }

    protected virtual void CaculateDecay (int i,int columnCount, int rowCount, ref RectTransform item,ref float xPos,  ref float yPos, ref float zPos)
    {
        if (IsDecay)
        {
            float indexDiffer = 0;
            float indexDifferAbs = 0;
            float sign = 1;
            // 计算子对象的x位置
            xPos += (columnCount) * Decay.PositionDecay.x;
            // 计算子对象的y位置
            yPos += (rowCount - MidItemIndex) * Decay.PositionDecay.y;
            if (fitType == FitType.FixedColumns)
            {
                // 计算子对象的z位置
                indexDiffer = rowCount - MidItemIndex;
                indexDifferAbs = Mathf.Abs(indexDiffer);
                sign = indexDiffer > 0 ? 1 : -1;
                zPos += -indexDifferAbs * Decay.PositionDecay.z;
            }
            else if (fitType == FitType.FixedRows)
            {
                // 计算子对象的z位置
                indexDiffer = columnCount - MidItemIndex;
                indexDifferAbs = Mathf.Abs(indexDiffer);
                sign = indexDiffer > 0 ? 1 : -1;
                zPos += indexDifferAbs * Decay.PositionDecay.z;
            }
            if (i == 0)
            {
                item.localRotation = Quaternion.Euler(Decay.RotationDecay);
                item.localScale = Decay.ScaleDecay;
                if (SideRotation == Quaternion.identity)
                {
                    SideRotation = item.localRotation;
                }
                if (SideScale == Vector3.back)
                {
                    SideScale = item.localScale;
                }
            }
        }
    }

    public virtual void Update()
    {
        foreach (var rect in rectChildren)
        {
            float normalizedDistance = 0;
            if (rect.position.x - MidItemPositon.x < 0)
            {
                normalizedDistance = DistanceNormalize(rect.position, MidItemPositon, LSidePosition);
            }
            else
            {
                normalizedDistance = DistanceNormalize(rect.position, MidItemPositon, RSidePosition);
            }
            // 根据物体相对于中间位置的方向调整旋转方向
            Quaternion baseRotation = rect.position.x < MidItemPositon.x ? Quaternion.Euler(0, SideRotation.eulerAngles.y, 0) : Quaternion.Euler(0, -SideRotation.eulerAngles.y, 0);
            rect.localRotation = Quaternion.Lerp(Quaternion_ZERO, baseRotation, normalizedDistance);
            rect.localScale = Vector3.Lerp(Vector3.one, SideScale, normalizedDistance);
            rect.position = new Vector3(rect.position.x, rect.position.y, Mathf.Lerp(MidItemPositon.z,LSidePosition.z,normalizedDistance));
        }
    }

    protected float DistanceNormalize(Vector3 selfPos, Vector3 start, Vector3 end)
    {
        if ((end - start).sqrMagnitude < 0.01f)
        {
            return 0f;
        }
        Vector3 direction = (end - start).normalized;
        float distanceAlongDirection = Vector3.Dot(selfPos - start, direction);
        float totalDistance = (end - start).magnitude;
        float normalizedDistance = distanceAlongDirection / totalDistance;
        return Mathf.Clamp01(normalizedDistance);
    }
    
    public override void CalculateLayoutInputVertical()
    {
        //throw new System.NotImplementedException();
    }

    public override void SetLayoutHorizontal()
    {
        //throw new System.NotImplementedException();
    }

    public override void SetLayoutVertical()
    {
        //throw new System.NotImplementedException();
    }

}