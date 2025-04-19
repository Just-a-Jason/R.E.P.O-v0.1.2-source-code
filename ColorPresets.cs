using System;
using UnityEngine;

// Token: 0x02000164 RID: 356
[CreateAssetMenu(fileName = "Color Preset", menuName = "Semi Presets/Color Preset")]
public class ColorPresets : ScriptableObject
{
	// Token: 0x06000BC7 RID: 3015 RVA: 0x00069158 File Offset: 0x00067358
	public Color GetColorMain()
	{
		return this.colorMain;
	}

	// Token: 0x06000BC8 RID: 3016 RVA: 0x00069160 File Offset: 0x00067360
	public Color GetColorLight()
	{
		return this.colorLight;
	}

	// Token: 0x06000BC9 RID: 3017 RVA: 0x00069168 File Offset: 0x00067368
	public Color GetColorDark()
	{
		return this.colorDark;
	}

	// Token: 0x0400133A RID: 4922
	public Color colorMain;

	// Token: 0x0400133B RID: 4923
	public Color colorLight;

	// Token: 0x0400133C RID: 4924
	public Color colorDark;
}
