using System;
using UnityEngine;

// Token: 0x0200012C RID: 300
[Serializable]
public class SpringFloat
{
	// Token: 0x040010FD RID: 4349
	public float damping = 0.5f;

	// Token: 0x040010FE RID: 4350
	public float speed = 10f;

	// Token: 0x040010FF RID: 4351
	[Space]
	public bool clamp;

	// Token: 0x04001100 RID: 4352
	public float min;

	// Token: 0x04001101 RID: 4353
	public float max = 1f;

	// Token: 0x04001102 RID: 4354
	internal float lastPosition;

	// Token: 0x04001103 RID: 4355
	internal float springVelocity;
}
