using System;
using UnityEngine;

// Token: 0x0200007F RID: 127
public class EnemyThinManAnim : MonoBehaviour
{
	// Token: 0x060004DA RID: 1242 RVA: 0x0002FD80 File Offset: 0x0002DF80
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
		this.animator.keepAnimatorStateOnDisable = true;
		this.randomOffsets = new float[6];
		for (int i = 0; i < this.randomOffsets.Length; i++)
		{
			this.randomOffsets[i] = Random.Range(0f, 6.2831855f);
		}
	}

	// Token: 0x060004DB RID: 1243 RVA: 0x0002FDDC File Offset: 0x0002DFDC
	private void Update()
	{
		this.growLoop.PlayLoop(this.tentaclesParent.activeSelf, 5f, 5f, 1f);
		if (this.controller.tentacleLerp > 0f)
		{
			this.backLight.intensity = this.tentacleCurve.Evaluate(this.controller.tentacleLerp) * 4f;
		}
		else
		{
			this.backLight.intensity = 0f;
		}
		if (this.controller.tentacleLerp > 0f)
		{
			if (!this.tentaclesParent.activeSelf)
			{
				this.tentaclesParent.SetActive(true);
			}
			this.tentaclesParent.transform.localScale = new Vector3(this.tentacleCurve.Evaluate(this.controller.tentacleLerp), this.tentacleCurve.Evaluate(this.controller.tentacleLerp), this.tentacleCurve.Evaluate(this.controller.tentacleLerp));
		}
		else if (this.tentaclesParent.activeSelf)
		{
			this.tentaclesParent.SetActive(false);
		}
		if (this.controller.tentacleLerp > 0f)
		{
			float num = this.controller.tentacleLerp * 20f;
			float num2 = Mathf.Lerp(10f, 1f, this.controller.tentacleLerp);
			this.tentacleR1.transform.localRotation = Quaternion.Euler(Mathf.Sin(num2 + this.randomOffsets[0]) * num, 0f, 0f);
			this.tentacleR2.transform.localRotation = Quaternion.Euler(Mathf.Sin(num2 + this.randomOffsets[1]) * num, 0f, 0f);
			this.tentacleR3.transform.localRotation = Quaternion.Euler(Mathf.Sin(num2 + this.randomOffsets[2]) * num, 0f, 0f);
			this.tentacleL1.transform.localRotation = Quaternion.Euler(Mathf.Sin(num2 + this.randomOffsets[3]) * num, 0f, 0f);
			this.tentacleL2.transform.localRotation = Quaternion.Euler(Mathf.Sin(num2 + this.randomOffsets[4]) * num, 0f, 0f);
			this.tentacleL3.transform.localRotation = Quaternion.Euler(Mathf.Sin(num2 + this.randomOffsets[5]) * num, 0f, 0f);
		}
		if (this.controller.currentState == EnemyThinMan.State.TentacleExtend || this.controller.currentState == EnemyThinMan.State.Damage)
		{
			if (!this.extendedTentacles.activeSelf)
			{
				this.tentaclesParent.SetActive(false);
				this.extendedPouch.SetActive(true);
				this.particleSmoke.Play();
				this.attack.Play(base.transform.position, 1f, 1f, 1f, 1f);
				GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, this.controller.playerTarget.transform.position, 0.3f);
				GameDirector.instance.CameraImpact.ShakeDistance(10f, 3f, 8f, this.controller.playerTarget.transform.position, 0.1f);
				this.extendedTentacles.SetActive(true);
			}
			if (this.controller.playerTarget)
			{
				float z = Vector3.Distance(this.controller.playerTarget.transform.position, this.extendedTentacles.transform.position);
				this.extendedTentacles.transform.localScale = new Vector3(1f, 1f, z);
				this.extendedTentacles.transform.LookAt(this.controller.playerTarget.PlayerVisionTarget.VisionTransform.position);
			}
		}
		else if (this.extendedTentacles.activeSelf)
		{
			Vector3 localScale = this.extendedTentacles.transform.localScale;
			localScale.z = Mathf.Lerp(localScale.z, 0f, 10f * Time.deltaTime);
			this.extendedTentacles.transform.localScale = localScale;
			if (this.extendedTentacles.transform.localScale.z <= 0.1f)
			{
				this.extendedTentacles.SetActive(false);
				this.extendedPouch.SetActive(false);
			}
		}
		if (this.rattleImpulse)
		{
			if (this.enemy.Health.healthCurrent > 0)
			{
				int num3 = Random.Range(1, 3);
				this.animator.SetTrigger("Rattle" + num3.ToString());
			}
			this.rattleImpulse = false;
		}
		if (this.enemy.CurrentState == EnemyState.Despawn && this.enemy.Health.healthCurrent > 0)
		{
			this.animator.SetBool("Despawn", true);
			if (this.despawnImpulse)
			{
				this.animator.SetTrigger("DespawnTrigger");
				this.despawnImpulse = false;
			}
		}
		else
		{
			this.animator.SetBool("Despawn", false);
			this.despawnImpulse = true;
		}
		if (this.enemy.IsStunned() && this.enemy.CurrentState != EnemyState.Despawn && this.enemy.Health.healthCurrent > 0)
		{
			this.animator.SetBool("Stun", true);
			if (this.stunImpulse)
			{
				this.animator.SetTrigger("StunTrigger");
				this.stunImpulse = false;
				return;
			}
		}
		else
		{
			this.animator.SetBool("Stun", false);
			this.stunImpulse = true;
		}
	}

	// Token: 0x060004DC RID: 1244 RVA: 0x00030390 File Offset: 0x0002E590
	public void NoticeSet()
	{
		if (this.enemy.Health.healthCurrent < 0)
		{
			return;
		}
		if (this.controller.playerTarget.isLocal)
		{
			float num = 30f;
			if (Vector3.Distance(this.controller.playerTarget.transform.position, this.enemy.transform.position) > 5f)
			{
				num = 20f;
			}
			CameraGlitch.Instance.PlayShort();
			CameraAim.Instance.AimTargetSet(this.controller.head.transform.position, 0.75f, 2f, this.controller.gameObject, 90);
			CameraZoom.Instance.OverrideZoomSet(num, 0.75f, 3f, 1f, this.controller.gameObject, 50);
			this.zoom.Play(base.transform.position, 1f, 1f, 1f, 1f);
		}
		this.animator.SetTrigger("Scream");
	}

	// Token: 0x060004DD RID: 1245 RVA: 0x000304A5 File Offset: 0x0002E6A5
	public void OnDeath()
	{
		this.animator.SetTrigger("Death");
	}

	// Token: 0x060004DE RID: 1246 RVA: 0x000304B8 File Offset: 0x0002E6B8
	public void Scream()
	{
		this.screamLocal.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.screamGlobal.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060004DF RID: 1247 RVA: 0x0003051C File Offset: 0x0002E71C
	public void DeathEffect()
	{
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.1f);
		this.particleImpact.Play();
		Quaternion rotation = Quaternion.LookRotation(-this.enemy.Health.hurtDirection.normalized);
		this.particleDirectionalBits.transform.rotation = rotation;
		this.particleDirectionalBits.Play();
		this.deathSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.enemy.EnemyParent.Despawn();
	}

	// Token: 0x060004E0 RID: 1248 RVA: 0x00030607 File Offset: 0x0002E807
	public void SetDespawn()
	{
		this.enemy.EnemyParent.Despawn();
	}

	// Token: 0x060004E1 RID: 1249 RVA: 0x0003061C File Offset: 0x0002E81C
	public void DespawnSmoke()
	{
		this.controller.SmokeEffect(this.controller.rb.position);
		this.teleportOut.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x040007C5 RID: 1989
	internal Animator animator;

	// Token: 0x040007C6 RID: 1990
	public EnemyThinMan controller;

	// Token: 0x040007C7 RID: 1991
	public Enemy enemy;

	// Token: 0x040007C8 RID: 1992
	public GameObject mesh;

	// Token: 0x040007C9 RID: 1993
	public Light backLight;

	// Token: 0x040007CA RID: 1994
	public GameObject tentaclesParent;

	// Token: 0x040007CB RID: 1995
	public GameObject extendedTentacles;

	// Token: 0x040007CC RID: 1996
	public GameObject extendedPouch;

	// Token: 0x040007CD RID: 1997
	public float tentacleSpeed;

	// Token: 0x040007CE RID: 1998
	public AnimationCurve tentacleCurve;

	// Token: 0x040007CF RID: 1999
	private float[] randomOffsets;

	// Token: 0x040007D0 RID: 2000
	private bool tentacleBackActive;

	// Token: 0x040007D1 RID: 2001
	public GameObject tentacleR1;

	// Token: 0x040007D2 RID: 2002
	public GameObject tentacleR2;

	// Token: 0x040007D3 RID: 2003
	public GameObject tentacleR3;

	// Token: 0x040007D4 RID: 2004
	public GameObject tentacleL1;

	// Token: 0x040007D5 RID: 2005
	public GameObject tentacleL2;

	// Token: 0x040007D6 RID: 2006
	public GameObject tentacleL3;

	// Token: 0x040007D7 RID: 2007
	internal bool rattleImpulse;

	// Token: 0x040007D8 RID: 2008
	public ParticleSystem particleSmoke;

	// Token: 0x040007D9 RID: 2009
	public ParticleSystem particleSmokeCalmFill;

	// Token: 0x040007DA RID: 2010
	public ParticleSystem particleImpact;

	// Token: 0x040007DB RID: 2011
	public ParticleSystem particleDirectionalBits;

	// Token: 0x040007DC RID: 2012
	[Space]
	public Sound teleportIn;

	// Token: 0x040007DD RID: 2013
	public Sound teleportOut;

	// Token: 0x040007DE RID: 2014
	[Space]
	public Sound notice;

	// Token: 0x040007DF RID: 2015
	[Space]
	public Sound growLoop;

	// Token: 0x040007E0 RID: 2016
	public Sound zoom;

	// Token: 0x040007E1 RID: 2017
	public Sound attack;

	// Token: 0x040007E2 RID: 2018
	public Sound screamLocal;

	// Token: 0x040007E3 RID: 2019
	public Sound screamGlobal;

	// Token: 0x040007E4 RID: 2020
	[Space]
	public Sound hurtSound;

	// Token: 0x040007E5 RID: 2021
	public Sound deathSound;

	// Token: 0x040007E6 RID: 2022
	private bool despawnImpulse;

	// Token: 0x040007E7 RID: 2023
	private bool stunImpulse;
}
