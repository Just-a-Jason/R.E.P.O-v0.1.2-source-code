using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000079 RID: 121
public class SemiPuke : MonoBehaviour
{
	// Token: 0x0600048F RID: 1167 RVA: 0x0002D6AD File Offset: 0x0002B8AD
	private void StateNone()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		this.PlayBaseParticles(false);
		if (this.pukeActiveTimer > 0f)
		{
			this.StateSet(SemiPuke.State.PukeStart);
		}
	}

	// Token: 0x06000490 RID: 1168 RVA: 0x0002D6DC File Offset: 0x0002B8DC
	private void StatePukeStart()
	{
		if (this.stateStart)
		{
			this.soundPukeStart.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.pukeLight.enabled = true;
			this.pukeLight.intensity = 0f;
			this.PlayAllParticles(true);
			this.stateStart = false;
		}
		this.PlayBaseParticles(true);
		this.StateSet(SemiPuke.State.Puke);
	}

	// Token: 0x06000491 RID: 1169 RVA: 0x0002D754 File Offset: 0x0002B954
	private void StatePuke()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		this.PlayBaseParticles(true);
		this.pukeLight.intensity = Mathf.Lerp(this.pukeLight.intensity, 1f, Time.deltaTime * 20f);
		this.pukeLight.intensity += Mathf.Sin(Time.time * 20f) * 0.05f;
		if (this.pukeActiveTimer <= 0f)
		{
			this.StateSet(SemiPuke.State.PukeEnd);
		}
	}

	// Token: 0x06000492 RID: 1170 RVA: 0x0002D7E0 File Offset: 0x0002B9E0
	private void StatePukeEnd()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
			this.soundPukeEnd.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.pukeLight.enabled = false;
			this.PlayAllParticles(false);
			this.pukeEnd.Play();
		}
		this.PlayBaseParticles(false);
		this.pukeLight.intensity = Mathf.Lerp(this.pukeLight.intensity, 0f, Time.deltaTime * 40f);
		if (this.pukeLight.intensity < 0.01f)
		{
			this.pukeLight.intensity = 0f;
			this.StateSet(SemiPuke.State.None);
		}
	}

	// Token: 0x06000493 RID: 1171 RVA: 0x0002D8A0 File Offset: 0x0002BAA0
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

	// Token: 0x06000494 RID: 1172 RVA: 0x0002D900 File Offset: 0x0002BB00
	private void StateMachine()
	{
		bool playing = this.pukeActiveTimer > 0f;
		this.soundPukeLoop.PlayLoop(playing, 2f, 2f, 1f);
		if (this.pukeActiveTimer > 0f)
		{
			this.pukeActiveTimer -= Time.deltaTime;
		}
		switch (this.state)
		{
		case SemiPuke.State.None:
			this.StateNone();
			return;
		case SemiPuke.State.PukeStart:
			this.StatePukeStart();
			return;
		case SemiPuke.State.Puke:
			this.StatePuke();
			return;
		case SemiPuke.State.PukeEnd:
			this.StatePukeEnd();
			return;
		default:
			return;
		}
	}

	// Token: 0x06000495 RID: 1173 RVA: 0x0002D98E File Offset: 0x0002BB8E
	private void Start()
	{
		this.baseParticles = new List<ParticleSystem>(this.BaseParticlesTransform.GetComponentsInChildren<ParticleSystem>());
	}

	// Token: 0x06000496 RID: 1174 RVA: 0x0002D9A6 File Offset: 0x0002BBA6
	private void Update()
	{
		this.StateMachine();
	}

	// Token: 0x06000497 RID: 1175 RVA: 0x0002D9B0 File Offset: 0x0002BBB0
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

	// Token: 0x06000498 RID: 1176 RVA: 0x0002DA20 File Offset: 0x0002BC20
	private void StateSet(SemiPuke.State _state)
	{
		if (this.state == _state)
		{
			return;
		}
		this.state = _state;
		this.stateStart = true;
	}

	// Token: 0x06000499 RID: 1177 RVA: 0x0002DA3C File Offset: 0x0002BC3C
	public void PukeActive(Vector3 _position, Quaternion _direction)
	{
		base.transform.localPosition = Vector3.zero;
		base.transform.localRotation = Quaternion.identity;
		base.transform.position = _position;
		base.transform.rotation = _direction;
		this.pukeActiveTimer = 0.2f;
	}

	// Token: 0x04000772 RID: 1906
	public SemiPuke.State state;

	// Token: 0x04000773 RID: 1907
	public Transform BaseParticlesTransform;

	// Token: 0x04000774 RID: 1908
	private float pukeActiveTimer;

	// Token: 0x04000775 RID: 1909
	private bool stateStart;

	// Token: 0x04000776 RID: 1910
	private bool baseParticlesPlaying;

	// Token: 0x04000777 RID: 1911
	public Light pukeLight;

	// Token: 0x04000778 RID: 1912
	public List<ParticleSystem> pukeParticles = new List<ParticleSystem>();

	// Token: 0x04000779 RID: 1913
	public ParticleSystem pukeEnd = new ParticleSystem();

	// Token: 0x0400077A RID: 1914
	private List<ParticleSystem> baseParticles = new List<ParticleSystem>();

	// Token: 0x0400077B RID: 1915
	public Sound soundPukeStart;

	// Token: 0x0400077C RID: 1916
	public Sound soundPukeEnd;

	// Token: 0x0400077D RID: 1917
	public Sound soundPukeLoop;

	// Token: 0x020002E0 RID: 736
	public enum State
	{
		// Token: 0x040024B2 RID: 9394
		None,
		// Token: 0x040024B3 RID: 9395
		PukeStart,
		// Token: 0x040024B4 RID: 9396
		Puke,
		// Token: 0x040024B5 RID: 9397
		PukeEnd
	}
}
