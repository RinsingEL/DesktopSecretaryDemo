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
    //��ȡPrompt
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
    //ÿ��1���Ӽ��һ��
    IEnumerator CheckTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(60f);
            if ((DData._eventItemList.Count != 0))
            {
                //������컹û�����ƻ���
                if (!DData._eventItemList.Exists(item => item._date == DateTime.Now.ToString("d")))
                    continue;
                List<DEvent> todayList = DData._eventItemList.Find(item => item._date == DateTime.Now.ToString("d"))._eventItemList;
                //������û�мƻ�
                if (todayList.Count == 0)
                    continue;
                //��ȡ��ǰʱ�䡢�ƻ�ʱ��
                foreach (DEvent dEvent in todayList)
                {
                    string TrueTime = DateTime.Now.ToString("HH:mm");
                    //�����ʱ���˴���������һ���Ӳż��һ�Σ����ֻ����һ��
                    if (dEvent._eventTime == TrueTime)
                    {
                        curPlan = dEvent;
                        ChatScript._ChatScript.AddHistory($"����{curPlan._eventName}��ʼ���¼�)");
                        //ϵͳ��Ϣ
                        SendData SysData = new("system", Prompt+ "���ڸպõ�ʱ�䣬���������Ҽƻ���" + dEvent._eventTime + "ִ�е��ճ̣�" + dEvent._eventName);
                        //Post���ݰ�
                        PostData postData = new();
                        postData.model = "gpt-3.5-turbo";
                        postData.messages = new()
                    {
                        SysData,
                     };
                        //����Ϊ��ʱ����
                        IsTime = true;
                        //����Ϊδȷ��
                        IsConfirm = false;
                        Timer(this);
                        //��������Э��
                        StartCoroutine(ChatScript._ChatScript.GetPostData(postData, RemindView.Instance.text, CallBack));
                    }
                }
           
            }


        }
    }

    IEnumerator CheckComfirm()
    {
        //���Ѻ��ټ��3min���Ƿ���ȷ��
        yield return new WaitForSeconds(180f);
        //��ʱ���˵���δȷ�ϣ��ͻ��ٴ�����
        if (!isConfirm && isTime)
        {
            //ϵͳ��Ϣ
            SendData SysData = new("system", Prompt + "��ո�������" + curPlan._eventName + "�����Һ���û�������鷳���ٴ�������");
            //Post���ݰ�
            PostData postData = new();
            postData.model = "gpt-3.5-turbo";
            postData.messages = new()
                {
                        SysData
                 };
            ChatScript._ChatScript.AddHistory("��δȷ�Ͽ�ʼ��");
            //��������Э��
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
