using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonthPlanController : MonoBehaviour
{
    public Button PreButton;
    public Button NextButton;
    void Start()
    {
        PreButton.onClick.AddListener(MonthPlanView.MView.UpMouth);
        NextButton.onClick.AddListener(MonthPlanView.MView.DownMouth);
    }
}
