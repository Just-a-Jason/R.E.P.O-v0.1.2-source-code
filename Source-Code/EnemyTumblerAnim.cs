using System;
using UnityEngine;

// Token: 0x02000082 RID: 130
public class EnemyTumblerAnim : MonoBehaviour
{
	// Token: 0x06000506 RID: 1286 RVA: 0x00032020 File Offset: 0x00030220
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
		this.animator.keepAnimatorStateOnDisable = true;
	}

	// Token: 0x06000507 RID: 1287 RVA: 0x0003203C File Offset: 0x0003023C
	private void Update()
	{
		if (this.enemy.Jump.jumping)
		{
			this.animator.SetBool("jumping", true);
			if (this.jumpImpulse)
			{
				this.jumpedTimer = 0f;
				this.animator.SetTrigger("Jump");
				this.animator.SetBool("falling", false);
				this.jumpImpulse = false;
			}
		}
		else
		{
			this.animator.SetBool("jumping", false);
			this.jumpImpulse = true;
		}
		this.jumpedTimer += Time.deltaTime;
		if (this.jumpedTimer > 0.5f)
		{
			if (this.enemy.Rigidbody.physGrabObject.rbVelocity.y < -0.1f)
			{
				this.animator.SetBool("falling", true);
			}
			else
			{
				this.animator.SetBool("falling", false);
			}
		}
		if (this.enemyTumbler.currentState == EnemyTumbler.State.Tell)
		{
			this.animator.SetBool("tell", true);
		}
		else
		{
			this.animator.SetBool("tell", false);
		}
		if (this.enemyTumbler.currentState == EnemyTumbler.State.Tumble)
		{
			this.animator.SetBool("tumble", true);
			this.tumble = true;
		}
		else
		{
			this.animator.SetBool("tumble", false);
			this.tumble = false;
		}
		this.sfxTumbleLoopLocal.PlayLoop(this.tumble, 5f, 5f, 1f);
		this.sfxTumbleLoopGlobal.PlayLoop(this.tumble, 5f, 5f, 1f);
		if (this.enemyTumbler.currentState == EnemyTumbler.State.Stunned)
		{
			if (this.stunImpulse)
			{
				this.animator.SetTrigger("Stun");
				this.stunImpulse = false;
			}
			this.animator.SetBool("stunned", true);
			this.stunned = true;
		}
		else
		{
			this.animator.SetBool("stunned", false);
			this.stunImpulse = true;
			this.stunned = false;
		}
		this.sfxStunnedLoop.PlayLoop(this.stunned, 5f, 5f, 1f);
		if (this.enemyTumbler.currentState == EnemyTumbler.State.Despawn)
		{
			this.animator.SetBool("despawning", true);
			return;
		}
		this.animator.SetBool("despawning", false);
	}

	// Token: 0x06000508 RID: 1288 RVA: 0x00032290 File Offset: 0x00030490
	public void OnSpawn()
	{
		this.animator.SetBool("stunned", false);
		this.animator.Play("Spawn", 0, 0f);
	}

	// Token: 0x06000509 RID: 1289 RVA: 0x000322BC File Offset: 0x000304BC
	private void OnDrawGizmos()
	{
		if (this.showGizmos)
		{
			Gizmos.matrix = Matrix4x4.TRS(base.transform.position, Quaternion.identity, new Vector3(1f, 0f, 1f));
			Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
			Gizmos.DrawWireSphere(Vector3.zero, this.gizmoMinDistance);
			Gizmos.color = new Color(0.9f, 0f, 0.1f, 0.5f);
			Gizmos.DrawWireSphere(Vector3.zero, this.gizmoMaxDistance);
		}
	}

	// Token: 0x0600050A RID: 1290 RVA: 0x0003235E File Offset: 0x0003055E
	public void SfxOnHurtColliderImpactAny()
	{
		this.sfxHurtColliderImpactAny.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600050B RID: 1291 RVA: 0x0003238C File Offset: 0x0003058C
	public void OnTumble()
	{
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.1f);
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.5f);
	}

	// Token: 0x0600050C RID: 1292 RVA: 0x000323F5 File Offset: 0x000305F5
	public void Despawn()
	{
		this.enemy.EnemyParent.Despawn();
	}

	// Token: 0x0600050D RID: 1293 RVA: 0x00032408 File Offset: 0x00030608
	public void SfxJump()
	{
		this.sfxJump.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraImpact.ShakeDistance(1f, 3f, 8f, base.transform.position, 0.1f);
		GameDirector.instance.CameraShake.ShakeDistance(1f, 3f, 8f, base.transform.position, 0.5f);
	}

	// Token: 0x0600050E RID: 1294 RVA: 0x0003249C File Offset: 0x0003069C
	public void SfxLand()
	{
		this.sfxLand.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraImpact.ShakeDistance(2f, 3f, 8f, base.transform.position, 0.1f);
		GameDirector.instance.CameraShake.ShakeDistance(2f, 3f, 8f, base.transform.position, 0.5f);
	}

	// Token: 0x0600050F RID: 1295 RVA: 0x00032530 File Offset: 0x00030730
	public void SfxNotice()
	{
		this.sfxNotice.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000510 RID: 1296 RVA: 0x0003255D File Offset: 0x0003075D
	public void SfxCleaverSwing()
	{
		this.sfxCleaverSwing.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000511 RID: 1297 RVA: 0x0003258A File Offset: 0x0003078A
	public void SfxCharge()
	{
		this.sfxCharge.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000512 RID: 1298 RVA: 0x000325B7 File Offset: 0x000307B7
	public void SfxHurt()
	{
		this.sfxHurt.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000513 RID: 1299 RVA: 0x000325E4 File Offset: 0x000307E4
	public void SfxMoveShort()
	{
		this.sfxMoveShort.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000514 RID: 1300 RVA: 0x00032611 File Offset: 0x00030811
	public void SfxMoveLong()
	{
		this.sfxMoveLong.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0400080E RID: 2062
	public Enemy enemy;

	// Token: 0x0400080F RID: 2063
	public EnemyTumbler enemyTumbler;

	// Token: 0x04000810 RID: 2064
	internal Animator animator;

	// Token: 0x04000811 RID: 2065
	public Materials.MaterialTrigger material;

	// Token: 0x04000812 RID: 2066
	private bool tumble;

	// Token: 0x04000813 RID: 2067
	private bool stunned;

	// Token: 0x04000814 RID: 2068
	private bool stunImpulse;

	// Token: 0x04000815 RID: 2069
	internal bool spawnImpulse;

	// Token: 0x04000816 RID: 2070
	private bool jumpImpulse;

	// Token: 0x04000817 RID: 2071
	private float jumpedTimer;

	// Token: 0x04000818 RID: 2072
	[Header("One Shots")]
	public Sound sfxJump;

	// Token: 0x04000819 RID: 2073
	public Sound sfxLand;

	// Token: 0x0400081A RID: 2074
	public Sound sfxNotice;

	// Token: 0x0400081B RID: 2075
	public Sound sfxCleaverSwing;

	// Token: 0x0400081C RID: 2076
	public Sound sfxCharge;

	// Token: 0x0400081D RID: 2077
	public Sound sfxHurt;

	// Token: 0x0400081E RID: 2078
	public Sound sfxMoveShort;

	// Token: 0x0400081F RID: 2079
	public Sound sfxMoveLong;

	// Token: 0x04000820 RID: 2080
	public Sound sfxHurtColliderImpactAny;

	// Token: 0x04000821 RID: 2081
	[Header("Loops")]
	public Sound sfxStunnedLoop;

	// Token: 0x04000822 RID: 2082
	public Sound sfxTumbleLoopLocal;

	// Token: 0x04000823 RID: 2083
	public Sound sfxTumbleLoopGlobal;

	// Token: 0x04000824 RID: 2084
	public bool showGizmos = true;

	// Token: 0x04000825 RID: 2085
	public float gizmoMinDistance = 3f;

	// Token: 0x04000826 RID: 2086
	public float gizmoMaxDistance = 8f;
}
