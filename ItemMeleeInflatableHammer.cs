using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000155 RID: 341
public class ItemMeleeInflatableHammer : MonoBehaviour
{
	// Token: 0x06000B6A RID: 2922 RVA: 0x00064D9F File Offset: 0x00062F9F
	private void Start()
	{
		this.particleScriptExplosion = base.GetComponent<ParticleScriptExplosion>();
		this.explosionPosition = base.transform.Find("Explosion Position");
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000B6B RID: 2923 RVA: 0x00064DCF File Offset: 0x00062FCF
	public void OnHit()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && Random.Range(0, 19) == 0)
		{
			if (SemiFunc.IsMultiplayer())
			{
				this.photonView.RPC("ExplosionRPC", RpcTarget.All, Array.Empty<object>());
				return;
			}
			this.ExplosionRPC();
		}
	}

	// Token: 0x06000B6C RID: 2924 RVA: 0x00064E08 File Offset: 0x00063008
	[PunRPC]
	public void ExplosionRPC()
	{
		ParticlePrefabExplosion particlePrefabExplosion = this.particleScriptExplosion.Spawn(this.explosionPosition.position, 0.5f, 0, 250, 1f, false, false, 1f);
		particlePrefabExplosion.SkipHurtColliderSetup = true;
		particlePrefabExplosion.HurtCollider.playerDamage = 0;
		particlePrefabExplosion.HurtCollider.enemyDamage = 250;
		particlePrefabExplosion.HurtCollider.physImpact = HurtCollider.BreakImpact.Heavy;
		particlePrefabExplosion.HurtCollider.physHingeDestroy = true;
		particlePrefabExplosion.HurtCollider.playerTumbleForce = 30f;
		particlePrefabExplosion.HurtCollider.playerTumbleTorque = 50f;
	}

	// Token: 0x04001271 RID: 4721
	private ParticleScriptExplosion particleScriptExplosion;

	// Token: 0x04001272 RID: 4722
	private Transform explosionPosition;

	// Token: 0x04001273 RID: 4723
	private PhotonView photonView;
}
