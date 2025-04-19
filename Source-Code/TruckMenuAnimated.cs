using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000EB RID: 235
public class TruckMenuAnimated : MonoBehaviour
{
	// Token: 0x06000835 RID: 2101 RVA: 0x0004F78C File Offset: 0x0004D98C
	private void Start()
	{
		this.animator = base.GetComponent<Animator>();
		this.animator.keepAnimatorStateOnDisable = true;
		this.photonView = base.GetComponent<PhotonView>();
		this.breakerCooldown = Random.Range(this.breakerCooldownMin, this.breakerCooldownMax);
		this.breakerTriggers.Add("SpeedUp");
		this.breakerTriggers.Add("SlowDown");
		this.breakerTriggers.Add("Swerve");
		this.breakerTriggers.Add("SkeletonHit");
		this.breakerTriggers.Add("TruckPass");
		this.breakerTriggers.Shuffle<string>();
	}

	// Token: 0x06000836 RID: 2102 RVA: 0x0004F830 File Offset: 0x0004DA30
	private void Update()
	{
		this.antennaMiddleTransform.rotation = SemiFunc.SpringQuaternionGet(this.antennaMiddleSpring, this.antennaMiddleTarget.rotation, -1f);
		this.frontPanelTransform.rotation = SemiFunc.SpringQuaternionGet(this.frontPanelSpring, this.frontPanelTarget.rotation, -1f);
		this.frontPanelTransform.localEulerAngles = new Vector3(0f, this.frontPanelTransform.localEulerAngles.y, 0f);
		this.windowRightTransform.rotation = SemiFunc.SpringQuaternionGet(this.windowRightSpring, this.windowRightTarget.rotation, -1f);
		this.windowRightTransform.localEulerAngles = new Vector3(this.windowRightTransform.localEulerAngles.x, 0f, 0f);
		this.windowLeftTransform.rotation = SemiFunc.SpringQuaternionGet(this.windowLeftSpring, this.windowLeftTarget.rotation, -1f);
		this.windowLeftTransform.localEulerAngles = new Vector3(this.windowLeftTransform.localEulerAngles.x, 0f, 0f);
		this.dishTransform.rotation = SemiFunc.SpringQuaternionGet(this.dishSpring, this.dishTarget.rotation, -1f);
		this.antennaBackTransform.rotation = SemiFunc.SpringQuaternionGet(this.antennaBackSpring, this.antennaBackTarget.rotation, -1f);
		if (SemiFunc.IsMasterClientOrSingleplayer() && this.animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
		{
			this.breakerCooldown -= Time.deltaTime;
			if (this.breakerCooldown <= 0f)
			{
				this.BreakerTrigger();
			}
		}
		this.soundLoop.PlayLoop(true, 2f, 2f, 1f);
	}

	// Token: 0x06000837 RID: 2103 RVA: 0x0004FA04 File Offset: 0x0004DC04
	private void BreakerTrigger()
	{
		this.breakerCooldown = Random.Range(this.breakerCooldownMin, this.breakerCooldownMax);
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("BreakerTriggerRPC", RpcTarget.All, new object[]
			{
				this.breakerTriggers[this.breakerTriggerIndex]
			});
		}
		else
		{
			this.BreakerTriggerRPC(this.breakerTriggers[this.breakerTriggerIndex]);
		}
		this.breakerTriggerIndex++;
		if (this.breakerTriggerIndex >= this.breakerTriggers.Count)
		{
			string b = this.breakerTriggers[this.breakerTriggers.Count - 1];
			this.breakerTriggers.Shuffle<string>();
			while (this.breakerTriggers[0] == b)
			{
				this.breakerTriggers.Shuffle<string>();
			}
			this.breakerTriggerIndex = 0;
		}
	}

	// Token: 0x06000838 RID: 2104 RVA: 0x0004FAE0 File Offset: 0x0004DCE0
	[PunRPC]
	private void BreakerTriggerRPC(string _triggerName)
	{
		if (!this.animator)
		{
			return;
		}
		this.animator.SetTrigger(_triggerName);
	}

	// Token: 0x06000839 RID: 2105 RVA: 0x0004FAFC File Offset: 0x0004DCFC
	public void SkeletonHitFirstImpulse()
	{
		this.soundSkeletonHit.Play(this.soundLoop.Source.transform.position, 1f, 1f, 1f, 1f);
		this.particleSkeletonBitsFirst.Play();
		this.particleSkeletonSmokeFirst.Play();
		GameDirector.instance.CameraShake.Shake(1f, 0.3f);
		GameDirector.instance.CameraImpact.Shake(0.5f, 0.1f);
	}

