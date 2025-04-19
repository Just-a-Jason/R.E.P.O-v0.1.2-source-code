using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000099 RID: 153
public class EnemyAttackStuckPhysObject : MonoBehaviour
{
	// Token: 0x060005D9 RID: 1497 RVA: 0x0003981C File Offset: 0x00037A1C
	private void Start()
	{
		this.Enemy = base.GetComponent<Enemy>();
	}

	// Token: 0x060005DA RID: 1498 RVA: 0x0003982C File Offset: 0x00037A2C
	private void Update()
	{
		if (this.AttackedTimer > 0f)
		{
			this.AttackedTimer -= Time.deltaTime;
		}
		if (this.CheckTimer > 0f)
		{
			this.CheckTimer -= Time.deltaTime;
			if (this.CheckTimer <= 0f)
			{
				this.CheckTimer = 0f;
				return;
			}
		}
		else if (this.Active)
		{
			this.Reset();
		}
	}

	// Token: 0x060005DB RID: 1499 RVA: 0x0003989E File Offset: 0x00037A9E
	public bool Check()
	{
		this.CheckTimer = 0.1f;
		if (this.Active)
		{
			return false;
		}
		if (this.Enemy.StuckCount >= this.StuckCount)
		{
			this.Get();
			return true;
		}
		return false;
	}

	// Token: 0x060005DC RID: 1500 RVA: 0x000398D4 File Offset: 0x00037AD4
	public void Get()
	{
		if (!this.Active)
		{
			Collider[] array = Physics.OverlapSphere(this.Enemy.Vision.VisionTransform.position, this.Range, LayerMask.GetMask(new string[]
			{
				"PhysGrabObject"
			}));
			float num = 1000f;
			PhysGrabObject physGrabObject = null;
			foreach (Collider collider in array)
			{
				if (!collider.GetComponentInParent<EnemyRigidbody>())
				{
					PhysGrabObject componentInParent = collider.GetComponentInParent<PhysGrabObject>();
					float num2 = Vector3.Distance(this.Enemy.Vision.VisionTransform.position, componentInParent.centerPoint);
					if (num2 < num)
					{
						num = num2;
						physGrabObject = componentInParent;
					}
				}
			}
			if (physGrabObject)
			{
				this.Active = true;
				this.TargetObject = physGrabObject;
				this.OnActiveImpulse.Invoke();
				this.Enemy.StuckCount = 0;
			}
		}
	}

	// Token: 0x060005DD RID: 1501 RVA: 0x000399AD File Offset: 0x00037BAD
	public void Reset()
	{
		this.Enemy.StuckCount = 0;
		this.TargetObject = null;
		this.Active = false;
	}

	// Token: 0x040009AD RID: 2477
	private Enemy Enemy;

	// Token: 0x040009AE RID: 2478
	public float Range = 1f;

	// Token: 0x040009AF RID: 2479
	public int StuckCount = 3;

	// Token: 0x040009B0 RID: 2480
	[Space]
	public UnityEvent OnActiveImpulse;

	// Token: 0x040009B1 RID: 2481
	internal bool Active;

	// Token: 0x040009B2 RID: 2482
	internal PhysGrabObject TargetObject;

	// Token: 0x040009B3 RID: 2483
	internal float AttackedTimer;

	// Token: 0x040009B4 RID: 2484
	private float CheckTimer;
}
