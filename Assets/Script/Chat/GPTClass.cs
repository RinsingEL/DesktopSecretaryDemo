using System.Collections.Generic;
using System;
using OpenAI_FunctionCalling;

namespace OpenAI_FunctionCalling
{
    /// <summary>
    /// �����࣬��������name��description��Parameter���ࣩ
    /// </summary>
    [Serializable]
    public class FunctionCalling
    {
        public string name;
        /*public string description = "Output Reply , 5 Emotions and the time to the user";*/
        public string description;
        /// <summary>
        /// ��Ҫ�Լ����properties��required;
        /// </summary>
        public Parameters parameters;
        [Serializable]
        public class Parameters
        {
            public string type = "object";
            /// <summary>
            /// ���д����࣬�̳�Properties
            /// </summary>
            public Properties properties;
            public string[] required;
        }
        [Serializable]
        public class Properties
        {

        }
    }
    [Serializable]
    public class IsZoneOut:FunctionCalling
    {
        public IsZoneOut()
        {
            name = "CheckZoneOut";
            description = "Determine whether browsing the webpage with the title below is helpful for learning.If it is useful, give user affirmation.If it is not, remind the user.";
            parameters = new ParametersZO();
        }
        [Serializable]
        public class ParametersZO: Parameters
        {
            public ParametersZO()
            {
                properties = new PropertiesZO();
                required = new string[] {
                "content", "result"
                };
            }
            [Serializable]
            public class PropertiesZO:Properties
            {
                public Result result = new();
                public Content content = new();
                [Serializable]
                public class Result
                {
                    public string type = "string";
                    public string description = "Determine whether every title is helpful to learning,only output 'true' and 'false'";
                }
                [Serializable]
                public class Content
                {
                    public string type = "string";
                    public string description = "Your reply to the user in three to four sentences in Chinese";
                }

            }

        }
    }
    // FunctionCalling���v����JsonScheme�Ƕ��x
    [Serializable]
    public class EmotionFunc:FunctionCalling
    {
        public EmotionFunc()
        {
            name = "getArguments";
            description = "Output reply content , 5 Emotions to the user,and scientifically create the timetable based on the event table";
            parameters = new Parameters();
        }

        [Serializable]
        public class ParametersE: Parameters
        {
            public ParametersE()
            {
                properties = new PropertiesE();
                required = new string[] {
                "content", "eventlist" ,"joyful", "anger", "shy", "anxiety","timetable"
                };
            }
        }

        [Serializable]
        public class PropertiesE:Properties
        {
            public Content reply = new Content();
            public EventList eventlist = new EventList();
            public Joyful joyful = new Joyful();
            public Anger anger = new Anger();
            public Shy shy = new Shy();
            public Anxiety anxiety = new Anxiety();
            public Timetable timetable = new Timetable();

            [Serializable]
            public class Content
            { 
                public string type = "string";
                public string description = "Text to reply to user in Chinese.";
            }
            public class EventList
            {
                public string type = "string";
                public string description = "Event list";
            }

            [Serializable]
            public class Joyful
            {
                public string type = "number";
                public string description = "Output joyful emotion as a value from 0 to 10.";
            }

            [Serializable]
            public class Anger
            {
                public string type = "number";
                public string description = "Output anger emotion as a value from 0 to 10.";
            }

            [Serializable]
            public class Shy
            {
                public string type = "number";
                public string description = "Output shy emotion as a value from 0 to 10.";
            }

            [Serializable]
            public class Anxiety
            {
                public string type = "number";
                public string description = "Output anxiety emotion as a value from 0 to 10.";
            }
            [Serializable]
            public class Timetable
            {
                public string type = "string";
                public string description = "Output all the time modified,The format is 'hh:mm'";
            }
        }
    }
    [Serializable]
    public class PlanSetFunc : FunctionCalling
    {
        public PlanSetFunc()
        {
            name = "�ƶ��ƻ�";
            description = "��δ����ʱ����¼������뵽ʱ����п��еĵط�";

        }
        [Serializable]
        public class ParametersP : Parameters
        {
            public ParametersP()
            {
                properties = new PropertiesP();
                required = new string[] {
                "timetable","eventlist"
                };
            }
        }
        //
        public class PropertiesP: Properties
        {
            public TimeTable timeTable = new();
            public EventList eventList = new();
            public class TimeTable
            {
                public string type = "string";
                public string description = "ʱ���б�ʱ��һ����\"hh:mm\"�ĸ�ʽ";
            }
            public class EventList
            {
                public string type = "string";
                public string description = "�¼��б���ʱ���һһ��Ӧ";
            }
        }
    }
}

//ChatGPT������
[Serializable]
public class ChatGPTCompletionRequestModel
{
    public string model;
    public List<ChatGPTMessageModel> messages;
    public FunctionCalling[] functions;
    public Function_call function_call;

    [Serializable]
    public class Function_call
    {
        public string name = "getArguments";
    }
}

[Serializable]
public class ChatGPTMessageModel
{
    public string role;
    public string content;
    public ChatGPTMessageModel(string _role, string _content)
    {
        role = _role;
        content = _content;
    }
}

//ChatGPT API�����Response���ܤ�ȡ�뤿��Υ��饹
[Serializable]
public class ChatGPTResponseModel
{
    public string id;
    public string @object;
    public int created;
    public string model;
    public Choice[] choices;
    public Usage usage;

    [Serializable]
    public class Choice
    {
        public int index;
        public ResponseChatGPTMessageModel message;
        public string finish_reason;
    }

    [Serializable]
    public class Usage
    {
        public int prompt_tokens;
        public int completion_tokens;
        public int total_tokens;
    }
}

[Serializable]
public class ResponseChatGPTMessageModel
{
    public string role;
    public string content;
    public int max_tokens = 100;
    public Function_call function_call;

    [Serializable]
    public class Function_call
    {
        public string name;
        public string arguments;
    }
}

// getJsonResponse()���������饹
[Serializable]
public class ArgumentsClass
{
    public string content;
    public int joyful;
    public int anger;
    public int shy;
    public int anxiety;
}
