using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000E9 RID: 233
public class ChargingStation : MonoBehaviour, IPunObservable
{
	// Token: 0x06000821 RID: 2081 RVA: 0x0004E982 File Offset: 0x0004CB82
	private void Awake()
	{
		ChargingStation.instance = this;
	}

	// Token: 0x06000822 RID: 2082 RVA: 0x0004E98C File Offset: 0x0004CB8C
	private void Start()
	{
		this.chargeRate = 0.05f;
		foreach (object obj in this.crystalCylinder)
		{
			Transform transform = (Transform)obj;
			this.crystals.Add(transform);
		}
		this.chargingStationEmissionMaterial = this.meshObject.GetComponent<Renderer>().material;
		this.chargeBar = base.transform.Find("Charge");
		this.photonView = base.GetComponent<PhotonView>();
		this.chargeArea = base.transform.Find("Charge Area");
		this.lockedTransform = base.transform.Find("Locked");
		this.light1 = base.transform.Find("Light1").GetComponent<Light>();
		this.light2 = base.transform.Find("Light2").GetComponent<Light>();
		if (!SemiFunc.RunIsShop())
		{
			if (this.lockedTransform)
			{
				Object.Destroy(this.lockedTransform.gameObject);
			}
		}
		else
		{
			if (this.subtleLight)
			{
				Object.Destroy(this.subtleLight);
			}
			if (this.chargeArea)
			{
				Object.Destroy(this.chargeArea.gameObject);
			}
			if (this.chargeBar)
			{
				Object.Destroy(this.chargeBar.gameObject);
			}
			Object.Destroy(this.light1.gameObject);
			Object.Destroy(this.light2.gameObject);
		}
		this.charge = 0f;
		this.chargeScale = 0f;
		this.chargeScaleTarget = 0f;
		this.chargeBar.localScale = new Vector3(0f, 1f, 1f);
		this.chargeInt = SemiFunc.StatGetItemsPurchased("Item Power Crystal");
		if (this.chargeInt <= 0)
		{
			this.OutOfCrystalsShutdown();
		}
		int num = StatsManager.instance.runStats["chargingStationCharge"];
		if (this.chargeInt > num)
		{
			if (this.chargeInt > this.chargeSegments)
			{
				this.chargeInt = this.chargeSegments;
			}
			this.charge = (float)this.chargeInt * (1f / (float)this.chargeSegments);
			this.chargeScale = this.charge;
			this.chargeScaleTarget = this.charge;
			this.chargeBar.localScale = new Vector3(this.chargeScale, 1f, 1f);
			StatsManager.instance.runStats["chargingStationCharge"] = this.chargeInt;
		}
		else
		{
			int num2 = num;
			float chargingStationCharge = StatsManager.instance.chargingStationCharge;
			this.chargeInt = num2;
			if (this.chargeInt > this.chargeSegments)
			{
				this.chargeInt = this.chargeSegments;
			}
			this.charge = chargingStationCharge;
			if (this.charge > (float)this.chargeInt * (1f / (float)this.chargeSegments))
			{
				this.charge = (float)this.chargeInt * (1f / (float)this.chargeSegments);
			}
			this.chargeScale = (float)this.chargeInt * (1f / (float)this.chargeSegments);
			this.chargeScaleTarget = (float)this.chargeInt * (1f / (float)this.chargeSegments);
			this.chargeBar.localScale = new Vector3(this.chargeScale, 1f, 1f);
			StatsManager.instance.runStats["chargingStationCharge"] = this.chargeInt;
		}
		base.StartCoroutine(this.MissionText());
		while (this.crystals.Count > this.chargeInt)
		{
			Object.Destroy(this.crystals[0].gameObject);
			this.crystals.RemoveAt(0);
			if (this.crystals.Count == 0)
			{
				this.OutOfCrystalsShutdown();
				break;
			}
		}
		if (RunManager.instance.levelsCompleted < 1)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06000823 RID: 2083 RVA: 0x0004ED88 File Offset: 0x0004CF88
	private void OutOfCrystalsShutdown()
	{
		this.chargingStationEmissionMaterial.SetColor("_EmissionColor", Color.black);
		this.light1.enabled = false;
		this.light2.enabled = false;
		Color color = new Color(0.1f, 0.1f, 0.2f);
		this.subtleLight.GetComponent<Light>().color = color;
	}

	// Token: 0x06000824 RID: 2084 RVA: 0x0004EDE9 File Offset: 0x0004CFE9
	public IEnumerator MissionText()
	{
		while (LevelGenerator.Instance.Generated)
		{
			yield return new WaitForSeconds(0.1f);
		}
		yield return new WaitForSeconds(2f);
		if (SemiFunc.RunIsLobby())
		{
			SemiFunc.UIFocusText("Enjoy the ride, recharge stuff and GEAR UP!", Color.white, AssetManager.instance.colorYellow, 3f);
		}
		yield break;
	}

	// Token: 0x06000825 RID: 2085 RVA: 0x0004EDF1 File Offset: 0x0004CFF1
	private void StopCharge()
	{
		if (SemiFunc.IsMasterClient())
		{
			this.photonView.RPC("StopChargeRPC", RpcTarget.All, Array.Empty<object>());
			return;
		}
		this.StopChargeRPC();
	}

	// Token: 0x06000826 RID: 2086 RVA: 0x0004EE17 File Offset: 0x0004D017
	[PunRPC]
	public void StopChargeRPC()
	{
		this.soundStop.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.isCharging = false;
	}

	// Token: 0x06000827 RID: 2087 RVA: 0x0004EE4B File Offset: 0x0004D04B
	private void StartCharge()
	{
		if (SemiFunc.IsMasterClient())
		{
			this.photonView.RPC("StartChargeRPC", RpcTarget.All, Array.Empty<object>());
			return;
		}
		this.StartChargeRPC();
	}

	// Token: 0x06000828 RID: 2088 RVA: 0x0004EE71 File Offset: 0x0004D071
	[PunRPC]
	public void StartChargeRPC()
	{
		this.soundStart.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.isCharging = true;
	}

	// Token: 0x06000829 RID: 2089 RVA: 0x0004EEA8 File Offset: 0x0004D0A8
	private void ChargeArea()
	{
		if (SemiFunc.RunIsShop())
		{
			return;
		}
		if (this.charge <= 0f)
		{
			if (this.isCharging)
			{
				this.isChargingPrev = this.isCharging;
				this.StopCharge();
				this.isCharging = false;
			}
			return;
		}
		this.chargeAreaCheckTimer += Time.deltaTime;
		if (this.chargeAreaCheckTimer > 0.5f)
		{
			Collider[] array = Physics.OverlapBox(this.chargeArea.position, this.chargeArea.localScale / 2f, this.chargeArea.localRotation, SemiFunc.LayerMaskGetPhysGrabObject());
			this.itemsCharging.Clear();
			Collider[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				ItemBattery componentInParent = array2[i].GetComponentInParent<ItemBattery>();
				if (componentInParent && componentInParent.batteryLifeInt < 6 && !this.itemsCharging.Contains(componentInParent))
				{
					this.itemsCharging.Add(componentInParent);
				}
			}
			this.chargeAreaCheckTimer = 0f;
		}
		bool flag = false;
		foreach (ItemBattery itemBattery in this.itemsCharging)
		{
			if (itemBattery.batteryLifeInt < 6)
			{
				itemBattery.ChargeBattery(base.gameObject, 30f);
				this.charge -= this.chargeRate * Time.deltaTime;
				flag = true;
				if (!this.isCharging)
				{
					this.StartCharge();
					this.isChargingPrev = this.isCharging;
					this.isCharging = true;
				}
			}
		}
		if (!flag && this.isCharging)
		{
			this.isChargingPrev = this.isCharging;
			this.StopCharge();
			this.isCharging = false;
		}
	}

	// Token: 0x0600082A RID: 2090 RVA: 0x0004F060 File Offset: 0x0004D260
	private void ChargingEffects()
	{
		if (this.isCharging)
		{
			TutorialDirector.instance.playerUsedChargingStation = true;
			this.crystalCylinder.localRotation = Quaternion.Euler(90f, 0f, Mathf.PingPong(Time.time * 150f, 5f) - 2.5f);
			int num = 0;
			foreach (Transform transform in this.crystals)
			{
				if (transform)
				{
					num++;
					float value = 0.1f + Mathf.PingPong((Time.time + (float)num) * 5f, 1f);
					Color value2 = Color.yellow * Mathf.LinearToGammaSpace(value);
					transform.GetComponent<Renderer>().material.SetColor("_EmissionColor", value2);
				}
			}
			this.crystalCooldown = 0f;
			return;
		}
		this.crystalCylinder.localRotation = Quaternion.Euler(90f, 0f, 0f);
		foreach (Transform transform2 in this.crystals)
		{
			if (transform2)
			{
				this.crystalCooldown += Time.deltaTime * 0.5f;
				float num2 = this.chargeCurve.Evaluate(this.crystalCooldown);
				float value3 = Mathf.Lerp(1f, 0.1f, num2);
				Color value4 = Color.yellow * Mathf.LinearToGammaSpace(value3);
				transform2.GetComponent<Renderer>().material.SetColor("_EmissionColor", value4);
				this.crystalCylinder.localRotation = Quaternion.Euler(90f, 0f, (Mathf.PingPong(Time.time * 250f, 10f) - 5f) * (1f - num2));
			}
		}
	}

	// Token: 0x0600082B RID: 2091 RVA: 0x0004F270 File Offset: 0x0004D470
	private void Update()
	{
		if (SemiFunc.RunIsShop())
		{
			return;
		}
		this.soundLoop.PlayLoop(this.isCharging, 2f, 2f, 1f);
		this.AnimateChargeBar();
		this.ChargingEffects();
		int count = this.crystals.Count;
		if (this.isCharging && count > 0)
		{
			float value = 0.5f + Mathf.PingPong(Time.time * 5f, 0.5f);
			Color value2 = Color.yellow * Mathf.LinearToGammaSpace(value);
			this.chargingStationEmissionMaterial.SetColor("_EmissionColor", value2);
			if (this.light1 && this.light2)
			{
				this.light1.enabled = true;
				this.light2.enabled = true;
				this.light1.intensity = 0.5f + Mathf.PingPong(Time.time * 5f, 0.5f);
				this.light2.intensity = 0.5f + Mathf.PingPong(Time.time * 5f, 0.5f);
			}
		}
		else if (this.light1 && this.light2)
		{
			this.chargingStationEmissionMaterial.SetColor("_EmissionColor", Color.black);
			this.light1.enabled = false;
			this.light2.enabled = false;
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (RunManager.instance.restarting)
		{
			return;
		}
		this.ChargeArea();
		float num = 1f / (float)this.chargeSegments;
		int num2 = Mathf.CeilToInt(this.charge / num);
		if (num2 > this.chargeSegments)
		{
			num2 = this.chargeSegments;
		}
		if (this.chargeInt != num2)
		{
			this.chargeInt = num2;
			this.UpdateChargeBar(this.chargeInt);
		}
	}

	// Token: 0x0600082C RID: 2092 RVA: 0x0004F440 File Offset: 0x0004D640
	private void AnimateChargeBar()
	{
		if (!this.chargeBar)
		{
			return;
		}
		if (this.chargeScale == this.chargeScaleTarget)
		{
			return;
		}
		this.chargeCurveTime += Time.deltaTime;
		this.chargeScale = Mathf.Lerp(this.chargeScale, this.chargeScaleTarget, this.chargeCurve.Evaluate(this.chargeCurveTime));
		this.chargeBar.localScale = new Vector3(this.chargeScale, 1f, 1f);
	}

	// Token: 0x0600082D RID: 2093 RVA: 0x0004F4C4 File Offset: 0x0004D6C4
	private void UpdateChargeBar(int segmentPassed)
	{
		if (SemiFunc.IsMasterClient())
		{
			this.photonView.RPC("UpdateChargeBarRPC", RpcTarget.All, new object[]
			{
				segmentPassed
			});
			return;
		}
		this.UpdateChargeBarRPC(segmentPassed);
	}

	// Token: 0x0600082E RID: 2094 RVA: 0x0004F4F8 File Offset: 0x0004D6F8
	private void DestroyCrystal()
	{
		if (this.crystals.Count < 1)
		{
			return;
		}
		Vector3 position = this.crystals[0].position + this.crystals[0].up * 0.1f;
		this.lightParticle.transform.position = position;
		this.fireflyParticles.transform.position = position;
		this.bitsParticles.transform.position = position;
		this.lightParticle.Play();
		this.fireflyParticles.Play();
		this.bitsParticles.Play();
		this.soundPowerCrystalBreak.Play(position, 1f, 1f, 1f, 1f);
		Object.Destroy(this.crystals[0].gameObject);
		this.crystals.RemoveAt(0);
		if (this.crystals.Count == 0)
		{
			this.OutOfCrystalsShutdown();
		}
	}

	// Token: 0x0600082F RID: 2095 RVA: 0x0004F5F0 File Offset: 0x0004D7F0
	[PunRPC]
	public void UpdateChargeBarRPC(int segmentPassed)
	{
		this.chargeCurveTime = 0f;
		float num = 1f / (float)this.chargeSegments;
		this.chargeScaleTarget = (float)segmentPassed * num;
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			StatsManager.instance.SetItemPurchase(this.item, StatsManager.instance.GetItemPurchased(this.item) - 1);
		}
		this.DestroyCrystal();
	}

	// Token: 0x06000830 RID: 2096 RVA: 0x0004F64F File Offset: 0x0004D84F
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (PhotonNetwork.IsMasterClient)
		{
			stream.SendNext(this.isCharging);
			return;
		}
		this.isCharging = (bool)stream.ReceiveNext();
	}

