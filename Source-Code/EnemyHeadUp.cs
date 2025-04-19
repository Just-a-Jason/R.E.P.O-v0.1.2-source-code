using System;
using UnityEngine;

// Token: 0x02000060 RID: 96
public class EnemyHeadUp : MonoBehaviour
{
	// Token: 0x06000313 RID: 787 RVA: 0x0001E47B File Offset: 0x0001C67B
	private void Start()
	{
		this.startPosition = base.transform.localPosition.y;
	}

	// Token: 0x06000314 RID: 788 RVA: 0x0001E494 File Offset: 0x0001C694
	private void Update()
	{
		if (!this.enemy.NavMeshAgent.IsDisabled() && this.enemy.CurrentState == EnemyState.Chase && this.enemy.StateChase.VisionTimer > 0f && !this.enemy.TargetPlayerAvatar.isDisabled && this.enemy.TargetPlayerAvatar.PlayerVisionTarget.VisionTransform.position.y > this.startPosition)
		{
			base.transform.position = Vector3.Lerp(base.transform.position, new Vector3(base.transform.position.x, this.enemy.TargetPlayerAvatar.PlayerVisionTarget.VisionTransform.position.y, base.transform.position.z), 1f * Time.deltaTime);
			return;
		}
		base.transform.localPosition = Vector3.Lerp(base.transform.localPosition, new Vector3(0f, this.startPosition, 0f), 5f * Time.deltaTime);
		if (this.enemy.CurrentState == EnemyState.Despawn || this.enemy.NavMeshAgent.IsDisabled())
		{
			base.transform.localPosition = new Vector3(0f, this.startPosition, 0f);
		}
	}

	// Token: 0x0400055D RID: 1373
	public Enemy enemy;

	// Token: 0x0400055E RID: 1374
	private float startPosition;
}
