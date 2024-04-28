
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

namespace PlanPanel
{
    public class DailyPlanModel
    {
        public class DEvent
        {
            public string _eventID;
            public string _eventName;
            public string _eventTime;
            public bool isFinish = false;
        }
        //�ձ�
        public class EventItemList
        {
            public string _date;
            public List<DEvent> _eventItemList = new List<DEvent>();
            public int completedCount = 0;
        }

        public List<EventItemList> _eventItemList = new List<EventItemList>();
        //��Ӧ�������ݵ��¼�
        private event UnityAction<DailyPlanModel> updateEvent;
        //������
        private static DailyPlanModel Ddata;
        public static DailyPlanModel DData
        {
            get
            {
                if (Ddata == null)
                {
                    DailyPlanModel obj = new DailyPlanModel();
                    obj.Init();
                    Ddata = obj;
                }
                return Ddata;
            }
        }
        //��ʼ��
        void Init()
        {
            if (File.Exists(Application.streamingAssetsPath + "/List.json"))
            {
                string json = "";
                //��ȡList�е���Ϣ
                using (StreamReader sr = new StreamReader(Application.streamingAssetsPath + "/List.json"))
                { json = sr.ReadToEnd(); };
                if (json != "{}")
                    _eventItemList = JsonConvert.DeserializeObject<List<EventItemList>>(json);
            }
            //��������ʱ���ϱ���
            AddListener(SaveData);
        }
        //��Ӷ���
        public void AddListener(UnityAction<DailyPlanModel> func)
        {
            updateEvent += func;
        }
        public void ReMoveListener(UnityAction<DailyPlanModel> func)
        {
            updateEvent -= func;
        }
        /// <summary>
        /// ��������ܹ��������ѣ����Ѹ���
        /// </summary>
        private void UpdateInfo()
        {
            if (updateEvent != null)
            {
                updateEvent(this);
            }
        }
        //�޸�
        public void CreateNewList(string date)
        {
            //����date���¼��б�
            EventItemList eventItemList = new EventItemList();
            eventItemList._date = date;
            DData._eventItemList.Add(eventItemList);
            //���б��������
            DData._eventItemList.Sort((event1, event2) => DateTime.Compare(DateTime.Parse(event1._date), DateTime.Parse(event2._date)));
            SaveData(this);
        }
        public void InsertNewEvent(string _input,string date)
        {
            //�����µ��¼���Ŀ
            string eventName = _input;//��������
            DEvent newEvent = new DEvent()
            {
                // ʹ��GUID��ΪΨһ��ʶ��
                _eventID = Guid.NewGuid().ToString(),
                _eventName = eventName,
                _eventTime = "00:00"
            };
            _eventItemList.Find(day => day._date == date)._eventItemList.Add(newEvent);
            //��������¼�
            _eventItemList.Find(day => day._date == date).
            _eventItemList.Sort((event1, event2) => DateTime.Compare(DateTime.Parse(event1._eventTime), DateTime.Parse(event2._eventTime)));
            //�����¼����ѣ�֪ͨ����Ѿ�����������
            UpdateInfo();
        }
        public void ModifyEventName(string _input,string id,string date)
        {
            DEvent cur = _eventItemList.Find(item => item._date == date)._eventItemList
            .Find(item => item._eventID == id);
            cur._eventName = _input;
            UpdateInfo();
        }
        public void ModifyEventTime(string _input, string id,string date)
        {
            DEvent cur = _eventItemList.Find(item => item._date == date)._eventItemList
            .Find(item => item._eventID == id);
            cur._eventTime = _input;
            UpdateInfo();
        }
        public void SetEventComplite(bool f,string id, string date)
        {
            List<DEvent> todayList = _eventItemList.Find(item => item._date == date)._eventItemList;
            if (todayList.Exists(item => item._eventID == id))
                todayList.Find(item => item._eventID == id).isFinish = f;
        }
        public void DeleteEvent(string id,string date)
        {
            List<DEvent> todayList = _eventItemList.Find(item => item._date == date)._eventItemList;
            if (todayList.Exists(item => item._eventID == id))
                todayList.Remove(todayList.Find(item => item._eventID == id));
            UpdateInfo();
        }
        /// <summary>
        /// ����date�������
        /// </summary>
        /// <param name="date">��������</param>
        public string CalculateCompletionRate(string date)
        {
            //������¼�����
            float TodayRate = 0f;
            float cCount = ++_eventItemList.Find(info => info._date == date).completedCount;
            float eCount = _eventItemList.Find(info => info._date == date)._eventItemList.Count;
            if (eCount != 0)
                //���������
                TodayRate = cCount / eCount;
            return TodayRate.ToString("0%");
        }
        private void SaveData(DailyPlanModel arg0)
        {
            //���д��List.Json��
            string eventJSON = JsonConvert.SerializeObject(_eventItemList);
            File.WriteAllText(Application.streamingAssetsPath + "/List.json", eventJSON);
        }
    }
}
