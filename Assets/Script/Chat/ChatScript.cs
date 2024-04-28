using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using OpenAI_FunctionCalling;
using static OpenAI_FunctionCalling.EmotionFunc;
using System.Security.AccessControl;
using Unity.VisualScripting.FullSerializer;
using PlanPanel;
using Live2D.Cubism.Core;
using Newtonsoft.Json;
using Unity.VisualScripting;
using System.IO;
using static PlanPanel.DailyPlanController;
namespace OpenAI_FunctionCalling
{
    public class ChatScript : MonoBehaviour
    {

        //API key
        [Header("输入openai的apikey")]
        [SerializeField] private string m_OpenAI_Key = "填写你的Key";
        // 定义Chat API的URL
        static public string m_ApiUrl = "https://openai.api2d.net/v1/chat/completions";
        //配置参数
        [SerializeField] private GetOpenAI.PostData m_PostDataSetting;
        //聊天UI层
        [SerializeField] private GameObject m_ChatPanel;
        //输入的信息
        [SerializeField] private InputField m_InputWord;
        //返回的信息
        [SerializeField] private Text m_TextBack;
        //计划表
        [SerializeField] private DailyPlanController DailyPlan;
        //记忆1
        [SerializeField] private string memory;
        private static ChatScript chatScript;
        public static ChatScript _ChatScript
        {
            get
            {
                if (chatScript == null)
                    chatScript = FindObjectOfType<ChatScript>();
                return chatScript;
            }
        }
        public void SetKey(string key)
        {
            m_OpenAI_Key = key;
        }
        public GetOpenAI.PostData GetSetting()
        {
            return m_PostDataSetting;
        }
        //发送信息
        public void SendData()
        {
            if (m_InputWord.text.Equals(""))
                return;
            //系统信息，并调用记忆
            GetNewMemory();
            GptTurboScript.SendData SysData = new("system", m_PostDataSetting.prompt);
            GptTurboScript.SendData MemData = new("system", "这是我们之间的记忆：" + memory);

            //记录聊天 
            m_ChatHistory.Add(m_InputWord.text);
            //聊天框的信息
            string _msg = m_InputWord.text;
            GptTurboScript.SendData UserData = new("user", _msg);
            //Post数据包
            GptTurboScript.PostData postData = new();
            postData.model = "gpt-3.5-turbo";
            postData.messages = new()
        {
            SysData,
            MemData,
            UserData
        };
            //发送数据、接收数据
            StartCoroutine(GetPostData(postData, m_TextBack, CallBack));

            //初始化
            m_InputWord.text = "";
            m_TextBack.text = "...";
        }
        public void GetNewMemory()
        {
            if (m_ChatHistory.Count <= 1)
                return;
            //string Commond = "请你总结对话和记忆缓存，关键信息保存为关键对话，无关紧要的总结成新缓存";
            string _predia = "请你总结对话和记忆缓存，关键信息保存为重要信息，无关紧要的总结成新缓存,以下是此前的对话";
            int StartIndex = Mathf.Max(m_ChatHistory.Count - 10, 0);
            for (int i = StartIndex; i <= m_ChatHistory.Count - 1; i++)
            {
                if (i % 2 == 1)
                    _predia += "你:";
                else
                    _predia += "我";
                _predia += m_ChatHistory[i];
                _predia += ";";
            }
            _predia = _predia + "记忆缓存：" + memory;
            //GptTurboScript.SendData SysData = new("user", Commond);
            GptTurboScript.SendData UserData = new("user", _predia);
            GptTurboScript.PostData postData = new();
            postData.model = "gpt-3.5-turbo";
            postData.messages = new()
        { 
            //SysData,
            UserData
        };
            StartCoroutine(GetMemoryData(postData, CallBackMemory));
        }


        //AI回复的信息
        public void CallBack(string _callback, Text m_TextBack)
        {
            _callback = _callback.Trim();
            m_TextBack.text = "";
            //开始逐个显示返回的文本
            m_WriteState = true;
            StartCoroutine(SetTextPerWord(_callback, m_TextBack));
            //记录聊天
            AddHistory(_callback);
        }
        public void AddHistory(string text)
        {
            m_ChatHistory.Add(text);
            string eventJSON = JsonConvert.SerializeObject(m_ChatHistory);
            File.WriteAllText(Application.streamingAssetsPath + "/ChatHistory.json", eventJSON);
        }

