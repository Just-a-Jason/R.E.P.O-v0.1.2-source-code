using System;
using UnityEngine;

// Token: 0x020000ED RID: 237
[CreateAssetMenu(fileName = "Screen Next Message Delay Preset", menuName = "Semi Presets/Screen Next Message Delay Preset")]
public class ScreenNextMessageDelaySettings : ScriptableObject
{
	// Token: 0x0600084D RID: 2125 RVA: 0x0005004D File Offset: 0x0004E24D
	public float GetDelay()
	{
		return this.delay;
	}

	// Token: 0x04000F46 RID: 3910
	public float delay = 1f;
}
