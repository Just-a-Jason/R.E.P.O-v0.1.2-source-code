using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000FB RID: 251
public class Materials : MonoBehaviour
{
	// Token: 0x060008CC RID: 2252 RVA: 0x0005436E File Offset: 0x0005256E
	private void Awake()
	{
		Materials.Instance = this;
	}

	// Token: 0x060008CD RID: 2253 RVA: 0x00054378 File Offset: 0x00052578
	public void Impulse(Vector3 origin, Vector3 direction, Materials.SoundType soundType, bool footstep, Materials.MaterialTrigger materialTrigger, Materials.HostType hostType)
	{
		Vector3 material = this.GetMaterial(origin, materialTrigger);
		if (this.LastMaterialList)
		{
			float volumeMultiplier = 1f;
			float falloffMultiplier = 1f;
			float offscreenVolumeMultiplier = 1f;
			float offscreenFalloffMultiplier = 1f;
			if (hostType == Materials.HostType.OtherPlayer)
			{
				volumeMultiplier = 0.5f;
			}
			else if (hostType == Materials.HostType.Enemy)
			{
				volumeMultiplier = 0.5f;
				falloffMultiplier = 0.5f;
				offscreenVolumeMultiplier = 0.25f;
				offscreenFalloffMultiplier = 0.25f;
			}
			switch (soundType)
			{
			case Materials.SoundType.Light:
				if (footstep)
				{
					if (this.LastMaterialList.RareFootstepLightMax > 0)
					{
						this.LastMaterialList.RareFootstepLightCurrent -= 1f;
						if (this.LastMaterialList.RareFootstepLightCurrent <= 0f)
						{
							this.LastMaterialList.RareFootstepLight.Play(material, volumeMultiplier, falloffMultiplier, offscreenVolumeMultiplier, offscreenFalloffMultiplier);
							this.LastMaterialList.RareFootstepLightCurrent = (float)Random.Range(this.LastMaterialList.RareFootstepLightMin, this.LastMaterialList.RareFootstepLightMax);
						}
					}
					this.LastMaterialList.FootstepLight.Play(material, volumeMultiplier, falloffMultiplier, offscreenVolumeMultiplier, offscreenFalloffMultiplier);
					return;
				}
				if (this.LastMaterialList.RareImpactLightMax > 0)
				{
					this.LastMaterialList.RareImpactLightCurrent -= 1f;
					if (this.LastMaterialList.RareImpactLightCurrent <= 0f)
					{
						this.LastMaterialList.RareImpactLight.Play(material, volumeMultiplier, falloffMultiplier, offscreenVolumeMultiplier, offscreenFalloffMultiplier);
						this.LastMaterialList.RareImpactLightCurrent = (float)Random.Range(this.LastMaterialList.RareImpactLightMin, this.LastMaterialList.RareImpactLightMax);
					}
				}
				this.LastMaterialList.ImpactLight.Play(material, volumeMultiplier, falloffMultiplier, offscreenVolumeMultiplier, offscreenFalloffMultiplier);
				return;
			case Materials.SoundType.Medium:
				if (footstep)
				{
					if (this.LastMaterialList.RareFootstepMediumMax > 0)
					{
						this.LastMaterialList.RareFootstepMediumCurrent -= 1f;
						if (this.LastMaterialList.RareFootstepMediumCurrent <= 0f)
						{
							this.LastMaterialList.RareFootstepMedium.Play(material, volumeMultiplier, falloffMultiplier, offscreenVolumeMultiplier, offscreenFalloffMultiplier);
							this.LastMaterialList.RareFootstepMediumCurrent = (float)Random.Range(this.LastMaterialList.RareFootstepMediumMin, this.LastMaterialList.RareFootstepMediumMax);
						}
					}
					this.LastMaterialList.FootstepMedium.Play(material, volumeMultiplier, falloffMultiplier, offscreenVolumeMultiplier, offscreenFalloffMultiplier);
					return;
				}
				if (this.LastMaterialList.RareImpactMediumMax > 0)
				{
					this.LastMaterialList.RareImpactMediumCurrent -= 1f;
					if (this.LastMaterialList.RareImpactMediumCurrent <= 0f)
					{
						this.LastMaterialList.RareImpactMedium.Play(material, volumeMultiplier, falloffMultiplier, offscreenVolumeMultiplier, offscreenFalloffMultiplier);
						this.LastMaterialList.RareImpactMediumCurrent = (float)Random.Range(this.LastMaterialList.RareImpactMediumMin, this.LastMaterialList.RareImpactMediumMax);
					}
				}
				this.LastMaterialList.ImpactMedium.Play(material, volumeMultiplier, falloffMultiplier, offscreenVolumeMultiplier, offscreenFalloffMultiplier);
				return;
			case Materials.SoundType.Heavy:
				if (footstep)
				{
					if (this.LastMaterialList.RareFootstepHeavyMax > 0)
					{
						this.LastMaterialList.RareFootstepHeavyCurrent -= 1f;
						if (this.LastMaterialList.RareFootstepHeavyCurrent <= 0f)
						{
							this.LastMaterialList.RareFootstepHeavy.Play(material, volumeMultiplier, falloffMultiplier, offscreenVolumeMultiplier, offscreenFalloffMultiplier);
							this.LastMaterialList.RareFootstepHeavyCurrent = (float)Random.Range(this.LastMaterialList.RareFootstepHeavyMin, this.LastMaterialList.RareFootstepHeavyMax);
						}
					}
					this.LastMaterialList.FootstepHeavy.Play(material, volumeMultiplier, falloffMultiplier, offscreenVolumeMultiplier, offscreenFalloffMultiplier);
					return;
				}
				if (this.LastMaterialList.RareImpactHeavyMax > 0)
				{
					this.LastMaterialList.RareImpactHeavyCurrent -= 1f;
					if (this.LastMaterialList.RareImpactHeavyCurrent <= 0f)
					{
						this.LastMaterialList.RareImpactHeavy.Play(material, volumeMultiplier, falloffMultiplier, offscreenVolumeMultiplier, offscreenFalloffMultiplier);
						this.LastMaterialList.RareImpactHeavyCurrent = (float)Random.Range(this.LastMaterialList.RareImpactHeavyMin, this.LastMaterialList.RareImpactHeavyMax);
					}
				}
				this.LastMaterialList.ImpactHeavy.Play(material, volumeMultiplier, falloffMultiplier, offscreenVolumeMultiplier, offscreenFalloffMultiplier);
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x060008CE RID: 2254 RVA: 0x00054758 File Offset: 0x00052958
	public void Slide(Vector3 origin, Materials.MaterialTrigger materialTrigger, float spatialBlend, bool isPlayer)
	{
		float volumeMultiplier = 1f;
		if (!isPlayer)
		{
			volumeMultiplier = 0.5f;
		}
		Vector3 material = this.GetMaterial(origin, materialTrigger);
		if (this.LastMaterialList)
		{
			this.LastMaterialList.SlideOneShot.SpatialBlend = spatialBlend;
			this.LastMaterialList.SlideOneShot.Play(material, volumeMultiplier, 1f, 1f, 1f);
		}
	}

	// Token: 0x060008CF RID: 2255 RVA: 0x000547C0 File Offset: 0x000529C0
	public void SlideLoop(Vector3 origin, Materials.MaterialTrigger materialTrigger, float spatialBlend, float pitchMultiplier)
	{
		Vector3 position = origin;
		bool flag = materialTrigger.SlidingLoopObject != null;
		if (!flag || materialTrigger.SlidingLoopObject.getMaterialTimer <= 0f)
		{
			position = this.GetMaterial(origin, materialTrigger);
			if (flag)
			{
				materialTrigger.SlidingLoopObject.getMaterialTimer = 0.25f;
			}
		}
		if (materialTrigger.LastMaterialList != null)
		{
			bool flag2 = false;
			if (!flag)
			{
				flag2 = true;
			}
			else if (materialTrigger.SlidingLoopObject.material != materialTrigger.LastMaterialList)
			{
				materialTrigger.SlidingLoopObject = null;
				flag2 = true;
			}
			if (flag2)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(AudioManager.instance.AudioMaterialSlidingLoop, position, Quaternion.identity, AudioManager.instance.SoundsParent);
				materialTrigger.SlidingLoopObject = gameObject.GetComponent<MaterialSlidingLoop>();
				materialTrigger.SlidingLoopObject.material = materialTrigger.LastMaterialList;
			}
			materialTrigger.SlidingLoopObject.activeTimer = 0.1f;
			materialTrigger.SlidingLoopObject.transform.position = position;
			materialTrigger.SlidingLoopObject.pitchMultiplier = pitchMultiplier;
		}
	}

	// Token: 0x060008D0 RID: 2256 RVA: 0x000548B8 File Offset: 0x00052AB8
	private Vector3 GetMaterial(Vector3 origin, Materials.MaterialTrigger materialTrigger)
	{
		origin = new Vector3(origin.x, origin.y + 0.1f, origin.z);
		Materials.Type _type = materialTrigger.LastMaterialType;
		RaycastHit raycastHit;
		if (Physics.Raycast(origin, Vector3.down, out raycastHit, 1f, this.LayerMask, QueryTriggerInteraction.Collide))
		{
			MaterialSurface component = raycastHit.collider.gameObject.GetComponent<MaterialSurface>();
			if (component)
			{
				_type = component.Type;
				origin = raycastHit.point;
			}
		}
		this.LastMaterialList = this.MaterialList.Find((MaterialPreset x) => x.Type == _type);
		materialTrigger.LastMaterialType = _type;
		materialTrigger.LastMaterialList = this.LastMaterialList;
		return origin;
	}

	// Token: 0x04000FF8 RID: 4088
	public static Materials Instance;

	// Token: 0x04000FF9 RID: 4089
	public LayerMask LayerMask;

	// Token: 0x04000FFA RID: 4090
	[Space]
	public List<MaterialPreset> MaterialList;

	// Token: 0x04000FFB RID: 4091
	private MaterialPreset LastMaterialList;

	// Token: 0x0200032C RID: 812
	public enum Type
	{
		// Token: 0x04002641 RID: 9793
		None,
		// Token: 0x04002642 RID: 9794
		Wood,
		// Token: 0x04002643 RID: 9795
		Rug,
		// Token: 0x04002644 RID: 9796
		Tile,
		// Token: 0x04002645 RID: 9797
		Stone,
		// Token: 0x04002646 RID: 9798
		Catwalk,
		// Token: 0x04002647 RID: 9799
		Snow,
		// Token: 0x04002648 RID: 9800
		Metal,
		// Token: 0x04002649 RID: 9801
		Wetmetal,
		// Token: 0x0400264A RID: 9802
		Gravel,
		// Token: 0x0400264B RID: 9803
		Grass,
		// Token: 0x0400264C RID: 9804
		Water
	}

	// Token: 0x0200032D RID: 813
	public enum SoundType
	{
		// Token: 0x0400264E RID: 9806
		Light,
		// Token: 0x0400264F RID: 9807
		Medium,
		// Token: 0x04002650 RID: 9808
		Heavy
	}

	// Token: 0x0200032E RID: 814
	public enum HostType
	{
		// Token: 0x04002652 RID: 9810
		LocalPlayer,
		// Token: 0x04002653 RID: 9811
		OtherPlayer,
		// Token: 0x04002654 RID: 9812
		Enemy
	}

	// Token: 0x0200032F RID: 815
	[Serializable]
	public class MaterialTrigger
	{
		// Token: 0x04002655 RID: 9813
		internal MaterialPreset LastMaterialList;

		// Token: 0x04002656 RID: 9814
		internal Materials.Type LastMaterialType;

		// Token: 0x04002657 RID: 9815
		internal MaterialSlidingLoop SlidingLoopObject;
	}
}
