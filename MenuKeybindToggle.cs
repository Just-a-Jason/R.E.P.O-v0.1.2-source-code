using System;
using UnityEngine;

// Token: 0x020001E2 RID: 482
public class MenuKeybindToggle : MonoBehaviour
{
	// Token: 0x0600100D RID: 4109 RVA: 0x00091D4F File Offset: 0x0008FF4F
	private void Start()
	{
	}

	// Token: 0x0600100E RID: 4110 RVA: 0x00091D51 File Offset: 0x0008FF51
	private void Update()
	{
	}

	// Token: 0x0600100F RID: 4111 RVA: 0x00091D53 File Offset: 0x0008FF53
	public void ToggleRebind1()
	{
		InputManager.instance.InputToggleRebind(this.inputKey, true);
	}

	// Token: 0x06001010 RID: 4112 RVA: 0x00091D66 File Offset: 0x0008FF66
	public void ToggleRebind2()
	{
		InputManager.instance.InputToggleRebind(this.inputKey, false);
	}

	// Token: 0x06001011 RID: 4113 RVA: 0x00091D7C File Offset: 0x0008FF7C
	public void FetchSetting()
	{
		MenuTwoOptions component = base.GetComponent<MenuTwoOptions>();
		MenuKeybindToggle component2 = base.GetComponent<MenuKeybindToggle>();
		component.startSettingFetch = InputManager.instance.InputToggleGet(component2.inputKey);
	}

	// Token: 0x04001AE7 RID: 6887
	public InputKey inputKey;
}
