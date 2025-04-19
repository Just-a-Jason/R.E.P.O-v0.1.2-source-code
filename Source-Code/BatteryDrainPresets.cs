using System;
using UnityEngine;

// Token: 0x02000163 RID: 355
[CreateAssetMenu(fileName = "Battery Drain Preset", menuName = "Semi Presets/Battery Drain Preset")]
public class BatteryDrainPresets : ScriptableObject
{
	// Token: 0x06000BC5 RID: 3013 RVA: 0x0006913D File Offset: 0x0006733D
	public float GetBatteryDrainRate()
	{
		return this.batteryDrainRate;
	}

	// Token: 0x04001339 RID: 4921
	public float batteryDrainRate = 0.1f;
}
