using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlanPanel
{
    public class DailyPlanView : MonoBehaviour
    {
        //日期下拉菜单
        [SerializeField] public Dropdown _dropdown;
        public List<Dropdown.OptionData> optionList = new List<Dropdown.OptionData>();
        [SerializeField] public Text label;
        public Text Date;
        [SerializeField] Text Complite;
        //事件名输入框
        [SerializeField] public Text m_InputWord;
        [SerializeField] public GameObject _inputMessage;
        //放置事件的层级
        [SerializeField] public GameObject _eventPanel;
        [SerializeField] public EventController _plan;

        private static DailyPlanView Dview;

        public static DailyPlanView DView
        {
            get
            {
                return Dview;
            }
        }
        void Awake()
        {
            if(Dview==null)
            {
                Dview = FindObjectOfType<DailyPlanView>();
                Dview.Init(DailyPlanModel.DData);
            }
            
        }
        void Init(DailyPlanModel Data)
        {
            //初始化预设体
            PrefabInit();
            //初始化下拉列表
            foreach (var _event in Data._eventItemList)
            {
                Dropdown.OptionData newOption = new Dropdown.OptionData();
                newOption.text = _event._date;
                optionList.Add(newOption);
                optionList.Sort((event1, event2) => DateTime.Compare(DateTime.Parse(event1.text), DateTime.Parse(event2.text)));
            }
            _dropdown.AddOptions(optionList);
            _dropdown.value = optionList.IndexOf(optionList.Find(opt => opt.text == DateTime.Now.ToString("d")));
            _dropdown.onValueChanged.AddListener(delegate {
                UpdateView(Data);
            });
            //初始化日期
            label.text = DateTime.Now.ToString("d");
            //有今天就把初始界面设置为今天
            if (Data._eventItemList.Exists(item => item._date == DateTime.Now.ToString("d")))
            {
                UpdateView(Data);
            }
            Complite.text = "0%";
            // Data更新的话View也更新
            Data.AddListener(UpdateView);
        }
        /// <summary>
        /// 更新界面
        /// </summary>
        /// <param name="Data"></param>
        public void UpdateView(DailyPlanModel Data)
        {
            List<DailyPlanModel.DEvent> todayList = Data._eventItemList.Find(item => item._date == label.text)._eventItemList;
            //显示列表
            CreatePlanPrefab(todayList);
        }
        void PrefabInit()
        {

            _inputMessage.SetActive(false);
            
        }
        /// <summary>
        /// 按顺序按日显示
        /// </summary>
        /// <param name="eventList">今日的事件列表</param>
        public void CreatePlanPrefab(List<DailyPlanModel.DEvent> eventList)
        {
            Date.text = label.text;
            int ChildCount = _eventPanel.transform.childCount;
            //销毁所有已经创建的对象
            for (int i = 0; i < ChildCount; i++)
            {
                Destroy(_eventPanel.transform.GetChild(i).gameObject);
            }
            foreach (DailyPlanModel.DEvent eventItem in eventList)
            {
                //_plan._dailyPlan = this;
                //_plan.focus = focus;
                EventController planPrefab = Instantiate(_plan, _eventPanel.transform);
                planPrefab.view._eventName.text = eventItem._eventName;
                planPrefab.view._eventTime.text = eventItem._eventTime;
                planPrefab._eventID = eventItem._eventID;
                planPrefab.isFinish = eventItem.isFinish;
                planPrefab.curPlan = eventItem;
                planPrefab.date = label.text;
                planPrefab.transform.SetParent(_eventPanel.transform);
            }
            //更新完成率
            Complite.text = DailyPlanModel.DData.CalculateCompletionRate(label.text);
        }
        public void CreateNewDropdown(string day)
        {
            //创建下拉菜单日期
            Dropdown.OptionData newOption = new Dropdown.OptionData();
            newOption.text = day;
            optionList.Add(newOption);
            //创建事情完成列表，对下拉列表排序
            optionList.Sort((event1, event2) => DateTime.Compare(DateTime.Parse(event1.text), DateTime.Parse(event2.text)));
            _dropdown.options = optionList;
        }
    } 
}