        /// <summary>
        /// 发送不带调用函数的信息
        /// </summary>
        /// <param name="postData">Post信息容器</param>
        /// <param name="m_TextBack">显示对话的Text</param>
        /// <param name="_callback">处理字符返回的函数，可以自定义</param>
        /// <returns></returns>
        public IEnumerator GetPostData(GptTurboScript.PostData postData, Text m_TextBack, System.Action<string, Text> _callback)
        {
            //Post
            using (UnityWebRequest request = new UnityWebRequest(m_ApiUrl, "POST"))
            {
                string _jsonText = JsonUtility.ToJson(postData);
                byte[] data = System.Text.Encoding.UTF8.GetBytes(_jsonText);
                request.uploadHandler = (UploadHandler)new UploadHandlerRaw(data);
                request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", string.Format("Bearer {0}", m_OpenAI_Key));

                yield return request.SendWebRequest();

                //发送完成后处理收到的信息
                if (request.responseCode == 200)
                {
                    string _msg = request.downloadHandler.text;
                    GptTurboScript.MessageBack _textback = JsonUtility.FromJson<GptTurboScript.MessageBack>(_msg);
                    if (_textback != null && _textback.choices.Count > 0)
                    {

                        string _backMsg = Regex.Replace(_textback.choices[0].message.content, @"[\r\n]", "").Replace("？", "");
                        _callback(_backMsg, m_TextBack);
                    }
                }
                else
                {
                    Debug.Log(request.error);
                }
            }
        }

        private void CallBackMemory(string _callback)
        {
            memory = _callback;
        }
        public IEnumerator GetMemoryData(GptTurboScript.PostData postData, System.Action<string> _callback)
        {
            //Post
            using (UnityWebRequest request = new UnityWebRequest(m_ApiUrl, "POST"))
            {
                string _jsonText = JsonUtility.ToJson(postData);
                byte[] data = System.Text.Encoding.UTF8.GetBytes(_jsonText);
                request.uploadHandler = (UploadHandler)new UploadHandlerRaw(data);
                request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", string.Format("Bearer {0}", m_OpenAI_Key));

                yield return request.SendWebRequest();

                //发送完成后处理收到的信息
                if (request.responseCode == 200)
                {
                    string _msg = request.downloadHandler.text;
                    GptTurboScript.MessageBack _textback = JsonUtility.FromJson<GptTurboScript.MessageBack>(_msg);
                    if (_textback != null && _textback.choices.Count > 0)
                    {

                        _callback(_textback.choices[0].message.content);

                    }
                }
                else
                {
                    Debug.Log(request.error);
                }
            }




        }

        public IEnumerator FunctionCalling(ChatGPTCompletionRequestModel chatGPTCompletionRequestModel, System.Action<string> _callback)
        {
            using (UnityWebRequest request = new UnityWebRequest(m_ApiUrl, "POST"))
            {

                /*string _jsonText = JsonUtility.ToJson(chatGPTCompletionRequestModel);*/
                string _jsonText = JsonConvert.SerializeObject(chatGPTCompletionRequestModel);
                byte[] data = System.Text.Encoding.UTF8.GetBytes(_jsonText);
                request.uploadHandler = (UploadHandler)new UploadHandlerRaw(data);
                request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", string.Format("Bearer {0}", m_OpenAI_Key));

                yield return request.SendWebRequest();

                //发送完成后处理收到的信息
                if (request.responseCode == 200)
                {
                    string _msg = request.downloadHandler.text;
                    ChatGPTResponseModel _textback = JsonUtility.FromJson<ChatGPTResponseModel>(_msg);
                    if (_textback != null && _textback.choices.Length > 0)
                    {
                        _callback(_textback.choices[0].message.function_call.arguments);
                    }
                }
                else
                {
                    Debug.Log(request.error);
                }
            }
        }

        #region 文字逐个显示以及自滚动
        [Header("文字显示")]
        //逐字显示的时间间隔
        [SerializeField] private float m_WordWaitTime = 0.4f;
        //是否显示完成
        [SerializeField] private bool m_WriteState = false;

        [Header("文字自滚动")]
        bool isStartScroll = false; //是否开始滚动
        [SerializeField] GameObject _content = null; //添加Text组件的Content
        float heightChange = 0;     //滚动的高度（根据当前文字数量和默认文本框的大小决定）
        public bool IsSpeeking = false;

