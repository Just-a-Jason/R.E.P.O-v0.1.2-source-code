using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000142 RID: 322
public class ItemBattery : MonoBehaviour
{
	// Token: 0x06000AD2 RID: 2770 RVA: 0x00060AFC File Offset: 0x0005ECFC
	private void Awake()
	{
		this.photonView = base.GetComponent<PhotonView>();
		this.itemAttributes = base.GetComponent<ItemAttributes>();
		if (!this.itemAttributes)
		{
			Debug.LogWarning("ItemBattery.cs: No ItemAttributes found on " + base.gameObject.name);
		}
	}

	// Token: 0x06000AD3 RID: 2771 RVA: 0x00060B48 File Offset: 0x0005ED48
	private void Start()
	{
		this.itemEquippable = base.GetComponent<ItemEquippable>();
		this.mainCamera = Camera.main;
		this.itemBatteryMaterial = this.batteryTransform.GetComponentInChildren<Renderer>();
		this.itemBatteryMaterial.material.SetColor("_Color", this.batteryColor);
		this.physGrabObject = base.GetComponentInChildren<PhysGrabObject>();
		if (SemiFunc.RunIsLevel() && TutorialDirector.instance.TutorialSettingCheck(DataDirector.Setting.TutorialChargingStation, 1))
		{
			this.tutorialCheck = true;
		}
	}

