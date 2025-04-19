using System;
using UnityEngine;

// Token: 0x020001A0 RID: 416
public class PlayerArmAnimation : MonoBehaviour
{
	// Token: 0x06000DFA RID: 3578 RVA: 0x0007E4E0 File Offset: 0x0007C6E0
	private void Start()
	{
		this.Player = PlayerController.instance;
		this.Voice = PlayerVoice.Instance;
		this.Animator = base.GetComponent<Animator>();
		this.Crouching = Animator.StringToHash("Crouching");
		this.Crawling = Animator.StringToHash("Crawling");
	}

	// Token: 0x06000DFB RID: 3579 RVA: 0x0007E530 File Offset: 0x0007C730
	private void Update()
	{
		if (this.Player.Crouching)
		{
			this.Animator.SetBool(this.Crouching, true);
		}
		else
		{
			this.Animator.SetBool(this.Crouching, false);
			this.Animator.SetBool(this.Crawling, false);
		}
		if (this.Player.Crawling)
		{
			this.Animator.SetBool(this.Crawling, true);
			return;
		}
		this.Animator.SetBool(this.Crawling, false);
	}

	// Token: 0x06000DFC RID: 3580 RVA: 0x0007E5B4 File Offset: 0x0007C7B4
	public void PlayCrouchHush()
	{
	}

	// Token: 0x06000DFD RID: 3581 RVA: 0x0007E5B6 File Offset: 0x0007C7B6
	public void PlayMoveShort()
	{
		this.MoveShort.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000DFE RID: 3582 RVA: 0x0007E5E3 File Offset: 0x0007C7E3
	public void PlayMoveLong()
	{
		this.MoveLong.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x040016DD RID: 5853
	private PlayerController Player;

	// Token: 0x040016DE RID: 5854
	private Animator Animator;

	// Token: 0x040016DF RID: 5855
	private int Crouching;

	// Token: 0x040016E0 RID: 5856
	private int Crawling;

	// Token: 0x040016E1 RID: 5857
	private PlayerVoice Voice;

	// Token: 0x040016E2 RID: 5858
	public Sound MoveShort;

	// Token: 0x040016E3 RID: 5859
	public Sound MoveLong;
}