        public IEnumerator SetTextPerWord(string _msg, Text m_TextBack)
        {
            int currentPos = 0;
            while (m_WriteState)
            {
                yield return new WaitForSeconds(m_WordWaitTime);
                IsSpeeking = true;
                //逐字
                currentPos++;
                if (_msg[currentPos - 1] == '，' || _msg[currentPos - 1] == '。')
                {
                    IsSpeeking = false;
                }
                m_TextBack.text = _msg.Substring(0, currentPos);
                m_WriteState = currentPos < _msg.Length;
                //处理滚动，每0.4秒都算一次高度差
                heightChange = _content.transform.parent.parent.GetComponent<RectTransform>().sizeDelta.y - _content.GetComponent<RectTransform>().sizeDelta.y;
                //当文字内容的y大小更大，那么就会滚动
                isStartScroll = (heightChange > 0) ? true : false;
                if (isStartScroll)
                    _content.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, heightChange);
            }
            IsSpeeking = false;
        }

        [SerializeField] CubismModel myModel;
        float t = 0;
        CubismParameter mouseParameter;
        [Range(-20f, 20f)]
        public float bodyAngle = 0f;

        void Start()
        {
            if (File.Exists(Application.streamingAssetsPath + "/ChatHistory.json"))
            {
                string json = "";
                using (StreamReader sr = new StreamReader(Application.streamingAssetsPath + "/ChatHistory.json"))
                { json = sr.ReadToEnd(); };
                if (json != "{}")
                    m_ChatHistory = JsonConvert.DeserializeObject<List<string>>(json);
            }
            mouseParameter = myModel.Parameters[18];
        }
        private void LateUpdate()
        {
            if (IsSpeeking)
                SpeekWithWord();
            else
                CloseMouth();
        }
        public void SpeekWithWord()
        {
            CubismParameter headParameter = myModel.Parameters[2];
            headParameter.Value = bodyAngle;
            t += Time.deltaTime * 10 * Random.Range(0.5f, 2.0f);
            float EndValue = Mathf.Sin(t) * 0.15f + 0.45f;
            mouseParameter.Value = EndValue;
        }
        public void CloseMouth()
        {
            mouseParameter.Value = Mathf.Lerp(mouseParameter.Value, 0, 0.5f * Time.deltaTime);
        }

        #endregion


        #region 聊天记录
        //保存聊天记录
        [SerializeField] private List<string> m_ChatHistory;
        //缓存已创建的聊天气泡
        [SerializeField] private List<GameObject> m_TempChatBox;
        //聊天记录显示层
        [SerializeField] private GameObject m_HistoryPanel;
        //聊天文本放置的层
        [SerializeField] private RectTransform m_rootTrans;
        //发送聊天气泡
        [SerializeField] private ChatPrefab m_PostChatPrefab;
        //回复的聊天气泡
        [SerializeField] private ChatPrefab m_RobotChatPrefab;
        //滚动条
        [SerializeField] private ScrollRect m_ScroTectObject;
        //获取聊天记录
        public void OpenAndGetHistory()
        {

            ClearChatBox();
            StartCoroutine(GetHistoryChatInfo());
        }
        //返回
        public void BackChatMode()
        {
            m_ChatPanel.SetActive(true);
            m_HistoryPanel.SetActive(false);
        }

        //清空已创建的对话框
        public void ClearChatBox()
        {
            while (m_TempChatBox.Count != 0)
            {
                if (m_TempChatBox[0])
                {
                    Destroy(m_TempChatBox[0].gameObject);
                    m_TempChatBox.RemoveAt(0);
                }
            }
            m_TempChatBox.Clear();
        }

        //获取聊天记录列表
        private IEnumerator GetHistoryChatInfo()
        {

            yield return new WaitForEndOfFrame();

            for (int i = 0; i < m_ChatHistory.Count; i++)
            {
                if (i % 2 == 0)
                {
                    ChatPrefab _sendChat = Instantiate(m_PostChatPrefab, m_rootTrans.transform);
                    _sendChat.SetText(m_ChatHistory[i]);
                    m_TempChatBox.Add(_sendChat.gameObject);
                    continue;
                }
                ChatPrefab _reChat = Instantiate(m_RobotChatPrefab, m_rootTrans.transform);
                _reChat.SetText(m_ChatHistory[i]);
                m_TempChatBox.Add(_reChat.gameObject);
            }

            //重新计算容器尺寸
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_rootTrans);
            StartCoroutine(TurnToLastLine());
        }

        private IEnumerator TurnToLastLine()
        {
            yield return new WaitForEndOfFrame();
            //滚动到最近的消息
            m_ScroTectObject.verticalNormalizedPosition = 0;
        }


        #endregion


    }
}