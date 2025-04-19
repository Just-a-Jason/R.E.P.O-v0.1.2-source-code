using System;
using UnityEngine;

// Token: 0x02000051 RID: 81
public class EnemyGnomeStunFly : MonoBehaviour
{
	// Token: 0x060002C8 RID: 712 RVA: 0x0001C360 File Offset: 0x0001A560
	private void Update()
	{
		if (this.enemyGnome.currentState == EnemyGnome.State.Stun && this.enemy.IsStunned() && (float)this.enemy.Rigidbody.physGrabObject.playerGrabbing.Count <= 0f && this.enemy.Rigidbody.physGrabObject.rbVelocity.magnitude > 2f)
		{
			this.soundTimer = 0.5f;
		}
		if (!this.enemy.isActiveAndEnabled)
		{
			this.spawnTimer = 2f;
			this.sound.PlayLoop(false, 5f, 50f, 1f);
		}
		else if (this.soundTimer > 0f && this.spawnTimer <= 0f)
		{
			this.sound.PlayLoop(true, 5f, 5f, 1f);
		}
		else
		{
			this.sound.PlayLoop(false, 5f, 5f, 1f);
		}
		if (this.spawnTimer > 0f)
		{
			this.spawnTimer -= Time.deltaTime;
		}
		if (this.soundTimer > 0f)
		{
			this.soundTimer -= Time.deltaTime;
		}
	}

	// Token: 0x040004D0 RID: 1232
	public Enemy enemy;

	// Token: 0x040004D1 RID: 1233
	public EnemyGnome enemyGnome;

	// Token: 0x040004D2 RID: 1234
	private float soundTimer;

	// Token: 0x040004D3 RID: 1235
	private float spawnTimer;

	// Token: 0x040004D4 RID: 1236
	[Space]
	public Sound sound;
}
