using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using UnityEngine;
using UnityEngine.UI;

public class FocusView : MonoBehaviour
{
    [SerializeField] Image conimage;
    //Ãæ°å
    public GameObject panel;
    public Text curName;
    public Text curTime;
    private static FocusView view;
    public static FocusView View
    {
        get
        {
            if (view == null)
                view = FindObjectOfType<FocusView>();
            return view;
        }
    }

    private void Awake()
    {
        
        curTime.text = "00:00";
        curName.text = " ";
    }
    public void ChangeImage(Sprite img)
    {
        conimage.sprite = img;
    }
}
