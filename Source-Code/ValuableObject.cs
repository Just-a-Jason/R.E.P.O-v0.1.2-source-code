using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000291 RID: 657
public class ValuableObject : MonoBehaviour
{
	// Token: 0x0600142F RID: 5167 RVA: 0x000B113F File Offset: 0x000AF33F
	private void Awake()
	{
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x06001430 RID: 5168 RVA: 0x000B1150 File Offset: 0x000AF350
	private void Start()
	{
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.roomVolumeCheck = base.GetComponent<RoomVolumeCheck>();
		this.navMeshObstacle = base.GetComponent<NavMeshObstacle>();
		if (this.navMeshObstacle)
		{
			Debug.LogError(base.gameObject.name + " has a NavMeshObstacle component. Please remove it.");
		}
		base.StartCoroutine(this.DollarValueSet());
		this.rigidBodyMass = this.physAttributePreset.mass;
		this.rb = base.GetComponent<Rigidbody>();
		if (this.rb)
		{
			this.rb.mass = this.rigidBodyMass;
		}
		this.physGrabObject.massOriginal = this.rigidBodyMass;
		if (!LevelGenerator.Instance.Generated)
		{
			ValuableDirector.instance.valuableSpawnAmount++;
			ValuableDirector.instance.valuableList.Add(this);
		}
		if (this.volumeType <= ValuableVolume.Type.Small)
		{
			this.physGrabObject.clientNonKinematic = true;
		}
	}

	// Token: 0x06001431 RID: 5169 RVA: 0x000B1244 File Offset: 0x000AF444
	private void AddToDollarHaulList()
	{
		if (GameManager.instance.gameMode == 1)
		{
			this.photonView.RPC("AddToDollarHaulListRPC", RpcTarget.All, Array.Empty<object>());
			return;
		}
		if (base.GetComponent<ValuableObject>() && !RoundDirector.instance.dollarHaulList.Contains(base.gameObject))
		{
			RoundDirector.instance.dollarHaulList.Add(base.gameObject);
		}
	}

	// Token: 0x06001432 RID: 5170 RVA: 0x000B12AE File Offset: 0x000AF4AE
	[PunRPC]
	public void AddToDollarHaulListRPC()
	{
		if (base.GetComponent<ValuableObject>() && !RoundDirector.instance.dollarHaulList.Contains(base.gameObject))
		{
			RoundDirector.instance.dollarHaulList.Add(base.gameObject);
		}
	}

	// Token: 0x06001433 RID: 5171 RVA: 0x000B12EC File Offset: 0x000AF4EC
	private void RemoveFromDollarHaulList()
	{
		if (GameManager.instance.gameMode == 1)
		{
			this.photonView.RPC("RemoveFromDollarHaulListRPC", RpcTarget.All, Array.Empty<object>());
			return;
		}
		if (base.GetComponent<ValuableObject>() && RoundDirector.instance.dollarHaulList.Contains(base.gameObject))
		{
			RoundDirector.instance.dollarHaulList.Remove(base.gameObject);
		}
	}

	// Token: 0x06001434 RID: 5172 RVA: 0x000B1357 File Offset: 0x000AF557
	[PunRPC]
	public void RemoveFromDollarHaulListRPC()
	{
		if (base.GetComponent<ValuableObject>() && RoundDirector.instance.dollarHaulList.Contains(base.gameObject))
		{
			RoundDirector.instance.dollarHaulList.Remove(base.gameObject);
		}
	}

	// Token: 0x06001435 RID: 5173 RVA: 0x000B1394 File Offset: 0x000AF594
	private void Update()
	{
		if (GameManager.instance.gameMode == 0 || PhotonNetwork.IsMasterClient)
		{
			if (this.inStartRoomCheckTimer > 0f)
			{
				this.inStartRoomCheckTimer -= Time.deltaTime;
			}
			else
			{
				bool flag = false;
				using (List<RoomVolume>.Enumerator enumerator = this.roomVolumeCheck.CurrentRooms.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.Extraction)
						{
							if (!this.inStartRoom)
							{
								this.AddToDollarHaulList();
								this.inStartRoom = true;
							}
							flag = true;
						}
					}
				}
				if (!flag && this.inStartRoom)
				{
					this.RemoveFromDollarHaulList();
					this.inStartRoom = false;
				}
				this.inStartRoomCheckTimer = 0.5f;
			}
		}
		this.DiscoverReminderLogic();
	}

	// Token: 0x06001436 RID: 5174 RVA: 0x000B1464 File Offset: 0x000AF664
	private IEnumerator DollarValueSet()
	{
		yield return new WaitForSeconds(0.05f);
		while (LevelGenerator.Instance.State <= LevelGenerator.LevelState.Valuable)
		{
			yield return new WaitForSeconds(0.05f);
		}
		if (!SemiFunc.IsMultiplayer())
		{
			this.DollarValueSetLogic();
		}
		else if (SemiFunc.IsMasterClient())
		{
			this.DollarValueSetLogic();
			this.photonView.RPC("DollarValueSetRPC", RpcTarget.Others, new object[]
			{
				this.dollarValueCurrent
			});
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			RoundDirector.instance.haulGoalMax += (int)this.dollarValueCurrent;
		}
		yield break;
	}

	// Token: 0x06001437 RID: 5175 RVA: 0x000B1474 File Offset: 0x000AF674
	private void DollarValueSetLogic()
	{
		if (this.dollarValueOverride != 0)
		{
			this.dollarValueOriginal = (float)this.dollarValueOverride;
			this.dollarValueCurrent = (float)this.dollarValueOverride;
		}
		else
		{
			this.dollarValueOriginal = Mathf.Round(Random.Range(this.valuePreset.valueMin, this.valuePreset.valueMax));
			this.dollarValueOriginal = Mathf.Round(this.dollarValueOriginal / 100f) * 100f;
			this.dollarValueCurrent = this.dollarValueOriginal;
		}
		this.dollarValueSet = true;
	}

	// Token: 0x06001438 RID: 5176 RVA: 0x000B14FC File Offset: 0x000AF6FC
	private void DiscoverReminderLogic()
	{
		if (this.discovered && !this.discoveredReminder)
		{
			if (this.discoveredReminderTimer > 0f)
			{
				this.discoveredReminderTimer -= Time.deltaTime;
				return;
			}
			this.discoveredReminderTimer = Random.Range(2f, 5f);
			if (!this.physGrabObject.impactDetector.inCart && PlayerController.instance.isActiveAndEnabled && Vector3.Distance(base.transform.position, PlayerController.instance.transform.position) > 20f)
			{
				this.discoveredReminder = true;
			}
		}
	}

	// Token: 0x06001439 RID: 5177 RVA: 0x000B159F File Offset: 0x000AF79F
	public void Discover(ValuableDiscoverGraphic.State _state)
	{
		if (!this.discovered)
		{
			if (!GameManager.Multiplayer())
			{
				this.DiscoverRPC();
			}
			else
			{
				this.photonView.RPC("DiscoverRPC", RpcTarget.All, Array.Empty<object>());
			}
		}
		ValuableDiscover.instance.New(this.physGrabObject, _state);
	}

	// Token: 0x0600143A RID: 5178 RVA: 0x000B15DF File Offset: 0x000AF7DF
	[PunRPC]
	private void DiscoverRPC()
	{
		this.discovered = true;
		Map.Instance.AddValuable(this);
	}

	// Token: 0x0600143B RID: 5179 RVA: 0x000B15F3 File Offset: 0x000AF7F3
	[PunRPC]
	public void DollarValueSetRPC(float value)
	{
		this.dollarValueOriginal = value;
		this.dollarValueCurrent = value;
		this.dollarValueSet = true;
	}

	// Token: 0x04002286 RID: 8838
	[Space(40f)]
	[Header("Presets")]
	public Durability durabilityPreset;

	// Token: 0x04002287 RID: 8839
	public Value valuePreset;

	// Token: 0x04002288 RID: 8840
	public PhysAttribute physAttributePreset;

	// Token: 0x04002289 RID: 8841
	public PhysAudio audioPreset;

	// Token: 0x0400228A RID: 8842
	[Range(0.5f, 3f)]
	public float audioPresetPitch = 1f;

	// Token: 0x0400228B RID: 8843
	public Gradient particleColors;

	// Token: 0x0400228C RID: 8844
	[Space(70f)]
	public ValuableVolume.Type volumeType;

	// Token: 0x0400228D RID: 8845
	public bool debugVolume = true;

	// Token: 0x0400228E RID: 8846
	private Mesh meshTiny;

	// Token: 0x0400228F RID: 8847
	private Mesh meshSmall;

	// Token: 0x04002290 RID: 8848
	private Mesh meshMedium;

	// Token: 0x04002291 RID: 8849
	private Mesh meshBig;

	// Token: 0x04002292 RID: 8850
	private Mesh meshWide;

	// Token: 0x04002293 RID: 8851
	private Mesh meshTall;

	// Token: 0x04002294 RID: 8852
	private Mesh meshVeryTall;

	// Token: 0x04002295 RID: 8853
	[Space(20f)]
	[HideInInspector]
	public float dollarValueOriginal = 100f;

	// Token: 0x04002296 RID: 8854
	[HideInInspector]
	public float dollarValueCurrent = 100f;

	// Token: 0x04002297 RID: 8855
	internal bool dollarValueSet;

	// Token: 0x04002298 RID: 8856
	internal int dollarValueOverride;

	// Token: 0x04002299 RID: 8857
	private float rigidBodyMass;

	// Token: 0x0400229A RID: 8858
	private Rigidbody rb;

	// Token: 0x0400229B RID: 8859
	private PhotonView photonView;

	// Token: 0x0400229C RID: 8860
	private NavMeshObstacle navMeshObstacle;

	// Token: 0x0400229D RID: 8861
	private bool inStartRoom;

	// Token: 0x0400229E RID: 8862
	private float inStartRoomCheckTimer;

	// Token: 0x0400229F RID: 8863
	internal RoomVolumeCheck roomVolumeCheck;

	// Token: 0x040022A0 RID: 8864
	internal PhysGrabObject physGrabObject;

	// Token: 0x040022A1 RID: 8865
	internal bool discovered;

	// Token: 0x040022A2 RID: 8866
	internal bool discoveredReminder;

	// Token: 0x040022A3 RID: 8867
	private float discoveredReminderTimer;
}
