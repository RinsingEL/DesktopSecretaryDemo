
using UnityEngine;
using UnityEngine.EventSystems;
using static PlanPanel.DailyPlanModel;

public class EventController : MonoBehaviour,IPointerExitHandler,IPointerEnterHandler
{
    //Model
    public string _eventID;
    public string date;
    public DEvent curPlan;
    public bool isFinish;
    public EventView view;

    bool IsOutSide=true;
    bool IsModify=false;
    void Awake()
    {
        if(view==null)
            view = GetComponent<EventView>();
        view.DeleteBut.onClick.AddListener(delegate { DData.DeleteEvent(_eventID,date); });
        view.FocusBut.onClick.AddListener(EnterFocus);
        if (isFinish)
        {
            view.IsFinish.isOn = true;
        }
        else
        {
            view.IsFinish.isOn = false;
        }
        view.IsFinish.onValueChanged.AddListener(SetComplite);

    }
    void Update()
    {
        CheckModify();
        CompleteEdit();
    }
    //Íê³ÉÐÞ¸Ä
    void CompleteEdit()
    {
        if (IsOutSide && Input.GetMouseButton(0) && IsModify)
        {
            if (view._modifyName.text != "")
            {
                view._eventName.text = view._modifyName.text;
                view._modifyName.gameObject.SetActive(false);
                DData.ModifyEventName(view._modifyName.text, _eventID, date);
                view._modifyName.text = "";
            }
            else if (view._modifyTime.text != "")
            {
                view._eventTime.text = view._modifyTime.text;
                view._modifyTime.gameObject.SetActive(false);
                DData.ModifyEventTime(view._modifyTime.text, _eventID, date);
                view._modifyTime.text = "";
            }
            else
            {
                view._modifyName.gameObject.SetActive(false);
                view._modifyTime.gameObject.SetActive(false);
            }
        }
    }
    public void SetComplite(bool input)
    {
        isFinish = input;
        DData.SetEventComplite(isFinish, _eventID, date);
    }
    public void EnterFocus()
    {
        UIManager.Instance.FocusButton();
        Focus.Instance.curEvent = curPlan;
        FocusView.View.curName.text = view._eventName.text;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        IsOutSide = false;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        IsOutSide = true;
    } 
    public void CheckModify()
    {
        IsModify = (view._modifyName.IsActive() || view._modifyTime.IsActive())?true:false;
    }

}
