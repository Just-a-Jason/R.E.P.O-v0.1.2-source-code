using System;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

// Token: 0x0200020C RID: 524
public class SessionManager : MonoBehaviour
{
	// Token: 0x06001127 RID: 4391 RVA: 0x000992FC File Offset: 0x000974FC
	private void Awake()
	{
		if (!SessionManager.instance)
		{
			SessionManager.instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
		foreach (string item in Microphone.devices)
		{
			this.micDeviceList.Add(item);
		}
	}

	// Token: 0x06001128 RID: 4392 RVA: 0x00099358 File Offset: 0x00097558
	private void Start()
	{
		bool flag = false;
		int num = 0;
		foreach (string a in this.micDeviceList)
		{
			if (a == DataDirector.instance.micDevice || DataDirector.instance.micDevice == "")
			{
				this.micDeviceCurrent = a;
				flag = true;
				break;
			}
			num++;
		}
		if (!flag && DataDirector.instance.micDevice != "NONE")
		{
			num = 0;
		}
		this.micDeviceCurrentIndex = num;
		DataDirector.instance.SettingValueSet(DataDirector.Setting.MicDevice, this.micDeviceCurrentIndex);
	}

	// Token: 0x06001129 RID: 4393 RVA: 0x00099414 File Offset: 0x00097614
	private void Update()
	{
		if (SemiFunc.FPSImpulse1())
		{
			foreach (string item in Microphone.devices)
			{
				if (!this.micDeviceList.Contains(item))
				{
					this.micDeviceList.Add(item);
				}
			}
		}
		if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.F12))
		{
			Application.OpenURL("file://" + Application.persistentDataPath + "/Player.log");
		}
	}

	// Token: 0x0600112A RID: 4394 RVA: 0x00099497 File Offset: 0x00097697
	public void CrownPlayer()
	{
		if (this.crownedPlayerSteamID.IsNullOrEmpty())
		{
			return;
		}
		if (SemiFunc.IsMasterClient() && SemiFunc.IsMultiplayer())
		{
			PunManager.instance.CrownPlayerSync(this.crownedPlayerSteamID);
		}
	}

	// Token: 0x0600112B RID: 4395 RVA: 0x000994C5 File Offset: 0x000976C5
	public PlayerAvatar CrownedPlayerGet()
	{
		return SemiFunc.PlayerAvatarGetFromSteamID(this.crownedPlayerSteamID);
	}

	// Token: 0x0600112C RID: 4396 RVA: 0x000994D2 File Offset: 0x000976D2
	public void ResetCrown()
	{
		this.crownedPlayerSteamID = "";
	}

	// Token: 0x0600112D RID: 4397 RVA: 0x000994DF File Offset: 0x000976DF
	public void Reset()
	{
		this.crownedPlayerSteamID = "";
	}

	// Token: 0x04001C70 RID: 7280
	public static SessionManager instance;

	// Token: 0x04001C71 RID: 7281
	internal string crownedPlayerSteamID;

	// Token: 0x04001C72 RID: 7282
	public GameObject crownPrefab;

	// Token: 0x04001C73 RID: 7283
	internal List<string> micDeviceList = new List<string>();

	// Token: 0x04001C74 RID: 7284
	internal string micDeviceCurrent;

	// Token: 0x04001C75 RID: 7285
	internal int micDeviceCurrentIndex;
}
