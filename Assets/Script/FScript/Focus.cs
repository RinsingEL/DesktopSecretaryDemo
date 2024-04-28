using System.Collections;
using UnityEngine;
using Button = UnityEngine.UI.Button;
using static PlanPanel.DailyPlanModel;
using UnityEngine.Events;

public class Focus : MonoBehaviour
{
    [SerializeField]Button conbutton;
    [SerializeField] Button finishButton;
    [SerializeField] Button hideButton;
    [SerializeField] Sprite Iplay;
    [SerializeField] Sprite Ipause;
    public DEvent curEvent;
    //计时器
    public float totalTime=0f;
    private float tempTime = 0f;
    //统计数据
    public int distractedCount = 0;
    public int glazeCount = 0;
    public float focusTime = 0f;

    public bool IsFinish=true;
    bool IsPause=false;
    bool isCounting = false;
    public event UnityAction Startfocus;
    public event UnityAction Stopfocus;
    private static Focus instance;
    public static Focus Instance
    {
        get
        {
            if(instance == null)
                instance = FindObjectOfType<Focus>();
            return instance;
        }
    }

    private void Awake()
    {

        conbutton.onClick.AddListener(ContinueEvent);
        finishButton.onClick.AddListener(FinishEvent);
        hideButton.onClick.AddListener(HidePanel);
        Startfocus += StartCounting;
        HidePanel();
    }
    IEnumerator StartEvent()
    {
        while (isCounting)
        {
            totalTime++;
            yield return new WaitForSeconds(1f);
            tempTime = totalTime;
            int minutes = Mathf.FloorToInt(totalTime / 60f); 
            int seconds = Mathf.FloorToInt(totalTime % 60f);
            FocusView.View.curTime.text = $"{minutes}:{seconds}";
        }
    }
    void StartCounting()
    {
        StartCoroutine(StartEvent());
    }
    public void EnterFocus(string name)
    {
        FocusView.View.curName.text = name;
    }
    //继续&开始
    public void ContinueEvent()
    {
        totalTime = 0f;
        if (IsPause)
            totalTime = tempTime;
        isCounting = true;
        IsFinish = false;
        IsPause = false;
        Startfocus();
        FocusView.View.ChangeImage(Ipause);
        conbutton.onClick.RemoveListener(ContinueEvent);
        conbutton.onClick.AddListener(PauseEvent);
    }
  
    public void PauseEvent()
    {
        IsPause = true;
        isCounting = false;
        FocusView.View.ChangeImage(Iplay);
        Stopfocus();
        conbutton.onClick.RemoveListener(PauseEvent);
        conbutton.onClick.AddListener(ContinueEvent);
    }
    public void FinishEvent()
    {
        tempTime = 0f;
        isCounting = false;
        focusTime = totalTime;
        curEvent.isFinish = true;
        if (IsPause)
            return;
        conbutton.onClick.RemoveListener(PauseEvent);
        conbutton.onClick.AddListener(ContinueEvent);
        FocusView.View.curTime.text = "0:0";
        Stopfocus();
    }
    public void HidePanel()
    {
        if (FocusView.View.panel.activeSelf)
            FocusView.View.panel.SetActive(false);
        else
            FocusView.View.panel.SetActive(true);
    }
}
