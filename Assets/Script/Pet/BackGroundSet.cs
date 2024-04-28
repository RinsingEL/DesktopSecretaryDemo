using UnityEngine;
using System.Runtime.InteropServices; // Ϊ��ʹ��DllImport
using System;



/// <summary>
/// �ó��򱳾�͸��
/// </summary>
public class BackGroundSet : MonoBehaviour
{
    private IntPtr hwnd;
    private int currentX;
    private int currentY;


    #region Win��������
    private struct MARGINS
    {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cyTopHeight;
        public int cyBottomHeight;
    }

    [DllImport("user32.dll")]
    static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
    [DllImport("user32.dll")]
    static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll")]
    static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    static extern int SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);

    [DllImport("Dwmapi.dll")]
    static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);

    /// <summary>
    ///���ô���ɴ�͸�����͸��.
    ///����1:������
    ///����2:͸����ɫ  0Ϊ��ɫ,���մ�000000��FFFFFF����ɫ,ת��Ϊ10���Ƶ�ֵ
    ///����3:͸����,���ó�255����ȫ͸��
    ///����4:͸����ʽ,1��ʾ���ô�����ɫΪ[����2]�Ĳ�������Ϊ͸��,2��ʾ����͸�������ô����͸����
    /// </summary>
    [DllImport("user32", EntryPoint = "SetLayeredWindowAttributes")]
    private static extern uint SetLayeredWindowAttributes(IntPtr hwnd, int crKey, int bAlpha, int dwFlags);

    // ���崰����ʽ,-16��ʾ�趨һ���µĴ��ڷ��
    private const int GWL_STYLE = -16;
    //�趨һ���µ���չ���
    private const int GWL_EXSTYLE = -20;
    
    private const int WS_EX_LAYERED = 0x00080000;
    private const int WS_BORDER = 0x00800000;
    private const int WS_CAPTION = 0x00C00000;
    private const int SWP_SHOWWINDOW = 0x0040;
    private const int LWA_COLORKEY = 0x00000001;
    private const int LWA_ALPHA = 0x00000002;
    private const int WS_EX_TRANSPARENT = 0x20;
    private const int WS_EX_TOPMOST = 0x00000008;
    private const int WS_EX_TOOLWINDOW = 0x00000080;
    private const int WS_EX_COMPOSITED = 0x02000000;


    #endregion

    void Awake()
    {
        Application.targetFrameRate = 60;
        Application.runInBackground = true;
        // ע���:\HKEY_CURRENT_USER\SOFTWARE\company name\product name  ��¼�˷ֱ��ʴ�С
        // Screen.SetResolution(800, 600, false);
        var productName = Application.productName;

#if !UNITY_EDITOR
        // ��ô��ھ��
        hwnd = FindWindow(null, productName); 

        // ���ô�������
        int intExTemp = GetWindowLong(hwnd, GWL_EXSTYLE); // ��õ�ǰ��ʽ
        SetWindowLong(hwnd, GWL_EXSTYLE, intExTemp | WS_EX_LAYERED | WS_EX_TOPMOST | WS_EX_TOOLWINDOW); // ��ǰ��ʽ����WS_EX_LAYERED     // WS_EX_TRANSPARENT �ղ��������͸��
        SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_BORDER & ~WS_CAPTION); // �ޱ߿��ޱ�����

        // ���ô���λ��Ϊ���½�
        currentX = Screen.currentResolution.width - 900;
        currentY = Screen.currentResolution.height  - 800;
        SetWindowPos(hwnd, -1, currentX, currentY, 1200, 900, SWP_SHOWWINDOW); // Screen.currentResolution.width / 4 height...

        // ��չ���ڵ��ͻ������� -> Ϊ��͸��
        var margins = new MARGINS() { cxLeftWidth = -1 }; // �߾���Ƕֵȷ���ڴ����Ĳ���չ��ܵľ��� -1Ϊû�д��ڱ߿�
        DwmExtendFrameIntoClientArea(hwnd, ref margins);     

        // ���ô�����ɫΪ0�Ĳ�������Ϊ͸��,�������ɴ�͸���������ģ���ϲ���͸
        SetLayeredWindowAttributes(hwnd, 0, 255, 1);
        //SetLayeredWindowAttributes(hwnd, 0, 255, 2); // ��Ϊ2ʱ��ʾЧ������ˣ������ܴ�͸���

        WinTray.Hide(hwnd);
#endif
    }
}

