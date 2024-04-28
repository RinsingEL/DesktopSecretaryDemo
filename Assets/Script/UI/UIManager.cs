using OpenAI_FunctionCalling;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance
    {
        get
        {
            if(instance==null)
            {
                instance = FindObjectOfType<UIManager>();
            }
            return instance;
        }
    }

    [SerializeField] public Camera camera;
    [SerializeField] WinTray winTray;
    [SerializeField] Text debugText;
    [SerializeField] ChatScript chatScript;
/*    [SerializeField] Calendar calendar;*/

    [Header("MENUS")]
    public GameObject mainMenu;
    public GameObject DialogMenu;
    public GameObject SettingMenu;
    public GameObject ScheduleMenu;

    [Header("SubMENUS")]
    public GameObject SubMenu;
    public GameObject PostMenu;
    public GameObject HistoryMenu;
    public GameObject PlanMenu;


    [Header("GUI")]
    public GameObject Panel;
    public GameObject PostPanelContain;
    public GameObject PostPanel;
    public GameObject HistoryPanel;

    public GameObject SettingPanel;

    public GameObject SchedulePanel;
    public GameObject DailyTarget;
    public GameObject WeekTarget;

    public GameObject FocusPanel;

    //Setting Parameter
    public float planPanelYoffset = 0;
    public Transform planTransform;
    public bool AllClose = true;
    // Start is called before the first frame update
    void Awake()
    {
        mainMenu.SetActive(false);
        SubMenu.SetActive(false);
        Panel.SetActive(true);
        PostPanelContain.SetActive(true);
        SchedulePanel.SetActive(true);
        planTransform = SchedulePanel.GetComponent<Transform>();
        DisablePanel();
    }
    void Update()
    {
        bool hasFocus = true;
        //OnApplicationFocus(hasFocus);
    }
    //触发聊天子按钮
    public void PostSubButton()
    {
        DisableMenu();
        SubMenu.SetActive(true);
        PostMenu.SetActive(true);
        AllClose = false;
    }
    //处理界面
    //触发聊天界面
    public void PostPanelButton()
    {
        DisablePanel();
        PostPanel.SetActive(true);
        AllClose = false;
    }
    public void HistoryPanelButton()
    {
        DisablePanel();
        HistoryPanel.SetActive(true);
        chatScript.OpenAndGetHistory();
        AllClose = false;
    }
    //处理设置菜单
    public void SettingPanelButton()
    {
        DisablePanel();
        SettingPanel.SetActive(true);
        AllClose = false;
    }
    //处理计划表菜单
    public void SchedulePanelButton()
    {
        DisableMenu();
        SubMenu.SetActive(true);
        PlanMenu.SetActive(true);
        AllClose = false;
    }
    public void DailyTargetButton()
    {
        DisablePanel();
        DailyTarget.SetActive(true);
        AllClose = false;
    }
    public void WeekTargetButton()
    {
        DisablePanel();
        WeekTarget.SetActive(true);
/*        calendar.OpenCalendar();*/
        AllClose = false;
    }
    public void DisablePanel()
    {
        AllClose = true;
        PostPanel.SetActive(false);
        HistoryPanel.SetActive(false);
        SettingPanel.SetActive(false);
        DailyTarget.SetActive(false); 
        WeekTarget.SetActive(false);
    }
    public void DisableMenu()
    {
        PostMenu.SetActive(false);
        PlanMenu.SetActive(false);
    }
    public void FocusButton()
    {
        DisablePanel();
        FocusPanel.SetActive(true);
        AllClose = false;
    }
    public void CloseFocuBut()
    {
        FocusPanel.SetActive(false);
    }

#if !UNITY_EDITOR
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            SubMenu.SetActive(false);
            mainMenu.SetActive(false);
            DisablePanel();
        }
    }
#endif
}