	// Token: 0x0600083A RID: 2106 RVA: 0x0004FB88 File Offset: 0x0004DD88
	public void SkeletonHitLastImpulse()
	{
		this.soundSkeletonHitSkull.Play(this.soundLoop.Source.transform.position, 1f, 1f, 1f, 1f);
		this.particleSkeletonBitsLast.Play();
		this.particleSkeletonSmokeLast.Play();
	}

	// Token: 0x0600083B RID: 2107 RVA: 0x0004FBE0 File Offset: 0x0004DDE0
	public void PlaySwerve()
	{
		this.soundSwerve.Play(this.soundLoop.Source.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600083C RID: 2108 RVA: 0x0004FC17 File Offset: 0x0004DE17
	public void PlaySpeedUp()
	{
		this.soundSpeedUp.Play(this.soundLoop.Source.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600083D RID: 2109 RVA: 0x0004FC4E File Offset: 0x0004DE4E
	public void PlaySlowDown()
	{
		this.soundSlowDown.Play(this.soundLoop.Source.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600083E RID: 2110 RVA: 0x0004FC85 File Offset: 0x0004DE85
	public void PlayBodyRustleLong01()
	{
		this.soundBodyRustleLong01.Play(this.soundLoop.Source.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600083F RID: 2111 RVA: 0x0004FCBC File Offset: 0x0004DEBC
	public void PlayBodyRustleLong02()
	{
		this.soundBodyRustleLong02.Play(this.soundLoop.Source.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000840 RID: 2112 RVA: 0x0004FCF3 File Offset: 0x0004DEF3
	public void PlayBodyRustleLong03()
	{
		this.soundBodyRustleLong03.Play(this.soundLoop.Source.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000841 RID: 2113 RVA: 0x0004FD2A File Offset: 0x0004DF2A
	public void PlayBodyRustleShort01()
	{
		this.soundBodyRustleShort01.Play(this.soundLoop.Source.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000842 RID: 2114 RVA: 0x0004FD61 File Offset: 0x0004DF61
	public void PlayBodyRustleShort02()
	{
		this.soundBodyRustleShort02.Play(this.soundLoop.Source.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000843 RID: 2115 RVA: 0x0004FD98 File Offset: 0x0004DF98
	public void PlayBodyRustleShort03()
	{
		this.soundBodyRustleShort03.Play(this.soundLoop.Source.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000844 RID: 2116 RVA: 0x0004FDCF File Offset: 0x0004DFCF
	public void PlaySwerveFast01()
	{
		this.soundSwerveFast01.Play(this.soundLoop.Source.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000845 RID: 2117 RVA: 0x0004FE06 File Offset: 0x0004E006
	public void PlaySwerveFast02()
	{
		this.soundSwerveFast02.Play(this.soundLoop.Source.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000846 RID: 2118 RVA: 0x0004FE3D File Offset: 0x0004E03D
	public void PlayFirePass()
	{
		this.soundFirePass.Play(this.soundLoop.Source.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000847 RID: 2119 RVA: 0x0004FE74 File Offset: 0x0004E074
	public void PlayFirePassSwerve01()
	{
		this.soundFirePassSwerve01.Play(this.soundLoop.Source.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000848 RID: 2120 RVA: 0x0004FEAB File Offset: 0x0004E0AB
	public void PlayFirePassSwerve02()
	{
		this.soundFirePassSwerve02.Play(this.soundLoop.Source.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000849 RID: 2121 RVA: 0x0004FEE4 File Offset: 0x0004E0E4
	private void OnDrawGizmos()
	{
		Gizmos.color = new Color(1f, 1f, 1f, 0.5f);
		Gizmos.matrix = this.antennaMiddleTarget.localToWorldMatrix;
		Gizmos.DrawLine(Vector3.zero, Vector3.up * 6f);
		Gizmos.matrix = this.frontPanelTarget.localToWorldMatrix;
		Gizmos.DrawLine(Vector3.zero, Vector3.right * 1.5f);
		Gizmos.matrix = this.windowRightTarget.localToWorldMatrix;
		Gizmos.DrawLine(Vector3.zero, Vector3.up * -2.5f);
		Gizmos.matrix = this.windowLeftTarget.localToWorldMatrix;
		Gizmos.DrawLine(Vector3.zero, Vector3.up * -2.5f);
		Gizmos.matrix = this.dishTarget.localToWorldMatrix;
		Gizmos.DrawLine(Vector3.zero, Vector3.up * 4f);
		Gizmos.matrix = this.antennaBackTarget.localToWorldMatrix;
		Gizmos.DrawLine(Vector3.zero, Vector3.up * 8f);
	}

	// Token: 0x04000F18 RID: 3864
	public Transform antennaMiddleTransform;

	// Token: 0x04000F19 RID: 3865
	public Transform antennaMiddleTarget;

	// Token: 0x04000F1A RID: 3866
	public SpringQuaternion antennaMiddleSpring;

	// Token: 0x04000F1B RID: 3867
	[Space(20f)]
	public Transform frontPanelTransform;

	// Token: 0x04000F1C RID: 3868
	public Transform frontPanelTarget;

	// Token: 0x04000F1D RID: 3869
	public SpringQuaternion frontPanelSpring;

	// Token: 0x04000F1E RID: 3870
	[Space(20f)]
	public Transform windowRightTransform;

	// Token: 0x04000F1F RID: 3871
	public Transform windowRightTarget;

	// Token: 0x04000F20 RID: 3872
	public SpringQuaternion windowRightSpring;

	// Token: 0x04000F21 RID: 3873
	[Space(20f)]
	public Transform windowLeftTransform;

	// Token: 0x04000F22 RID: 3874
	public Transform windowLeftTarget;

	// Token: 0x04000F23 RID: 3875
	public SpringQuaternion windowLeftSpring;

	// Token: 0x04000F24 RID: 3876
	[Space(20f)]
	public Transform dishTransform;

	// Token: 0x04000F25 RID: 3877
	public Transform dishTarget;

	// Token: 0x04000F26 RID: 3878
	public SpringQuaternion dishSpring;

	// Token: 0x04000F27 RID: 3879
	[Space(20f)]
	public Transform antennaBackTransform;

	// Token: 0x04000F28 RID: 3880
	public Transform antennaBackTarget;

	// Token: 0x04000F29 RID: 3881
	public SpringQuaternion antennaBackSpring;

	// Token: 0x04000F2A RID: 3882
	private Animator animator;

	// Token: 0x04000F2B RID: 3883
	private PhotonView photonView;

	// Token: 0x04000F2C RID: 3884
	private float breakerCooldown;

	// Token: 0x04000F2D RID: 3885
	private float breakerCooldownMin = 8f;

	// Token: 0x04000F2E RID: 3886
	private float breakerCooldownMax = 16f;

	// Token: 0x04000F2F RID: 3887
	private List<string> breakerTriggers = new List<string>();

	// Token: 0x04000F30 RID: 3888
	private int breakerTriggerIndex;

	// Token: 0x04000F31 RID: 3889
	public ParticleSystem particleSkeletonBitsFirst;

	// Token: 0x04000F32 RID: 3890
	public ParticleSystem particleSkeletonSmokeFirst;

	// Token: 0x04000F33 RID: 3891
	public ParticleSystem particleSkeletonBitsLast;

	// Token: 0x04000F34 RID: 3892
	public ParticleSystem particleSkeletonSmokeLast;

	// Token: 0x04000F35 RID: 3893
	public Sound soundLoop;

	// Token: 0x04000F36 RID: 3894
	[Space]
	public Sound soundSwerve;

	// Token: 0x04000F37 RID: 3895
	public Sound soundSpeedUp;

	// Token: 0x04000F38 RID: 3896
	public Sound soundSlowDown;

	// Token: 0x04000F39 RID: 3897
	[Space]
	public Sound soundBodyRustleLong01;

	// Token: 0x04000F3A RID: 3898
	public Sound soundBodyRustleLong02;

	// Token: 0x04000F3B RID: 3899
	public Sound soundBodyRustleLong03;

	// Token: 0x04000F3C RID: 3900
	[Space]
	public Sound soundBodyRustleShort01;

	// Token: 0x04000F3D RID: 3901
	public Sound soundBodyRustleShort02;

	// Token: 0x04000F3E RID: 3902
	public Sound soundBodyRustleShort03;

	// Token: 0x04000F3F RID: 3903
	[Space]
	public Sound soundSkeletonHit;

	// Token: 0x04000F40 RID: 3904
	public Sound soundSkeletonHitSkull;

	// Token: 0x04000F41 RID: 3905
	[Space]
	public Sound soundSwerveFast01;

	// Token: 0x04000F42 RID: 3906
	public Sound soundSwerveFast02;

	// Token: 0x04000F43 RID: 3907
	[Space]
	public Sound soundFirePass;

	// Token: 0x04000F44 RID: 3908
	public Sound soundFirePassSwerve01;

	// Token: 0x04000F45 RID: 3909
	public Sound soundFirePassSwerve02;
}
