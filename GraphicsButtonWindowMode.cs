using System;
using UnityEngine;

// Token: 0x0200011F RID: 287
public class GraphicsButtonWindowMode : MonoBehaviour
{
	// Token: 0x06000954 RID: 2388 RVA: 0x000571F4 File Offset: 0x000553F4
	private void Awake()
	{
		GraphicsButtonWindowMode.instance = this;
		this.slider = base.GetComponent<MenuSlider>();
	}

	// Token: 0x06000955 RID: 2389 RVA: 0x00057208 File Offset: 0x00055408
	public void UpdateSlider()
	{
		this.slider.Start();
	}

	// Token: 0x06000956 RID: 2390 RVA: 0x00057215 File Offset: 0x00055415
	public void ButtonPressed()
	{
		GraphicsManager.instance.UpdateWindowMode(true);
	}

	// Token: 0x040010AF RID: 4271
	public static GraphicsButtonWindowMode instance;

	// Token: 0x040010B0 RID: 4272
	private MenuSlider slider;
}
