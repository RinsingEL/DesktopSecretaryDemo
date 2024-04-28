using OpenAI_FunctionCalling;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static OpenAI_FunctionCalling.FunctionCalling;
using static PlanPanel.DailyPlanModel;
using static PlanPanel.DailyPlanView;
namespace PlanPanel 
{
    public class DailyPlanController : MonoBehaviour
    {
        //交互按钮
        [SerializeField] Button addEventBut;
        [SerializeField] Button SetPlanBut;
        void Awake()
        {
            addEventBut.onClick.AddListener(PutOutInput);
            SetPlanBut.onClick.AddListener(SetDailyPlan);
        }

        /// <summary>
        /// 添加今天事件的按钮逻辑,修改Model的Data
        /// </summary>
        public void PutOutInput()
        {
            //第一次点击按钮打开输入框
            if (!DView._inputMessage.activeSelf)
            {
                DView._inputMessage.SetActive(true);
                return;
            }
            if (DView.m_InputWord.text == "")
                return;
            //没有label日期就先创建列表
            if (!DData._eventItemList.Exists(day => day._date == DView.label.text))
            {
                //先创建今天列表
                DData.CreateNewList(DView.m_InputWord.text);
                //再插入到今天的列表
                DData.InsertNewEvent(DView.m_InputWord.text, DView.label.text);
                //创建今天的下拉菜单日期
                DView.CreateNewDropdown(DateTime.Now.ToString());
            }
            //有就加入已经创建的日程列表的项目列表里
            else
            {
                DData.InsertNewEvent(DView.m_InputWord.text, DView.label.text);
            }
            DView.m_InputWord.text = "";

        }
        #region gpt调用，自动规划时间
        public void SetDailyPlan()
        {
            //缓存发送的信息列表
            List<ChatGPTMessageModel> m_message = new List<ChatGPTMessageModel>();
            ChatGPTMessageModel chatGPTMessageModel = new ChatGPTMessageModel("assistants", "帮我安排日程，能更科学地修改我的日程表");

            // 使用 string.Join 将列表中的元素连接成一个字符串
            string concatenatedEventNames = string.Empty;
            foreach (DEvent e in DData._eventItemList.Find(item => item._date == DailyPlanView.DView.label.text)._eventItemList)
                concatenatedEventNames += e._eventName;
            // 获取 _eventTime 列表
            string concatenatedEventTimes = string.Empty;
            foreach (DEvent e in DData._eventItemList.Find(item => item._date == DailyPlanView.DView.label.text)._eventItemList)
            {
                if (e._eventTime == "00:00")
                    concatenatedEventTimes += "Not scheduled";
                else
                    concatenatedEventTimes += e._eventTime;
            }
            // 使用 string.Join 将列表中的元素连接成一个字符串

            ChatGPTMessageModel chatGPTMessageMode2 = new ChatGPTMessageModel("user", "temperature = 0.1 ,Please help me modify the schedule below:" + "there are only：" + concatenatedEventNames + "；The times are：" + concatenatedEventTimes);
            /*m_message.Add(chatGPTMessageModel);*/
            m_message.Add(chatGPTMessageMode2);
            FunctionCalling newFunc = new FunctionCalling()
            {
                name = "SetPlan",
                description = "Insert unscheduled events into free places in the schedule",
                parameters = new Parameters()
                {
                    properties = new PropertiesP(),
                    required = new string[] { "timetable", "eventlist" }

                }

            };
            FunctionCalling[] Eventfunction = { newFunc };
            //用_postData获取inspect里的PostData 
            ChatGPTCompletionRequestModel chatGPTCompletionRequestModel = new ChatGPTCompletionRequestModel
            {
                model = ChatScript._ChatScript.GetSetting().model,
                messages = m_message,
                functions = Eventfunction,
                function_call = new ChatGPTCompletionRequestModel.Function_call() { name = "SetPlan" }

            };
            StartCoroutine(ChatScript._ChatScript.FunctionCalling(chatGPTCompletionRequestModel, callback));

        }
        /// <summary>
        /// GPT类
        /// </summary>
        [Serializable]
        public class PropertiesP : Properties
        {
            public TimeTable timeTable = new();
            public EventList eventList = new();
            [Serializable]
            public class TimeTable
            {
                public string type = "string";
                public string description = "Time list, the time must be in the format of \"hh:mm\"";
            }
            [Serializable]
            public class EventList
            {
                public string type = "string";
                public string description = "Event list, one-to-one correspondence with the schedule";
            }
        }
        [Serializable]
        public class ArgumentsClass
        {
            public string timeTable;
            public string eventList;
        }
        ArgumentsClass obj;
        void callback(string input)
        {
            obj = JsonUtility.FromJson<ArgumentsClass>(input);
            string t = obj.timeTable;
            List<string> tlist = new(t.Split(","));
            int i = 0;
            foreach (string s in tlist)
            {
                DData._eventItemList.Find(item => item._date == DView.label.text)._eventItemList[i]._eventTime = s;
                i++;
            }
            List<DEvent> todayList = DData._eventItemList.Find(item => item._date == DView.label.text)._eventItemList;
            //显示列表
            DailyPlanView.DView.CreatePlanPrefab(todayList);
        }
        #endregion
    }

}


