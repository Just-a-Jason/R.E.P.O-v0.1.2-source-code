using System;
using UnityEngine;

// Token: 0x0200012D RID: 301
[Serializable]
public class SpringQuaternion
{
	// Token: 0x04001104 RID: 4356
	public float damping = 0.5f;

	// Token: 0x04001105 RID: 4357
	public float speed = 10f;

	// Token: 0x04001106 RID: 4358
	[Space]
	public bool clamp;

	// Token: 0x04001107 RID: 4359
	public float maxAngle = 20f;

	// Token: 0x04001108 RID: 4360
	internal Quaternion lastRotation;

	// Token: 0x04001109 RID: 4361
	internal Vector3 springVelocity = Vector3.zero;

	// Token: 0x0400110A RID: 4362
	internal bool setup;
}
