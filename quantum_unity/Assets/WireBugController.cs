using System.Collections;
using System.Collections.Generic;
using Photon.Deterministic;
using UnityEngine;
using UnityEngine.UI;

public class WireBugController : MonoBehaviour
{
    [SerializeField]
    private bool _isActive;
    public GameObject Active;
    public GameObject Inactive;
    public Image ProcessImage;

    public void SetActive(bool flag)
    {
        _isActive = flag;
        if (flag)
        {
            Active.SetActive(true);
            Inactive.SetActive(false);
        }
        else
        {
            Active.SetActive(false);
            Inactive.SetActive(true);
        }
    }
    
    public void SetProcess(FP process)
    {
        ProcessImage.fillAmount = 1 - (float)process;
    }
}
