using Microsoft.Win32;
using OpenAI_FunctionCalling;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    public InputField inputField;
    public Toggle IsOpenWithWin;
    public Toggle IsOpenTarget;
    [SerializeField] GameObject Target;
    private void Awake()
    {
        IsOpenTarget.isOn = true;
        IsOpenWithWin.onValueChanged.AddListener(delegate {
            if (IsOpenWithWin.isOn)
                OpenWithWin();
            else
                NotOpenWithWin();
        });
        IsOpenTarget.onValueChanged.AddListener(delegate
        {
            Target.SetActive(IsOpenTarget.isOn);
        }
        );
        inputField.onEndEdit.AddListener(delegate {
            ChatScript._ChatScript.SetKey(inputField.text);
        });
    }
    public void CloseTarget()
    {
        IsOpenTarget.isOn = false;
    }
    public static void OpenWithWin()
    {
        // ��ӵ� ��ǰ��½�û��� ע���������
        RegistryKey RKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
        /*RKey.SetValue("DesktopSecretary", @"F:\Game Build\DesktopPet_t2\DesktopSecretary.exe");*/
        
        RKey.SetValue("DesktopSecretary", System.Environment.CurrentDirectory+@"\DesktopSecretary.exe");
    }
    public static void NotOpenWithWin()
    {
        // ��ӵ� ��ǰ��½�û��� ע���������
        RegistryKey RKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
        RKey.DeleteValue("DesktopSecretary");
    }
}
