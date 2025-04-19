using System;
using UnityEngine;

// Token: 0x0200012B RID: 299
[Serializable]
public class SpringVector3
{
	// Token: 0x040010F7 RID: 4343
	public float damping = 0.5f;

	// Token: 0x040010F8 RID: 4344
	public float speed = 10f;

	// Token: 0x040010F9 RID: 4345
	[Space]
	public bool clamp;

	// Token: 0x040010FA RID: 4346
	public float maxDistance = 1f;

	// Token: 0x040010FB RID: 4347
	internal Vector3 lastPosition;

	// Token: 0x040010FC RID: 4348
	internal Vector3 springVelocity = Vector3.zero;
}
