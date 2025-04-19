using System;
using UnityEngine;

// Token: 0x0200012F RID: 303
public class SemiLogger : MonoBehaviour
{
	// Token: 0x06000A74 RID: 2676 RVA: 0x0005CC4C File Offset: 0x0005AE4C
	[HideInCallstack]
	public static void Log(SemiFunc.User _user, object _message, GameObject _obj = null, Color? color = null)
	{
		if (SemiFunc.DebugUser(_user))
		{
			string arg = ColorUtility.ToHtmlStringRGB(color ?? Color.Lerp(Color.gray, Color.white, 0.4f));
			Debug.Log(string.Format("<color=#{0}>{1}</color>", arg, _message), _obj);
		}
	}

	// Token: 0x06000A75 RID: 2677 RVA: 0x0005CCA1 File Offset: 0x0005AEA1
	[HideInCallstack]
	public static void LogAxel(object _message, GameObject _obj = null, Color? color = null)
	{
		SemiLogger.Log(SemiFunc.User.Axel, _message, _obj, color);
	}

	// Token: 0x06000A76 RID: 2678 RVA: 0x0005CCAC File Offset: 0x0005AEAC
	[HideInCallstack]
	public static void LogJannek(object _message, GameObject _obj = null, Color? color = null)
	{
		SemiLogger.Log(SemiFunc.User.Jannek, _message, _obj, color);
	}

	// Token: 0x06000A77 RID: 2679 RVA: 0x0005CCB7 File Offset: 0x0005AEB7
	[HideInCallstack]
	public static void LogRobin(object _message, GameObject _obj = null, Color? color = null)
	{
		SemiLogger.Log(SemiFunc.User.Robin, _message, _obj, color);
	}

	// Token: 0x06000A78 RID: 2680 RVA: 0x0005CCC2 File Offset: 0x0005AEC2
	[HideInCallstack]
	public static void LogRuben(object _message, GameObject _obj = null, Color? color = null)
	{
		SemiLogger.Log(SemiFunc.User.Ruben, _message, _obj, color);
	}

	// Token: 0x06000A79 RID: 2681 RVA: 0x0005CCCD File Offset: 0x0005AECD
	[HideInCallstack]
	public static void LogWalter(object _message, GameObject _obj = null, Color? color = null)
	{
		SemiLogger.Log(SemiFunc.User.Walter, _message, _obj, color);
	}
}
