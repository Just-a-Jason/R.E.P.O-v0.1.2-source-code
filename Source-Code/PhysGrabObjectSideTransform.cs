using System;
using UnityEngine;

// Token: 0x02000191 RID: 401
public class PhysGrabObjectSideTransform : MonoBehaviour
{
	// Token: 0x06000D3D RID: 3389 RVA: 0x00075D4B File Offset: 0x00073F4B
	private void Start()
	{
		this.meshRenderer = base.GetComponent<MeshRenderer>();
		this.prevPosition = base.transform.position;
	}

	// Token: 0x06000D3E RID: 3390 RVA: 0x00075D6C File Offset: 0x00073F6C
	private void FixedUpdate()
	{
		float num = Vector3.Distance(this.prevPosition, base.transform.position) / Time.fixedDeltaTime * 5f;
		if (num > this.velocity)
		{
			this.velocity = num;
			this.velocityResetTimer = 0.1f;
		}
		if (this.velocityResetTimer > 0f)
		{
			this.velocityResetTimer -= Time.fixedDeltaTime;
		}
		else
		{
			this.velocity = 0f;
		}
		this.prevPosition = base.transform.position;
	}

	// Token: 0x04001589 RID: 5513
	[HideInInspector]
	public Vector3 prevPosition;

	// Token: 0x0400158A RID: 5514
	[HideInInspector]
	public float velocity;

	// Token: 0x0400158B RID: 5515
	private float velocityResetTimer;

	// Token: 0x0400158C RID: 5516
	private float impactTimer;

	// Token: 0x0400158D RID: 5517
	private MeshRenderer meshRenderer;
}
