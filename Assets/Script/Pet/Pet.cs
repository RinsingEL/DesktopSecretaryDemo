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
    //鼠标点击外侧关闭所有
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
/*        GptTurboScript.SendData SysData = new("system", "刚刚我拍了拍你，似乎有什么事情找你，请你用一到两句话回答");
        //Post数据包
        GptTurboScript.PostData postData = new();
        postData.model = "gpt-3.5-turbo";
        postData.messages = new()
        {
            SysData,
        };*/
        //发送数据、接收数据
/*        StartCoroutine(ChatScript.GetPostData(postData, Greettext, CallBack));*/
    }
    

    void GreetRandom()
    {
        string dia = "请问有什么我可以效劳的吗。1怎么了吗，您需要我为您处理什么事情吗1非常愿意效劳，请问有什么需要我帮助您处理的吗？1有什么事情需要我来处理吗1我随时待命，请问有什么事情需要我帮忙处理吗1噢，请问您需要我帮忙处理什么事情吗？1我在的，您找我有什么事情需要处理吗？1请问有什么我可以帮助您的吗？";
        //发送随机问候语
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
    //对话框容器
    [SerializeField] private GameObject textcontain;
    private void CallBack(string _callback, Text m_TextBack)
    {
        
        textcontain.SetActive(true);
        _callback = _callback.Trim();
        m_TextBack.text = _callback;
    }
}
