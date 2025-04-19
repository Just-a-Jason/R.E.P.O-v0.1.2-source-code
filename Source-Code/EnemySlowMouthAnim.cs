using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000074 RID: 116
public class EnemySlowMouthAnim : MonoBehaviour
{
	// Token: 0x06000456 RID: 1110 RVA: 0x0002AFC4 File Offset: 0x000291C4
	private void StateIdle()
	{
		if (this.stateStart)
		{
			this.jawOpen = 0f;
			this.talkVolume = 0f;
			this.stateStart = false;
		}
		this.CodeAnimatedTalk(1f);
		this.AnimateEyes(5f);
		this.Paddle(5f, 20f);
	}

	// Token: 0x06000457 RID: 1111 RVA: 0x0002B01C File Offset: 0x0002921C
	private void StatePuke()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		float targetFloat = 30f + Mathf.Sin(Time.time * 40f) * 10f;
		this.upperJaw.localRotation = Quaternion.Euler(this.upperJawStartRot - this.jawOpen, 0f, 0f);
		this.lowerJaw.localRotation = Quaternion.Euler(this.lowerJawStartRot + this.jawOpen, 0f, 0f);
		this.jawOpen = SemiFunc.SpringFloatGet(this.jawSpring, targetFloat, -1f);
		this.AnimateEyes(30f);
		this.EyeScaleSitter(6f);
	}

	// Token: 0x06000458 RID: 1112 RVA: 0x0002B0D0 File Offset: 0x000292D0
	private void StateStunned()
	{
		if (this.stateStart)
		{
			this.jawOpen = 20f;
			this.stateStart = false;
		}
		this.AnimateEyes(20f);
		this.EyeScaleSitter(4f);
		float targetFloat = 30f + Mathf.Sin(Time.time * 40f) * 10f;
		this.upperJaw.localRotation = Quaternion.Euler(this.upperJawStartRot - this.jawOpen, 0f, 0f);
		this.lowerJaw.localRotation = Quaternion.Euler(this.lowerJawStartRot + this.jawOpen, 0f, 0f);
		this.jawOpen = SemiFunc.SpringFloatGet(this.jawSpring, targetFloat, -1f);
		this.Paddle(30f, 10f);
	}

	// Token: 0x06000459 RID: 1113 RVA: 0x0002B1A0 File Offset: 0x000293A0
	private void StateTargetting()
	{
		if (this.stateStart)
		{
			this.jawOpen = 0f;
			this.stateStart = false;
		}
		this.AnimateEyes(20f);
		this.EyeScaleSitter(4f);
		float targetFloat = 30f + Mathf.Sin(Time.time * 40f) * 10f;
		this.upperJaw.localRotation = Quaternion.Euler(this.upperJawStartRot - this.jawOpen, 0f, 0f);
		this.lowerJaw.localRotation = Quaternion.Euler(this.lowerJawStartRot + this.jawOpen, 0f, 0f);
		this.jawOpen = SemiFunc.SpringFloatGet(this.jawSpring, targetFloat, -1f);
		this.Paddle(20f, 12f);
	}

	// Token: 0x0600045A RID: 1114 RVA: 0x0002B26F File Offset: 0x0002946F
	private void StateAttached()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
		}
	}

	// Token: 0x0600045B RID: 1115 RVA: 0x0002B280 File Offset: 0x00029480
	private void StateAggro()
	{
		if (this.stateStart)
		{
			this.jawOpen = 0f;
			this.stateStart = false;
		}
		this.AnimateEyes(20f);
		this.EyeScaleSitter(4f);
		this.CodeAnimatedTalk(1f);
		this.Paddle(10f, 20f);
	}

	// Token: 0x0600045C RID: 1116 RVA: 0x0002B2D8 File Offset: 0x000294D8
	private void StateSpawnDespawn()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		this.upperJaw.localRotation = Quaternion.Slerp(this.upperJaw.localRotation, Quaternion.Euler(0f, 0f, 0f), Time.deltaTime * 5f);
		this.lowerJaw.localRotation = Quaternion.Slerp(this.lowerJaw.localRotation, Quaternion.Euler(0f, 0f, 0f), Time.deltaTime * 5f);
		this.Paddle(5f, 20f);
	}

	// Token: 0x0600045D RID: 1117 RVA: 0x0002B378 File Offset: 0x00029578
	private void StateDeath()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
		}
	}

	// Token: 0x0600045E RID: 1118 RVA: 0x0002B38C File Offset: 0x0002958C
	private void StateLeave()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
			this.jawOpen = 0f;
			this.talkVolume = 0f;
		}
		float targetFloat = 5f + Mathf.Sin(Time.time * 40f) * 5f;
		this.upperJaw.localRotation = Quaternion.Euler(this.upperJawStartRot - this.jawOpen, 0f, 0f);
		this.lowerJaw.localRotation = Quaternion.Euler(this.lowerJawStartRot + this.jawOpen, 0f, 0f);
		this.jawOpen = SemiFunc.SpringFloatGet(this.jawSpring, targetFloat, -1f);
		this.Paddle(10f, 20f);
	}

	// Token: 0x0600045F RID: 1119 RVA: 0x0002B450 File Offset: 0x00029650
	private void EyesLookAtTarget()
	{
	}

	// Token: 0x06000460 RID: 1120 RVA: 0x0002B454 File Offset: 0x00029654
	private void CodeAnimatedTalk(float _multiplier = 1f)
	{
		if (SemiFunc.FPSImpulse15())
		{
			float[] array = new float[1024];
			this.audioSource.GetSpectrumData(array, 0, FFTWindow.Hamming);
			float num = array[0] * 50000f * _multiplier;
			if (num > 20f)
			{
				num = 20f;
			}
			this.talkVolume = num;
			this.jawSpring.springVelocity += Random.Range(-25f, 0f);
		}
		this.upperJaw.localRotation = Quaternion.Euler(this.upperJawStartRot - this.jawOpen, 0f, 0f);
		this.lowerJaw.localRotation = Quaternion.Euler(this.lowerJawStartRot + this.jawOpen, 0f, 0f);
		this.jawSpring.damping = 0.2f;
		this.jawSpring.speed = 25f;
		this.jawOpen = SemiFunc.SpringFloatGet(this.jawSpring, this.talkVolume, -1f);
	}

	// Token: 0x06000461 RID: 1121 RVA: 0x0002B550 File Offset: 0x00029750
	private void Start()
	{
		this.eyeLeftSpringScale = new SpringFloat();
		this.eyeLeftSpringScale.damping = 0.01f;
		this.eyeLeftSpringScale.speed = 40f;
		this.eyeRightSpringScale = new SpringFloat();
		this.eyeRightSpringScale.damping = 0.01f;
		this.eyeRightSpringScale.speed = 40f;
		this.eyeRotationSpring = new SpringQuaternion();
		this.eyeRotationSpring.damping = 0.01f;
		this.eyeRotationSpring.speed = 40f;
		this.startPos = base.transform.position;
		this.directionRotation = new SpringQuaternion();
		this.directionRotation.damping = 0.5f;
		this.directionRotation.speed = 10f;
		this.upperJawStartRot = this.upperJaw.localEulerAngles.x;
		this.upperJawStartRot = this.lowerJaw.localEulerAngles.x;
		this.audioSource = base.GetComponent<AudioSource>();
		this.jawSpring = new SpringFloat();
		this.jawSpring.damping = 0.12f;
		this.jawSpring.speed = 12f;
		this.eyeTarget = PlayerAvatar.instance.PlayerVisionTarget.VisionTransform;
		this.particleSystems.AddRange(this.particleTransforn.GetComponentsInChildren<ParticleSystem>());
	}

	// Token: 0x06000462 RID: 1122 RVA: 0x0002B6A8 File Offset: 0x000298A8
	private void StateMachine()
	{
		foreach (Transform transform in this.eyes)
		{
			if (transform == this.eyes[0])
			{
				transform.localScale = Vector3.one * SemiFunc.SpringFloatGet(this.eyeLeftSpringScale, 1f, -1f);
			}
			else
			{
				transform.localScale = Vector3.one * SemiFunc.SpringFloatGet(this.eyeRightSpringScale, 1f, -1f);
			}
		}
		switch (this.state)
		{
		case EnemySlowMouthAnim.State.Idle:
			this.StateIdle();
			break;
		case EnemySlowMouthAnim.State.Puke:
			this.StatePuke();
			break;
		case EnemySlowMouthAnim.State.Stunned:
			this.StateStunned();
			break;
		case EnemySlowMouthAnim.State.Targetting:
			this.StateTargetting();
			break;
		case EnemySlowMouthAnim.State.Attached:
			this.StateAttached();
			break;
		case EnemySlowMouthAnim.State.Aggro:
			this.StateAggro();
			break;
		case EnemySlowMouthAnim.State.SpawnDespawn:
			this.StateSpawnDespawn();
			break;
		case EnemySlowMouthAnim.State.Death:
			this.StateDeath();
			break;
		case EnemySlowMouthAnim.State.Leave:
			this.StateLeave();
			break;
		}
		if (this.state == EnemySlowMouthAnim.State.SpawnDespawn || this.state == EnemySlowMouthAnim.State.Death)
		{
			this.PlayParticles(false);
			return;
		}
		if (this.jawOpen > 15f)
		{
			this.PlayParticles(true);
			return;
		}
		this.PlayParticles(false);
	}

	// Token: 0x06000463 RID: 1123 RVA: 0x0002B80C File Offset: 0x00029A0C
	public void UpdateState(EnemySlowMouthAnim.State _state)
	{
		if (this.state == _state)
		{
			return;
		}
		this.state = _state;
		this.stateStart = true;
	}

	// Token: 0x06000464 RID: 1124 RVA: 0x0002B828 File Offset: 0x00029A28
	private void Update()
	{
		this.StateMachine();
		if (this.paddleTimer > 0f)
		{
			this.paddleTimer -= Time.deltaTime;
		}
		if (this.paddleTimer <= 0f)
		{
			this.visualsTransform.localRotation = Quaternion.Slerp(this.visualsTransform.localRotation, Quaternion.identity, Time.deltaTime * 5f);
		}
		if (this.talkVolume > 0f)
		{
			this.talkVolume = Mathf.Lerp(this.talkVolume, 0f, Time.deltaTime * 5f);
		}
		if (this.jawOpen > 0f)
		{
			this.jawOpen = Mathf.Lerp(this.jawOpen, 0f, Time.deltaTime * 5f);
		}
	}

	// Token: 0x06000465 RID: 1125 RVA: 0x0002B8F0 File Offset: 0x00029AF0
	private void AnimateEyes(float _eyeJitterAmount)
	{
		if (SemiFunc.FPSImpulse15())
		{
			this.eyeRotationSpring.springVelocity += Random.insideUnitSphere * _eyeJitterAmount;
		}
		foreach (Transform transform in this.eyes)
		{
			transform.localRotation = SemiFunc.SpringQuaternionGet(this.eyeRotationSpring, Quaternion.identity, -1f);
		}
	}

	// Token: 0x06000466 RID: 1126 RVA: 0x0002B980 File Offset: 0x00029B80
	private void EyeScaleSitter(float _amount)
	{
		this.eyeLeftSpringScale.springVelocity += Random.Range(0f, _amount);
		this.eyeRightSpringScale.springVelocity += Random.Range(0f, _amount);
	}

	// Token: 0x06000467 RID: 1127 RVA: 0x0002B9BC File Offset: 0x00029BBC
	public void PlayParticles(bool _play)
	{
		if (this.particlesPlaying == _play)
		{
			return;
		}
		this.particlesPlaying = _play;
		foreach (ParticleSystem particleSystem in this.particleSystems)
		{
			if (_play)
			{
				particleSystem.Play();
			}
			else
			{
				particleSystem.Stop();
			}
		}
	}

	// Token: 0x06000468 RID: 1128 RVA: 0x0002BA2C File Offset: 0x00029C2C
	private void Paddle(float _speed, float _amount)
	{
		float num = Mathf.Sin(Time.time * _speed) * _amount;
		this.visualsTransform.localRotation = Quaternion.Euler(0f, num * 2f, 0f);
		this.paddleTimer = 0.1f;
	}

	// Token: 0x0400070F RID: 1807
	public Transform visualsTransform;

	// Token: 0x04000710 RID: 1808
	private Vector3 startPos;

	// Token: 0x04000711 RID: 1809
	private Vector3 prevPos;

	// Token: 0x04000712 RID: 1810
	private SpringQuaternion directionRotation;

	// Token: 0x04000713 RID: 1811
	public Transform upperJaw;

	// Token: 0x04000714 RID: 1812
	public Transform lowerJaw;

	// Token: 0x04000715 RID: 1813
	public Transform headTransform;

	// Token: 0x04000716 RID: 1814
	private float upperJawStartRot;

	// Token: 0x04000717 RID: 1815
	private float lowerJawStartRot;

	// Token: 0x04000718 RID: 1816
	public Sound talkLoop;

	// Token: 0x04000719 RID: 1817
	private AudioSource audioSource;

	// Token: 0x0400071A RID: 1818
	private SpringFloat jawSpring;

	// Token: 0x0400071B RID: 1819
	private float jawOpen;

	// Token: 0x0400071C RID: 1820
	public List<Transform> eyes = new List<Transform>();

	// Token: 0x0400071D RID: 1821
	private Transform eyeTarget;

	// Token: 0x0400071E RID: 1822
	public Transform eyesMiddle;

	// Token: 0x0400071F RID: 1823
	private Quaternion eyeRotation;

	// Token: 0x04000720 RID: 1824
	private SpringQuaternion eyeRotationSpring;

	// Token: 0x04000721 RID: 1825
	public Transform particleTransforn;

	// Token: 0x04000722 RID: 1826
	private List<ParticleSystem> particleSystems = new List<ParticleSystem>();

	// Token: 0x04000723 RID: 1827
	private bool particlesPlaying;

	// Token: 0x04000724 RID: 1828
	public EnemySlowMouth enemySlowMouth;

	// Token: 0x04000725 RID: 1829
	private bool stateStart;

	// Token: 0x04000726 RID: 1830
	private SpringFloat eyeLeftSpringScale;

	// Token: 0x04000727 RID: 1831
	private SpringFloat eyeRightSpringScale;

	// Token: 0x04000728 RID: 1832
	private float paddleTimer;

	// Token: 0x04000729 RID: 1833
	private float talkVolume;

	// Token: 0x0400072A RID: 1834
	public EnemySlowMouthAnim.State state;

	// Token: 0x020002DD RID: 733
	public enum State
	{
		// Token: 0x0400249E RID: 9374
		Idle,
		// Token: 0x0400249F RID: 9375
		Puke,
		// Token: 0x040024A0 RID: 9376
		Stunned,
		// Token: 0x040024A1 RID: 9377
		Targetting,
		// Token: 0x040024A2 RID: 9378
		Attached,
		// Token: 0x040024A3 RID: 9379
		Aggro,
		// Token: 0x040024A4 RID: 9380
		SpawnDespawn,
		// Token: 0x040024A5 RID: 9381
		Death,
		// Token: 0x040024A6 RID: 9382
		Leave
	}
}
