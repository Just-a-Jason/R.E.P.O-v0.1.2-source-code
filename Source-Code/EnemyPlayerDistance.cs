using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000A0 RID: 160
public class EnemyPlayerDistance : MonoBehaviour
{
	// Token: 0x06000626 RID: 1574 RVA: 0x0003B249 File Offset: 0x00039449
	private void Start()
	{
		this.Enemy = base.GetComponent<Enemy>();
		this.LogicActive = true;
		base.StartCoroutine(this.Logic());
	}

	// Token: 0x06000627 RID: 1575 RVA: 0x0003B26B File Offset: 0x0003946B
	private void OnDisable()
	{
		this.LogicActive = false;
		base.StopAllCoroutines();
	}

	// Token: 0x06000628 RID: 1576 RVA: 0x0003B27A File Offset: 0x0003947A
	private void OnEnable()
	{
		if (!this.LogicActive)
		{
			this.LogicActive = true;
			base.StartCoroutine(this.Logic());
		}
	}

	// Token: 0x06000629 RID: 1577 RVA: 0x0003B298 File Offset: 0x00039498
	private IEnumerator Logic()
	{
		for (;;)
		{
			this.PlayerDistanceLocal = 999f;
			this.PlayerDistanceClosest = 999f;
			foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
			{
				float num = Vector3.Distance(this.CheckTransform.position, playerAvatar.PlayerVisionTarget.VisionTransform.position);
				if (playerAvatar.isLocal)
				{
					this.PlayerDistanceLocal = num;
				}
				if (!playerAvatar.isDisabled && num < this.PlayerDistanceClosest)
				{
					this.PlayerDistanceClosest = num;
				}
			}
			yield return new WaitForSeconds(0.25f);
		}
		yield break;
	}

	// Token: 0x04000A1A RID: 2586
	private Enemy Enemy;

	// Token: 0x04000A1B RID: 2587
	public Transform CheckTransform;

	// Token: 0x04000A1C RID: 2588
	private bool LogicActive;

	// Token: 0x04000A1D RID: 2589
	internal float PlayerDistanceLocal = 1000f;

	// Token: 0x04000A1E RID: 2590
	internal float PlayerDistanceClosest = 1000f;
}
