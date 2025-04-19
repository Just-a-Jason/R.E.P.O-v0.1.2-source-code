using System;
using UnityEngine;

// Token: 0x020000E3 RID: 227
public class ModulePropSwitch : MonoBehaviour
{
	// Token: 0x0600080B RID: 2059 RVA: 0x0004E3E0 File Offset: 0x0004C5E0
	public void Setup()
	{
		int num = 0;
		while ((float)num < this.Module.transform.localRotation.eulerAngles.y)
		{
			num += 90;
			this.ConnectionSide++;
			if (this.ConnectionSide > ModulePropSwitch.Connection.Left)
			{
				this.ConnectionSide = ModulePropSwitch.Connection.Top;
			}
		}
		if (this.ConnectionSide == ModulePropSwitch.Connection.Top && this.Module.ConnectingTop)
		{
			this.Connected = true;
		}
		else if (this.ConnectionSide == ModulePropSwitch.Connection.Right && this.Module.ConnectingRight)
		{
			this.Connected = true;
		}
		else if (this.ConnectionSide == ModulePropSwitch.Connection.Bot && this.Module.ConnectingBottom)
		{
			this.Connected = true;
		}
		else if (this.ConnectionSide == ModulePropSwitch.Connection.Left && this.Module.ConnectingLeft)
		{
			this.Connected = true;
		}
		if (this.Connected)
		{
			this.NotConnectedParent.SetActive(false);
			this.ConnectedParent.SetActive(true);
			return;
		}
		this.NotConnectedParent.SetActive(true);
		this.ConnectedParent.SetActive(false);
	}

	// Token: 0x0600080C RID: 2060 RVA: 0x0004E4E8 File Offset: 0x0004C6E8
	public void Toggle()
	{
		if (this.DebugSwitch)
		{
			this.DebugSwitch = false;
			this.DebugState = "Connected";
			this.NotConnectedParent.SetActive(false);
			this.ConnectedParent.SetActive(true);
			return;
		}
		this.DebugSwitch = true;
		this.DebugState = "Not Connected";
		this.NotConnectedParent.SetActive(true);
		this.ConnectedParent.SetActive(false);
	}

	// Token: 0x04000ED0 RID: 3792
	internal Module Module;

	// Token: 0x04000ED1 RID: 3793
	public GameObject ConnectedParent;

	// Token: 0x04000ED2 RID: 3794
	public GameObject NotConnectedParent;

	// Token: 0x04000ED3 RID: 3795
	private bool Connected;

	// Token: 0x04000ED4 RID: 3796
	[Space(20f)]
	public ModulePropSwitch.Connection ConnectionSide;

	// Token: 0x04000ED5 RID: 3797
	[HideInInspector]
	public string DebugState = "...";

	// Token: 0x04000ED6 RID: 3798
	[HideInInspector]
	public bool DebugSwitch;

	// Token: 0x02000312 RID: 786
	public enum Connection
	{
		// Token: 0x040025D1 RID: 9681
		Top,
		// Token: 0x040025D2 RID: 9682
		Right,
		// Token: 0x040025D3 RID: 9683
		Bot,
		// Token: 0x040025D4 RID: 9684
		Left
	}
}
