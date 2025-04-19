using System;
using UnityEngine;

// Token: 0x020001B2 RID: 434
public class PlayerCollisionStand : MonoBehaviour
{
	// Token: 0x06000E9D RID: 3741 RVA: 0x00083EE1 File Offset: 0x000820E1
	private void Awake()
	{
		PlayerCollisionStand.instance = this;
		this.Collider = base.GetComponent<CapsuleCollider>();
	}

	// Token: 0x06000E9E RID: 3742 RVA: 0x00083EF8 File Offset: 0x000820F8
	public bool CheckBlocked()
	{
		if (this.setBlockedTimer > 0f)
		{
			return true;
		}
		Vector3 point = base.transform.position + this.Offset + Vector3.up * this.Collider.radius;
		Vector3 point2 = base.transform.position + this.Offset + Vector3.up * this.Collider.height - Vector3.up * this.Collider.radius;
		return Physics.OverlapCapsule(point, point2, this.Collider.radius, this.LayerMask).Length != 0;
	}

	// Token: 0x06000E9F RID: 3743 RVA: 0x00083FB1 File Offset: 0x000821B1
	private void Update()
	{
		if (this.setBlockedTimer > 0f)
		{
			this.setBlockedTimer -= Time.deltaTime;
		}
		base.transform.position = this.TargetTransform.position;
	}

	// Token: 0x06000EA0 RID: 3744 RVA: 0x00083FE8 File Offset: 0x000821E8
	public void SetBlocked()
	{
		this.setBlockedTimer = 0.25f;
		PlayerCollision.instance.SetCrouchCollision();
	}

	// Token: 0x04001822 RID: 6178
	public static PlayerCollisionStand instance;

	// Token: 0x04001823 RID: 6179
	public PlayerCollisionController CollisionController;

	// Token: 0x04001824 RID: 6180
	private CapsuleCollider Collider;

	// Token: 0x04001825 RID: 6181
	public LayerMask LayerMask;

	// Token: 0x04001826 RID: 6182
	public Transform TargetTransform;

	// Token: 0x04001827 RID: 6183
	public Vector3 Offset;

	// Token: 0x04001828 RID: 6184
	private bool checkActive;

	// Token: 0x04001829 RID: 6185
	private float setBlockedTimer;
}
