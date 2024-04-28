using PlanPanel;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RemindController : MonoBehaviour
{
    //确认完成
    [SerializeField] Button StartBut;
    [SerializeField] Button ComBut;
    private void Awake()
    {
        StartBut.onClick.AddListener(StartEvent);
        ComBut.onClick.AddListener(SetComliteCount);
    }
    public void StartEvent()
    {
        RemindScript.Instance.IsConfirm = true;
        RemindScript.Instance.IsTime = false;
        UIManager.Instance.FocusButton();
        Focus.Instance.curEvent = RemindScript.Instance.curPlan;
        FocusView.View.curName.text = RemindScript.Instance.curPlan._eventName;
        RemindView.Instance.textcontain.SetActive(false);
    }
    public void SetComliteCount()
    {
        DailyPlanModel.DData.SetEventComplite(true,RemindScript.Instance.curPlan._eventID, DateTime.Now.ToString("d"));
        RemindView.Instance.textcontain.SetActive(false);
    }
    public void PutOffEvent()
    {
        RemindScript.Instance.IsTime = false;
        //改变事件的属性：
    }
}
