using System;
using UnityEngine;

// Token: 0x020001CE RID: 462
public class ParticleScriptExplosion : MonoBehaviour
{
	// Token: 0x06000F77 RID: 3959 RVA: 0x0008DD10 File Offset: 0x0008BF10
	private void Start()
	{
		this.explosionPrefab = Resources.Load<GameObject>("Effects/Part Prefab Explosion");
	}

	// Token: 0x06000F78 RID: 3960 RVA: 0x0008DD24 File Offset: 0x0008BF24
	public ParticlePrefabExplosion Spawn(Vector3 position, float size, int damage, int enemyDamage, float forceMulti = 1f, bool onlyParticleEffect = false, bool disableSound = false, float shakeMultiplier = 1f)
	{
		if (size < 0.25f)
		{
			if (!disableSound)
			{
				this.explosionPreset.explosionSoundSmall.Play(position, 1f, 1f, 1f, 1f);
				this.explosionPreset.explosionSoundSmallGlobal.Play(position, 1f, 1f, 1f, 1f);
			}
			if (shakeMultiplier != 0f)
			{
				GameDirector.instance.CameraImpact.ShakeDistance(3f * shakeMultiplier, 3f, 6f, base.transform.position, 0.2f);
				GameDirector.instance.CameraShake.ShakeDistance(3f * shakeMultiplier, 3f, 6f, base.transform.position, 0.5f);
			}
		}
		else if (size < 0.5f)
		{
			if (!disableSound)
			{
				this.explosionPreset.explosionSoundMedium.Play(position, 1f, 1f, 1f, 1f);
				this.explosionPreset.explosionSoundMediumGlobal.Play(position, 1f, 1f, 1f, 1f);
			}
			if (shakeMultiplier != 0f)
			{
				GameDirector.instance.CameraImpact.ShakeDistance(5f * shakeMultiplier, 4f, 8f, base.transform.position, 0.2f);
				GameDirector.instance.CameraShake.ShakeDistance(5f * shakeMultiplier, 4f, 8f, base.transform.position, 0.5f);
			}
		}
		else
		{
			if (!disableSound)
			{
				this.explosionPreset.explosionSoundBig.Play(position, 1f, 1f, 1f, 1f);
				this.explosionPreset.explosionSoundBigGlobal.Play(position, 1f, 1f, 1f, 1f);
			}
			if (shakeMultiplier != 0f)
			{
				GameDirector.instance.CameraImpact.ShakeDistance(10f * shakeMultiplier, 6f, 12f, base.transform.position, 0.2f);
				GameDirector.instance.CameraShake.ShakeDistance(5f * shakeMultiplier, 6f, 12f, base.transform.position, 0.5f);
			}
		}
		ParticlePrefabExplosion component = Object.Instantiate<GameObject>(this.explosionPrefab, position, Quaternion.identity).GetComponent<ParticlePrefabExplosion>();
		component.forceMultiplier = this.explosionPreset.explosionForceMultiplier * forceMulti;
		component.explosionSize = size;
		component.explosionDamage = damage;
		component.explosionDamageEnemy = enemyDamage;
		component.lightColorOverTime = this.explosionPreset.lightColor;
		component.particleFire.colorOverLifetime.color = this.explosionPreset.explosionColors;
		component.particleSmoke.colorOverLifetime.color = this.explosionPreset.smokeColors;
		component.particleFire.Play();
		component.particleSmoke.Play();
		component.light.enabled = true;
		return component;
	}

	// Token: 0x04001A49 RID: 6729
	public ExplosionPreset explosionPreset;

	// Token: 0x04001A4A RID: 6730
	private GameObject explosionPrefab;
}
