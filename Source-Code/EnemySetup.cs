using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200008D RID: 141
[CreateAssetMenu(fileName = "Enemy - _____", menuName = "Other/Enemy Setup", order = 0)]
public class EnemySetup : ScriptableObject
{
	// Token: 0x04000923 RID: 2339
	public List<GameObject> spawnObjects;

	// Token: 0x04000924 RID: 2340
	public bool levelsCompletedCondition;

	// Token: 0x04000925 RID: 2341
	[Range(0f, 10f)]
	public int levelsCompletedMin;

	// Token: 0x04000926 RID: 2342
	[Range(0f, 10f)]
	public int levelsCompletedMax = 10;

	// Token: 0x04000927 RID: 2343
	[Space]
	public RarityPreset rarityPreset;

	// Token: 0x04000928 RID: 2344
	[Space]
	public int runsPlayed;
}
