using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000F3 RID: 243
public class TruckDoor : MonoBehaviour
{
	// Token: 0x0600089F RID: 2207 RVA: 0x00052B14 File Offset: 0x00050D14
	private void Start()
	{
		this.playerInTruckCheckTimer = 2f;
		this.startYPosition = base.transform.position.y;
		base.StartCoroutine(this.DelayedStart());
	}

	// Token: 0x060008A0 RID: 2208 RVA: 0x00052B44 File Offset: 0x00050D44
	private IEnumerator DelayedStart()
	{
		while (!SemiFunc.LevelGenDone())
		{
			yield return new WaitForSeconds(0.3f);
		}
		while (!this.extractionPointNearest)
		{
			yield return new WaitForSeconds(0.1f);
			this.extractionPointNearest = SemiFunc.ExtractionPointGetNearest(base.transform.position);
		}
		this.timeToCheck = true;
		yield break;
	}

	// Token: 0x060008A1 RID: 2209 RVA: 0x00052B54 File Offset: 0x00050D54
	private void Update()
	{
		if (this.timeToCheck)
		{
			if (this.playerInTruckCheckTimer > 0f)
			{
				this.playerInTruckCheckTimer -= Time.deltaTime;
			}
			else
			{
				this.playerInTruckCheckTimer = 0.5f;
				if (!this.introActivationDone && !SemiFunc.PlayersAllInTruck())
				{
					this.introActivationDone = true;
					if (!TutorialDirector.instance.tutorialActive)
					{
						this.extractionPointNearest.ActivateTheFirstExtractionPointAutomaticallyWhenAPlayerLeaveTruck();
					}
				}
			}
		}
		if (this.doorDelay > 0f && SemiFunc.LevelGenDone())
		{
			this.doorDelay -= Time.deltaTime;
		}
		if (this.doorDelay <= 0f && this.doorEval < 1f)
		{
			if (!this.doorOpen)
			{
				this.doorOpen = true;
				if (SemiFunc.RunIsShop())
				{
					SemiFunc.UIFocusText("Buy stuff in the shop", Color.white, AssetManager.instance.colorYellow, 3f);
				}
				GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 8f, base.transform.position, 0.1f);
				this.doorLoopStart.Play(base.transform.position, 1f, 1f, 1f, 1f);
			}
			float num = this.doorCurve.Evaluate(this.doorEval);
			this.doorEval += 1.5f * Time.deltaTime;
			base.transform.position = new Vector3(base.transform.position.x, this.startYPosition + 2.5f * num, base.transform.position.z);
		}
		if (this.doorEval >= 1f && !this.fullyOpen)
		{
			this.fullyOpen = true;
			GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.1f);
			this.doorLoopEnd.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.doorSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		}
	}

	// Token: 0x04000FA0 RID: 4000
	public Sound doorLoopStart;

	// Token: 0x04000FA1 RID: 4001
	public Sound doorLoopEnd;

	// Token: 0x04000FA2 RID: 4002
	public Sound doorSound;

	// Token: 0x04000FA3 RID: 4003
	private float startYPosition;

	// Token: 0x04000FA4 RID: 4004
	private bool fullyOpen;

	// Token: 0x04000FA5 RID: 4005
	private float doorEval;

	// Token: 0x04000FA6 RID: 4006
	public AnimationCurve doorCurve;

	// Token: 0x04000FA7 RID: 4007
	public Transform doorMesh;

	// Token: 0x04000FA8 RID: 4008
	private float doorDelay = 2f;

	// Token: 0x04000FA9 RID: 4009
	private bool doorOpen;

	// Token: 0x04000FAA RID: 4010
	private ExtractionPoint extractionPointNearest;

	// Token: 0x04000FAB RID: 4011
	private float playerInTruckCheckTimer;

	// Token: 0x04000FAC RID: 4012
	private bool timeToCheck;

	// Token: 0x04000FAD RID: 4013
	private bool introActivationDone;
}
