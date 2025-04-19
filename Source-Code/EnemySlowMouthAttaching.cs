using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000075 RID: 117
public class EnemySlowMouthAttaching : MonoBehaviour
{
	// Token: 0x0600046A RID: 1130 RVA: 0x0002BA94 File Offset: 0x00029C94
	private void Start()
	{
		this.springQuaternion = new SpringQuaternion();
		this.springQuaternion.damping = 0.5f;
		this.springQuaternion.speed = 20f;
		this.springFloatScale = new SpringFloat();
		this.springFloatScale.damping = 0.5f;
		this.springFloatScale.speed = 20f;
		this.startRotationTopJaw = this.topJaw.localRotation;
		this.startRotationBottomJaw = this.bottomJaw.localRotation;
		this.startPosition = base.transform.position;
		this.particleSystems = new List<ParticleSystem>(this.particleTransform.GetComponentsInChildren<ParticleSystem>());
		this.GoTime();
	}

	// Token: 0x0600046B RID: 1131 RVA: 0x0002BB48 File Offset: 0x00029D48
	public void GoTime()
	{
		this.PlayParticles(false);
		if (this.targetPlayerAvatar.isLocal)
		{
			CameraGlitch.Instance.PlayLong();
		}
		this.SetTarget(this.targetPlayerAvatar);
		this.springFloatScale.springVelocity = 50f;
		this.isActive = true;
		this.targetScale = 1f;
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 8f, base.transform.position, 0.1f);
		GameDirector.instance.CameraImpact.ShakeDistance(2f, 3f, 8f, base.transform.position, 0.1f);
		this.soundAttachVO.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600046C RID: 1132 RVA: 0x0002BC28 File Offset: 0x00029E28
	private void PlayParticles(bool finalPlay)
	{
		foreach (ParticleSystem particleSystem in this.particleSystems)
		{
			particleSystem.Play();
			if (finalPlay)
			{
				particleSystem.transform.parent = null;
				Object.Destroy(particleSystem.gameObject, 4f);
			}
		}
	}

