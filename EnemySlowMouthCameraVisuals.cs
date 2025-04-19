using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000076 RID: 118
public class EnemySlowMouthCameraVisuals : MonoBehaviour
{
	// Token: 0x06000474 RID: 1140 RVA: 0x0002C3D4 File Offset: 0x0002A5D4
	private void Start()
	{
		this.jawPositionSpring = new SpringVector3();
		this.jawPositionSpring.damping = 0.5f;
		this.jawPositionSpring.speed = 20f;
		this.topJawSpring = new SpringQuaternion();
		this.topJawSpring.damping = 0.5f;
		this.topJawSpring.speed = 20f;
		this.botJawSpring = new SpringQuaternion();
		this.botJawSpring.damping = 0.5f;
		this.botJawSpring.speed = 20f;
		this.playerAvatar = PlayerAvatar.instance;
		this.topJawStartRotation = this.topJawTransform.localRotation;
		this.botJawStartRotation = this.botJawTransform.localRotation;
		this.topJawTargetRotation = this.topJawStartRotation;
		this.botJawTargetRotation = this.topJawStartRotation;
		this.jawStartPosition = base.transform.localPosition;
	}

	// Token: 0x06000475 RID: 1141 RVA: 0x0002C4B8 File Offset: 0x0002A6B8
	private void StateIntro()
	{
		if (this.stateStart)
		{
			this.stateTimer = 1f;
			float num = 125f;
			this.topJawTransform.localRotation = Quaternion.Euler(num, 0f, 0f);
			this.botJawTransform.localRotation = Quaternion.Euler(-num, 0f, 0f);
			this.topJawSpring.lastRotation = this.topJawTransform.localRotation;
			this.botJawSpring.lastRotation = this.botJawTransform.localRotation;
			this.jawTargetPosition = this.jawStartPosition;
			base.transform.localPosition = this.jawTargetPosition + new Vector3(0f, -1f, -1f);
			this.jawPositionSpring.lastPosition = base.transform.localPosition;
			this.stateStart = false;
		}
		float t = this.curveIntroOutro.Evaluate(1f - this.stateTimer);
		this.openAngleTarget = Mathf.LerpUnclamped(125f, 0f, t);
		this.topJawTargetRotation = this.topJawStartRotation * Quaternion.Euler(this.openAngleTarget, 0f, 0f);
		this.botJawTargetRotation = this.botJawStartRotation * Quaternion.Euler(-this.openAngleTarget, 0f, 0f);
		if (this.stateTimer < 0f)
		{
			this.StateSet(EnemySlowMouthCameraVisuals.State.Idle);
		}
	}

	// Token: 0x06000476 RID: 1142 RVA: 0x0002C628 File Offset: 0x0002A828
	private void StateIdle()
	{
		if (this.stateStart)
		{
			this.botJawTargetRotation = this.botJawStartRotation;
			this.topJawTargetRotation = this.topJawStartRotation;
			this.jawTargetPosition = this.jawStartPosition;
			this.stateStart = false;
			if (this.statePrev == EnemySlowMouthCameraVisuals.State.Puke)
			{
				GameDirector.instance.CameraShake.ShakeDistance(8f, 3f, 8f, base.transform.position, 0.1f);
				GameDirector.instance.CameraImpact.ShakeDistance(12f, 3f, 8f, base.transform.position, 0.1f);
			}
		}
		this.botJawTargetRotation = this.botJawStartRotation * Quaternion.Euler(-2f * Mathf.Sin(Time.time * 2f), 0f, 0f);
		this.topJawTargetRotation = this.topJawStartRotation * Quaternion.Euler(2f * Mathf.Sin(Time.time * 2f), 0f, 0f);
		if (SemiFunc.IsMultiplayer() && this.playerAvatar && this.playerAvatar.voiceChat)
		{
			this.botJawSpring.speed = 20f;
			this.topJawSpring.speed = 20f;
			float num = this.playerAvatar.voiceChat.clipLoudness * 100f;
			num = Mathf.Clamp(num, 0f, 10f);
			this.topJawTargetRotation *= Quaternion.Euler(this.topJawStartRotation.x + num, this.topJawStartRotation.y, this.topJawStartRotation.z);
			this.botJawTargetRotation *= Quaternion.Euler(this.botJawStartRotation.x - num, this.botJawStartRotation.y, this.botJawStartRotation.z);
		}
		if (SemiFunc.FPSImpulse15())
		{
			this.topJawSpring.springVelocity = Random.insideUnitSphere * 0.2f;
			this.botJawSpring.springVelocity = Random.insideUnitSphere * 0.2f;
		}
	}

