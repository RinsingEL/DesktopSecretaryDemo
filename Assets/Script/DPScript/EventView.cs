using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EventView : MonoBehaviour, IPointerDownHandler
{
    public Button DeleteBut;
    public Toggle IsFinish;
    public Button FocusBut;
    public Text _eventName;
    public Text _eventTime;
    public InputField _modifyName;
    public InputField _modifyTime;


    void Awake()
    {
        _modifyName.gameObject.SetActive(false);
        _modifyTime.gameObject.SetActive(false);
    }
    //˫���޸��¼������¼�ʱ��
    public void OnPointerDown(PointerEventData eventData)
    {
        GameObject currentObject = eventData.pointerCurrentRaycast.gameObject;
        //˫�����ѶԻ���
        if (eventData.clickCount == 2)
        {
            if (currentObject == _eventName.gameObject)
                _modifyName.gameObject.SetActive(true);
            else if (currentObject == _eventTime.gameObject)
                _modifyTime.gameObject.SetActive(true);
        }
    }
}
