using System;
using UnityEngine;

// Token: 0x020001EF RID: 495
public class MenuSetting : MonoBehaviour
{
	// Token: 0x06001079 RID: 4217 RVA: 0x00094B0B File Offset: 0x00092D0B
	private void Start()
	{
		this.FetchValues();
	}

	// Token: 0x0600107A RID: 4218 RVA: 0x00094B13 File Offset: 0x00092D13
	public void FetchValues()
	{
		this.settingValue = DataDirector.instance.SettingValueFetch(this.setting);
		this.settingName = DataDirector.instance.SettingNameGet(this.setting);
	}

	// Token: 0x04001B8E RID: 7054
	public DataDirector.Setting setting;

	// Token: 0x04001B8F RID: 7055
	internal string settingName;

	// Token: 0x04001B90 RID: 7056
	internal int settingValue;
}
