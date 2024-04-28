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
        //������ť
        [SerializeField] Button addEventBut;
        [SerializeField] Button SetPlanBut;
        void Awake()
        {
            addEventBut.onClick.AddListener(PutOutInput);
            SetPlanBut.onClick.AddListener(SetDailyPlan);
        }

        /// <summary>
        /// ��ӽ����¼��İ�ť�߼�,�޸�Model��Data
        /// </summary>
        public void PutOutInput()
        {
            //��һ�ε����ť�������
            if (!DView._inputMessage.activeSelf)
            {
                DView._inputMessage.SetActive(true);
                return;
            }
            if (DView.m_InputWord.text == "")
                return;
            //û��label���ھ��ȴ����б�
            if (!DData._eventItemList.Exists(day => day._date == DView.label.text))
            {
                //�ȴ��������б�
                DData.CreateNewList(DView.m_InputWord.text);
                //�ٲ��뵽������б�
                DData.InsertNewEvent(DView.m_InputWord.text, DView.label.text);
                //��������������˵�����
                DView.CreateNewDropdown(DateTime.Now.ToString());
            }
            //�оͼ����Ѿ��������ճ��б����Ŀ�б���
            else
            {
                DData.InsertNewEvent(DView.m_InputWord.text, DView.label.text);
            }
            DView.m_InputWord.text = "";

        }
        #region gpt���ã��Զ��滮ʱ��
        public void SetDailyPlan()
        {
            //���淢�͵���Ϣ�б�
            List<ChatGPTMessageModel> m_message = new List<ChatGPTMessageModel>();
            ChatGPTMessageModel chatGPTMessageModel = new ChatGPTMessageModel("assistants", "���Ұ����ճ̣��ܸ���ѧ���޸��ҵ��ճ̱�");

            // ʹ�� string.Join ���б��е�Ԫ�����ӳ�һ���ַ���
            string concatenatedEventNames = string.Empty;
            foreach (DEvent e in DData._eventItemList.Find(item => item._date == DailyPlanView.DView.label.text)._eventItemList)
                concatenatedEventNames += e._eventName;
            // ��ȡ _eventTime �б�
            string concatenatedEventTimes = string.Empty;
            foreach (DEvent e in DData._eventItemList.Find(item => item._date == DailyPlanView.DView.label.text)._eventItemList)
            {
                if (e._eventTime == "00:00")
                    concatenatedEventTimes += "Not scheduled";
                else
                    concatenatedEventTimes += e._eventTime;
            }
            // ʹ�� string.Join ���б��е�Ԫ�����ӳ�һ���ַ���

            ChatGPTMessageModel chatGPTMessageMode2 = new ChatGPTMessageModel("user", "temperature = 0.1 ,Please help me modify the schedule below:" + "there are only��" + concatenatedEventNames + "��The times are��" + concatenatedEventTimes);
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
            //��_postData��ȡinspect���PostData 
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
        /// GPT��
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
            //��ʾ�б�
            DailyPlanView.DView.CreatePlanPrefab(todayList);
        }
        #endregion
    }

}


