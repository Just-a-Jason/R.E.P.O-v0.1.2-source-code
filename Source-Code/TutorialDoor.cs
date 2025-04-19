using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000262 RID: 610
public class TutorialDoor : MonoBehaviour
{
	// Token: 0x060012F3 RID: 4851 RVA: 0x000A5AE1 File Offset: 0x000A3CE1
	private void Start()
	{
		this.doorEndYPos = 7.42f;
		this.doorLight.intensity = 0f;
	}

	// Token: 0x060012F4 RID: 4852 RVA: 0x000A5B00 File Offset: 0x000A3D00
	private void ThirtyFPS()
	{
		if (this.thirtyFPSUpdateTimer > 0f)
		{
			this.thirtyFPSUpdateTimer -= Time.deltaTime;
			this.thirtyFPSUpdateTimer = Mathf.Max(0f, this.thirtyFPSUpdateTimer);
			return;
		}
		this.thirtyFPSUpdate = true;
		this.thirtyFPSUpdateTimer = 0.033333335f;
	}

	// Token: 0x060012F5 RID: 4853 RVA: 0x000A5B58 File Offset: 0x000A3D58
	private void Update()
	{
		this.StateMachine();
		if (this.fillBarProgress != this.fillBarProgressPrev)
		{
			if (this.fillBarProgress > this.fillBarProgressPrev)
			{
				this.soundGoUp.Pitch = 1f + this.fillBarProgress / 11f;
				this.soundGoUp.Play(this.animationTransform.position, 1f, 1f, 1f, 1f);
				this.particlesBleep1.Play();
				this.particlesBleep2.Play();
			}
			else
			{
				this.soundGoDown.Pitch = 1f + this.fillBarProgress / 11f;
				this.soundGoDown.Play(this.animationTransform.position, 1f, 1f, 1f, 1f);
			}
			this.fillBarProgressPrev = this.fillBarProgress;
		}
	}

	// Token: 0x060012F6 RID: 4854 RVA: 0x000A5C40 File Offset: 0x000A3E40
	private void StateMachine()
	{
		this.ThirtyFPS();
		switch (this.currentState)
		{
		case 0:
			this.StateClosed();
			break;
		case 1:
			this.StateSuccess();
			break;
		case 2:
			this.StateUnlock();
			break;
		case 3:
			this.StateOpening();
			break;
		case 4:
			this.StateOpen();
			break;
		}
		this.EmojiScreenGlitchLogic();
		this.thirtyFPSUpdate = false;
		this.stateTimer += Time.deltaTime;
		if (this.stateTimer > 1000000f)
		{
			this.stateTimer = 0f;
		}
	}

	// Token: 0x060012F7 RID: 4855 RVA: 0x000A5CD3 File Offset: 0x000A3ED3
	private void StateSet(int _state)
	{
		this.prevState = this.currentState;
		this.stateTimer = 0f;
		this.currentState = _state;
		this.stateStart = true;
		this.animationDone = false;
		this.animationProgress = 0f;
		this.animationImpactDone = false;
	}