	// Token: 0x04000EF3 RID: 3827
	public static ChargingStation instance;

	// Token: 0x04000EF4 RID: 3828
	private PhotonView photonView;

	// Token: 0x04000EF5 RID: 3829
	private Transform chargeBar;

	// Token: 0x04000EF6 RID: 3830
	internal float charge = 1f;

	// Token: 0x04000EF7 RID: 3831
	private float chargeScale = 1f;

	// Token: 0x04000EF8 RID: 3832
	private float chargeScaleTarget = 1f;

	// Token: 0x04000EF9 RID: 3833
	internal int chargeInt;

	// Token: 0x04000EFA RID: 3834
	private int chargeSegments = 6;

	// Token: 0x04000EFB RID: 3835
	private float chargeRate = 0.05f;

	// Token: 0x04000EFC RID: 3836
	public AnimationCurve chargeCurve;

	// Token: 0x04000EFD RID: 3837
	private float chargeCurveTime;

	// Token: 0x04000EFE RID: 3838
	private Transform chargeArea;

	// Token: 0x04000EFF RID: 3839
	private float chargeAreaCheckTimer;

	// Token: 0x04000F00 RID: 3840
	private List<ItemBattery> itemsCharging = new List<ItemBattery>();

	// Token: 0x04000F01 RID: 3841
	private Transform lockedTransform;

