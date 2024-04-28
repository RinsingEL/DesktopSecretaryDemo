using OpenAI_FunctionCalling;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RemindView : MonoBehaviour
{
    //view+controller
    //对话框
    public Text text;
    //对话框容器
    public GameObject textcontain;

    public static RemindView Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<RemindView>();
            return instance;
        }
    }
    private static RemindView instance;

    void Awake()
    {
        RemindScript.Instance.AddListener(delegate { textcontain.SetActive(true); });
    }
    public GameObject but1;
    public GameObject but2;
    public IEnumerator remindFunc(string intext)
    {
        textcontain.SetActive(true);
        but1.SetActive(false);
        but2.SetActive(false);
        text.text = intext;
        ChatScript._ChatScript.IsSpeeking = true;
        yield return new WaitForSeconds(10f);
        text.text = "";
        but1.SetActive(true);
        but2.SetActive(true);
        textcontain.SetActive(false);
        ChatScript._ChatScript.IsSpeeking = false;
    }
}
