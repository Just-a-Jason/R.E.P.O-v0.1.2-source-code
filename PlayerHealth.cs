using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001B7 RID: 439
public class PlayerHealth : MonoBehaviour
{
	// Token: 0x06000EDF RID: 3807 RVA: 0x000876CC File Offset: 0x000858CC
	private void Awake()
	{
		this.photonView = base.GetComponent<PhotonView>();
		if (!this.isMenuAvatar)
		{
			this.playerAvatar = base.GetComponent<PlayerAvatar>();
		}
		else
		{
			this.playerAvatar = PlayerAvatar.instance;
		}
		if (!this.isMenuAvatar && !SemiFunc.RunIsLobbyMenu() && (!GameManager.Multiplayer() || this.photonView.IsMine))
		{
			base.StartCoroutine(this.Fetch());
		}
		this.materialEffectCurve = AssetManager.instance.animationCurveImpact;
		this.renderers = new List<MeshRenderer>();
		this.renderers.AddRange(this.meshParent.GetComponentsInChildren<MeshRenderer>(true));
		foreach (MeshRenderer meshRenderer in this.renderers)
		{
			Material material = null;
			foreach (Material material2 in this.sharedMaterials)
			{
				if (meshRenderer.sharedMaterial.name == material2.name)
				{
					material = material2;
					meshRenderer.sharedMaterial = this.instancedMaterials[this.sharedMaterials.IndexOf(material2)];
				}
			}
			if (!material)
			{
				string name = meshRenderer.sharedMaterial.name;
				material = meshRenderer.sharedMaterial;
				this.sharedMaterials.Add(material);
				this.instancedMaterials.Add(meshRenderer.material);
				if (name == "Player Avatar - Body")
				{
					this.bodyMaterial = meshRenderer.sharedMaterial;
				}
				if (name == "Player Avatar - Health")
				{
					this.healthMaterial = meshRenderer.sharedMaterial;
				}
				if (name == "Player Avatar - Eye")
				{
					this.eyeMaterial = meshRenderer.sharedMaterial;
				}
				if (name == "Player Avatar - Pupil")
				{
					this.pupilMaterial = meshRenderer.sharedMaterial;
				}
			}
		}
		this.materialHurtColor = Shader.PropertyToID("_ColorOverlay");
		this.materialHurtAmount = Shader.PropertyToID("_ColorOverlayAmount");
		this.healthMaterialAmount = Shader.PropertyToID("_OffsetX");
	}

	// Token: 0x06000EE0 RID: 3808 RVA: 0x0008790C File Offset: 0x00085B0C
	private void Start()
	{
		foreach (DebugComputerCheck debugComputerCheck in Object.FindObjectsOfType<DebugComputerCheck>())
		{
			if (debugComputerCheck.Active && debugComputerCheck.PlayerDebug && debugComputerCheck.GodMode)
			{
				this.godMode = true;
			}
		}
	}

