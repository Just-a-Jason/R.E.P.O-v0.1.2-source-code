using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000B6 RID: 182
public class Interaction : MonoBehaviour
{
	// Token: 0x060006DC RID: 1756 RVA: 0x00041216 File Offset: 0x0003F416
	private void Start()
	{
		base.StartCoroutine(this.Add());
	}

	// Token: 0x060006DD RID: 1757 RVA: 0x00041225 File Offset: 0x0003F425
	private IEnumerator Add()
	{
		while (!CleanDirector.instance.RemoveExcessSpots)
		{
			yield return new WaitForSeconds(0.1f);
		}
		yield break;
	}

	// Token: 0x04000B99 RID: 2969
	public Interaction.InteractionType Type;

	// Token: 0x04000B9A RID: 2970
	public Sprite Sprite;

	// Token: 0x020002F5 RID: 757
	public enum InteractionType
	{
		// Token: 0x04002533 RID: 9523
		None,
		// Token: 0x04002534 RID: 9524
		VacuumCleaner,
		// Token: 0x04002535 RID: 9525
		Duster,
		// Token: 0x04002536 RID: 9526
		Sledgehammer,
		// Token: 0x04002537 RID: 9527
		DirtFinder,
		// Token: 0x04002538 RID: 9528
		Picker
	}
}
