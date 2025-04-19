using System;
using System.Runtime.InteropServices;
using UnityEngine;

// Token: 0x02000131 RID: 305
public class WindowManager : MonoBehaviour
{
	// Token: 0x06000A7D RID: 2685
	[DllImport("user32.dll")]
	public static extern bool SetWindowText(IntPtr hwnd, string lpString);

	// Token: 0x06000A7E RID: 2686
	[DllImport("user32.dll")]
	public static extern IntPtr FindWindow(string className, string windowName);

	// Token: 0x06000A7F RID: 2687 RVA: 0x0005CCF8 File Offset: 0x0005AEF8
	private void Awake()
	{
		if (!WindowManager.instance)
		{
			WindowManager.instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
			WindowManager.SetWindowText(WindowManager.FindWindow(null, "Repo"), "R.E.P.O.");
			return;
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x0400110B RID: 4363
	public static WindowManager instance;
}