	// Token: 0x06000EE1 RID: 3809 RVA: 0x00087950 File Offset: 0x00085B50
	private IEnumerator Fetch()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return new WaitForSeconds(0.1f);
		}
		int num = StatsManager.instance.GetPlayerHealth(SemiFunc.PlayerGetSteamID(this.playerAvatar));
		if (num <= 0)
		{
			num = 1;
		}
		this.health = num;
		this.maxHealth = 100 + StatsManager.instance.GetPlayerMaxHealth(SemiFunc.PlayerGetSteamID(this.playerAvatar));
		this.health = Mathf.Clamp(this.health, 0, this.maxHealth);
		if (SemiFunc.RunIsArena())
		{
			this.health = this.maxHealth;
		}
		StatsManager.instance.SetPlayerHealth(SemiFunc.PlayerGetSteamID(this.playerAvatar), this.health, false);
		if (GameManager.Multiplayer())
		{
			this.photonView.RPC("UpdateHealthRPC", RpcTarget.Others, new object[]
			{
				this.health,
				this.maxHealth,
				true
			});
		}
		this.healthSet = true;
		yield break;
	}

	// Token: 0x06000EE2 RID: 3810 RVA: 0x00087960 File Offset: 0x00085B60
	private void Update()
	{
		if (this.playerAvatar.isLocal)
		{
			if (this.overrideEyeMaterialTimer > 0f)
			{
				this.overrideEyeMaterialTimer -= Time.deltaTime;
				if (!this.overrideEyeActive)
				{
					this.overrideEyeActive = true;
				}
			}
			else if (this.overrideEyeActive)
			{
				this.overrideEyeActive = false;
				this.overrideEyePriority = -999;
			}
			if (SemiFunc.IsMultiplayer() && (this.overrideEyeActive != this.overrideEyeActivePrevious || this.overrideEyeState != this.overrideEyeStatePrevious))
			{
				this.overrideEyeActivePrevious = this.overrideEyeActive;
				this.overrideEyeStatePrevious = this.overrideEyeState;
				this.photonView.RPC("EyeMaterialOverrideRPC", RpcTarget.Others, new object[]
				{
					this.overrideEyeState,
					this.overrideEyeActive
				});
			}
		}
		if (this.overrideEyeActive)
		{
			this.overrideEyeMaterialLerp += 3f * Time.deltaTime;
		}
		else
		{
			this.overrideEyeMaterialLerp -= 3f * Time.deltaTime;
		}
		this.overrideEyeMaterialLerp = Mathf.Clamp01(this.overrideEyeMaterialLerp);
		if (this.overrideEyeMaterialLerp != this.overrideEyeMaterialLerpPrevious)
		{
			this.overrideEyeMaterialLerpPrevious = this.overrideEyeMaterialLerp;
			float num = AssetManager.instance.animationCurveEaseInOut.Evaluate(this.overrideEyeMaterialLerp);
			this.eyeMaterial.SetFloat(this.materialHurtAmount, num);
			this.eyeMaterial.SetColor(this.materialHurtColor, this.overrideEyeMaterialColor);
			this.pupilMaterial.SetFloat(this.materialHurtAmount, num);
			this.pupilMaterial.SetColor(this.materialHurtColor, this.overridePupilMaterialColor);
			if (this.overrideEyeMaterialLerp <= 0f)
			{
				this.eyeLight.gameObject.SetActive(false);
			}
			else if (!this.eyeLight.gameObject.activeSelf)
			{
				this.eyeLight.gameObject.SetActive(true);
			}
			this.eyeLight.color = this.overrideEyeLightColor;
			this.eyeLight.intensity = this.overrideEyeLightIntensity * num;
		}
		if (this.materialEffect)
		{
			this.materialEffectLerp += 2.5f * Time.deltaTime;
			this.materialEffectLerp = Mathf.Clamp01(this.materialEffectLerp);
			if (this.playerAvatar.deadSet && !this.playerAvatar.isDisabled)
			{
				this.materialEffectLerp = Mathf.Clamp(this.materialEffectLerp, 0f, 0.1f);
			}
			foreach (Material material in this.instancedMaterials)
			{
				if (material != this.eyeMaterial && material != this.pupilMaterial)
				{
					material.SetFloat(this.materialHurtAmount, this.materialEffectCurve.Evaluate(this.materialEffectLerp));
				}
			}
			if (this.hurtFreeze && this.materialEffectLerp > 0.2f)
			{
				this.hurtFreeze = false;
			}
			if (this.materialEffectLerp >= 1f)
			{
				this.materialEffect = false;
				foreach (Material material2 in this.instancedMaterials)
				{
					if (material2 != this.eyeMaterial && material2 != this.pupilMaterial)
					{
						material2.SetFloat(this.materialHurtAmount, 0f);
					}
				}
			}
			this.hurtFreezeTimer = 0f;
			if (!this.overrideEyeActive)
			{
				this.eyeMaterial.SetFloat(this.materialHurtAmount, this.materialEffectCurve.Evaluate(this.materialEffectLerp));
				this.eyeMaterial.SetColor(this.materialHurtColor, Color.white);
				this.pupilMaterial.SetFloat(this.materialHurtAmount, this.materialEffectCurve.Evaluate(this.materialEffectLerp));
				this.pupilMaterial.SetColor(this.materialHurtColor, Color.black);
			}
		}
		else if (this.hurtFreeze)
		{
			this.hurtFreezeTimer -= Time.deltaTime;
			if (this.hurtFreezeTimer <= 0f)
			{
				this.hurtFreeze = false;
			}
		}
		if (this.isMenuAvatar)
		{
			this.health = this.playerAvatar.playerHealth.health;
		}
		if ((this.isMenuAvatar || (GameManager.Multiplayer() && !this.playerAvatar.isLocal)) && this.healthPrevious != this.health)
		{
			float num2 = (float)this.health / (float)this.maxHealth;
			float num3 = Mathf.Lerp(0.98f, 0f, num2);
			if (num3 <= 0f)
			{
				num3 = -0.5f;
			}
			this.healthMaterial.SetFloat(this.healthMaterialAmount, num3);
			int nameID = Shader.PropertyToID("_AlbedoColor");
			int nameID2 = Shader.PropertyToID("_EmissionColor");
			Color value = this.healthMaterialColor.Evaluate(num2);
			this.healthMaterial.SetColor(nameID, value);
			value.a = this.healthMaterial.GetColor(nameID2).a;
			this.healthMaterial.SetColor(nameID2, value);
			this.healthPrevious = this.health;
		}
		if (this.invincibleTimer > 0f)
		{
			this.invincibleTimer -= Time.deltaTime;
		}
	}

	// Token: 0x06000EE3 RID: 3811 RVA: 0x00087EBC File Offset: 0x000860BC
	public void HurtFreezeOverride(float _time)
	{
		this.hurtFreeze = true;
		this.hurtFreezeTimer = _time;
	}

	// Token: 0x06000EE4 RID: 3812 RVA: 0x00087ECC File Offset: 0x000860CC
	public void Death()
	{
		this.health = 0;
		StatsManager.instance.SetPlayerHealth(SemiFunc.PlayerGetSteamID(this.playerAvatar), this.health, false);
		if (GameManager.Multiplayer())
		{
			this.photonView.RPC("UpdateHealthRPC", RpcTarget.Others, new object[]
			{
				this.health,
				this.maxHealth,
				true
			});
		}
	}

	// Token: 0x06000EE5 RID: 3813 RVA: 0x00087F3F File Offset: 0x0008613F
	public void MaterialEffectOverride(PlayerHealth.Effect _effect)
	{
		if (!GameManager.Multiplayer())
		{
			this.MaterialEffectOverrideRPC((int)_effect);
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.photonView.RPC("MaterialEffectOverrideRPC", RpcTarget.All, new object[]
			{
				(int)_effect
			});
		}
	}

	// Token: 0x06000EE6 RID: 3814 RVA: 0x00087F78 File Offset: 0x00086178
	[PunRPC]
	public void MaterialEffectOverrideRPC(int _effect)
	{
		this.materialEffect = true;
		this.materialEffectLerp = 0f;
		Color white = Color.white;
		if (_effect == 0)
		{
			white = new Color(1f, 0.94f, 0f);
			if (!this.playerAvatar.isLocal)
			{
				this.upgradeOther.Play(base.transform.position, 1f, 1f, 1f, 1f);
			}
		}
		foreach (Material material in this.instancedMaterials)
		{
			if (material != this.eyeMaterial && material != this.pupilMaterial)
			{
				material.SetColor(this.materialHurtColor, white);
			}
		}
	}

	// Token: 0x06000EE7 RID: 3815 RVA: 0x00088058 File Offset: 0x00086258
	public void Hurt(int damage, bool savingGrace, int enemyIndex = -1)
	{
		if (this.invincibleTimer > 0f)
		{
			return;
		}
		if (damage <= 0)
		{
			return;
		}
		if (GameManager.Multiplayer() && !this.photonView.IsMine)
		{
			return;
		}
		if (this.playerAvatar.deadSet || this.godMode)
		{
			return;
		}
		if (savingGrace && damage <= 25 && this.health > 5 && this.health <= 20)
		{
			this.health -= damage;
			if (this.health <= 0)
			{
				this.health = Random.Range(1, 5);
			}
		}
		else
		{
			this.health -= damage;
		}
		if (this.health <= 0)
		{
			this.playerAvatar.PlayerDeath(enemyIndex);
			this.health = 0;
			return;
		}
		if ((float)damage >= 25f)
		{
			CameraGlitch.Instance.PlayLongHurt();
		}
		else
		{
			CameraGlitch.Instance.PlayShortHurt();
		}
		StatsManager.instance.SetPlayerHealth(SemiFunc.PlayerGetSteamID(this.playerAvatar), this.health, false);
		if (GameManager.Multiplayer())
		{
			this.photonView.RPC("UpdateHealthRPC", RpcTarget.Others, new object[]
			{
				this.health,
				this.maxHealth,
				true
			});
		}
	}

	// Token: 0x06000EE8 RID: 3816 RVA: 0x00088190 File Offset: 0x00086390
	public void HurtOther(int damage, Vector3 hurtPosition, bool savingGrace, int enemyIndex = -1)
	{
		if (!GameManager.Multiplayer())
		{
			this.Hurt(damage, savingGrace, enemyIndex);
			return;
		}
		this.photonView.RPC("HurtOtherRPC", RpcTarget.All, new object[]
		{
			damage,
			hurtPosition,
			savingGrace,
			enemyIndex
		});
	}

	// Token: 0x06000EE9 RID: 3817 RVA: 0x000881EC File Offset: 0x000863EC
	[PunRPC]
	public void HurtOtherRPC(int damage, Vector3 hurtPosition, bool savingGrace, int enemyIndex)
	{
		if (this.photonView.IsMine && (hurtPosition == Vector3.zero || Vector3.Distance(this.playerAvatar.transform.position, hurtPosition) < 2f))
		{
			this.Hurt(damage, savingGrace, enemyIndex);
		}
	}

	// Token: 0x06000EEA RID: 3818 RVA: 0x0008823C File Offset: 0x0008643C
	public void Heal(int healAmount, bool effect = true)
	{
		if (healAmount <= 0 || this.health == this.maxHealth)
		{
			return;
		}
		if (GameManager.Multiplayer() && !this.photonView.IsMine)
		{
			return;
		}
		if (this.playerAvatar.isDisabled || this.godMode)
		{
			return;
		}
		if (effect && this.health != 0)
		{
			if ((float)healAmount >= 25f)
			{
				CameraGlitch.Instance.PlayLongHeal();
			}
			else
			{
				CameraGlitch.Instance.PlayShortHeal();
			}
		}
		this.health += healAmount;
		this.health = Mathf.Clamp(this.health, 0, this.maxHealth);
		StatsManager.instance.SetPlayerHealth(SemiFunc.PlayerGetSteamID(this.playerAvatar), this.health, false);
		if (GameManager.Multiplayer())
		{
			this.photonView.RPC("UpdateHealthRPC", RpcTarget.Others, new object[]
			{
				this.health,
				this.maxHealth,
				effect
			});
		}
	}

	// Token: 0x06000EEB RID: 3819 RVA: 0x00088336 File Offset: 0x00086536
	public void HealOther(int healAmount, bool effect)
	{
		if (!GameManager.Multiplayer())
		{
			this.Heal(healAmount, effect);
			return;
		}
		this.photonView.RPC("HealOtherRPC", RpcTarget.All, new object[]
		{
			healAmount,
			effect
		});
	}

	// Token: 0x06000EEC RID: 3820 RVA: 0x00088371 File Offset: 0x00086571
	[PunRPC]
	public void HealOtherRPC(int healAmount, bool effect)
	{
		if (this.photonView.IsMine)
		{
			this.Heal(healAmount, effect);
		}
	}

	// Token: 0x06000EED RID: 3821 RVA: 0x00088388 File Offset: 0x00086588
	[PunRPC]
	public void UpdateHealthRPC(int healthNew, int healthMax, bool effect)
	{
		this.maxHealth = healthMax;
		if (!this.healthSet)
		{
			this.health = healthNew;
			this.healthSet = true;
		}
		else
		{
			if (effect)
			{
				this.materialEffect = true;
				if (!this.playerAvatar.deadSet)
				{
					this.materialEffectLerp = 0f;
				}
				if (healthNew < this.health || healthNew == 0)
				{
					this.hurtOther.Play(base.transform.position, 1f, 1f, 1f, 1f);
					this.hurtFreeze = true;
					using (List<Material>.Enumerator enumerator = this.instancedMaterials.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							Material material = enumerator.Current;
							if (material != this.eyeMaterial && material != this.pupilMaterial)
							{
								material.SetColor(this.materialHurtColor, Color.red);
							}
						}
						goto IL_11D;
					}
				}
				if (this.health != 0)
				{
					this.healOther.Play(base.transform.position, 1f, 1f, 1f, 1f);
				}
				this.SetMaterialGreen();
			}
			IL_11D:
			this.health = healthNew;
		}
		StatsManager.instance.SetPlayerHealth(SemiFunc.PlayerGetSteamID(this.playerAvatar), this.health, false);
	}

	// Token: 0x06000EEE RID: 3822 RVA: 0x000884E8 File Offset: 0x000866E8
	public void SetMaterialGreen()
	{
		foreach (Material material in this.instancedMaterials)
		{
			if (material != this.eyeMaterial && material != this.pupilMaterial)
			{
				material.SetColor(this.materialHurtColor, new Color(0f, 1f, 0.25f));
			}
		}
	}

	// Token: 0x06000EEF RID: 3823 RVA: 0x00088570 File Offset: 0x00086770
	private void EyeMaterialSetup()
	{
		if (this.overrideEyeMaterialLerp >= 1f)
		{
			this.overrideEyeMaterialLerp = 0.8f;
		}
		if (this.overrideEyeState == PlayerHealth.EyeOverrideState.Red)
		{
			this.overrideEyeMaterialColor = Color.red;
			this.overridePupilMaterialColor = Color.white;
			this.overrideEyeLightColor = Color.red;
			this.overrideEyeLightIntensity = 5f;
			return;
		}
		if (this.overrideEyeState == PlayerHealth.EyeOverrideState.Green)
		{
			this.overrideEyeMaterialColor = Color.green;
			this.overridePupilMaterialColor = Color.white;
			this.overrideEyeLightColor = Color.green;
			this.overrideEyeLightIntensity = 5f;
			return;
		}
		if (this.overrideEyeState == PlayerHealth.EyeOverrideState.Love)
		{
			this.overrideEyeMaterialColor = new Color(1f, 0f, 0.5f);
			this.overridePupilMaterialColor = new Color(0.2f, 0f, 0.2f);
			this.overrideEyeLightColor = new Color(0.4f, 0f, 0f);
			this.overrideEyeLightIntensity = 1f;
			return;
		}
		if (this.overrideEyeState == PlayerHealth.EyeOverrideState.CeilingEye)
		{
			this.overrideEyeMaterialColor = new Color(1f, 0.4f, 0f);
			this.overridePupilMaterialColor = new Color(1f, 1f, 0f);
			this.overrideEyeLightColor = new Color(1f, 0.4f, 0f);
			this.overrideEyeLightIntensity = 1f;
		}
	}

	// Token: 0x06000EF0 RID: 3824 RVA: 0x000886C6 File Offset: 0x000868C6
	public void EyeMaterialOverride(PlayerHealth.EyeOverrideState _state, float _time, int _priority)
	{
		if (_priority < this.overrideEyePriority)
		{
			return;
		}
		this.overrideEyePriority = _priority;
		if (this.overrideEyeState != _state)
		{
			this.overrideEyeState = _state;
			this.EyeMaterialSetup();
		}
		this.overrideEyeMaterialTimer = _time;
	}

	// Token: 0x06000EF1 RID: 3825 RVA: 0x000886F6 File Offset: 0x000868F6
	[PunRPC]
	public void EyeMaterialOverrideRPC(PlayerHealth.EyeOverrideState _state, bool _active)
	{
		this.overrideEyeActive = _active;
		if (this.overrideEyeState != _state)
		{
			this.overrideEyeState = _state;
			this.EyeMaterialSetup();
		}
	}

	// Token: 0x06000EF2 RID: 3826 RVA: 0x00088715 File Offset: 0x00086915
	public void InvincibleSet(float _time)
	{
		this.invincibleTimer = _time;
	}

	// Token: 0x040018FE RID: 6398
	public bool isMenuAvatar;

	// Token: 0x040018FF RID: 6399
	private PlayerAvatar playerAvatar;

	// Token: 0x04001900 RID: 6400
	private PhotonView photonView;

	// Token: 0x04001901 RID: 6401
	internal bool hurtFreeze;

	// Token: 0x04001902 RID: 6402
	private float hurtFreezeTimer;

	// Token: 0x04001903 RID: 6403
	private bool healthSet;

	// Token: 0x04001904 RID: 6404
	internal int health = 100;

	// Token: 0x04001905 RID: 6405
	private int healthPrevious;

	// Token: 0x04001906 RID: 6406
	internal int maxHealth = 100;

	// Token: 0x04001907 RID: 6407
	private bool godMode;

	// Token: 0x04001908 RID: 6408
	public Transform meshParent;

	// Token: 0x04001909 RID: 6409
	public Light eyeLight;

	// Token: 0x0400190A RID: 6410
	private List<MeshRenderer> renderers;

	// Token: 0x0400190B RID: 6411
	private List<Material> sharedMaterials = new List<Material>();

	// Token: 0x0400190C RID: 6412
	internal List<Material> instancedMaterials = new List<Material>();

	// Token: 0x0400190D RID: 6413
	private int materialHurtAmount;

	// Token: 0x0400190E RID: 6414
	private int materialHurtColor;

	// Token: 0x0400190F RID: 6415
	internal Material bodyMaterial;

	// Token: 0x04001910 RID: 6416
	internal Material eyeMaterial;

	// Token: 0x04001911 RID: 6417
	internal Material pupilMaterial;

	// Token: 0x04001912 RID: 6418
	private Material healthMaterial;

	// Token: 0x04001913 RID: 6419
	private int healthMaterialAmount;

	// Token: 0x04001914 RID: 6420
	public Gradient healthMaterialColor;

	// Token: 0x04001915 RID: 6421
	private bool materialEffect;

	// Token: 0x04001916 RID: 6422
	private Color materialEffectColor;

	// Token: 0x04001917 RID: 6423
	private AnimationCurve materialEffectCurve;

	// Token: 0x04001918 RID: 6424
	private float materialEffectLerp;

	// Token: 0x04001919 RID: 6425
	public Sound hurtOther;

	// Token: 0x0400191A RID: 6426
	public Sound healOther;

	// Token: 0x0400191B RID: 6427
	public Sound upgradeOther;

	// Token: 0x0400191C RID: 6428
	private float overrideEyeMaterialLerp;

	// Token: 0x0400191D RID: 6429
	private float overrideEyeMaterialLerpPrevious;

	// Token: 0x0400191E RID: 6430
	private float overrideEyeMaterialTimer;

	// Token: 0x0400191F RID: 6431
	private Color overrideEyeMaterialColor;

	// Token: 0x04001920 RID: 6432
	private Color overridePupilMaterialColor;

	// Token: 0x04001921 RID: 6433
	private Color overrideEyeLightColor;

	// Token: 0x04001922 RID: 6434
	private float overrideEyeLightIntensity;

	// Token: 0x04001923 RID: 6435
	private bool overrideEyeActive;

	// Token: 0x04001924 RID: 6436
	private bool overrideEyeActivePrevious;

	// Token: 0x04001925 RID: 6437
	private int overrideEyePriority = -999;

	// Token: 0x04001926 RID: 6438
	private PlayerHealth.EyeOverrideState overrideEyeState;

	// Token: 0x04001927 RID: 6439
	private PlayerHealth.EyeOverrideState overrideEyeStatePrevious;

	// Token: 0x04001928 RID: 6440
	private float invincibleTimer;

	// Token: 0x02000375 RID: 885
	public enum Effect
	{
		// Token: 0x0400279C RID: 10140
		Upgrade
	}

	// Token: 0x02000376 RID: 886
	public enum EyeOverrideState
	{
		// Token: 0x0400279E RID: 10142
		None,
		// Token: 0x0400279F RID: 10143
		Red,
		// Token: 0x040027A0 RID: 10144
		Green,
		// Token: 0x040027A1 RID: 10145
		Love,
		// Token: 0x040027A2 RID: 10146
		CeilingEye
	}
}
