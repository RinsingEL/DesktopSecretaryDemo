
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
        //日表
        public class EventItemList
        {
            public string _date;
            public List<DEvent> _eventItemList = new List<DEvent>();
            public int completedCount = 0;
        }

        public List<EventItemList> _eventItemList = new List<EventItemList>();
        //响应更新数据的事件
        private event UnityAction<DailyPlanModel> updateEvent;
        //单例化
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
        //初始化
        void Init()
        {
            if (File.Exists(Application.streamingAssetsPath + "/List.json"))
            {
                string json = "";
                //读取List中的信息
                using (StreamReader sr = new StreamReader(Application.streamingAssetsPath + "/List.json"))
                { json = sr.ReadToEnd(); };
                if (json != "{}")
                    _eventItemList = JsonConvert.DeserializeObject<List<EventItemList>>(json);
            }
            //更新数据时马上保存
            AddListener(SaveData);
        }
        //添加订阅
        public void AddListener(UnityAction<DailyPlanModel> func)
        {
            updateEvent += func;
        }
        public void ReMoveListener(UnityAction<DailyPlanModel> func)
        {
            updateEvent -= func;
        }
        /// <summary>
        /// 这个方法能够发出提醒，提醒更新
        /// </summary>
        private void UpdateInfo()
        {
            if (updateEvent != null)
            {
                updateEvent(this);
            }
        }
        //修改
        public void CreateNewList(string date)
        {
            //创建date的事件列表
            EventItemList eventItemList = new EventItemList();
            eventItemList._date = date;
            DData._eventItemList.Add(eventItemList);
            //对列表进行排序
            DData._eventItemList.Sort((event1, event2) => DateTime.Compare(DateTime.Parse(event1._date), DateTime.Parse(event2._date)));
            SaveData(this);
        }
        public void InsertNewEvent(string _input,string date)
        {
            //创建新的事件项目
            string eventName = _input;//输入内容
            DEvent newEvent = new DEvent()
            {
                // 使用GUID作为唯一标识符
                _eventID = Guid.NewGuid().ToString(),
                _eventName = eventName,
                _eventTime = "00:00"
            };
            _eventItemList.Find(day => day._date == date)._eventItemList.Add(newEvent);
            //排序各个事件
            _eventItemList.Find(day => day._date == date).
            _eventItemList.Sort((event1, event2) => DateTime.Compare(DateTime.Parse(event1._eventTime), DateTime.Parse(event2._eventTime)));
            //发出事件提醒，通知大家已经更新了数据
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
        /// 计算date的完成率
        /// </summary>
        /// <param name="date">输入日期</param>
        public string CalculateCompletionRate(string date)
        {
            //今天的事件个数
            float TodayRate = 0f;
            float cCount = ++_eventItemList.Find(info => info._date == date).completedCount;
            float eCount = _eventItemList.Find(info => info._date == date)._eventItemList.Count;
            if (eCount != 0)
                //计算完成率
                TodayRate = cCount / eCount;
            return TodayRate.ToString("0%");
        }
        private void SaveData(DailyPlanModel arg0)
        {
            //结果写入List.Json中
            string eventJSON = JsonConvert.SerializeObject(_eventItemList);
            File.WriteAllText(Application.streamingAssetsPath + "/List.json", eventJSON);
        }
    }
}
