using System;
using UnityEngine;

// Token: 0x02000124 RID: 292
[CreateAssetMenu(fileName = "Durability ", menuName = "Phys Object/Durability Preset", order = 1)]
public class Durability : ScriptableObject
{
	// Token: 0x040010C4 RID: 4292
	[Range(0f, 100f)]
	public float fragility = 100f;

	// Token: 0x040010C5 RID: 4293
	[Range(0f, 100f)]
	public float durability = 100f;
}
