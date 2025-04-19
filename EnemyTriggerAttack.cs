using System;
using UnityEngine;

// Token: 0x020000A3 RID: 163
public class EnemyTriggerAttack : MonoBehaviour
{
	// Token: 0x06000649 RID: 1609 RVA: 0x0003CC94 File Offset: 0x0003AE94
	private void OnTriggerStay(Collider other)
	{
		if (!LevelGenerator.Instance.Generated)
		{
			return;
		}
		if (this.TriggerCheckTimer > 0f)
		{
			return;
		}
		this.TriggerCheckTimerSet = true;
		if (this.Enemy.CurrentState == EnemyState.Chase || this.Enemy.CurrentState == EnemyState.LookUnder)
		{
			PlayerTrigger component = other.GetComponent<PlayerTrigger>();
			if (component)
			{
				bool flag = false;
				if (this.Enemy.CurrentState == EnemyState.LookUnder && this.Enemy.StateLookUnder.WaitDone)
				{
					flag = true;
				}
				bool chaseCanReach = this.Enemy.StateChase.ChaseCanReach;
				PlayerAvatar playerAvatar = component.PlayerAvatar;
				if (playerAvatar.isDisabled || (!this.Enemy.Vision.VisionTriggered[playerAvatar.photonView.ViewID] && !flag))
				{
					return;
				}
				bool flag2 = true;
				bool flag3 = false;
				if (!chaseCanReach || flag)
				{
					flag2 = false;
					flag3 = true;
				}
				Vector3 position = playerAvatar.PlayerVisionTarget.VisionTransform.transform.position;
				RaycastHit[] array = Physics.RaycastAll(this.VisionTransform.position, position - this.VisionTransform.position, (position - this.VisionTransform.position).magnitude, this.VisionMask);
				bool flag4 = false;
				foreach (RaycastHit raycastHit in array)
				{
					if (!raycastHit.transform.CompareTag("Enemy") && !raycastHit.transform.GetComponent<PlayerTumble>())
					{
						flag4 = true;
					}
				}
				if (flag4)
				{
					if (!flag3)
					{
						flag2 = false;
					}
				}
				else if (flag3)
				{
					flag2 = true;
				}
				if (flag2)
				{
					this.Attack = true;
				}
			}
		}
		if (this.Enemy.CurrentState != EnemyState.ChaseBegin)
		{
			bool flag5 = false;
			int num = 0;
			Vector3 vector = Vector3.zero;
			PhysGrabObject componentInParent = other.GetComponentInParent<PhysGrabObject>();
			StaticGrabObject componentInParent2 = other.GetComponentInParent<StaticGrabObject>();
			if (componentInParent)
			{
				flag5 = true;
				num = componentInParent.playerGrabbing.Count;
				vector = componentInParent.midPoint;
				if (componentInParent.GetComponent<EnemyRigidbody>())
				{
					flag5 = false;
				}
			}
			else if (componentInParent2)
			{
				flag5 = true;
				num = componentInParent2.playerGrabbing.Count;
				vector = componentInParent2.transform.position;
			}
			if (flag5 && num > 0 && Vector3.Distance(base.transform.position, vector) < this.Enemy.Vision.VisionDistance)
			{
				Vector3 direction = vector - this.VisionTransform.position;
				if (Vector3.Dot(this.VisionTransform.forward, direction.normalized) > 0.8f)
				{
					RaycastHit raycastHit2;
					bool flag6 = Physics.Raycast(this.Enemy.Vision.VisionTransform.position, direction, out raycastHit2, direction.magnitude, this.VisionMask);
					bool flag7 = true;
					if (flag6)
					{
						if (componentInParent)
						{
							if (raycastHit2.collider.GetComponentInParent<PhysGrabObject>() != componentInParent)
							{
								flag7 = false;
							}
						}
						else if (componentInParent2 && raycastHit2.collider.GetComponentInParent<StaticGrabObject>() != componentInParent2)
						{
							flag7 = false;
						}
					}
					if (flag7 && this.Enemy.HasStateInvestigate)
					{
						this.Enemy.StateInvestigate.Set(vector);
					}
				}
			}
		}
	}

	// Token: 0x0600064A RID: 1610 RVA: 0x0003CFD1 File Offset: 0x0003B1D1
	private void Update()
	{
		if (this.TriggerCheckTimerSet)
		{
			this.TriggerCheckTimer = 0.2f;
			this.TriggerCheckTimerSet = false;
			return;
		}
		if (this.TriggerCheckTimer > 0f)
		{
			this.TriggerCheckTimer -= Time.deltaTime;
		}
	}

	// Token: 0x04000A88 RID: 2696
	public Enemy Enemy;

	// Token: 0x04000A89 RID: 2697
	public LayerMask VisionMask;

	// Token: 0x04000A8A RID: 2698
	public Transform VisionTransform;

	// Token: 0x04000A8B RID: 2699
	private bool TriggerCheckTimerSet;

	// Token: 0x04000A8C RID: 2700
	private float TriggerCheckTimer;

	// Token: 0x04000A8D RID: 2701
	internal bool Attack;
}