	// Token: 0x06000477 RID: 1143 RVA: 0x0002C860 File Offset: 0x0002AA60
	private void StatePuke()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
			Vector3 localScale = this.pukeCapsuleTransform.localScale;
			localScale.x = 0f;
			GameDirector.instance.CameraShake.ShakeDistance(8f, 3f, 8f, base.transform.position, 0.1f);
			GameDirector.instance.CameraImpact.ShakeDistance(12f, 3f, 8f, base.transform.position, 0.1f);
			this.pukeCapsuleTransform.localScale = localScale;
		}
		GameDirector.instance.CameraShake.ShakeDistance(4f, 3f, 8f, base.transform.position, 0.1f);
		this.semiPuke.PukeActive(this.semiPuke.transform.position, this.playerTarget.localCameraTransform.rotation);
		this.pukeTimer = 0.2f;
		this.botJawTargetRotation = this.botJawStartRotation * Quaternion.Euler(-25f, 0f, 0f);
		this.topJawTargetRotation = this.topJawStartRotation * Quaternion.Euler(25f, 0f, 0f);
		if (SemiFunc.FPSImpulse15())
		{
			this.topJawSpring.springVelocity = Random.insideUnitSphere * 5f;
			this.botJawSpring.springVelocity = Random.insideUnitSphere * 5f;
		}
	}

	// Token: 0x06000478 RID: 1144 RVA: 0x0002C9E8 File Offset: 0x0002ABE8
	private void StateOutro()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
			this.stateTimer = 1f;
		}
		float t = this.curveIntroOutro.Evaluate(1f - this.stateTimer);
		this.openAngleTarget = Mathf.LerpUnclamped(0f, 125f, t);
		this.topJawTargetRotation = this.topJawStartRotation * Quaternion.Euler(this.openAngleTarget, 0f, 0f);
		this.botJawTargetRotation = this.botJawStartRotation * Quaternion.Euler(-this.openAngleTarget, 0f, 0f);
		this.jawTargetPosition = Vector3.LerpUnclamped(this.jawStartPosition, this.jawStartPosition + new Vector3(0f, 0.2f, -0.4f), t);
		if (this.stateTimer < 0f)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06000479 RID: 1145 RVA: 0x0002CAD4 File Offset: 0x0002ACD4
	private void StateSynchingWithParentEnemy()
	{
		bool flag = this.enemySlowMouth.currentState == EnemySlowMouth.State.Puke;
		if (this.enemySlowMouth.currentState == EnemySlowMouth.State.Attached || this.enemySlowMouth.currentState == EnemySlowMouth.State.Puke || this.enemySlowMouth.currentState == EnemySlowMouth.State.Detach)
		{
			if (flag)
			{
				this.StateSet(EnemySlowMouthCameraVisuals.State.Puke);
				return;
			}
			if (this.state != EnemySlowMouthCameraVisuals.State.Intro)
			{
				this.StateSet(EnemySlowMouthCameraVisuals.State.Idle);
				return;
			}
		}
		else
		{
			this.StateSet(EnemySlowMouthCameraVisuals.State.Outro);
		}
	}

	// Token: 0x0600047A RID: 1146 RVA: 0x0002CB48 File Offset: 0x0002AD48
	private void StateMachine()
	{
		this.StateSynchingWithParentEnemy();
		if (this.stateTimer > 0f)
		{
			this.stateTimer -= Time.deltaTime;
		}
		switch (this.state)
		{
		case EnemySlowMouthCameraVisuals.State.Intro:
			this.StateIntro();
			return;
		case EnemySlowMouthCameraVisuals.State.Idle:
			this.StateIdle();
			return;
		case EnemySlowMouthCameraVisuals.State.Puke:
			this.StatePuke();
			return;
		case EnemySlowMouthCameraVisuals.State.Outro:
			this.StateOutro();
			return;
		default:
			return;
		}
	}

	// Token: 0x0600047B RID: 1147 RVA: 0x0002CBB3 File Offset: 0x0002ADB3
	public void StateSet(EnemySlowMouthCameraVisuals.State newState)
	{
		if (this.state != newState)
		{
			this.statePrev = this.state;
			this.state = newState;
			this.stateStart = true;
			this.stateTimer = 0f;
		}
	}

	// Token: 0x0600047C RID: 1148 RVA: 0x0002CBE4 File Offset: 0x0002ADE4
	private void PukeParticles(bool _play)
	{
		foreach (ParticleSystem particleSystem in this.pukeParticles)
		{
			if (_play)
			{
				if (!particleSystem.isPlaying)
				{
					particleSystem.Play();
				}
			}
			else if (particleSystem.isPlaying)
			{
				particleSystem.Stop();
			}
		}
	}

	// Token: 0x0600047D RID: 1149 RVA: 0x0002CC54 File Offset: 0x0002AE54
	private void Update()
	{
		this.StateMachine();
		if (this.pukeTimer > 0f)
		{
			this.PukeParticles(true);
			this.pukeTimer -= Time.deltaTime;
			this.pukeCapsuleTransform.localScale = Vector3.Lerp(this.pukeCapsuleTransform.localScale, Vector3.one, Time.deltaTime * 5f);
			this.pukeCapsuleTransform.localScale += Vector3.one * Mathf.Sin(Time.time * 30f) * 0.01f;
			this.pukeCapsuleTransform.localScale += Vector3.one * Mathf.Sin(Time.time * 60f) * 0.01f;
			Vector2 textureOffset = this.pukeCapsuleRenderer.material.GetTextureOffset("_MainTex");
			textureOffset.x -= Time.deltaTime * 1.5f;
			this.pukeCapsuleRenderer.material.SetTextureOffset("_MainTex", textureOffset);
			if (!this.pukeCapsuleTransform.gameObject.activeSelf)
			{
				this.pukeCapsuleTransform.localScale = Vector3.zero;
				this.pukeCapsuleTransform.gameObject.SetActive(true);
			}
		}
		else
		{
			this.PukeParticles(false);
			this.pukeCapsuleTransform.localScale = Vector3.Lerp(this.pukeCapsuleTransform.localScale, Vector3.zero, Time.deltaTime * 5f);
			if (this.pukeCapsuleTransform.localScale.x < 0.01f && this.pukeCapsuleTransform.gameObject.activeSelf)
			{
				this.pukeCapsuleTransform.localScale = Vector3.zero;
				this.pukeCapsuleTransform.gameObject.SetActive(false);
			}
		}
		base.transform.localPosition = SemiFunc.SpringVector3Get(this.jawPositionSpring, this.jawTargetPosition, -1f);
		this.topJawTransform.localRotation = SemiFunc.SpringQuaternionGet(this.topJawSpring, this.topJawTargetRotation, -1f);
		this.botJawTransform.localRotation = SemiFunc.SpringQuaternionGet(this.botJawSpring, this.botJawTargetRotation, -1f);
	}

	// Token: 0x04000740 RID: 1856
	internal EnemySlowMouth enemySlowMouth;

	// Token: 0x04000741 RID: 1857
	internal EnemySlowMouthCameraVisuals.State state;

	// Token: 0x04000742 RID: 1858
	internal EnemySlowMouthCameraVisuals.State statePrev;

	// Token: 0x04000743 RID: 1859
	public SemiPuke semiPuke;

	// Token: 0x04000744 RID: 1860
	public AnimationCurve curveIntroOutro;

	// Token: 0x04000745 RID: 1861
	public Transform pukeCapsuleTransform;

	// Token: 0x04000746 RID: 1862
	public MeshRenderer pukeCapsuleRenderer;

	// Token: 0x04000747 RID: 1863
	private float curveEval;

	// Token: 0x04000748 RID: 1864
	private PlayerAvatar playerAvatar;

	// Token: 0x04000749 RID: 1865
	public Transform topJawTransform;

	// Token: 0x0400074A RID: 1866
	public Transform botJawTransform;

	// Token: 0x0400074B RID: 1867
	public GameObject puke;

	// Token: 0x0400074C RID: 1868
	private Quaternion topJawStartRotation;

	// Token: 0x0400074D RID: 1869
	private Quaternion botJawStartRotation;

	// Token: 0x0400074E RID: 1870
	private Quaternion topJawTargetRotation;

	// Token: 0x0400074F RID: 1871
	private Quaternion botJawTargetRotation;

	// Token: 0x04000750 RID: 1872
	private Vector3 jawStartPosition;

	// Token: 0x04000751 RID: 1873
	private Vector3 jawTargetPosition;

	// Token: 0x04000752 RID: 1874
	private SpringQuaternion topJawSpring;

	// Token: 0x04000753 RID: 1875
	private SpringQuaternion botJawSpring;

	// Token: 0x04000754 RID: 1876
	private SpringVector3 jawPositionSpring;

	// Token: 0x04000755 RID: 1877
	private bool stateStart = true;

	// Token: 0x04000756 RID: 1878
	private float stateTimer;

	// Token: 0x04000757 RID: 1879
	private float openAngleTarget = 125f;

	// Token: 0x04000758 RID: 1880
	private float pukeTimer;

	// Token: 0x04000759 RID: 1881
	internal PlayerAvatar playerTarget;

	// Token: 0x0400075A RID: 1882
	public List<ParticleSystem> pukeParticles;

	// Token: 0x020002DE RID: 734
	public enum State
	{
		// Token: 0x040024A8 RID: 9384
		Intro,
		// Token: 0x040024A9 RID: 9385
		Idle,
		// Token: 0x040024AA RID: 9386
		Puke,
		// Token: 0x040024AB RID: 9387
		Outro
	}
}
