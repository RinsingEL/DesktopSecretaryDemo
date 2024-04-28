using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlanPanel
{
    public class DailyPlanView : MonoBehaviour
    {
        //���������˵�
        [SerializeField] public Dropdown _dropdown;
        public List<Dropdown.OptionData> optionList = new List<Dropdown.OptionData>();
        [SerializeField] public Text label;
        public Text Date;
        [SerializeField] Text Complite;
        //�¼��������
        [SerializeField] public Text m_InputWord;
        [SerializeField] public GameObject _inputMessage;
        //�����¼��Ĳ㼶
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
            //��ʼ��Ԥ����
            PrefabInit();
            //��ʼ�������б�
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
            //��ʼ������
            label.text = DateTime.Now.ToString("d");
            //�н���Ͱѳ�ʼ��������Ϊ����
            if (Data._eventItemList.Exists(item => item._date == DateTime.Now.ToString("d")))
            {
                UpdateView(Data);
            }
            Complite.text = "0%";
            // Data���µĻ�ViewҲ����
            Data.AddListener(UpdateView);
        }
        /// <summary>
        /// ���½���
        /// </summary>
        /// <param name="Data"></param>
        public void UpdateView(DailyPlanModel Data)
        {
            List<DailyPlanModel.DEvent> todayList = Data._eventItemList.Find(item => item._date == label.text)._eventItemList;
            //��ʾ�б�
            CreatePlanPrefab(todayList);
        }
        void PrefabInit()
        {

            _inputMessage.SetActive(false);
            
        }
        /// <summary>
        /// ��˳������ʾ
        /// </summary>
        /// <param name="eventList">���յ��¼��б�</param>
        public void CreatePlanPrefab(List<DailyPlanModel.DEvent> eventList)
        {
            Date.text = label.text;
            int ChildCount = _eventPanel.transform.childCount;
            //���������Ѿ������Ķ���
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
            //���������
            Complite.text = DailyPlanModel.DData.CalculateCompletionRate(label.text);
        }
        public void CreateNewDropdown(string day)
        {
            //���������˵�����
            Dropdown.OptionData newOption = new Dropdown.OptionData();
            newOption.text = day;
            optionList.Add(newOption);
            //������������б��������б�����
            optionList.Sort((event1, event2) => DateTime.Compare(DateTime.Parse(event1.text), DateTime.Parse(event2.text)));
            _dropdown.options = optionList;
        }
    } 
}
