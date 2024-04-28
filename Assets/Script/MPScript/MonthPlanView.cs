using PlanPanel;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;
using static PlanPanel.DailyPlanController;
using static PlanPanel.DailyPlanView;
using static PlanPanel.DailyPlanModel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Menu;

public class MonthPlanView : MonoBehaviour
{
    //Model层
    int int_NewYear;
    int int_NewMouth;
    int int_NewDay;
    /// <summary>
    /// 得到月份的天数
    /// </summary>
    /// <param name="year">年</param>
    /// <param name="mouth">月</param>
    /// <returns></returns>
    public static int GetMouthDays(int year, int mouth)
    {
        return DateTime.DaysInMonth(year, mouth);
    }
    /// <summary>
    /// 得到天数为礼拜几
    /// </summary>
    /// <param name="year">年</param>
    /// <param name="mouth">月</param>
    /// <param name="day">日</param>
    /// <returns></returns>
    public static int GetDayOfWeek(int year, int mouth, int day)
    {

        DateTime dt = new DateTime(year, mouth, day);

        switch (dt.DayOfWeek.ToString())
        {
            case "Monday": return 1;
            case "Tuesday": return 2;
            case "Wednesday": return 3;
            case "Thursday": return 4;
            case "Friday": return 5;
            case "Saturday": return 6;
            case "Sunday": return 7;
        }
        return 0;
    }

    public Text txt_ShowDayData;
    public RectTransform trans_parent;
    public GameObject prefebObj;
    public GameObject eventObj;

    private Image curBtnImage;
    private Color curColor;
    private static MonthPlanView Mview;
    public static MonthPlanView MView
    {
        get
        {
            if (Mview == null)
                Mview = FindObjectOfType<MonthPlanView>();
            return Mview;
        }
    }
    void Start()
    {
        DateTime nowDateTime = DateTime.Now;
        int_NewYear = nowDateTime.Year;
        int_NewMouth = nowDateTime.Month;
        int_NewDay = nowDateTime.Day;
        txt_ShowDayData.text = int_NewYear + "年" + int_NewMouth + "月" + int_NewDay + "号";
        ShowLeftDayList(int_NewYear, int_NewMouth, int_NewDay);//根据获取到的时间展示日历表
    }
    public void OpenCalendar()
    {
        ShowLeftDayList(int_NewYear, int_NewMouth, int_NewDay);
    }
    void ShowLeftDayList(int year, int mouth, int day)
    {
        ClearChildren(trans_parent);//
        txt_ShowDayData.text = year + "年" + mouth + "月" + day + "号";
        GameObject itemTpl = prefebObj;

        int days = GetMouthDays(year, mouth);//这一年的这一个月有多少天
        int week = GetDayOfWeek(year, mouth, 1);//得到每月一号是星期几
                                                //
        for (int i = 1; i < week; i++)//从每月一号开始排列子物体，举个例子若某月一号是星期三，则把星期三前（即1号之前的）的背景图片去掉 但是仍要占据位置
        {
            GameObject go = InstantiateItemSetParent(itemTpl, trans_parent);//
            go.transform.localScale = Vector3.one;
            go.transform.Find("txt").GetComponent<Text>().text = "";
            /*Destroy(go.transform.GetChild(0));*/
            Destroy(go.transform.GetComponent<Image>());
        }

        for (int k = 1; k <= days; k++)//选择的那一个月有多少天，克隆出来几个子物体
        {
            GameObject go = InstantiateItemSetParent(itemTpl, trans_parent);
            go.transform.localScale = Vector3.one;
            int NewDay = k;
            go.transform.Find("txt").GetComponent<Text>().text = k + "";

                foreach (EventItemList eventItem in DData._eventItemList)
                {
                    string selectday = int_NewYear + "/" + int_NewMouth + "/" + NewDay + "";
                    if (eventItem._date == selectday)
                        foreach (DEvent evt in eventItem._eventItemList)
                        {
                            GameObject e = InstantiateItemSetParent(eventObj, go.transform);
                            e.transform.Find("txt").GetComponent<Text>().text = evt._eventName;
                            e.GetComponent<Image>().enabled = true;
                            e.GetComponent<Button>().enabled = true;
                            e.transform.localScale = Vector3.one;
                        }
                }
            
            Button btn = go.GetComponent<Button>();
            btn.onClick.AddListener(delegate
            {
                int_NewDay = NewDay;
                txt_ShowDayData.text = int_NewYear + "年" + int_NewMouth + "月" + int_NewDay + "号";
                string selectday = int_NewYear + "/" + int_NewMouth + "/" + int_NewDay+"";

                if (curBtnImage != null)//如果获取到的按钮图片不为空，不管连续两次点击是否是同一个按钮都要把背景图片还原成未点击的样子
                {
                    curBtnImage.color = curColor;
                }
                curBtnImage = btn.GetComponent<Image>();//点击该按钮获取按钮上的背景图片
                curColor = curBtnImage.color;//背景图片初始颜色保存起来
                curBtnImage.color = Color.green;//然后给点击的按钮赋予新的颜色以区别其他的按钮
                                                //绿色表示选中
                if(DData._eventItemList.Exists(item=>item._date==selectday))
                {
                    UIManager.Instance.DailyTargetButton();
                    DView._dropdown.value = DView.optionList.IndexOf(DView.optionList.Find(item => item.text == selectday));
                }
                else
                {
                    DData.CreateNewList(selectday);
                    DView.CreateNewDropdown(selectday);
                }
                    

            });

            Image im = btn.GetComponent<Image>();//下面主要是想周末与其他日期显示区分开
            if (GetDayOfWeek(year, mouth, k) == 6 || GetDayOfWeek(year, mouth, k) == 7)
            {
                im.color = /*new Vector4(17, 66, 170, 1)*/Color.gray;
                
            }
            else
            {
                im.color = /*new Vector4(108, 141, 213,1)*/Color.gray;
            }
            if(DView.optionList.Exists(item => item.text == int_NewYear + "/" + int_NewMouth + "/" + NewDay + ""))
                im.color = new Vector4(69, 115, 213, 1);
        }
    }
    //清空子物体
    public void UpMouth()//往前
    {
        if (int_NewMouth - 1 == 0)
        {
            int_NewYear -= 1;
            int_NewMouth = 12;
        }
        else
        {
            int_NewMouth -= 1;
        }

        ShowLeftDayList(int_NewYear, int_NewMouth, 1);
    }
    public void DownMouth()//往后
    {
        if (int_NewMouth + 1 == 13)
        {
            int_NewYear += 1;
            int_NewMouth = 1;
        }
        else
        {
            int_NewMouth += 1;
        }

        ShowLeftDayList(int_NewYear, int_NewMouth, 1);
    }
    void ClearChildren(Transform pos)
    {
        for (int i = 0; i < pos.childCount; i++)
            Destroy(pos.GetChild(i).gameObject);
        // pos.DetachChildren();//如果你想销毁层级的根，而不销毁子物体是很有用的。
    }
    //实例化并设置父物体
    GameObject InstantiateItemSetParent(GameObject item, Transform parent)
    {
        GameObject go = Instantiate(item);
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;
        go.transform.localEulerAngles = Vector3.zero;
        go.transform.SetParent(parent);
        return go;
    }
}
