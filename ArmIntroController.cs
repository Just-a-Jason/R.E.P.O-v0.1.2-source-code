using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200019F RID: 415
public class ArmIntroController : MonoBehaviour
{
	// Token: 0x06000DF1 RID: 3569 RVA: 0x0007E365 File Offset: 0x0007C565
	public void Start()
	{
		this.Animator.enabled = false;
		base.transform.parent = this.CameraTransform;
		this.Hide.SetActive(false);
		base.StartCoroutine(this.StartIntro());
	}

	// Token: 0x06000DF2 RID: 3570 RVA: 0x0007E39D File Offset: 0x0007C59D
	public void Update()
	{
		PlayerController.instance.CrouchDisable(0.1f);
	}

	// Token: 0x06000DF3 RID: 3571 RVA: 0x0007E3AE File Offset: 0x0007C5AE
	private IEnumerator StartIntro()
	{
		while (GameDirector.instance.currentState != GameDirector.gameState.Main)
		{
			yield return null;
		}
		if (this.DebugDisable)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			yield return new WaitForSeconds(this.WaitTimer);
			this.Animator.enabled = true;
			this.Hide.SetActive(true);
		}
		yield break;
	}

	// Token: 0x06000DF4 RID: 3572 RVA: 0x0007E3BD File Offset: 0x0007C5BD
	public void AnimationDone()
	{
		Object.Destroy(base.gameObject);
	}

	// Token: 0x06000DF5 RID: 3573 RVA: 0x0007E3CC File Offset: 0x0007C5CC
	public void PlayGlovePull()
	{
		this.GlovePull.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraImpact.Shake(0.25f, 1f);
	}

	// Token: 0x06000DF6 RID: 3574 RVA: 0x0007E420 File Offset: 0x0007C620
	public void PlayGloveSnap()
	{
		this.GloveSnap.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraImpact.Shake(1f, 0.1f);
	}

	// Token: 0x06000DF7 RID: 3575 RVA: 0x0007E471 File Offset: 0x0007C671
	public void PlayMoveShort()
	{
		this.MoveShort.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000DF8 RID: 3576 RVA: 0x0007E49E File Offset: 0x0007C69E
	public void PlayMoveLong()
	{
		this.MoveLong.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x040016D4 RID: 5844
	public bool DebugDisable;

	// Token: 0x040016D5 RID: 5845
	[Space]
	public Animator Animator;

	// Token: 0x040016D6 RID: 5846
	public Transform CameraTransform;

	// Token: 0x040016D7 RID: 5847
	public GameObject Hide;

	// Token: 0x040016D8 RID: 5848
	[Space]
	public float WaitTimer = 0.25f;

	// Token: 0x040016D9 RID: 5849
	[Space]
	public Sound MoveShort;

	// Token: 0x040016DA RID: 5850
	public Sound MoveLong;

	// Token: 0x040016DB RID: 5851
	public Sound GlovePull;

	// Token: 0x040016DC RID: 5852
	public Sound GloveSnap;
}
