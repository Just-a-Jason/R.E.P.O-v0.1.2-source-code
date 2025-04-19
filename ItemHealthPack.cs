using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200014E RID: 334
public class ItemHealthPack : MonoBehaviour
{
	// Token: 0x06000B1C RID: 2844 RVA: 0x00063014 File Offset: 0x00061214
	private void Start()
	{
		this.itemToggle = base.GetComponent<ItemToggle>();
		this.itemEquippable = base.GetComponent<ItemEquippable>();
		this.itemAttributes = base.GetComponent<ItemAttributes>();
		this.photonView = base.GetComponent<PhotonView>();
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.material = this.mesh.material;
		this.materialEmissionOriginal = this.material.GetColor(this.materialPropertyEmission);
	}

	// Token: 0x06000B1D RID: 2845 RVA: 0x00063088 File Offset: 0x00061288
	private void Update()
	{
		if (SemiFunc.RunIsShop())
		{
			return;
		}
		this.LightLogic();
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (this.itemToggle.toggleState && !this.used)
		{
			PlayerAvatar playerAvatar = SemiFunc.PlayerAvatarGetFromPhotonID(this.itemToggle.playerTogglePhotonID);
			if (playerAvatar)
			{
				if (playerAvatar.playerHealth.health >= playerAvatar.playerHealth.maxHealth)
				{
					if (SemiFunc.IsMultiplayer())
					{
						this.photonView.RPC("RejectRPC", RpcTarget.All, Array.Empty<object>());
					}
					else
					{
						this.RejectRPC();
					}
					this.itemToggle.ToggleItem(false, -1);
					this.physGrabObject.rb.AddForce(Vector3.up * 2f, ForceMode.Impulse);
					this.physGrabObject.rb.AddTorque(-this.physGrabObject.transform.right * 0.05f, ForceMode.Impulse);
					return;
				}
				playerAvatar.playerHealth.HealOther(this.healAmount, true);
				int num = StatsManager.instance.itemsPurchased[this.itemAttributes.item.itemAssetName];
				StatsManager.instance.ItemRemove(this.itemAttributes.instanceName);
				this.physGrabObject.impactDetector.indestructibleBreakEffects = true;
				if (SemiFunc.IsMultiplayer())
				{
					this.photonView.RPC("UsedRPC", RpcTarget.All, Array.Empty<object>());
					return;
				}
				this.UsedRPC();
			}
		}
	}

	// Token: 0x06000B1E RID: 2846 RVA: 0x000631FC File Offset: 0x000613FC
	private void LightLogic()
	{
		if (this.used && this.lightIntensityLerp < 1f)
		{
			this.lightIntensityLerp += 1f * Time.deltaTime;
			this.propLight.lightComponent.intensity = this.lightIntensityCurve.Evaluate(this.lightIntensityLerp);
			this.propLight.originalIntensity = this.propLight.lightComponent.intensity;
			this.material.SetColor(this.materialPropertyEmission, Color.Lerp(Color.black, this.materialEmissionOriginal, this.lightIntensityCurve.Evaluate(this.lightIntensityLerp)));
		}
	}

	// Token: 0x06000B1F RID: 2847 RVA: 0x000632AC File Offset: 0x000614AC
	[PunRPC]
	private void UsedRPC()
	{
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 1f, 6f, base.transform.position, 0.2f);
		this.itemToggle.ToggleDisable(true);
		this.itemAttributes.DisableUI(true);
		Object.Destroy(this.itemEquippable);
		ParticleSystem[] array = this.particles;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Play();
		}
		this.soundUse.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.used = true;
	}

	// Token: 0x06000B20 RID: 2848 RVA: 0x0006335C File Offset: 0x0006155C
	[PunRPC]
	private void RejectRPC()
	{
		PlayerAvatar playerAvatar = SemiFunc.PlayerAvatarGetFromPhotonID(this.itemToggle.playerTogglePhotonID);
		if (playerAvatar.isLocal)
		{
			playerAvatar.physGrabber.ReleaseObjectRPC(false, 1f);
		}
		ParticleSystem[] array = this.rejectParticles;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Play();
		}
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 1f, 6f, base.transform.position, 0.2f);
		this.soundReject.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000B21 RID: 2849 RVA: 0x0006340C File Offset: 0x0006160C
	public void OnDestroy()
	{
		foreach (ParticleSystem particleSystem in this.particles)
		{
			if (particleSystem && particleSystem.isPlaying)
			{
				particleSystem.transform.SetParent(null);
				particleSystem.main.stopAction = ParticleSystemStopAction.Destroy;
			}
		}
		foreach (ParticleSystem particleSystem2 in this.rejectParticles)
		{
			if (particleSystem2 && particleSystem2.isPlaying)
			{
				particleSystem2.transform.SetParent(null);
				particleSystem2.main.stopAction = ParticleSystemStopAction.Destroy;
			}
		}
	}

	// Token: 0x0400120D RID: 4621
	public int healAmount;

	// Token: 0x0400120E RID: 4622
	private ItemToggle itemToggle;

	// Token: 0x0400120F RID: 4623
	private ItemEquippable itemEquippable;

	// Token: 0x04001210 RID: 4624
	private ItemAttributes itemAttributes;

	// Token: 0x04001211 RID: 4625
	private PhotonView photonView;

	// Token: 0x04001212 RID: 4626
	private PhysGrabObject physGrabObject;

	// Token: 0x04001213 RID: 4627
	[Space]
	public ParticleSystem[] particles;

	// Token: 0x04001214 RID: 4628
	public ParticleSystem[] rejectParticles;

	// Token: 0x04001215 RID: 4629
	[Space]
	public PropLight propLight;

	// Token: 0x04001216 RID: 4630
	public AnimationCurve lightIntensityCurve;

	// Token: 0x04001217 RID: 4631
	private float lightIntensityLerp;

	// Token: 0x04001218 RID: 4632
	public MeshRenderer mesh;

	// Token: 0x04001219 RID: 4633
	private Material material;

	// Token: 0x0400121A RID: 4634
	private Color materialEmissionOriginal;

	// Token: 0x0400121B RID: 4635
	private int materialPropertyEmission = Shader.PropertyToID("_EmissionColor");

	// Token: 0x0400121C RID: 4636
	[Space]
	public Sound soundUse;

	// Token: 0x0400121D RID: 4637
	public Sound soundReject;

	// Token: 0x0400121E RID: 4638
	private bool used;
}
