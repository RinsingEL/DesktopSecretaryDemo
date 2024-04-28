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
    //Model��
    int int_NewYear;
    int int_NewMouth;
    int int_NewDay;
    /// <summary>
    /// �õ��·ݵ�����
    /// </summary>
    /// <param name="year">��</param>
    /// <param name="mouth">��</param>
    /// <returns></returns>
    public static int GetMouthDays(int year, int mouth)
    {
        return DateTime.DaysInMonth(year, mouth);
    }
    /// <summary>
    /// �õ�����Ϊ��ݼ�
    /// </summary>
    /// <param name="year">��</param>
    /// <param name="mouth">��</param>
    /// <param name="day">��</param>
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
        txt_ShowDayData.text = int_NewYear + "��" + int_NewMouth + "��" + int_NewDay + "��";
        ShowLeftDayList(int_NewYear, int_NewMouth, int_NewDay);//���ݻ�ȡ����ʱ��չʾ������
    }
    public void OpenCalendar()
    {
        ShowLeftDayList(int_NewYear, int_NewMouth, int_NewDay);
    }
    void ShowLeftDayList(int year, int mouth, int day)
    {
        ClearChildren(trans_parent);//
        txt_ShowDayData.text = year + "��" + mouth + "��" + day + "��";
        GameObject itemTpl = prefebObj;

        int days = GetMouthDays(year, mouth);//��һ�����һ�����ж�����
        int week = GetDayOfWeek(year, mouth, 1);//�õ�ÿ��һ�������ڼ�
                                                //
        for (int i = 1; i < week; i++)//��ÿ��һ�ſ�ʼ���������壬�ٸ�������ĳ��һ���������������������ǰ����1��֮ǰ�ģ��ı���ͼƬȥ�� ������Ҫռ��λ��
        {
            GameObject go = InstantiateItemSetParent(itemTpl, trans_parent);//
            go.transform.localScale = Vector3.one;
            go.transform.Find("txt").GetComponent<Text>().text = "";
            /*Destroy(go.transform.GetChild(0));*/
            Destroy(go.transform.GetComponent<Image>());
        }

        for (int k = 1; k <= days; k++)//ѡ�����һ�����ж����죬��¡��������������
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
                txt_ShowDayData.text = int_NewYear + "��" + int_NewMouth + "��" + int_NewDay + "��";
                string selectday = int_NewYear + "/" + int_NewMouth + "/" + int_NewDay+"";

                if (curBtnImage != null)//�����ȡ���İ�ťͼƬ��Ϊ�գ������������ε���Ƿ���ͬһ����ť��Ҫ�ѱ���ͼƬ��ԭ��δ���������
                {
                    curBtnImage.color = curColor;
                }
                curBtnImage = btn.GetComponent<Image>();//����ð�ť��ȡ��ť�ϵı���ͼƬ
                curColor = curBtnImage.color;//����ͼƬ��ʼ��ɫ��������
                curBtnImage.color = Color.green;//Ȼ�������İ�ť�����µ���ɫ�����������İ�ť
                                                //��ɫ��ʾѡ��
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

            Image im = btn.GetComponent<Image>();//������Ҫ������ĩ������������ʾ���ֿ�
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
    //���������
    public void UpMouth()//��ǰ
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
    public void DownMouth()//����
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
        // pos.DetachChildren();//����������ٲ㼶�ĸ������������������Ǻ����õġ�
    }
    //ʵ���������ø�����
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
