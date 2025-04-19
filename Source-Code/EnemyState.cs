using System;

// Token: 0x02000087 RID: 135
public enum EnemyState
{
	// Token: 0x04000880 RID: 2176
	None,
	// Token: 0x04000881 RID: 2177
	Spawn,
	// Token: 0x04000882 RID: 2178
	Roaming,
	// Token: 0x04000883 RID: 2179
	ChaseBegin,
	// Token: 0x04000884 RID: 2180
	Chase,
	// Token: 0x04000885 RID: 2181
	ChaseSlow,
	// Token: 0x04000886 RID: 2182
	ChaseEnd,
	// Token: 0x04000887 RID: 2183
	Investigate,
	// Token: 0x04000888 RID: 2184
	Sneak,
	// Token: 0x04000889 RID: 2185
	Stunned,
	// Token: 0x0400088A RID: 2186
	LookUnder,
	// Token: 0x0400088B RID: 2187
	Despawn
}
