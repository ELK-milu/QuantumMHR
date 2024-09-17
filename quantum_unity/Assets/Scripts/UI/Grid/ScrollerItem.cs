using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
[Serializable]
public class ScrollerItem : MonoBehaviour
{
    private FlexibleGridLayout _flexibleGridLayout;
    public ItemData ThisItemData;
    public TextMeshProUGUI IndexText;
    
    void Start()
    {
        _flexibleGridLayout = GetComponentInParent<FlexibleGridLayout>();
    }

    public void SetData (ItemData data)
    {
        ThisItemData = data;
        IndexText.text = data.index.ToString();
    }

    public void ClearData()
    {
        ThisItemData = new ItemData();
        IndexText.text = "NULL";
    }

}