	// Token: 0x06000AD4 RID: 2772 RVA: 0x00060BC1 File Offset: 0x0005EDC1
	private IEnumerator BatteryInit()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return new WaitForSeconds(0.2f);
		}
		while (base.GetComponent<ItemAttributes>().instanceName == null)
		{
			yield return new WaitForSeconds(0.2f);
		}
		if (SemiFunc.RunIsArena())
		{
			StatsManager.instance.SetBatteryLevel(this.itemAttributes.instanceName, 100);
		}
		this.batteryLife = (float)StatsManager.instance.GetBatteryLevel(this.itemAttributes.instanceName);
		if (this.batteryLife > 0f)
		{
			this.batteryLifeInt = (int)Mathf.Round(this.batteryLife / 16.6f);
			this.batteryColor = this.itemAttributes.colorPreset.GetColorLight();
		}
		else
		{
			this.batteryLife = 0f;
			this.batteryLifeInt = 0;
			this.batteryColor = this.itemAttributes.colorPreset.GetColorLight();
		}
		this.BatteryFullPercentChange(this.batteryLifeInt, false);
		yield break;
	}

	// Token: 0x06000AD5 RID: 2773 RVA: 0x00060BD0 File Offset: 0x0005EDD0
	public void SetBatteryLife(int _batteryLife)
	{
		if (this.batteryLife > 0f)
		{
			this.batteryLife = (float)_batteryLife;
			this.batteryLifeInt = (int)Mathf.Round(this.batteryLife / 16.6f);
		}
		else
		{
			this.batteryLife = 0f;
			this.batteryLifeInt = 0;
		}
		this.batteryColor = this.itemAttributes.colorPreset.GetColorLight();
		this.BatteryFullPercentChange(this.batteryLifeInt, false);
	}

	// Token: 0x06000AD6 RID: 2774 RVA: 0x00060C41 File Offset: 0x0005EE41
	public void OverrideBatteryShow(float time = 0.1f)
	{
		this.showTimer = time;
	}

	// Token: 0x06000AD7 RID: 2775 RVA: 0x00060C4A File Offset: 0x0005EE4A
	public void ChargeBattery(GameObject chargerObject, float chargeAmount)
	{
		if (!this.chargerList.Contains(chargerObject))
		{
			this.chargerList.Add(chargerObject);
			this.chargeRate += chargeAmount;
		}
		this.chargeTimer = 0.1f;
	}

	// Token: 0x06000AD8 RID: 2776 RVA: 0x00060C80 File Offset: 0x0005EE80
	private void FixedUpdate()
	{
		if (this.showTimer > 0f)
		{
			this.showTimer -= Time.fixedDeltaTime;
			this.showBattery = true;
		}
		else
		{
			this.showBattery = false;
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (this.chargeTimer > 0f && this.batteryLife < 99f)
		{
			this.batteryLife = Mathf.Clamp(this.batteryLife + this.chargeRate * Time.fixedDeltaTime, 0f, 100f);
			if (!this.isCharging)
			{
				this.BatteryChargeToggle(true);
			}
			this.chargeTimer -= Time.fixedDeltaTime;
		}
		else if (this.chargeRate != 0f)
		{
			this.chargeRate = 0f;
			this.chargeTimer = 0f;
			this.chargerList.Clear();
			this.BatteryChargeToggle(false);
		}
		if (this.drainTimer > 0f && this.batteryLife > 0f)
		{
			this.batteryLife = Mathf.Clamp(this.batteryLife - this.drainRate * Time.fixedDeltaTime, 0f, 100f);
			this.drainTimer -= Time.fixedDeltaTime;
			return;
		}
		if (this.drainRate != 0f)
		{
			this.drainRate = 0f;
			this.drainTimer = 0f;
		}
	}

	// Token: 0x06000AD9 RID: 2777 RVA: 0x00060DD8 File Offset: 0x0005EFD8
	private void Update()
	{
		if (this.itemAttributes.shopItem && SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.batteryLife = 100f;
		}
		this.BatteryLookAt();
		this.BatteryChargingVisuals();
		if (SemiFunc.RunIsLobby() && this.batteryLifeInt < 6)
		{
			this.OverrideBatteryShow(0.1f);
		}
		if (this.showBattery && !this.itemBatteryMaterial.gameObject.activeSelf)
		{
			this.itemBatteryMaterial.gameObject.SetActive(true);
			this.BatteryOffsetTexture(this.batteryLifeInt);
		}
		if (this.tutorialCheck && this.batteryLife <= 0f && SemiFunc.FPSImpulse15() && this.physGrabObject.playerGrabbing.Count > 0)
		{
			using (List<PhysGrabber>.Enumerator enumerator = this.physGrabObject.playerGrabbing.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.isLocal && TutorialDirector.instance.TutorialSettingCheck(DataDirector.Setting.TutorialChargingStation, 1))
					{
						TutorialDirector.instance.ActivateTip("Charging Station", 2f, false);
						this.tutorialCheck = false;
					}
				}
			}
		}
		if (this.batteryActive)
		{
			if (SemiFunc.IsMasterClientOrSingleplayer() && this.autoDrain && !this.itemEquippable.isEquipped)
			{
				this.batteryLife -= this.batteryDrainRate * Time.deltaTime;
			}
			if (this.batteryLifeInt <= 1)
			{
				if (this.batteryLifeInt == 1)
				{
					this.batteryOutBlinkTimer += Time.deltaTime;
				}
				else
				{
					this.batteryOutBlinkTimer += 5f * Time.deltaTime;
				}
				if (this.batteryOutBlinkTimer >= 1f)
				{
					if (!this.lowBatteryBeep)
					{
						this.itemBatteryMaterial.material.SetTextureOffset("_MainTex", new Vector2(0f, 0.278f));
						if (this.batteryLifeInt < 1)
						{
							AssetManager.instance.batteryLowBeep.Play(base.transform.position, 1f, 1f, 1f, 1f);
							this.itemBatteryMaterial.material.SetTextureOffset("_MainTex", new Vector2(0f, 0.044f));
						}
						this.lowBatteryBeep = true;
					}
				}
				else if (this.lowBatteryBeep)
				{
					this.itemBatteryMaterial.material.SetTextureOffset("_MainTex", new Vector2(0.375f, 0.278f));
					this.lowBatteryBeep = false;
				}
				if (this.batteryOutBlinkTimer >= 2f)
				{
					this.batteryOutBlinkTimer = 0f;
				}
			}
			if (!this.itemBatteryMaterial.gameObject.activeSelf)
			{
				this.itemBatteryMaterial.gameObject.SetActive(true);
				this.BatteryOffsetTexture(this.batteryLifeInt);
			}
		}
		else if (!this.showBattery && this.itemBatteryMaterial.gameObject.activeSelf && !this.isCharging)
		{
			this.itemBatteryMaterial.gameObject.SetActive(false);
			this.BatteryOffsetTexture(this.batteryLifeInt);
		}
		if (GameManager.instance.gameMode == 0 || (GameManager.instance.gameMode == 1 && PhotonNetwork.IsMasterClient))
		{
			if (this.batteryLifeInt == 0 && this.batteryLife >= 17f)
			{
				this.BatteryFullPercentChange(1, true);
			}
			else if (this.batteryLifeInt == 1 && this.batteryLife >= 34f)
			{
				this.BatteryFullPercentChange(2, true);
			}
			else if (this.batteryLifeInt == 2 && this.batteryLife >= 50f)
			{
				this.BatteryFullPercentChange(3, true);
			}
			else if (this.batteryLifeInt == 3 && this.batteryLife >= 67f)
			{
				this.BatteryFullPercentChange(4, true);
			}
			else if (this.batteryLifeInt == 4 && this.batteryLife >= 84f)
			{
				this.BatteryFullPercentChange(5, true);
			}
			else if (this.batteryLifeInt == 5 && this.batteryLife >= 99f)
			{
				this.BatteryFullPercentChange(6, true);
			}
			if (this.batteryLifeInt == 6 && this.batteryLife <= 84f)
			{
				this.BatteryFullPercentChange(5, false);
				return;
			}
			if (this.batteryLifeInt == 5 && this.batteryLife <= 67f)
			{
				this.BatteryFullPercentChange(4, false);
				return;
			}
			if (this.batteryLifeInt == 4 && this.batteryLife <= 50f)
			{
				this.BatteryFullPercentChange(3, false);
				return;
			}
			if (this.batteryLifeInt == 3 && this.batteryLife <= 34f)
			{
				this.BatteryFullPercentChange(2, false);
				return;
			}
			if (this.batteryLifeInt == 2 && this.batteryLife <= 17f)
			{
				this.BatteryFullPercentChange(1, false);
				return;
			}
			if (this.batteryLifeInt == 1 && this.batteryLife <= 0f)
			{
				this.BatteryFullPercentChange(0, false);
			}
		}
	}

	// Token: 0x06000ADA RID: 2778 RVA: 0x00061288 File Offset: 0x0005F488
	public void RemoveFullBar(int _bars)
	{
		if (SemiFunc.RunIsShop())
		{
			return;
		}
		if (this.batteryLifeInt > 0)
		{
			this.batteryLifeInt -= _bars;
			if (this.batteryLifeInt <= 0)
			{
				this.batteryLifeInt = 0;
				this.batteryLife = 0f;
			}
			else
			{
				this.batteryLife = (float)this.batteryLifeInt * 16.6f;
			}
			this.BatteryFullPercentChange(this.batteryLifeInt, false);
		}
	}

	// Token: 0x06000ADB RID: 2779 RVA: 0x000612F1 File Offset: 0x0005F4F1
	public void BatteryToggle(bool toggle)
	{
		if (SemiFunc.IsMultiplayer())
		{
			if (SemiFunc.IsMasterClientOrSingleplayer())
			{
				this.photonView.RPC("BatteryToggleRPC", RpcTarget.All, new object[]
				{
					toggle
				});
				return;
			}
		}
		else
		{
			this.BatteryToggleRPC(toggle);
		}
	}

	// Token: 0x06000ADC RID: 2780 RVA: 0x00061329 File Offset: 0x0005F529
	[PunRPC]
	public void BatteryToggleRPC(bool toggle)
	{
		this.batteryActive = toggle;
	}

	// Token: 0x06000ADD RID: 2781 RVA: 0x00061334 File Offset: 0x0005F534
	private void BatteryLookAt()
	{
		if (!this.showBattery && !this.batteryActive && !this.isCharging)
		{
			return;
		}
		this.batteryTransform.LookAt(this.mainCamera.transform);
		float d = Vector3.Distance(this.batteryTransform.position, this.mainCamera.transform.position);
		this.batteryTransform.localScale = Vector3.one * d * 0.8f;
		if (this.batteryTransform.localScale.x > 3f)
		{
			this.batteryTransform.localScale = Vector3.one * 3f;
		}
		this.batteryTransform.Rotate(0f, 180f, 0f);
		this.batteryTransform.position = base.transform.position + Vector3.up * this.upOffset;
	}

	// Token: 0x06000ADE RID: 2782 RVA: 0x00061428 File Offset: 0x0005F628
	private void BatteryChargingVisuals()
	{
		if (this.isCharging)
		{
			if (!this.itemBatteryMaterial.gameObject.activeSelf)
			{
				this.itemBatteryMaterial.gameObject.SetActive(true);
			}
			this.chargingBlinkTimer += Time.deltaTime;
			if (this.chargingBlinkTimer > 0.5f)
			{
				this.chargingBlink = !this.chargingBlink;
				if (this.chargingBlink)
				{
					this.BatteryOffsetTexture(this.batteryLifeInt + 1);
				}
				else
				{
					this.BatteryOffsetTexture(this.batteryLifeInt);
				}
				this.chargingBlinkTimer = 0f;
			}
		}
	}

	// Token: 0x06000ADF RID: 2783 RVA: 0x000614C0 File Offset: 0x0005F6C0
	private void BatteryChargeToggle(bool toggle)
	{
		if (SemiFunc.IsMultiplayer())
		{
			if (SemiFunc.IsMasterClientOrSingleplayer())
			{
				this.photonView.RPC("BatteryChargeStartRPC", RpcTarget.All, new object[]
				{
					toggle
				});
				return;
			}
		}
		else
		{
			this.BatteryChargeStartRPC(toggle);
		}
	}

	// Token: 0x06000AE0 RID: 2784 RVA: 0x000614F8 File Offset: 0x0005F6F8
	[PunRPC]
	private void BatteryChargeStartRPC(bool toggle)
	{
		this.isCharging = toggle;
		this.BatteryOffsetTexture(this.batteryLifeInt);
	}

	// Token: 0x06000AE1 RID: 2785 RVA: 0x00061510 File Offset: 0x0005F710
	private void BatteryOffsetTexture(int batteryLevel)
	{
		if (!this.itemBatteryMaterial)
		{
			return;
		}
		Color red = this.batteryColor;
		switch (batteryLevel)
		{
		case 0:
			this.itemBatteryMaterial.material.SetTextureOffset("_MainTex", new Vector2(0f, 0.044f));
			if (!this.isCharging)
			{
				red = Color.red;
			}
			this.itemBatteryMaterial.material.SetColor("_Color", red);
			return;
		case 1:
			this.itemBatteryMaterial.material.SetTextureOffset("_MainTex", new Vector2(0.375f, 0.278f));
			if (!this.isCharging)
			{
				red = Color.red;
			}
			this.itemBatteryMaterial.material.SetColor("_Color", red);
			return;
		case 2:
			this.itemBatteryMaterial.material.SetTextureOffset("_MainTex", new Vector2(0f, 0.278f));
			this.itemBatteryMaterial.material.SetColor("_Color", red);
			return;
		case 3:
			this.itemBatteryMaterial.material.SetTextureOffset("_MainTex", new Vector2(0.375f, 0.512f));
			this.itemBatteryMaterial.material.SetColor("_Color", red);
			return;
		case 4:
			this.itemBatteryMaterial.material.SetTextureOffset("_MainTex", new Vector2(0f, 0.512f));
			this.itemBatteryMaterial.material.SetColor("_Color", red);
			return;
		case 5:
			this.itemBatteryMaterial.material.SetTextureOffset("_MainTex", new Vector2(0.375f, 0.745f));
			this.itemBatteryMaterial.material.SetColor("_Color", red);
			return;
		case 6:
			this.itemBatteryMaterial.material.SetTextureOffset("_MainTex", new Vector2(0f, 0.745f));
			this.itemBatteryMaterial.material.SetColor("_Color", red);
			return;
		default:
			return;
		}
	}

	// Token: 0x06000AE2 RID: 2786 RVA: 0x00061710 File Offset: 0x0005F910
	private void BatteryFullPercentChangeLogic(int batteryLevel, bool charge)
	{
		if (this.batteryLifeInt > batteryLevel && batteryLevel == 1 && this.batteryActive)
		{
			AssetManager.instance.batteryLowWarning.Play(base.transform.position, 1f, 1f, 1f, 1f);
		}
		this.batteryLifeInt = batteryLevel;
		if (this.batteryLifeInt != 0)
		{
			this.batteryLife = (float)this.batteryLifeInt * 16.6f;
		}
		else
		{
			this.batteryLife = 0f;
		}
		SemiFunc.StatSetBattery(this.itemAttributes.instanceName, (int)this.batteryLife);
		this.BatteryOffsetTexture(this.batteryLifeInt);
		if (this.batteryActive || charge)
		{
			if (charge)
			{
				AssetManager.instance.batteryChargeSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
				return;
			}
			AssetManager.instance.batteryDrainSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		}
	}

	// Token: 0x06000AE3 RID: 2787 RVA: 0x00061820 File Offset: 0x0005FA20
	private void BatteryFullPercentChange(int batteryLifeInt, bool charge = false)
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.BatteryFullPercentChangeLogic(batteryLifeInt, charge);
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.photonView.RPC("BatteryFullPercentChangeRPC", RpcTarget.All, new object[]
			{
				batteryLifeInt,
				charge
			});
		}
	}

	// Token: 0x06000AE4 RID: 2788 RVA: 0x00061872 File Offset: 0x0005FA72
	[PunRPC]
	private void BatteryFullPercentChangeRPC(int batteryLifeInt, bool charge)
	{
		this.BatteryFullPercentChangeLogic(batteryLifeInt, charge);
	}

	// Token: 0x06000AE5 RID: 2789 RVA: 0x0006187C File Offset: 0x0005FA7C
	public void Drain(float amount)
	{
		this.drainRate = amount;
		this.drainTimer = 0.1f;
	}

	// Token: 0x04001190 RID: 4496
	public bool isUnchargable;

	// Token: 0x04001191 RID: 4497
	public Transform batteryTransform;

	// Token: 0x04001192 RID: 4498
	private Camera mainCamera;

	// Token: 0x04001193 RID: 4499
	public float upOffset = 0.5f;

	// Token: 0x04001194 RID: 4500
	[HideInInspector]
	public bool batteryActive;

	// Token: 0x04001195 RID: 4501
	[HideInInspector]
	public float batteryLife = 100f;

	// Token: 0x04001196 RID: 4502
	internal int batteryLifeInt = 6;

	// Token: 0x04001197 RID: 4503
	private Renderer itemBatteryMaterial;

	// Token: 0x04001198 RID: 4504
	private float batteryOutBlinkTimer;

	// Token: 0x04001199 RID: 4505
	private PhotonView photonView;

	// Token: 0x0400119A RID: 4506
	[HideInInspector]
	public Color batteryColor;

	// Token: 0x0400119B RID: 4507
	private float chargeTimer;

	// Token: 0x0400119C RID: 4508
	private float chargeRate;

	// Token: 0x0400119D RID: 4509
	private List<GameObject> chargerList = new List<GameObject>();

	// Token: 0x0400119E RID: 4510
	internal bool isCharging;

	// Token: 0x0400119F RID: 4511
	private float chargingBlinkTimer;

	// Token: 0x040011A0 RID: 4512
	private bool chargingBlink;

	// Token: 0x040011A1 RID: 4513
	private bool lowBatteryBeep;

	// Token: 0x040011A2 RID: 4514
	private ItemAttributes itemAttributes;

	// Token: 0x040011A3 RID: 4515
	private float showTimer;

	// Token: 0x040011A4 RID: 4516
	private bool showBattery;

	// Token: 0x040011A5 RID: 4517
	public bool autoDrain = true;

	// Token: 0x040011A6 RID: 4518
	private ItemEquippable itemEquippable;

	// Token: 0x040011A7 RID: 4519
	public bool onlyShowWhenItemToggleIsOn;

	// Token: 0x040011A8 RID: 4520
	public float batteryDrainRate = 1f;

	// Token: 0x040011A9 RID: 4521
	private float drainRate;

	// Token: 0x040011AA RID: 4522
	private float drainTimer;

	// Token: 0x040011AB RID: 4523
	private bool tutorialCheck;

	// Token: 0x040011AC RID: 4524
	private PhysGrabObject physGrabObject;
}
