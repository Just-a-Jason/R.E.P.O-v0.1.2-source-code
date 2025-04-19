using System;
using UnityEngine;

// Token: 0x020001D0 RID: 464
[CreateAssetMenu(fileName = "Effect Presets", menuName = "Effect Presets/Explosion Preset", order = 1)]
public class ExplosionPreset : ScriptableObject
{
	// Token: 0x04001A4D RID: 6733
	[Header("Settings")]
	[Space(5f)]
	public float explosionForceMultiplier = 1f;

	// Token: 0x04001A4E RID: 6734
	[Space(20f)]
	[Header("Colors")]
	[Space(5f)]
	public Gradient explosionColors;

	// Token: 0x04001A4F RID: 6735
	public Gradient smokeColors;

	// Token: 0x04001A50 RID: 6736
	public Gradient lightColor;

	// Token: 0x04001A51 RID: 6737
	[Space(20f)]
	[Header("Sounds")]
	[Space(5f)]
	public Sound explosionSoundSmall;

	// Token: 0x04001A52 RID: 6738
	public Sound explosionSoundSmallGlobal;

	// Token: 0x04001A53 RID: 6739
	public Sound explosionSoundMedium;

	// Token: 0x04001A54 RID: 6740
	public Sound explosionSoundMediumGlobal;

	// Token: 0x04001A55 RID: 6741
	public Sound explosionSoundBig;

	// Token: 0x04001A56 RID: 6742
	public Sound explosionSoundBigGlobal;
}
