using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200009A RID: 154
public class EnemyGrounded : MonoBehaviour
{
	// Token: 0x060005DF RID: 1503 RVA: 0x000399E4 File Offset: 0x00037BE4
	private void Awake()
	{
		this.enemy.Grounded = this;
		this.enemy.HasGrounded = true;
		if (!this.boxCollider.isTrigger)
		{
			Debug.LogError("EnemyGrounded: Collider is not a trigger on " + this.enemy.EnemyParent.name);
		}
		if (this.boxCollider.transform.localScale != Vector3.one)
		{
			Debug.LogError("EnemyGrounded: Scale is not 1 on " + this.enemy.EnemyParent.name);
		}
		if (this.boxCollider.transform.localPosition != Vector3.zero)
		{
			Debug.LogError("EnemyGrounded: Position is not 0 on " + this.enemy.EnemyParent.name);
		}
		base.StartCoroutine(this.ColliderCheck());
	}

	// Token: 0x060005E0 RID: 1504 RVA: 0x00039AB8 File Offset: 0x00037CB8
	private void OnDisable()
	{
		base.StopAllCoroutines();
		this.logicActive = false;
	}

	// Token: 0x060005E1 RID: 1505 RVA: 0x00039AC7 File Offset: 0x00037CC7
	private void OnEnable()
	{
		if (!this.logicActive)
		{
			base.StartCoroutine(this.ColliderCheck());
		}
	}

	// Token: 0x060005E2 RID: 1506 RVA: 0x00039ADE File Offset: 0x00037CDE
	private IEnumerator ColliderCheck()
	{
		this.logicActive = true;
		yield return new WaitForSeconds(0.1f);
		for (;;)
		{
			this.grounded = false;
			Vector3 vector = this.boxCollider.transform.TransformVector(this.boxCollider.size * 0.5f);
			vector.x = Mathf.Abs(vector.x);
			vector.y = Mathf.Abs(vector.y);
			vector.z = Mathf.Abs(vector.z);
			Collider[] array = Physics.OverlapBox(this.boxCollider.bounds.center, vector, this.boxCollider.transform.rotation, LayerMask.GetMask(new string[]
			{
				"Default",
				"PhysGrabObject",
				"PhysGrabObjectHinge",
				"PhysGrabObjectCart"
			}), QueryTriggerInteraction.Ignore);
			if (array.Length != 0)
			{
				foreach (Collider collider in array)
				{
					if (!collider.GetComponentInParent<EnemyRigidbody>())
					{
						if (this.enemy.HasJump && this.enemy.Jump.surfaceJump)
						{
							EnemyJumpSurface component = collider.GetComponent<EnemyJumpSurface>();
							if (component)
							{
								Vector3 rhs = this.enemy.transform.forward;
								if (this.enemy.HasRigidbody)
								{
									rhs = this.enemy.transform.position - this.enemy.Rigidbody.transform.position;
								}
								if (Vector3.Dot(component.transform.TransformDirection(component.jumpDirection), rhs) > 0.5f)
								{
									this.enemy.Jump.SurfaceJumpTrigger(component.transform.TransformDirection(component.jumpDirection));
								}
							}
						}
						if (this.groundedDisableTimer <= 0f)
						{
							this.grounded = true;
						}
					}
				}
			}
			if (this.enemy.HasJump && this.enemy.Jump)
			{
				this.groundedDisableTimer -= 0.05f;
				yield return new WaitForSeconds(0.05f);
			}
			else
			{
				this.groundedDisableTimer -= 0.25f;
				yield return new WaitForSeconds(0.25f);
			}
		}
		yield break;
	}

	// Token: 0x060005E3 RID: 1507 RVA: 0x00039AED File Offset: 0x00037CED
	public void GroundedDisable(float _time)
	{
		this.groundedDisableTimer = _time;
	}

	// Token: 0x040009B5 RID: 2485
	public Enemy enemy;

	// Token: 0x040009B6 RID: 2486
	internal bool grounded;

	// Token: 0x040009B7 RID: 2487
	public BoxCollider boxCollider;

	// Token: 0x040009B8 RID: 2488
	private bool logicActive;

	// Token: 0x040009B9 RID: 2489
	private float groundedDisableTimer;
}
