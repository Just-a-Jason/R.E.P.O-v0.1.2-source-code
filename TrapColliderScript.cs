using System;
using UnityEngine;

// Token: 0x02000231 RID: 561
public class TrapColliderScript : MonoBehaviour
{
	// Token: 0x060011EB RID: 4587 RVA: 0x0009F0B4 File Offset: 0x0009D2B4
	private void OnTriggerEnter(Collider other)
	{
		PlayerTrigger component = other.GetComponent<PlayerTrigger>();
		if (component && !GameDirector.instance.LevelCompleted)
		{
			PlayerAvatar playerAvatar = component.PlayerAvatar;
			if (playerAvatar.isLocal && other.gameObject.CompareTag("Player") && !GameDirector.instance.LevelEnemyChasing && TrapDirector.instance.TrapCooldown <= 0f && !PlayerController.instance.Crouching)
			{
				this.TrapCollision = true;
				this.triggerPlayer = playerAvatar;
			}
		}
	}

	// Token: 0x060011EC RID: 4588 RVA: 0x0009F134 File Offset: 0x0009D334
	private void OnDrawGizmos()
	{
		Gizmos.color = new Color(1f, 0.95f, 0f, 0.2f);
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawCube(Vector3.zero, Vector3.one);
	}

	// Token: 0x060011ED RID: 4589 RVA: 0x0009F173 File Offset: 0x0009D373
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(1f, 0.95f, 0f, 0.5f);
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawCube(Vector3.zero, Vector3.one);
	}

	// Token: 0x04001E3A RID: 7738
	[HideInInspector]
	public bool TrapCollision;

	// Token: 0x04001E3B RID: 7739
	[HideInInspector]
	public float TrapCollisionForce;

	// Token: 0x04001E3C RID: 7740
	public PlayerAvatar triggerPlayer;
}
