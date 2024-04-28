using UnityEngine;
using System.Runtime.InteropServices; // 为了使用DllImport
using System;



/// <summary>
/// 让程序背景透明
/// </summary>
public class BackGroundSet : MonoBehaviour
{
    private IntPtr hwnd;
    private int currentX;
    private int currentY;


    #region Win函数常量
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
    ///设置窗体可穿透点击的透明.
    ///参数1:窗体句柄
    ///参数2:透明颜色  0为黑色,按照从000000到FFFFFF的颜色,转换为10进制的值
    ///参数3:透明度,设置成255就是全透明
    ///参数4:透明方式,1表示将该窗口颜色为[参数2]的部分设置为透明,2表示根据透明度设置窗体的透明度
    /// </summary>
    [DllImport("user32", EntryPoint = "SetLayeredWindowAttributes")]
    private static extern uint SetLayeredWindowAttributes(IntPtr hwnd, int crKey, int bAlpha, int dwFlags);

    // 定义窗体样式,-16表示设定一个新的窗口风格
    private const int GWL_STYLE = -16;
    //设定一个新的扩展风格
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
        // 注册表:\HKEY_CURRENT_USER\SOFTWARE\company name\product name  记录了分辨率大小
        // Screen.SetResolution(800, 600, false);
        var productName = Application.productName;

#if !UNITY_EDITOR
        // 获得窗口句柄
        hwnd = FindWindow(null, productName); 

        // 设置窗体属性
        int intExTemp = GetWindowLong(hwnd, GWL_EXSTYLE); // 获得当前样式
        SetWindowLong(hwnd, GWL_EXSTYLE, intExTemp | WS_EX_LAYERED | WS_EX_TOPMOST | WS_EX_TOOLWINDOW); // 当前样式加上WS_EX_LAYERED     // WS_EX_TRANSPARENT 收不到点击的透明
        SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_BORDER & ~WS_CAPTION); // 无边框、无标题栏

        // 设置窗体位置为右下角
        currentX = Screen.currentResolution.width - 900;
        currentY = Screen.currentResolution.height  - 800;
        SetWindowPos(hwnd, -1, currentX, currentY, 1200, 900, SWP_SHOWWINDOW); // Screen.currentResolution.width / 4 height...

        // 扩展窗口到客户端区域 -> 为了透明
        var margins = new MARGINS() { cxLeftWidth = -1 }; // 边距内嵌值确定在窗口四侧扩展框架的距离 -1为没有窗口边框
        DwmExtendFrameIntoClientArea(hwnd, ref margins);     

        // 将该窗口颜色为0的部分设置为透明,即背景可穿透点击，人物模型上不穿透
        SetLayeredWindowAttributes(hwnd, 0, 255, 1);
        //SetLayeredWindowAttributes(hwnd, 0, 255, 2); // 设为2时显示效果变好了，但不能穿透点击

        WinTray.Hide(hwnd);
#endif
    }
}

