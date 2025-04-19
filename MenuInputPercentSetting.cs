using System;
using UnityEngine;

// Token: 0x020001E1 RID: 481
public class MenuInputPercentSetting : MonoBehaviour
{
	// Token: 0x0600100B RID: 4107 RVA: 0x00091D12 File Offset: 0x0008FF12
	private void Start()
	{
		this.menuSlider = base.GetComponent<MenuSlider>();
		this.menuSlider.SetBar((float)InputManager.instance.inputPercentSettings[this.setting] / 100f);
	}

	// Token: 0x04001AE5 RID: 6885
	public InputPercentSetting setting;

	// Token: 0x04001AE6 RID: 6886
	private MenuSlider menuSlider;
}
