using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200021B RID: 539
public class RoundDirector : MonoBehaviour
{
	// Token: 0x06001173 RID: 4467 RVA: 0x0009B7F3 File Offset: 0x000999F3
	private void Awake()
	{
		RoundDirector.instance = this;
		this.physGrabObjects = new List<PhysGrabObject>();
	}

	// Token: 0x06001174 RID: 4468 RVA: 0x0009B806 File Offset: 0x00099A06
	private IEnumerator StartRound()
	{
		while (!LevelGenerator.Instance.Generated && this.haulGoalMax == 0)
		{
			yield return new WaitForSeconds(0.5f);
		}
		yield return new WaitForSeconds(0.1f);
		if (GameManager.instance.gameMode == 0 || PhotonNetwork.IsMasterClient)
		{
			float num = (float)this.haulGoalMax * 0.7f;
			num *= this.haulGoalCurve.Evaluate(SemiFunc.RunGetDifficultyMultiplier());
			if (this.debugLowHaul || SemiFunc.RunIsTutorial() || SemiFunc.RunIsRecording())
			{
				num = (float)(1000 * this.extractionPoints);
			}
			this.StartRoundLogic((int)num);
			if (GameManager.instance.gameMode == 1)
			{
				this.photonView.RPC("StartRoundRPC", RpcTarget.All, new object[]
				{
					this.haulGoal
				});
			}
		}
		this.extractionPointsFetched = true;
		yield break;
	}

	// Token: 0x06001175 RID: 4469 RVA: 0x0009B815 File Offset: 0x00099A15
	private void StartRoundLogic(int value)
	{
		this.currentHaul = 0;
		this.currentHaulMax = 0;
		this.deadPlayers = 0;
		this.haulGoal = value;
	}

	// Token: 0x06001176 RID: 4470 RVA: 0x0009B833 File Offset: 0x00099A33
	[PunRPC]
	private void StartRoundRPC(int value)
	{
		this.StartRoundLogic(value);
	}

	// Token: 0x06001177 RID: 4471 RVA: 0x0009B83C File Offset: 0x00099A3C
	private void Start()
	{
		this.photonView = base.GetComponent<PhotonView>();
		this.photonView.TransferOwnership(PhotonNetwork.MasterClient);
		this.extractionPointsFetched = false;
		base.StartCoroutine(this.StartRound());
	}

	// Token: 0x06001178 RID: 4472 RVA: 0x0009B86E File Offset: 0x00099A6E
	public void PhysGrabObjectAdd(PhysGrabObject _physGrabObject)
	{
		if (this.physGrabObjects.Contains(_physGrabObject))
		{
			return;
		}
		this.physGrabObjects.Add(_physGrabObject);
	}

	// Token: 0x06001179 RID: 4473 RVA: 0x0009B88B File Offset: 0x00099A8B
	public void PhysGrabObjectRemove(PhysGrabObject _physGrabObject)
	{
		if (!this.physGrabObjects.Contains(_physGrabObject))
		{
			return;
		}
		this.physGrabObjects.Remove(_physGrabObject);
	}

	// Token: 0x0600117A RID: 4474 RVA: 0x0009B8AC File Offset: 0x00099AAC
	private void Update()
	{
		this.currentHaul = 0;
		this.currentHaulMax = 0;
		if (this.dollarHaulList.Count > 0)
		{
			foreach (GameObject gameObject in this.dollarHaulList)
			{
				if (!gameObject)
				{
					this.dollarHaulList.Remove(gameObject);
				}
				else
				{
					this.currentHaul += (int)gameObject.GetComponent<ValuableObject>().dollarValueCurrent;
					this.currentHaulMax += (int)gameObject.GetComponent<ValuableObject>().dollarValueOriginal;
				}
			}
		}
	}

