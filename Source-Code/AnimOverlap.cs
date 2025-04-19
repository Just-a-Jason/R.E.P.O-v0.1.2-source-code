using System;
using UnityEngine;

// Token: 0x0200020F RID: 527
public class AnimOverlap : MonoBehaviour
{
	// Token: 0x06001138 RID: 4408 RVA: 0x00099BBA File Offset: 0x00097DBA
	private void Update()
	{
	}

	// Token: 0x04001CB9 RID: 7353
	public Transform targetFollow;

	// Token: 0x04001CBA RID: 7354
	private float previousX;

	// Token: 0x04001CBB RID: 7355
	private float previousY;

	// Token: 0x04001CBC RID: 7356
	private Quaternion targetAngle;

	// Token: 0x04001CBD RID: 7357
	[Header("Rotation X")]
	public float springFreqRotX = 15f;

	// Token: 0x04001CBE RID: 7358
	public float springDampingRotX = 0.5f;

	// Token: 0x04001CBF RID: 7359
	private float targetRotX;

	// Token: 0x04001CC0 RID: 7360
	private float currentRotX;

	// Token: 0x04001CC1 RID: 7361
	private float velocityRotX;

	// Token: 0x04001CC2 RID: 7362
	private SpringUtils.tDampedSpringMotionParams springParamsRotX = new SpringUtils.tDampedSpringMotionParams();

	// Token: 0x04001CC3 RID: 7363
	[Header("Rotation Y")]
	public float springFreqRotY = 15f;

	// Token: 0x04001CC4 RID: 7364
	public float springDampingRotY = 0.5f;

	// Token: 0x04001CC5 RID: 7365
	private float targetRotY;

	// Token: 0x04001CC6 RID: 7366
	private float currentRotY;

	// Token: 0x04001CC7 RID: 7367
	private float velocityRotY;

	// Token: 0x04001CC8 RID: 7368
	private SpringUtils.tDampedSpringMotionParams springParamsRotY = new SpringUtils.tDampedSpringMotionParams();

	// Token: 0x04001CC9 RID: 7369
	[Header("Rotation Z")]
	public float springFreqRotZ = 15f;

	// Token: 0x04001CCA RID: 7370
	public float springDampingRotZ = 0.5f;

	// Token: 0x04001CCB RID: 7371
	private float targetRotZ;

	// Token: 0x04001CCC RID: 7372
	private float currentRotZ;

	// Token: 0x04001CCD RID: 7373
	private float velocityRotZ;

	// Token: 0x04001CCE RID: 7374
	private SpringUtils.tDampedSpringMotionParams springParamsRotZ = new SpringUtils.tDampedSpringMotionParams();

	// Token: 0x04001CCF RID: 7375
	private float velocity;
}
