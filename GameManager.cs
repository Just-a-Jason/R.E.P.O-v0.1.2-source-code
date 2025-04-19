using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001D4 RID: 468
public class GameManager : MonoBehaviour
{
	// Token: 0x17000004 RID: 4
	// (get) Token: 0x06000FB2 RID: 4018 RVA: 0x0008FA53 File Offset: 0x0008DC53
	// (set) Token: 0x06000FB3 RID: 4019 RVA: 0x0008FA5B File Offset: 0x0008DC5B
	public int gameMode { get; private set; }

	// Token: 0x06000FB4 RID: 4020 RVA: 0x0008FA64 File Offset: 0x0008DC64
	private void Awake()
	{
		if (!GameManager.instance)
		{
			GameManager.instance = this;
			this.gameMode = 0;
			Object.DontDestroyOnLoad(base.gameObject);
			return;
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x06000FB5 RID: 4021 RVA: 0x0008FA96 File Offset: 0x0008DC96
	public void SetGameMode(int mode)
	{
		this.gameMode = mode;
	}

	// Token: 0x06000FB6 RID: 4022 RVA: 0x0008FA9F File Offset: 0x0008DC9F
	public static bool Multiplayer()
	{
		return GameManager.instance.gameMode == 1;
	}

	// Token: 0x06000FB7 RID: 4023 RVA: 0x0008FAAE File Offset: 0x0008DCAE
	public void PlayerMicrophoneSettingSet(string _name, float _value)
	{
		this.playerMicrophoneSettings[_name] = _value;
	}

	// Token: 0x06000FB8 RID: 4024 RVA: 0x0008FABD File Offset: 0x0008DCBD
	public float PlayerMicrophoneSettingGet(string _name)
	{
		if (this.playerMicrophoneSettings.ContainsKey(_name))
		{
			return this.playerMicrophoneSettings[_name];
		}
		return 0.5f;
	}

	// Token: 0x04001A7F RID: 6783
	public static GameManager instance;

	// Token: 0x04001A81 RID: 6785
	public bool localTest;

	// Token: 0x04001A82 RID: 6786
	internal Dictionary<string, float> playerMicrophoneSettings = new Dictionary<string, float>();
}