	// Token: 0x0600117B RID: 4475 RVA: 0x0009B960 File Offset: 0x00099B60
	public void RequestExtractionPointActivation(int photonViewID)
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			this.photonView.RPC("RequestExtractionPointActivationRPC", RpcTarget.MasterClient, new object[]
			{
				photonViewID
			});
			return;
		}
		this.RequestExtractionPointActivationRPC(photonViewID);
	}

	// Token: 0x0600117C RID: 4476 RVA: 0x0009B991 File Offset: 0x00099B91
	[PunRPC]
	public void RequestExtractionPointActivationRPC(int photonViewID)
	{
		if (!this.extractionPointActive)
		{
			this.extractionPointActive = true;
			this.photonView.RPC("ExtractionPointActivateRPC", RpcTarget.All, new object[]
			{
				photonViewID
			});
		}
	}

	// Token: 0x0600117D RID: 4477 RVA: 0x0009B9C2 File Offset: 0x00099BC2
	public void ExtractionPointActivate(int photonViewID)
	{
		if (this.extractionPointActive)
		{
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.photonView.RPC("ExtractionPointActivateRPC", RpcTarget.All, new object[]
			{
				photonViewID
			});
		}
	}

	// Token: 0x0600117E RID: 4478 RVA: 0x0009B9F4 File Offset: 0x00099BF4
	[PunRPC]
	public void ExtractionPointActivateRPC(int photonViewID)
	{
		RoundDirector.instance.extractionPointDeductionDone = false;
		this.extractionPointActive = true;
		GameObject gameObject = PhotonView.Find(photonViewID).gameObject;
		this.extractionPointCurrent = gameObject.GetComponent<ExtractionPoint>();
		this.extractionPointCurrent.ButtonPress();
		this.ExtractionPointsLock(gameObject);
	}

	// Token: 0x0600117F RID: 4479 RVA: 0x0009BA40 File Offset: 0x00099C40
	public void ExtractionPointsLock(GameObject exceptMe)
	{
		foreach (GameObject gameObject in this.extractionPointList)
		{
			if (gameObject != exceptMe)
			{
				gameObject.GetComponent<ExtractionPoint>().isLocked = true;
			}
		}
	}

	// Token: 0x06001180 RID: 4480 RVA: 0x0009BAA4 File Offset: 0x00099CA4
	public void ExtractionCompleted()
	{
		this.extractionPointsCompleted++;
		if (this.extractionPointsCompleted < this.extractionPoints && SemiFunc.RunIsLevel() && TutorialDirector.instance.TutorialSettingCheck(DataDirector.Setting.TutorialMultipleExtractions, 1))
		{
			TutorialDirector.instance.ActivateTip("Multiple Extractions", 2f, false);
		}
	}

	// Token: 0x06001181 RID: 4481 RVA: 0x0009BAF8 File Offset: 0x00099CF8
	public void ExtractionCompletedAllCheck()
	{
		if (SemiFunc.RunIsShop())
		{
			return;
		}
		if (this.extractionPointsCompleted >= this.extractionPoints - 1)
		{
			this.allExtractionPointsCompleted = true;
			if (SemiFunc.IsMasterClientOrSingleplayer())
			{
				if (SemiFunc.IsMultiplayer())
				{
					this.photonView.RPC("ExtractionCompletedAllRPC", RpcTarget.All, Array.Empty<object>());
					return;
				}
				this.ExtractionCompletedAllRPC();
			}
		}
	}

	// Token: 0x06001182 RID: 4482 RVA: 0x0009BB4F File Offset: 0x00099D4F
	public void ExtractionPointsUnlock()
	{
		if (SemiFunc.IsMultiplayer())
		{
			if (SemiFunc.IsMasterClient())
			{
				this.photonView.RPC("ExtractionPointsUnlockRPC", RpcTarget.All, Array.Empty<object>());
				return;
			}
		}
		else
		{
			this.ExtractionPointsUnlockRPC();
		}
	}

	// Token: 0x06001183 RID: 4483 RVA: 0x0009BB7C File Offset: 0x00099D7C
	[PunRPC]
	public void ExtractionPointsUnlockRPC()
	{
		foreach (GameObject gameObject in this.extractionPointList)
		{
			gameObject.GetComponent<ExtractionPoint>().isLocked = false;
		}
		this.extractionPointCurrent = null;
	}

	// Token: 0x06001184 RID: 4484 RVA: 0x0009BBDC File Offset: 0x00099DDC
	public void HaulCheck()
	{
		this.currentHaul = 0;
		List<GameObject> list = new List<GameObject>();
		foreach (GameObject gameObject in this.dollarHaulList)
		{
			if (!gameObject)
			{
				list.Add(gameObject);
			}
			else
			{
				ValuableObject component = gameObject.GetComponent<ValuableObject>();
				if (component)
				{
					component.roomVolumeCheck.CheckSet();
					if (!component.roomVolumeCheck.inExtractionPoint)
					{
						list.Add(gameObject);
					}
					else
					{
						this.currentHaul += (int)component.dollarValueCurrent;
					}
				}
			}
		}
		foreach (GameObject item in list)
		{
			this.dollarHaulList.Remove(item);
		}
	}

	// Token: 0x06001185 RID: 4485 RVA: 0x0009BCD0 File Offset: 0x00099ED0
	[PunRPC]
	private void ExtractionCompletedAllRPC()
	{
		AudioScare.instance.PlayImpact();
		this.lightsTurnOffSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.Shake(3f, 1f);
		GameDirector.instance.CameraImpact.Shake(5f, 0.1f);
		if (SemiFunc.RunIsLevel() && TutorialDirector.instance.TutorialSettingCheck(DataDirector.Setting.TutorialFinalExtraction, 1))
		{
			TutorialDirector.instance.ActivateTip("Final Extraction", 0.5f, false);
		}
	}

	// Token: 0x06001186 RID: 4486 RVA: 0x0009BD70 File Offset: 0x00099F70
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(this.currentHaul);
			stream.SendNext(this.haulGoal);
			return;
		}
		this.currentHaul = (int)stream.ReceiveNext();
		this.haulGoal = (int)stream.ReceiveNext();
	}

	// Token: 0x04001D5F RID: 7519
	private PhotonView photonView;

	// Token: 0x04001D60 RID: 7520
	public AnimationCurve haulGoalCurve;

	// Token: 0x04001D61 RID: 7521
	internal bool debugLowHaul;

	// Token: 0x04001D62 RID: 7522
	internal int currentHaul;

	// Token: 0x04001D63 RID: 7523
	internal int totalHaul;

	// Token: 0x04001D64 RID: 7524
	internal int currentHaulMax;

	// Token: 0x04001D65 RID: 7525
	internal int haulGoal;

	// Token: 0x04001D66 RID: 7526
	internal int haulGoalMax;

	// Token: 0x04001D67 RID: 7527
	internal int deadPlayers;

	// Token: 0x04001D68 RID: 7528
	internal int extractionPoints;

	// Token: 0x04001D69 RID: 7529
	internal int extractionPointSurplus;

	// Token: 0x04001D6A RID: 7530
	internal int extractionPointsCompleted;

	// Token: 0x04001D6B RID: 7531
	internal int extractionHaulGoal;

	// Token: 0x04001D6C RID: 7532
	internal bool extractionPointActive;

	// Token: 0x04001D6D RID: 7533
	internal List<GameObject> extractionPointList = new List<GameObject>();

	// Token: 0x04001D6E RID: 7534
	internal ExtractionPoint extractionPointCurrent;

	// Token: 0x04001D6F RID: 7535
	internal bool extractionPointsFetched;

	// Token: 0x04001D70 RID: 7536
	[HideInInspector]
	public bool extractionPointDeductionDone;

	// Token: 0x04001D71 RID: 7537
	internal bool allExtractionPointsCompleted;

	// Token: 0x04001D72 RID: 7538
	public static RoundDirector instance;

	// Token: 0x04001D73 RID: 7539
	internal List<PhysGrabObject> physGrabObjects;

	// Token: 0x04001D74 RID: 7540
	public List<GameObject> dollarHaulList = new List<GameObject>();

	// Token: 0x04001D75 RID: 7541
	[Space]
	public Sound lightsTurnOffSound;
}