	// Token: 0x04000F02 RID: 3842
	public GameObject meshObject;

	// Token: 0x04000F03 RID: 3843
	private Material chargingStationEmissionMaterial;

	// Token: 0x04000F04 RID: 3844
	private bool isCharging;

	// Token: 0x04000F05 RID: 3845
	private bool isChargingPrev;

	// Token: 0x04000F06 RID: 3846
	private Light light1;

	// Token: 0x04000F07 RID: 3847
	private Light light2;

	// Token: 0x04000F08 RID: 3848
	public Sound soundStart;

	// Token: 0x04000F09 RID: 3849
	public Sound soundStop;

	// Token: 0x04000F0A RID: 3850
	public Sound soundLoop;

	// Token: 0x04000F0B RID: 3851
	public Transform crystalCylinder;

	// Token: 0x04000F0C RID: 3852
	public List<Transform> crystals = new List<Transform>();

	// Token: 0x04000F0D RID: 3853
	public ParticleSystem lightParticle;

	// Token: 0x04000F0E RID: 3854
	public ParticleSystem fireflyParticles;

	// Token: 0x04000F0F RID: 3855
	public ParticleSystem bitsParticles;

	// Token: 0x04000F10 RID: 3856
	public Sound soundPowerCrystalBreak;

	// Token: 0x04000F11 RID: 3857
	private float crystalCooldown;

	// Token: 0x04000F12 RID: 3858
	public Item item;

	// Token: 0x04000F13 RID: 3859
	public GameObject subtleLight;
}
