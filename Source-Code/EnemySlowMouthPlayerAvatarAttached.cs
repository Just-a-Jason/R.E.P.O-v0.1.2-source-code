using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000078 RID: 120
public class EnemySlowMouthPlayerAvatarAttached : MonoBehaviour
{
	// Token: 0x06000484 RID: 1156 RVA: 0x0002D238 File Offset: 0x0002B438
	private void Start()
	{
		this.loudnessSpring = new SpringFloat();
		this.loudnessSpring.damping = 0.5f;
		this.loudnessSpring.speed = 20f;
		this.springFloatScale = new SpringFloat();
		this.springFloatScale.damping = 0.35f;
		this.springFloatScale.speed = 10f;
		base.transform.localScale = Vector3.one * 2f;
		this.springFloatScale.lastPosition = 2f;
		this.playerVoiceChat = this.playerTarget.voiceChat;
	}

	// Token: 0x06000485 RID: 1157 RVA: 0x0002D2D6 File Offset: 0x0002B4D6
	private void StateIntro()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		this.loudnessTarget = 0f;
		this.scaleTarget = 1f;
	}

	// Token: 0x06000486 RID: 1158 RVA: 0x0002D2FD File Offset: 0x0002B4FD
	private void StateIdle()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		this.loudnessTarget = 0f;
		this.scaleTarget = 1f;
	}

	// Token: 0x06000487 RID: 1159 RVA: 0x0002D324 File Offset: 0x0002B524
	private void StatePuke()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		float num = Mathf.Sin(Time.time * 40f) * 0.05f;
		this.loudnessTarget = 0.2f + num;
		this.scaleTarget = 1f;
		this.semiPuke.PukeActive(this.semiPuke.transform.position, this.playerTarget.localCameraTransform.rotation);
	}

	// Token: 0x06000488 RID: 1160 RVA: 0x0002D39C File Offset: 0x0002B59C
	private void StateOutro()
	{
		if (this.stateStart)
		{
			this.particles.gameObject.SetActive(true);
			this.stateStart = false;
		}
		this.loudnessTarget = 0f;
		this.scaleTarget = 0f;
		if (base.transform.localScale.x < 0.05f)
		{
			this.enemySlowMouth.UpdateState(EnemySlowMouth.State.Detach);
			Object.Destroy(this.jawBot.gameObject);
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06000489 RID: 1161 RVA: 0x0002D420 File Offset: 0x0002B620
	private void StateMachine()
	{
		switch (this.state)
		{
		case EnemySlowMouthPlayerAvatarAttached.State.Intro:
			this.StateIntro();
			return;
		case EnemySlowMouthPlayerAvatarAttached.State.Idle:
			this.StateIdle();
			return;
		case EnemySlowMouthPlayerAvatarAttached.State.Puke:
			this.StatePuke();
			return;
		case EnemySlowMouthPlayerAvatarAttached.State.Outro:
			this.StateOutro();
			return;
		default:
			return;
		}
	}

	// Token: 0x0600048A RID: 1162 RVA: 0x0002D468 File Offset: 0x0002B668
	private void Update()
	{
		if (this.playerVoiceChat)
		{
			this.playerVoiceChat.OverrideClipLoudnessAnimationValue(this.loudnessAdd);
		}
		else if (this.playerTarget)
		{
			this.playerVoiceChat = this.playerTarget.voiceChat;
		}
		this.loudnessAdd = SemiFunc.SpringFloatGet(this.loudnessSpring, this.loudnessTarget, -1f);
		Quaternion rotation = this.playerTarget.playerAvatarVisuals.playerEyes.eyeLeft.rotation;
		Quaternion rotation2 = this.playerTarget.playerAvatarVisuals.playerEyes.eyeRight.rotation;
		this.eyeTransforms[0].rotation = rotation;
		this.eyeTransforms[1].rotation = rotation2;
		this.StateSynchingWithParentEnemy();
		this.StateMachine();
		base.transform.localScale = Vector3.one * SemiFunc.SpringFloatGet(this.springFloatScale, this.scaleTarget, -1f);
		this.jawBot.localScale = Vector3.one * SemiFunc.SpringFloatGet(this.springFloatScale, this.scaleTarget, -1f);
	}

	// Token: 0x0600048B RID: 1163 RVA: 0x0002D58C File Offset: 0x0002B78C
	private void StateSynchingWithParentEnemy()
	{
		if (!this.enemySlowMouth)
		{
			this.StateSet(EnemySlowMouthPlayerAvatarAttached.State.Outro);
			return;
		}
		bool flag = this.enemySlowMouth.currentState == EnemySlowMouth.State.Puke;
		if (this.enemySlowMouth.currentState == EnemySlowMouth.State.Attached || this.enemySlowMouth.currentState == EnemySlowMouth.State.Puke || this.enemySlowMouth.currentState == EnemySlowMouth.State.Detach)
		{
			if (flag)
			{
				this.StateSet(EnemySlowMouthPlayerAvatarAttached.State.Puke);
				return;
			}
			if (this.state != EnemySlowMouthPlayerAvatarAttached.State.Intro)
			{
				this.StateSet(EnemySlowMouthPlayerAvatarAttached.State.Idle);
				return;
			}
		}
		else
		{
			this.StateSet(EnemySlowMouthPlayerAvatarAttached.State.Outro);
		}
	}

	// Token: 0x0600048C RID: 1164 RVA: 0x0002D612 File Offset: 0x0002B812
	private void StateSet(EnemySlowMouthPlayerAvatarAttached.State _state)
	{
		if (this.state != _state)
		{
			this.state = _state;
			this.stateStart = true;
		}
	}

	// Token: 0x0600048D RID: 1165 RVA: 0x0002D62C File Offset: 0x0002B82C
	private void OnDisable()
	{
		if (this.playerTarget.isDisabled)
		{
			this.enemySlowMouth.UpdateState(EnemySlowMouth.State.Detach);
			this.enemySlowMouth.detachPosition = this.playerTarget.localCameraPosition;
			this.enemySlowMouth.detachRotation = this.playerTarget.localCameraRotation;
			Object.Destroy(this.jawBot.gameObject);
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x04000764 RID: 1892
	public Transform jawBot;

	// Token: 0x04000765 RID: 1893
	public Transform particles;

	// Token: 0x04000766 RID: 1894
	private SpringFloat springFloatScale;

	// Token: 0x04000767 RID: 1895
	internal EnemySlowMouth enemySlowMouth;

	// Token: 0x04000768 RID: 1896
	internal SemiPuke semiPuke;

	// Token: 0x04000769 RID: 1897
	private float scaleTarget = 1f;

	// Token: 0x0400076A RID: 1898
	private bool stateStart;

	// Token: 0x0400076B RID: 1899
	internal PlayerAvatar playerTarget;

	// Token: 0x0400076C RID: 1900
	public List<Transform> eyeTransforms;

	// Token: 0x0400076D RID: 1901
	private PlayerVoiceChat playerVoiceChat;

	// Token: 0x0400076E RID: 1902
	private float loudnessAdd;

	// Token: 0x0400076F RID: 1903
	private SpringFloat loudnessSpring;

	// Token: 0x04000770 RID: 1904
	private float loudnessTarget;

	// Token: 0x04000771 RID: 1905
	public EnemySlowMouthPlayerAvatarAttached.State state;

	// Token: 0x020002DF RID: 735
	public enum State
	{
		// Token: 0x040024AD RID: 9389
		Intro,
		// Token: 0x040024AE RID: 9390
		Idle,
		// Token: 0x040024AF RID: 9391
		Puke,
		// Token: 0x040024B0 RID: 9392
		Outro
	}
}
