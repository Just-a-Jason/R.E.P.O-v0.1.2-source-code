using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000D7 RID: 215
public class ArenaPlatform : MonoBehaviour
{
	// Token: 0x06000783 RID: 1923 RVA: 0x000473DD File Offset: 0x000455DD
	private void Start()
	{
		this.lights = new List<ArenaLight>();
		this.lights.AddRange(base.GetComponentsInChildren<ArenaLight>());
		this.meshRenderer.material.SetColor("_EmissionColor", Color.black);
	}

	// Token: 0x06000784 RID: 1924 RVA: 0x00047415 File Offset: 0x00045615
	private void StateIdle()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
		}
	}

	// Token: 0x06000785 RID: 1925 RVA: 0x00047428 File Offset: 0x00045628
	private void StateWarning()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
			this.lights.ForEach(delegate(ArenaLight light)
			{
				light.TurnOnArenaWarningLight();
			});
			this.meshRenderer.material.SetColor("_EmissionColor", Color.red);
		}
		Color b = new Color(0.3f, 0f, 0f);
		this.meshRenderer.material.SetColor("_EmissionColor", Color.Lerp(this.meshRenderer.material.GetColor("_EmissionColor"), b, Time.deltaTime * 2f));
	}

	// Token: 0x06000786 RID: 1926 RVA: 0x000474DC File Offset: 0x000456DC
	private void StateGoDown()
	{
		if (this.stateStart)
		{
			DirtFinderMapFloor[] array = this.map;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].MapObject.Hide();
			}
			this.stateStart = false;
		}
		if (base.transform.position.y > -60f)
		{
			base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y - 30f * Time.deltaTime, base.transform.position.z);
			return;
		}
		this.StateSet(ArenaPlatform.States.End);
	}

	// Token: 0x06000787 RID: 1927 RVA: 0x00047585 File Offset: 0x00045785
	private void StateEnd()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x06000788 RID: 1928 RVA: 0x000475A4 File Offset: 0x000457A4
	private void StateMachine()
	{
		switch (this.currentState)
		{
		case ArenaPlatform.States.Idle:
			this.StateIdle();
			return;
		case ArenaPlatform.States.Warning:
			this.StateWarning();
			return;
		case ArenaPlatform.States.GoDown:
			this.StateGoDown();
			return;
		case ArenaPlatform.States.End:
			this.StateEnd();
			return;
		default:
			return;
		}
	}

	// Token: 0x06000789 RID: 1929 RVA: 0x000475EA File Offset: 0x000457EA
	private void Update()
	{
		this.StateMachine();
		if (this.stateTimer > 0f)
		{
			this.stateTimer -= Time.deltaTime;
		}
	}

	// Token: 0x0600078A RID: 1930 RVA: 0x00047614 File Offset: 0x00045814
	public void PulsateLights()
	{
		this.lights.ForEach(delegate(ArenaLight light)
		{
			light.PulsateLight();
		});
		this.meshRenderer.material.SetColor("_EmissionColor", Color.red);
	}

	// Token: 0x0600078B RID: 1931 RVA: 0x00047665 File Offset: 0x00045865
	public void StateSet(ArenaPlatform.States state)
	{
		this.currentState = state;
		this.stateStart = true;
	}

	// Token: 0x04000D3E RID: 3390
	private List<ArenaLight> lights;

	// Token: 0x04000D3F RID: 3391
	internal ArenaPlatform.States currentState;

	// Token: 0x04000D40 RID: 3392
	private bool stateStart;

	// Token: 0x04000D41 RID: 3393
	private float stateTimer;

	// Token: 0x04000D42 RID: 3394
	public MeshRenderer meshRenderer;

	// Token: 0x04000D43 RID: 3395
	[Space]
	public DirtFinderMapFloor[] map;

	// Token: 0x020002FD RID: 765
	public enum States
	{
		// Token: 0x04002560 RID: 9568
		Idle,
		// Token: 0x04002561 RID: 9569
		Warning,
		// Token: 0x04002562 RID: 9570
		GoDown,
		// Token: 0x04002563 RID: 9571
		End
	}
}