	// Token: 0x0600046D RID: 1133 RVA: 0x0002BC9C File Offset: 0x00029E9C
	private void SpawnPlayerJaw()
	{
		if (!this.targetPlayerAvatar.isLocal)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.topJawPrefab, base.transform.position, base.transform.rotation);
			GameObject gameObject2 = Object.Instantiate<GameObject>(this.bottomJawPrefab, base.transform.position, base.transform.rotation);
			EnemySlowMouthPlayerAvatarAttached component = gameObject.GetComponent<EnemySlowMouthPlayerAvatarAttached>();
			component.jawBot = gameObject2.transform;
			Transform attachPointJawTop = this.targetPlayerAvatar.playerAvatarVisuals.attachPointJawTop;
			Transform attachPointJawBottom = this.targetPlayerAvatar.playerAvatarVisuals.attachPointJawBottom;
			gameObject.transform.parent = attachPointJawTop;
			component.playerTarget = this.targetPlayerAvatar;
			component.enemySlowMouth = this.enemySlowMouth;
			component.semiPuke = gameObject2.GetComponentInChildren<SemiPuke>();
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.rotation = Quaternion.identity;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject2.transform.parent = attachPointJawBottom;
			gameObject2.transform.localPosition = Vector3.zero;
			gameObject2.transform.rotation = Quaternion.identity;
			gameObject2.transform.localRotation = Quaternion.identity;
			return;
		}
		Transform localCameraTransform = this.targetPlayerAvatar.localCameraTransform;
		GameObject gameObject3 = Object.Instantiate<GameObject>(this.localPlayerJaw, base.transform.position, Quaternion.identity, localCameraTransform);
		gameObject3.transform.localPosition = Vector3.zero;
		gameObject3.transform.localRotation = Quaternion.identity;
		EnemySlowMouthCameraVisuals component2 = gameObject3.GetComponent<EnemySlowMouthCameraVisuals>();
		component2.enemySlowMouth = this.enemySlowMouth;
		component2.playerTarget = this.targetPlayerAvatar;
	}

	// Token: 0x0600046E RID: 1134 RVA: 0x0002BE34 File Offset: 0x0002A034
	private void Update()
	{
		bool flag = !this.targetTransform || !this.targetPlayerAvatar;
		if (!this.isActive)
		{
			return;
		}
		if (flag)
		{
			this.Detach();
			return;
		}
		Quaternion targetRotation = Quaternion.LookRotation(this.targetTransform.position - base.transform.position);
		base.transform.rotation = SemiFunc.SpringQuaternionGet(this.springQuaternion, targetRotation, -1f);
		float d = SemiFunc.SpringFloatGet(this.springFloatScale, this.targetScale, -1f);
		base.transform.localScale = Vector3.one * d;
		float num = Vector3.Distance(base.transform.position, this.targetTransform.position);
		float num2 = num * 2f;
		if (num2 < 4f)
		{
			num2 = 4f;
		}
		if (num2 > 10f)
		{
			num2 = 10f;
		}
		base.transform.position = Vector3.MoveTowards(base.transform.position, this.targetTransform.position, Time.deltaTime * num2);
		if (num < 1f)
		{
			this.targetScale = 2.5f;
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (this.enemySlowMouth.currentState != EnemySlowMouth.State.Attack)
			{
				if (!this.targetPlayerAvatar.isDisabled && !this.enemySlowMouth.IsPossessed())
				{
					this.AttachToPlayer();
				}
				else
				{
					this.Detach();
				}
			}
			if (this.targetPlayerAvatar.isLocal)
			{
				GameDirector.instance.CameraShake.Shake(4f, 0.1f);
			}
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (!this.targetPlayerAvatar.isDisabled && !this.enemySlowMouth.IsPossessed())
		{
			if (num < 0.1f)
			{
				this.AttachToPlayer();
				this.enemySlowMouth.UpdateState(EnemySlowMouth.State.Attached);
				this.isActive = false;
			}
		}
		else
		{
			this.Detach();
		}
		if (this.targetPlayerAvatar.isLocal)
		{
			CameraAim.Instance.AimTargetSet(base.transform.position, 0.2f, 20f, base.gameObject, 100);
			CameraZoom.Instance.OverrideZoomSet(30f, 0.1f, 8f, 1f, base.gameObject, 50);
		}
		this.tentaclesTransform.localScale = new Vector3(1f + Mathf.Sin(Time.time * 40f) * 0.2f, 1f + Mathf.Sin(Time.time * 60f) * 0.1f, 1f);
		this.tentaclesTransform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(Time.time * 20f) * 10f);
		this.topJaw.localRotation = this.startRotationTopJaw * Quaternion.Euler(Mathf.Sin(Time.time * 60f) * 3f, 0f, 0f);
		this.bottomJaw.localRotation = this.startRotationBottomJaw * Quaternion.Euler(Mathf.Sin(Time.time * 60f) * 10f, 0f, 0f);
		foreach (Transform transform in this.eyeTransforms)
		{
			transform.localScale = new Vector3(1.5f + Mathf.Sin(Time.time * 40f) * 0.5f, 1.5f + Mathf.Sin(Time.time * 60f) * 0.5f, 1.5f);
		}
	}

	// Token: 0x0600046F RID: 1135 RVA: 0x0002C1E8 File Offset: 0x0002A3E8
	private void AttachToPlayer()
	{
		if (this.targetPlayerAvatar.isDisabled || this.targetPlayerAvatar.GetComponentInChildren<EnemySlowMouthPlayerAvatarAttached>())
		{
			return;
		}
		this.SpawnPlayerJaw();
		this.Despawn();
	}

	// Token: 0x06000470 RID: 1136 RVA: 0x0002C218 File Offset: 0x0002A418
	private void Detach()
	{
		this.soundAttach.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.PlayParticles(true);
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.enemySlowMouth.detachPosition = base.transform.position;
			this.enemySlowMouth.detachRotation = base.transform.rotation;
			this.enemySlowMouth.UpdateState(EnemySlowMouth.State.Detach);
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x06000471 RID: 1137 RVA: 0x0002C2A4 File Offset: 0x0002A4A4
	private void Despawn()
	{
		this.soundAttach.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.PlayParticles(true);
		if (this.targetPlayerAvatar.isLocal)
		{
			GameDirector.instance.CameraImpact.Shake(8f, 0.1f);
			GameDirector.instance.CameraShake.Shake(5f, 0.1f);
			CameraGlitch.Instance.PlayLong();
		}
		else
		{
			GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 8f, base.transform.position, 0.1f);
			GameDirector.instance.CameraImpact.ShakeDistance(2f, 3f, 8f, base.transform.position, 0.1f);
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x06000472 RID: 1138 RVA: 0x0002C395 File Offset: 0x0002A595
	public void SetTarget(PlayerAvatar _playerAvatar)
	{
		this.targetTransform = SemiFunc.PlayerGetFaceEyeTransform(_playerAvatar);
		this.targetPlayerAvatar = _playerAvatar;
	}

	// Token: 0x0400072B RID: 1835
	internal EnemySlowMouth enemySlowMouth;

	// Token: 0x0400072C RID: 1836
	public Transform tentaclesTransform;

	// Token: 0x0400072D RID: 1837
	public List<Transform> eyeTransforms = new List<Transform>();

	// Token: 0x0400072E RID: 1838
	public Transform topJaw;

	// Token: 0x0400072F RID: 1839
	public Transform bottomJaw;

	// Token: 0x04000730 RID: 1840
	public Transform particleTransform;

	// Token: 0x04000731 RID: 1841
	private List<ParticleSystem> particleSystems = new List<ParticleSystem>();

	// Token: 0x04000732 RID: 1842
	private Quaternion startRotationTopJaw;

	// Token: 0x04000733 RID: 1843
	private Quaternion startRotationBottomJaw;

	// Token: 0x04000734 RID: 1844
	internal Transform targetTransform;

	// Token: 0x04000735 RID: 1845
	public PlayerAvatar targetPlayerAvatar;

	// Token: 0x04000736 RID: 1846
	private SpringQuaternion springQuaternion;

	// Token: 0x04000737 RID: 1847
	private bool isActive;

	// Token: 0x04000738 RID: 1848
	private Vector3 startPosition;

	// Token: 0x04000739 RID: 1849
	private SpringFloat springFloatScale;

	// Token: 0x0400073A RID: 1850
	private float targetScale = 1f;

	// Token: 0x0400073B RID: 1851
	public GameObject topJawPrefab;

	// Token: 0x0400073C RID: 1852
	public GameObject bottomJawPrefab;

	// Token: 0x0400073D RID: 1853
	public GameObject localPlayerJaw;

	// Token: 0x0400073E RID: 1854
	[Space(20f)]
	public Sound soundAttachVO;

	// Token: 0x0400073F RID: 1855
	public Sound soundAttach;
}
