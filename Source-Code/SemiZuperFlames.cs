using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000275 RID: 629
public class SemiZuperFlames : MonoBehaviour
{
	// Token: 0x06001372 RID: 4978 RVA: 0x000AA620 File Offset: 0x000A8820
	private void StateNone()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		this.PlayBaseParticles(false);
		if (this.flamesActive)
		{
			this.StateSet(SemiZuperFlames.State.FlamesStart);
		}
	}

	// Token: 0x06001373 RID: 4979 RVA: 0x000AA648 File Offset: 0x000A8848
	private void StateFlamesStart()
	{
		if (this.stateStart)
		{
			this.soundPukeStart.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.flameLight.enabled = true;
			this.flameLight.intensity = 0f;
			this.PlayAllParticles(true);
			this.stateStart = false;
		}
		this.PlayBaseParticles(true);
		this.StateSet(SemiZuperFlames.State.Flames);
	}

	// Token: 0x06001374 RID: 4980 RVA: 0x000AA6C0 File Offset: 0x000A88C0
	private void StateFlames()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		this.PlayBaseParticles(true);
		this.flameLight.intensity = Mathf.Lerp(this.flameLight.intensity, 1f, Time.deltaTime * 20f);
		this.flameLight.intensity += Mathf.Sin(Time.time * 20f) * 0.05f;
		if (!this.flamesActive)
		{
			this.StateSet(SemiZuperFlames.State.FlamesEnd);
		}
	}

	// Token: 0x06001375 RID: 4981 RVA: 0x000AA748 File Offset: 0x000A8948
	private void StateFlamesEnd()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
			this.soundPukeEnd.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.flameLight.enabled = false;
			this.PlayAllParticles(false);
			this.pukeEnd.Play();
		}
		this.PlayBaseParticles(false);
		this.flameLight.intensity = Mathf.Lerp(this.flameLight.intensity, 0f, Time.deltaTime * 40f);
		if (this.flameLight.intensity < 0.01f)
		{
			this.flameLight.intensity = 0f;
			this.StateSet(SemiZuperFlames.State.None);
		}
	}

	// Token: 0x06001376 RID: 4982 RVA: 0x000AA808 File Offset: 0x000A8A08
	private void PlayAllParticles(bool _play)
	{
		foreach (ParticleSystem particleSystem in this.pukeParticles)
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

	// Token: 0x06001377 RID: 4983 RVA: 0x000AA868 File Offset: 0x000A8A68
	private void StateMachine()
	{
		bool playing = this.flamesActive;
		this.soundPukeLoop.PlayLoop(playing, 2f, 2f, 1f);
		switch (this.state)
		{
		case SemiZuperFlames.State.None:
			this.StateNone();
			return;
		case SemiZuperFlames.State.FlamesStart:
			this.StateFlamesStart();
			return;
		case SemiZuperFlames.State.Flames:
			this.StateFlames();
			return;
		case SemiZuperFlames.State.FlamesEnd:
			this.StateFlamesEnd();
			return;
		default:
			return;
		}
	}

	// Token: 0x06001378 RID: 4984 RVA: 0x000AA8D0 File Offset: 0x000A8AD0
	private void Start()
	{
		this.baseParticles = new List<ParticleSystem>(this.BaseParticlesTransform.GetComponentsInChildren<ParticleSystem>());
	}

	// Token: 0x06001379 RID: 4985 RVA: 0x000AA8E8 File Offset: 0x000A8AE8
	private void Update()
	{
		this.StateMachine();
	}

	// Token: 0x0600137A RID: 4986 RVA: 0x000AA8F0 File Offset: 0x000A8AF0
	private void PlayBaseParticles(bool _play)
	{
		if (this.baseParticlesPlaying == _play)
		{
			return;
		}
		foreach (ParticleSystem particleSystem in this.baseParticles)
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
		this.baseParticlesPlaying = _play;
	}

	// Token: 0x0600137B RID: 4987 RVA: 0x000AA960 File Offset: 0x000A8B60
	private void StateSet(SemiZuperFlames.State _state)
	{
		if (this.state == _state)
		{
			return;
		}
		this.state = _state;
		this.stateStart = true;
	}

	// Token: 0x0600137C RID: 4988 RVA: 0x000AA97C File Offset: 0x000A8B7C
	public void FlamesActive(Vector3 _position, Quaternion _direction)
	{
		base.transform.localPosition = Vector3.zero;
		base.transform.localRotation = Quaternion.identity;
		base.transform.position = _position;
		base.transform.rotation = _direction;
		this.flamesActive = true;
	}

	// Token: 0x0600137D RID: 4989 RVA: 0x000AA9C8 File Offset: 0x000A8BC8
	public void FlamesInactive()
	{
		this.flamesActive = false;
	}

	// Token: 0x04002139 RID: 8505
	public SemiZuperFlames.State state;

	// Token: 0x0400213A RID: 8506
	public Transform BaseParticlesTransform;

	// Token: 0x0400213B RID: 8507
	private bool flamesActive;

	// Token: 0x0400213C RID: 8508
	private bool stateStart;

	// Token: 0x0400213D RID: 8509
	private bool baseParticlesPlaying;

	// Token: 0x0400213E RID: 8510
	public Light flameLight;

	// Token: 0x0400213F RID: 8511
	public List<ParticleSystem> pukeParticles = new List<ParticleSystem>();

	// Token: 0x04002140 RID: 8512
	public ParticleSystem pukeEnd = new ParticleSystem();

	// Token: 0x04002141 RID: 8513
	private List<ParticleSystem> baseParticles = new List<ParticleSystem>();

	// Token: 0x04002142 RID: 8514
	public Sound soundPukeStart;

	// Token: 0x04002143 RID: 8515
	public Sound soundPukeEnd;

	// Token: 0x04002144 RID: 8516
	public Sound soundPukeLoop;

	// Token: 0x020003BE RID: 958
	public enum State
	{
		// Token: 0x040028CE RID: 10446
		None,
		// Token: 0x040028CF RID: 10447
		FlamesStart,
		// Token: 0x040028D0 RID: 10448
		Flames,
		// Token: 0x040028D1 RID: 10449
		FlamesEnd
	}
}
