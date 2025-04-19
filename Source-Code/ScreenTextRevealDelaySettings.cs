using System;
using UnityEngine;

// Token: 0x020000EE RID: 238
[CreateAssetMenu(fileName = "Screen Text Reveal Delay Preset", menuName = "Semi Presets/Screen Text Reveal Delay Preset")]
public class ScreenTextRevealDelaySettings : ScriptableObject
{
	// Token: 0x0600084F RID: 2127 RVA: 0x00050068 File Offset: 0x0004E268
	public float GetDelay()
	{
		return this.delay;
	}

	// Token: 0x04000F47 RID: 3911
	public float delay = 0.1f;
}
