using OpenAI_FunctionCalling;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Pet : MonoBehaviour/*,IPointerDownHandler*/
{
    /*    public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.clickCount == 2)
                uIManager.mainMenu.SetActive(true);
        }*/
    private void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0)&&UIManager.Instance.AllClose)
        {
            CheckMousePos();
        }
    }

    private RaycastHit2D[] raycastHit = new RaycastHit2D[10];
    //��������ر�����
    void CheckMousePos()
    {
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z + 10);
        Vector3 mousePosWorld = UIManager.Instance.camera.ScreenToWorldPoint(mousePos);
        if (Physics2D.RaycastNonAlloc(mousePosWorld, new Vector2(0, 0), raycastHit) > 0)
        {

            UIManager.Instance.mainMenu.SetActive(true);
            if(!IsActive)
                StartCoroutine(Speek());
        }
    }
    [SerializeField] Text Greettext;
    private void Greet()
    {
/*        GptTurboScript.SendData SysData = new("system", "�ո����������㣬�ƺ���ʲô�������㣬������һ�����仰�ش�");
        //Post���ݰ�
        GptTurboScript.PostData postData = new();
        postData.model = "gpt-3.5-turbo";
        postData.messages = new()
        {
            SysData,
        };*/
        //�������ݡ���������
/*        StartCoroutine(ChatScript.GetPostData(postData, Greettext, CallBack));*/
    }
    

    void GreetRandom()
    {
        string dia = "������ʲô�ҿ���Ч�͵���1��ô��������Ҫ��Ϊ������ʲô������1�ǳ�Ը��Ч�ͣ�������ʲô��Ҫ�Ұ������������1��ʲô������Ҫ����������1����ʱ������������ʲô������Ҫ�Ұ�æ������1�ޣ���������Ҫ�Ұ�æ����ʲô������1���ڵģ���������ʲô������Ҫ������1������ʲô�ҿ��԰���������";
        //��������ʺ���
        List<string> GreetDia = new List<string>(dia.Split("1"));
        textcontain.SetActive(true);
        int index = UnityEngine.Random.Range(0, 7);
        Greettext.text = GreetDia[index];

    }
    
    bool IsActive = false;

    
    IEnumerator Speek()
    {
        GreetRandom();
        IsActive = true;
        ChatScript._ChatScript.IsSpeeking = true;
        yield return new WaitForSeconds(3f);
        ChatScript._ChatScript.IsSpeeking = false;
        yield return new WaitForSeconds(5f);
        IsActive = false;
        textcontain.SetActive(false);
    }
    //�Ի�������
    [SerializeField] private GameObject textcontain;
    private void CallBack(string _callback, Text m_TextBack)
    {
        
        textcontain.SetActive(true);
        _callback = _callback.Trim();
        m_TextBack.text = _callback;
    }
}
