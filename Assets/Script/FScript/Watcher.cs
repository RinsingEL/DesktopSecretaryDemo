using Mono.Data.Sqlite;
using OpenAI_FunctionCalling;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static OpenAI_FunctionCalling.IsZoneOut;

public class Watcher : MonoBehaviour
{
    //���
    bool Iszoneout = false;
    float zoneOutTime;
    protected Mono.Data.Sqlite.SqliteConnection _sqlConn;
    string dbPath = Application.streamingAssetsPath + "/History.db";
    private static Watcher instance;
    public static Watcher Instance
    {
        get
        {
            if(instance==null)
            {
                instance = FindObjectOfType<Watcher>();
            }
            return instance;
        }
    }
    private void Awake()
    {
        ChromeHistory = GetHistory();
        Focus.Instance.Startfocus += StartWatch;
        Focus.Instance.Stopfocus += StopWatch;
    }
    void InputHistory()
    {
        string sourceFilePath = "C:\\Users\\64379\\AppData\\Local\\Google\\Chrome\\User Data\\Default\\History"; // ԭʼ�ļ�·��
        string destinationFilePath = Application.streamingAssetsPath + "/History.db"; // Ŀ���ļ�·��

        try
        {

            File.Copy(sourceFilePath, destinationFilePath, true);
            Debug.Log("�ļ����Ƴɹ���");
        }
        catch (Exception ex)
        {
            Debug.Log("��������" + ex.Message);
        }
    }
    string ChromeHistory = "";
    public string GetHistory()
    {
        InputHistory();
        _sqlConn = new SqliteConnection(new SqliteConnectionStringBuilder() { DataSource = dbPath }.ToString());
        _sqlConn.Open();
        SqliteCommand command = new SqliteCommand("SELECT * FROM urls ORDER BY id DESC LIMIT 3;", _sqlConn);
        //ִ�в�ȡ�ý��
        var reader = command.ExecuteReader();
        string res = "����ĵ�����������3�����������ҳ�ı���:";
        int index = 1;
        while (reader.Read())
        {
            //��ȡ�ڶ�������
            var obj = reader[2];
            res = res + index + "��" + (obj.ToString());
            index++;
        }
        reader.Close();
        return res;
    }

    public void CheckHistory()
    {
        string curHis = string.Empty;
        curHis = GetHistory();
        //�������������ҳ�ж�
        if (ChromeHistory != curHis)
        {
            ChromeHistory = curHis;
            //���͸�GPT�ж��Ƿ����
            curHis = ChatScript._ChatScript.GetSetting().prompt + "����ԭ�ƻ���" + FocusView.View.curName.text + "��" + curHis;
            ChatGPTMessageModel UserData = new ChatGPTMessageModel("system", curHis);
            List<ChatGPTMessageModel> ChatList = new List<ChatGPTMessageModel>()
        {
            UserData
        };
            IsZoneOut isZoneOut = new()
            {
                parameters = new ParametersZO()
            };
            FunctionCalling[] functions = { isZoneOut };
            ChatGPTCompletionRequestModel.Function_call function_Call = new ChatGPTCompletionRequestModel.Function_call()
            {
                name = "CheckZoneOut"
            };
            ChatGPTCompletionRequestModel chatGPTCompletionRequestModel = new()
            {
                model = ChatScript._ChatScript.GetSetting().model,
                messages = ChatList,
                functions = functions,
                function_call = function_Call
            };
            StartCoroutine(ChatScript._ChatScript.FunctionCalling(chatGPTCompletionRequestModel, callback));
            if (Iszoneout)
                ;
        }
    }
    void callback(string isLeaning)
    {
        string res = JsonUtility.FromJson<ArgumentsClass>(isLeaning).content;
        if (res == null)
            res = "������Ҫרע��";
        StartCoroutine(RemindView.Instance.remindFunc(res));
        if ("false" == JsonUtility.FromJson<ArgumentsClass>(isLeaning).result)
            zoneOutTime++;
    }
    public class ArgumentsClass
    {
        public string result;
        public string content;
    }
    IEnumerator WatchCor()
    {
        while (true)
        {
            float time = UnityEngine.Random.Range(300, 600);
            yield return new WaitForSeconds(time);
            CheckHistory();
        }
    }
    Coroutine curCoroutine;
    void StartWatch()
    {
        curCoroutine = StartCoroutine(WatchCor());
    }
    void StopWatch()
    {
        StopCoroutine(curCoroutine);
    }
}
