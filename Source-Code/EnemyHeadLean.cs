using System;
using UnityEngine;

// Token: 0x0200005C RID: 92
public class EnemyHeadLean : MonoBehaviour
{
	// Token: 0x0600030B RID: 779 RVA: 0x0001E16C File Offset: 0x0001C36C
	private void Update()
	{
		if (this.Enemy.FreezeTimer > 0f)
		{
			return;
		}
		if (this.Enemy.NavMeshAgent.AgentVelocity.magnitude < 0.1f)
		{
			base.transform.localRotation = Quaternion.Lerp(base.transform.localRotation, Quaternion.Euler(0f, 0f, 0f), 50f * Time.deltaTime);
			return;
		}
		float x = Mathf.Clamp(this.Enemy.NavMeshAgent.AgentVelocity.magnitude * this.Amount, -this.MaxAmount, this.MaxAmount);
		base.transform.localRotation = Quaternion.Lerp(base.transform.localRotation, Quaternion.Euler(x, 0f, 0f), this.Speed * Time.deltaTime);
	}

	// Token: 0x0400054D RID: 1357
	public Enemy Enemy;

	// Token: 0x0400054E RID: 1358
	[Space]
	public float Amount = -500f;

	// Token: 0x0400054F RID: 1359
	public float MaxAmount = 20f;

	// Token: 0x04000550 RID: 1360
	public float Speed = 10f;
}
