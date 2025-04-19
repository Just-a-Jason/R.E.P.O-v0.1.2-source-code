using System;
using System.Collections;
using UnityEngine;

// Token: 0x020001B1 RID: 433
public class PlayerCollisionGrounded : MonoBehaviour
{
	// Token: 0x06000E95 RID: 3733 RVA: 0x00083E66 File Offset: 0x00082066
	private void Awake()
	{
		PlayerCollisionGrounded.instance = this;
		this.Collider = base.GetComponent<SphereCollider>();
	}

	// Token: 0x06000E96 RID: 3734 RVA: 0x00083E7A File Offset: 0x0008207A
	private void Start()
	{
		this.ColliderCheckActivate();
	}

	// Token: 0x06000E97 RID: 3735 RVA: 0x00083E82 File Offset: 0x00082082
	private void OnEnable()
	{
		this.ColliderCheckActivate();
	}

	// Token: 0x06000E98 RID: 3736 RVA: 0x00083E8A File Offset: 0x0008208A
	private void OnDisable()
	{
		this.colliderCheckActive = false;
		base.StopAllCoroutines();
	}

	// Token: 0x06000E99 RID: 3737 RVA: 0x00083E99 File Offset: 0x00082099
	private void ColliderCheckActivate()
	{
		if (!this.colliderCheckActive)
		{
			this.colliderCheckActive = true;
			base.StartCoroutine(this.ColliderCheck());
		}
	}

	// Token: 0x06000E9A RID: 3738 RVA: 0x00083EB7 File Offset: 0x000820B7
	private IEnumerator ColliderCheck()
	{
		for (;;)
		{
			this.GroundedTimer -= 1f * Time.deltaTime;
			this.physRiding = false;
			if (this.CollisionController.GroundedDisableTimer <= 0f)
			{
				Collider[] array = Physics.OverlapSphere(base.transform.position, this.Collider.radius, this.LayerMask, QueryTriggerInteraction.Ignore);
				if (array.Length != 0)
				{
					int num = 0;
					if (LevelGenerator.Instance.Generated)
					{
						foreach (Collider collider in array)
						{
							if (collider.gameObject.CompareTag("Phys Grab Object"))
							{
								PhysGrabObject physGrabObject = collider.gameObject.GetComponent<PhysGrabObject>();
								if (!physGrabObject)
								{
									physGrabObject = collider.gameObject.GetComponentInParent<PhysGrabObject>();
								}
								if (physGrabObject)
								{
									if (!PlayerController.instance.JumpGroundedObjects.Contains(physGrabObject))
									{
										PlayerController.instance.JumpGroundedObjects.Add(physGrabObject);
									}
									if (physGrabObject.GetComponent<PlayerTumble>())
									{
										num++;
									}
									else if (physGrabObject.roomVolumeCheck.currentSize.magnitude > 1f)
									{
										this.physRiding = true;
										this.physRidingID = physGrabObject.photonView.ViewID;
										this.physRidingPosition = physGrabObject.photonView.transform.InverseTransformPoint(PlayerController.instance.transform.position);
									}
								}
							}
						}
					}
					if (num != array.Length)
					{
						this.GroundedTimer = 0.1f;
						this.Grounded = true;
					}
				}
			}
			if (this.GroundedTimer < 0f)
			{
				this.Grounded = false;
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x06000E9B RID: 3739 RVA: 0x00083EC6 File Offset: 0x000820C6
	private void Update()
	{
		this.CollisionController.Grounded = this.Grounded;
	}

	// Token: 0x04001818 RID: 6168
	public static PlayerCollisionGrounded instance;

	// Token: 0x04001819 RID: 6169
	public PlayerCollisionController CollisionController;

	// Token: 0x0400181A RID: 6170
	internal bool Grounded;

	// Token: 0x0400181B RID: 6171
	private float GroundedTimer;

	// Token: 0x0400181C RID: 6172
	public LayerMask LayerMask;

	// Token: 0x0400181D RID: 6173
	private SphereCollider Collider;

	// Token: 0x0400181E RID: 6174
	[HideInInspector]
	public bool physRiding;

	// Token: 0x0400181F RID: 6175
	[HideInInspector]
	public int physRidingID;

	// Token: 0x04001820 RID: 6176
	[HideInInspector]
	public Vector3 physRidingPosition;

	// Token: 0x04001821 RID: 6177
	private bool colliderCheckActive;
}
