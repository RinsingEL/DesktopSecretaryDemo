using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static GptTurboScript;
using UnityEngine.UI;
using OpenAI_FunctionCalling;
using static PlanPanel.DailyPlanModel;
using UnityEngine.Events;

public class RemindScript : MonoBehaviour
{
    //获取Prompt
    public string Prompt = "";
    public DEvent curPlan;
    private event UnityAction<RemindScript> Timer;
    private bool isConfirm = false;
    private bool isTime = false;
    public static RemindScript Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<RemindScript>();
            return instance;
        }
    }
    private static RemindScript instance;

    public bool IsTime { get => isTime; set => isTime = value; }
    public bool IsConfirm { get => isConfirm; set => isConfirm = value; }

    void Start()
    {
        Prompt = ChatScript._ChatScript.GetSetting().prompt;
        StartCoroutine(CheckTime());
    }
    //每隔1分钟检查一次
    IEnumerator CheckTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(60f);
            if ((DData._eventItemList.Count != 0))
            {
                //假如今天还没创建计划表：
                if (!DData._eventItemList.Exists(item => item._date == DateTime.Now.ToString("d")))
                    continue;
                List<DEvent> todayList = DData._eventItemList.Find(item => item._date == DateTime.Now.ToString("d"))._eventItemList;
                //若今天没有计划
                if (todayList.Count == 0)
                    continue;
                //读取当前时间、计划时间
                foreach (DEvent dEvent in todayList)
                {
                    string TrueTime = DateTime.Now.ToString("HH:mm");
                    //如果到时间了触发，由于一分钟才检查一次，因此只触发一次
                    if (dEvent._eventTime == TrueTime)
                    {
                        curPlan = dEvent;
                        ChatScript._ChatScript.AddHistory($"（到{curPlan._eventName}开始的事件)");
                        //系统信息
                        SendData SysData = new("system", Prompt+ "现在刚好到时间，请你提醒我计划在" + dEvent._eventTime + "执行的日程：" + dEvent._eventName);
                        //Post数据包
                        PostData postData = new();
                        postData.model = "gpt-3.5-turbo";
                        postData.messages = new()
                    {
                        SysData,
                     };
                        //设置为到时间了
                        IsTime = true;
                        //设置为未确认
                        IsConfirm = false;
                        Timer(this);
                        //发送提醒协程
                        StartCoroutine(ChatScript._ChatScript.GetPostData(postData, RemindView.Instance.text, CallBack));
                    }
                }
           
            }


        }
    }

    IEnumerator CheckComfirm()
    {
        //提醒后再检测3min内是否点击确认
        yield return new WaitForSeconds(180f);
        //到时间了但是未确认，就会再次提醒
        if (!isConfirm && isTime)
        {
            //系统信息
            SendData SysData = new("system", Prompt + "你刚刚提醒我" + curPlan._eventName + "但是我好像没听到，麻烦你再次提醒我");
            //Post数据包
            PostData postData = new();
            postData.model = "gpt-3.5-turbo";
            postData.messages = new()
                {
                        SysData
                 };
            ChatScript._ChatScript.AddHistory("（未确认开始）");
            //发送提醒协程
            StartCoroutine(ChatScript._ChatScript.GetPostData(postData, RemindView.Instance.text, CallBack));

        }
    }
    
    private void CallBack(string _callback, Text m_TextBack)
    {
        RemindView.Instance.textcontain.SetActive(true);
        ChatScript._ChatScript.CallBack(_callback, m_TextBack);
        StartCoroutine(CheckComfirm());
    }
    public void AddListener(UnityAction<RemindScript> func)
    {
        Timer += func;
    }
    public void ReMoveListener(UnityAction<RemindScript> func)
    {
        Timer -= func;
    }
}