	// Token: 0x060012F8 RID: 4856 RVA: 0x000A5D14 File Offset: 0x000A3F14
	private void EffectEmoji()
	{
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 8f, this.animationTransform.position, 0.1f);
		this.soundSuccess.Play(this.animationTransform.position, 1f, 1f, 1f, 1f);
		this.lightParticle.transform.localPosition = new Vector3(-0.82f, 3.3f, 0f);
		this.lightParticle.Play();
		this.doorLight.color = new Color(1f, 0.5f, 0f, 1f);
		this.doorLight.range = 10f;
		this.doorLight.intensity = 4f;
	}

	// Token: 0x060012F9 RID: 4857 RVA: 0x000A5DF0 File Offset: 0x000A3FF0
	private void EffectScreenRotateStart()
	{
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 8f, this.animationTransform.position, 0.1f);
		this.soundUnlock.Play(this.animationTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060012FA RID: 4858 RVA: 0x000A5E58 File Offset: 0x000A4058
	private void EffectScreenRotateEnd()
	{
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, this.animationTransform.position, 0.1f);
		this.soundUnlockEnd.Play(this.animationTransform.position, 1f, 1f, 1f, 1f);
		this.particlesUnlock.Play();
		this.lightParticle.transform.localPosition = new Vector3(-0.82f, 3.3f, 0f);
		this.lightParticle.Play();
	}

	// Token: 0x060012FB RID: 4859 RVA: 0x000A5EF8 File Offset: 0x000A40F8
	private void EffectLatchStart()
	{
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 8f, this.animationTransform.position, 0.1f);
		this.soundLatches.Play(this.animationTransform.position, 1f, 1f, 1f, 1f);
		this.lightParticle.transform.localPosition = new Vector3(-0.82f, 3.3f, 4.57f);
		this.lightParticle.Play();
		this.lightParticle2.transform.localPosition = new Vector3(-0.82f, 3.3f, -4.57f);
		this.lightParticle2.Play();
		this.latchLamp1.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", new Color(0f, 1f, 0f, 1f));
		this.latchLamp2.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", new Color(0f, 1f, 0f, 1f));
	}

	// Token: 0x060012FC RID: 4860 RVA: 0x000A6024 File Offset: 0x000A4224
	private void EffectLatchEnd()
	{
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.1f);
		this.soundLatchesEnd.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.particlesLatch1.Play();
		this.particlesLatch2.Play();
		this.lightParticle.transform.localPosition = new Vector3(-0.82f, 3.3f, 3.3f);
		this.lightParticle.Play();
		this.lightParticle2.transform.localPosition = new Vector3(-0.82f, 3.3f, -3.3f);
		this.lightParticle2.Play();
	}

	// Token: 0x060012FD RID: 4861 RVA: 0x000A6100 File Offset: 0x000A4300
	private void EffectDoorOpenStart()
	{
		GameDirector.instance.CameraShake.ShakeDistance(8f, 3f, 8f, this.animationTransform.position, 0.1f);
		this.soundDoorOpen.Play(this.animationTransform.position, 1f, 1f, 1f, 1f);
		this.particlesDoorSmoke.Play();
	}

	// Token: 0x060012FE RID: 4862 RVA: 0x000A6171 File Offset: 0x000A4371
	private void EffectDoorMove()
	{
		this.soundDoorMove.Play(this.animationTransform.position, 1f, 1f, 1f, 1f);
		this.particlesOpen.gameObject.SetActive(true);
	}

	// Token: 0x060012FF RID: 4863 RVA: 0x000A61B0 File Offset: 0x000A43B0
	private void EffectDoorOpenEnd()
	{
		GameDirector.instance.CameraShake.ShakeDistance(8f, 3f, 8f, this.animationTransform.position, 0.1f);
		this.soundSlamCeiling.Play(this.animationTransform.position, 1f, 1f, 1f, 1f);
		this.particlesCeiling.Play();
		Object.Destroy(this.doorLight);
	}

	// Token: 0x06001300 RID: 4864 RVA: 0x000A622C File Offset: 0x000A442C
	private void StateClosed()
	{
		if (TutorialDirector.instance.currentPage > this.tutorialPage)
		{
			this.StateSet(1);
		}
		float num = TutorialUI.instance.progressBarCurrent * 130f;
		this.fillBarProgress = (float)Mathf.FloorToInt(num / 11f);
		for (int i = 0; i < this.fillBars.Count; i++)
		{
			this.fillBars[i].localScale = new Vector3(1f, 1f, Mathf.Clamp01(this.fillBarProgress / 11f));
		}
	}

	// Token: 0x06001301 RID: 4865 RVA: 0x000A62BD File Offset: 0x000A44BD
	private void EmojiSet(string emoji)
	{
		this.doorText.text = "<size=100>|</size>" + emoji + "<size=100>|</size>";
	}

	// Token: 0x06001302 RID: 4866 RVA: 0x000A62DC File Offset: 0x000A44DC
	private void EmojiScreenGlitch(Color color)
	{
		if (this.emojiScreenGlitchTimer <= 0f)
		{
			this.soundEmojiGlitch.Play(this.doorText.transform.position, 1f, 1f, 1f, 1f);
		}
		this.emojiScreenGlitchTimer = 0.2f;
		this.emojiScreenGlitch.SetActive(true);
		this.doorText.enabled = false;
		this.emojiScreenGlitch.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", color);
	}

	// Token: 0x06001303 RID: 4867 RVA: 0x000A6364 File Offset: 0x000A4564
	private void EmojiScreenGlitchLogic()
	{
		if (this.emojiDelay > 0f)
		{
			return;
		}
		this.currentEmoji = this.doorText.text;
		if (this.prevEmoji != this.currentEmoji)
		{
			this.prevEmoji = this.currentEmoji;
			this.EmojiScreenGlitch(Color.yellow);
		}
		if (this.emojiScreenGlitchTimer <= 0f)
		{
			return;
		}
		Vector2 textureOffset = this.emojiScreenGlitch.GetComponent<MeshRenderer>().material.GetTextureOffset("_MainTex");
		textureOffset.y += Time.deltaTime * 15f;
		this.emojiScreenGlitch.GetComponent<MeshRenderer>().material.SetTextureOffset("_MainTex", textureOffset);
		this.emojiScreenGlitchTimer -= Time.deltaTime;
		if (this.thirtyFPSUpdate)
		{
			float num = Random.Range(0.1f, 1f);
			this.emojiScreenGlitch.GetComponent<MeshRenderer>().material.SetTextureScale("_MainTex", new Vector2(num, num));
		}
		if (this.emojiScreenGlitchTimer <= 0f)
		{
			this.emojiScreenGlitch.SetActive(false);
			this.doorText.enabled = true;
		}
	}

	// Token: 0x06001304 RID: 4868 RVA: 0x000A6484 File Offset: 0x000A4684
	private void StateSuccess()
	{
		if (this.stateStart)
		{
			this.EmojiSet("<sprite name=creepycrying>");
			this.EffectEmoji();
			this.stateStart = false;
			for (int i = 0; i < this.fillBars.Count; i++)
			{
				this.fillBars[i].localScale = new Vector3(1f, 1f, 1f);
			}
		}
		if (!this.animationDone)
		{
			if (this.animationProgress == 0f)
			{
				this.EffectScreenRotateStart();
			}
			this.animationProgress += 2f * Time.deltaTime;
			float t = this.animationCurve.Evaluate(this.animationProgress);
			this.screenTransform.localRotation = Quaternion.Euler(Mathf.LerpUnclamped(0f, 45f, t), 0f, 0f);
			if (this.animationProgress >= 0.53f && !this.animationImpactDone)
			{
				this.EffectScreenRotateEnd();
				this.animationImpactDone = true;
			}
			if (this.animationProgress >= 1f)
			{
				this.animationDone = true;
			}
		}
		if (this.stateTimer > 1f)
		{
			this.StateSet(2);
		}
	}

	// Token: 0x06001305 RID: 4869 RVA: 0x000A65A8 File Offset: 0x000A47A8
	private void StateUnlock()
	{
		if (this.stateStart)
		{
			this.EffectLatchStart();
			this.stateStart = false;
		}
		this.animationProgress += 1.5f * Time.deltaTime;
		float t = this.animationCurve.Evaluate(this.animationProgress);
		if (this.animationProgress > 0.53f && !this.animationImpactDone)
		{
			this.animationImpactDone = true;
			this.EffectLatchEnd();
		}
		this.latchTransform.localScale = new Vector3(this.latchTransform.localScale.x, this.latchTransform.localScale.y, Mathf.LerpUnclamped(1f, 0.8f, t));
		if (this.animationProgress >= 1f)
		{
			this.animationDone = true;
			this.StateSet(3);
		}
	}

	// Token: 0x06001306 RID: 4870 RVA: 0x000A6674 File Offset: 0x000A4874
	private void StateOpening()
	{
		if (this.stateStart)
		{
			this.EffectDoorOpenStart();
			this.stateStart = false;
		}
		if (this.animationProgress > 0.53f && !this.moveDone)
		{
			this.moveDone = true;
			this.EffectDoorMove();
		}
		this.animationProgress += Time.deltaTime;
		float t = this.animationCurveDoor.Evaluate(this.animationProgress);
		this.animationTransform.position = new Vector3(this.animationTransform.position.x, Mathf.LerpUnclamped(0f, this.doorEndYPos, t), this.animationTransform.position.z);
		if (this.animationProgress > 0.53f && !this.animationImpactDone)
		{
			this.animationImpactDone = true;
			this.EffectDoorOpenEnd();
		}
		if (this.animationProgress >= 1f)
		{
			this.animationDone = true;
			this.StateSet(4);
		}
	}

	// Token: 0x06001307 RID: 4871 RVA: 0x000A675A File Offset: 0x000A495A
	private void StateOpen()
	{
	}

	// Token: 0x04001FF8 RID: 8184
	public int tutorialPage;

	// Token: 0x04001FF9 RID: 8185
	public AnimationCurve animationCurve;

	// Token: 0x04001FFA RID: 8186
	public AnimationCurve animationCurveDoor;

	// Token: 0x04001FFB RID: 8187
	private float doorEndYPos;

	// Token: 0x04001FFC RID: 8188
	private float animationProgress;

	// Token: 0x04001FFD RID: 8189
	private bool animationDone;

	// Token: 0x04001FFE RID: 8190
	private int prevState;

	// Token: 0x04001FFF RID: 8191
	private int currentState;

	// Token: 0x04002000 RID: 8192
	private float stateTimer;

	// Token: 0x04002001 RID: 8193
	private bool stateStart;

	// Token: 0x04002002 RID: 8194
	public Transform latchTransform;

	// Token: 0x04002003 RID: 8195
	public Transform screenTransform;

	// Token: 0x04002004 RID: 8196
	private bool animationImpactDone;

	// Token: 0x04002005 RID: 8197
	public GameObject emojiScreenGlitch;

	// Token: 0x04002006 RID: 8198
	public TextMeshPro doorText;

	// Token: 0x04002007 RID: 8199
	private string prevEmoji;

	// Token: 0x04002008 RID: 8200
	private string currentEmoji;

	// Token: 0x04002009 RID: 8201
	private float emojiScreenGlitchTimer;

	// Token: 0x0400200A RID: 8202
	private float emojiDelay;

	// Token: 0x0400200B RID: 8203
	private bool thirtyFPSUpdate;

	// Token: 0x0400200C RID: 8204
	public Sound soundEmojiGlitch;

	// Token: 0x0400200D RID: 8205
	private float thirtyFPSUpdateTimer;

	// Token: 0x0400200E RID: 8206
	public List<Transform> fillBars = new List<Transform>();

	// Token: 0x0400200F RID: 8207
	private float fillBarProgress;

	// Token: 0x04002010 RID: 8208
	private float fillBarProgressPrev = -1f;

	// Token: 0x04002011 RID: 8209
	private bool moveDone;

	// Token: 0x04002012 RID: 8210
	public Transform animationTransform;

	// Token: 0x04002013 RID: 8211
	[FormerlySerializedAs("light")]
	public Light doorLight;

	// Token: 0x04002014 RID: 8212
	public Transform latchLamp1;

	// Token: 0x04002015 RID: 8213
	public Transform latchLamp2;

	// Token: 0x04002016 RID: 8214
	public ParticleSystem particlesCeiling;

	// Token: 0x04002017 RID: 8215
	public Transform particlesOpen;

	// Token: 0x04002018 RID: 8216
	public ParticleSystem particlesUnlock;

	// Token: 0x04002019 RID: 8217
	public ParticleSystem particlesLatch1;

	// Token: 0x0400201A RID: 8218
	public ParticleSystem particlesLatch2;

	// Token: 0x0400201B RID: 8219
	public ParticleSystem particlesDoorSmoke;

	// Token: 0x0400201C RID: 8220
	public ParticleSystem particlesBleep1;

	// Token: 0x0400201D RID: 8221
	public ParticleSystem particlesBleep2;

	// Token: 0x0400201E RID: 8222
	public ParticleSystem lightParticle;

	// Token: 0x0400201F RID: 8223
	public ParticleSystem lightParticle2;

	// Token: 0x04002020 RID: 8224
	public Sound soundGoUp;

	// Token: 0x04002021 RID: 8225
	public Sound soundGoDown;

	// Token: 0x04002022 RID: 8226
	public Sound soundSuccess;

	// Token: 0x04002023 RID: 8227
	public Sound soundUnlock;

	// Token: 0x04002024 RID: 8228
	public Sound soundUnlockEnd;

	// Token: 0x04002025 RID: 8229
	public Sound soundLatches;

	// Token: 0x04002026 RID: 8230
	public Sound soundLatchesEnd;

	// Token: 0x04002027 RID: 8231
	public Sound soundDoorOpen;

	// Token: 0x04002028 RID: 8232
	public Sound soundDoorMove;

	// Token: 0x04002029 RID: 8233
	public Sound soundSlamCeiling;

	// Token: 0x020003B9 RID: 953
	private enum DoorState
	{
		// Token: 0x040028B6 RID: 10422
		Closed,
		// Token: 0x040028B7 RID: 10423
		Success,
		// Token: 0x040028B8 RID: 10424
		Unlock,
		// Token: 0x040028B9 RID: 10425
		Opening,
		// Token: 0x040028BA RID: 10426
		Open
	}
}
