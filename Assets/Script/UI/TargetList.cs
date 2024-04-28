using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class TargetList : MonoBehaviour
{
    [SerializeField] GameObject inputobj;
    [SerializeField] InputField inputName;
    [SerializeField] InputField inputTime;
    [SerializeField] GameObject panel;
    [SerializeField] GameObject prefab;
    public class Target
    {
        public string Name;
        public string SpendTime;
        public string EventID;
        public Target(string name, string spendTime)
        {
            Name = name;
            SpendTime = spendTime;
            EventID = Guid.NewGuid().ToString();
        }
    }
    public List<Target> targets = new List<Target>();
    private static TargetList instance;
    public static TargetList Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<TargetList>();
            return instance;
        }

    }

    private void Start()
    {
        inputobj.SetActive(false);
        if(File.Exists(Application.streamingAssetsPath + "/Targets.json"))
            ReadList();
    }
    private void ReadList()
    {
        string json = "";
        //读取List中的信息
        using (StreamReader sr = new StreamReader(Application.streamingAssetsPath + "/Targets.json"))
        { json = sr.ReadToEnd(); };
         if (json != "{}")
            targets = JsonConvert.DeserializeObject<List<Target>>(json);
        ShowTarget();
    }
    private void SaveList()
    {
        string Json = JsonConvert.SerializeObject(targets);
        File.WriteAllText(Application.streamingAssetsPath + "/Targets.json", Json);
    }
    
    public void AddTargetBut()
    {
        if (!inputobj.activeSelf)
        {
            inputobj.SetActive(true);
            return;
        }
        InputTarget();
        SaveList();
        ShowTarget();
    }
    public void DeleteTargetBut(string id)
    {
        targets.Remove(Instance.targets.Find(item => item.EventID == id));
        SaveList();
        ShowTarget();
    }
    public void InputTarget()
    {
        if (inputName.text == "")
        {
            inputobj.SetActive(false);
            return;
        } 
        string name = inputName.text;
        string spendtime = inputTime.text;
        Target newtarget = new Target(name,spendtime);
        targets.Add(newtarget);
        inputName.text="";
        inputTime.text="";
    }
    public void ShowTarget()
    {
        ClearFront();
        foreach(Target target in targets)
        {
            GameObject tObj =  Instantiate(prefab, panel.transform);
            tObj.transform.Find("name").GetComponent<Text>().text = target.Name;
            tObj.transform.Find("time").GetComponent<Text>().text = target.SpendTime;
            tObj.transform.Find("Delete").GetComponent<Button>().onClick.AddListener(delegate { DeleteTargetBut(target.EventID); });
        }
        
    }
    private void ClearFront()
    {
        for (int i = 0; i < panel.transform.childCount; i++)
            Destroy(panel.transform.GetChild(i).gameObject);
    }
}
