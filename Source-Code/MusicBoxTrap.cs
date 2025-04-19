using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200022D RID: 557
public class MusicBoxTrap : Trap
{
	// Token: 0x060011D4 RID: 4564 RVA: 0x0009DEF4 File Offset: 0x0009C0F4
	protected override void Start()
	{
		base.Start();
		this.rb = base.GetComponent<Rigidbody>();
		this.physgrabObject = base.GetComponent<PhysGrabObject>();
		this.MusicBoxDancer.gameObject.SetActive(false);
		this.PedestalTransform.gameObject.SetActive(false);
		this.colliderLidCollision = this.colliderLid.GetComponent<CollisionFree>();
		this.colliderDancersCollision = this.colliderDancers.GetComponent<CollisionFree>();
	}

	// Token: 0x060011D5 RID: 4565 RVA: 0x0009DF64 File Offset: 0x0009C164
	protected override void Update()
	{
		base.Update();
		if (!this.trapTriggered && this.physgrabObject.grabbed)
		{
			this.trapStart = true;
		}
		if (this.trapStart && !this.MusicBoxOpenAnimationActive && !this.MusicBoxCloseAnimationActive)
		{
			this.MusicBoxStart();
		}
		this.MusicBoxMusic.PlayLoop(this.MusicBoxPlaying, 2f, 2f, 1f);
		if (this.openTheBox && !this.colliderDancersCollision.colliding && !this.colliderLidCollision.colliding)
		{
			if (GameManager.instance.gameMode == 0)
			{
				float musicTime = Random.Range(0f, this.MusicBoxMusic.Source.clip.length);
				this.OpenTheBox(musicTime);
			}
			else if (PhotonNetwork.IsMasterClient)
			{
				float num = Random.Range(0f, this.MusicBoxMusic.Source.clip.length);
				this.photonView.RPC("OpenTheBox", RpcTarget.All, new object[]
				{
					num
				});
			}
			this.openTheBox = false;
		}
		if (this.MusicBoxOpenAnimationActive)
		{
			this.MusicBoxPlaying = true;
			this.MusicBoxLidProgress += Time.deltaTime;
			float num2 = this.MusicBoxLidCurve.Evaluate(this.MusicBoxLidProgress / this.MusicBoxLidDuration);
			this.MusicBoxLid.localRotation = Quaternion.Euler(-90f + -num2 * 100f, 0f, 0f);
			float num3 = this.MusicBoxLidRattlerCurve.Evaluate(this.MusicBoxLidProgress / this.MusicBoxLidDuration);
			this.MusicBoxRattler.localRotation = Quaternion.Euler(0f, 0f, -num3 * 300f);
			this.PedestalTransform.localScale = new Vector3(1f, Mathf.Lerp(0.15f, 1f, num2), 1f);
			float num4 = Mathf.Lerp(0.5f, 3f, num2);
			this.MusicBoxDancer.localScale = new Vector3(num4, num4, num4);
			if (this.MusicBoxLidProgress >= this.MusicBoxLidDuration)
			{
				this.MusicBoxOpenAnimationActive = false;
				this.MusicBoxLidProgress = 0f;
			}
		}
		if (this.MusicBoxCloseAnimationActive)
		{
			this.MusicBoxLidProgress += Time.deltaTime;
			float num5 = 1f - this.MusicBoxLidCurve.Evaluate(this.MusicBoxLidProgress / this.MusicBoxLidDuration);
			this.MusicBoxLid.localRotation = Quaternion.Euler(-90f + -num5 * 100f, 0f, 0f);
			float num6 = this.MusicBoxLidRattlerCurve.Evaluate(this.MusicBoxLidProgress / this.MusicBoxLidDuration);
			this.MusicBoxRattler.localRotation = Quaternion.Euler(0f, 0f, -num6 * 300f);
			this.PedestalTransform.localScale = new Vector3(1f, Mathf.Lerp(0.15f, 1f, num5), 1f);
			float num7 = Mathf.Lerp(0.5f, 3f, num5);
			this.MusicBoxDancer.localScale = new Vector3(num7, num7, num7);
			if (this.MusicBoxLidProgress >= this.MusicBoxLidDuration)
			{
				this.MusicBoxCloseAnimationActive = false;
				this.MusicBoxLidProgress = 0f;
				this.MusicBoxPlaying = false;
				this.MusicBoxDancer.gameObject.SetActive(false);
				this.PedestalTransform.gameObject.SetActive(false);
			}
		}
		if (this.MusicBoxPlaying)
		{
			if (SemiFunc.IsMasterClientOrSingleplayer())
			{
				Quaternion turnX = Quaternion.Euler(0f, 180f, 0f);
				Quaternion turnY = Quaternion.Euler(0f, 0f, 0f);
				Quaternion identity = Quaternion.identity;
				bool flag = false;
				using (List<PhysGrabber>.Enumerator enumerator = this.physGrabObject.playerGrabbing.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.isRotating)
						{
							flag = true;
						}
					}
				}
				if (!flag)
				{
					this.physGrabObject.TurnXYZ(turnX, turnY, identity);
				}
			}
			if (PhysGrabber.instance && PhysGrabber.instance.grabbedObject == this.rb)
			{
				CameraAim.Instance.AimTargetSoftSet(this.physGrabObject.transform.position + CameraAim.Instance.transform.right * 100f, 0.01f, 1f, 1f, base.gameObject, 100);
				PhysGrabber.instance.OverrideGrabDistance(1f);
			}
			this.enemyInvestigate = true;
			this.MusicBoxDancer.Rotate(0f, 0f, 40f * Time.deltaTime);
			this.MusicBoxDancerSpin.Rotate(0f, 20f * Time.deltaTime, 0f);
			if (!this.MusicBoxOpenAnimationActive)
			{
				float num8 = Mathf.Sin(Time.time * 50f) * 0.1f + Mathf.Sin(Time.time * 20f) * 0.1f - Mathf.Sin(Time.time * 70f) * 0.1f;
				float num9 = Mathf.Sin(Time.time * 70f) * 0.1f + Mathf.Sin(Time.time * 10f) * 0.1f - Mathf.Sin(Time.time * 50f) * 0.1f;
				this.MusicBoxRattler.localRotation = Quaternion.Euler(-num9 * 5f, 0f, num8 * 5f);
			}
			if (!this.physgrabObject.grabbed && !this.MusicBoxOpenAnimationActive)
			{
				this.MusicBoxStop();
			}
		}
	}

	// Token: 0x060011D6 RID: 4566 RVA: 0x0009E520 File Offset: 0x0009C720
	public void MusicBoxStop()
	{
		this.MusicBoxPlaying = false;
		this.MusicBoxCloseAnimationActive = true;
		this.trapTriggered = false;
		this.trapStart = false;
		this.MusicBoxCloseSound.Play(this.physgrabObject.centerPoint, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 8f, base.transform.position, 0.1f);
		this.colliderDancers.GetComponent<BoxCollider>().isTrigger = true;
		this.colliderDancers.tag = "Untagged";
		this.colliderDancers.gameObject.layer = 13;
		this.colliderLid.GetComponent<BoxCollider>().isTrigger = true;
		this.colliderLid.tag = "Untagged";
		this.colliderLid.gameObject.layer = 13;
	}

	// Token: 0x060011D7 RID: 4567 RVA: 0x0009E608 File Offset: 0x0009C808
	public void MusicBoxStart()
	{
		if (!this.trapTriggered)
		{
			this.trapTriggered = true;
			this.openTheBox = true;
		}
	}

	// Token: 0x060011D8 RID: 4568 RVA: 0x0009E620 File Offset: 0x0009C820
	[PunRPC]
	private void OpenTheBox(float musicTime)
	{
		GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 8f, base.transform.position, 0.1f);
		this.MusicBoxOpenAnimationActive = true;
		this.MusicBoxOpenSound.Play(this.physgrabObject.centerPoint, 1f, 1f, 1f, 1f);
		this.MusicBoxDancer.gameObject.SetActive(true);
		this.PedestalTransform.gameObject.SetActive(true);
		this.openTheBox = false;
		this.colliderDancers.GetComponent<BoxCollider>().isTrigger = false;
		this.colliderDancers.tag = "Phys Grab Object";
		this.colliderDancers.gameObject.layer = 16;
		this.colliderLid.GetComponent<BoxCollider>().isTrigger = false;
		this.colliderLid.tag = "Phys Grab Object";
		this.colliderLid.gameObject.layer = 16;
		this.MusicBoxMusic.StartTimeOverride = musicTime;
	}

	// Token: 0x04001DFC RID: 7676
	public Transform colliderLid;

	// Token: 0x04001DFD RID: 7677
	public Transform colliderDancers;

	// Token: 0x04001DFE RID: 7678
	private PhysGrabObject physgrabObject;

	// Token: 0x04001DFF RID: 7679
	private CollisionFree colliderLidCollision;

	// Token: 0x04001E00 RID: 7680
	private CollisionFree colliderDancersCollision;

	// Token: 0x04001E01 RID: 7681
	public Transform MusicBoxRattler;

	// Token: 0x04001E02 RID: 7682
	public Transform MusicBoxDancerSpin;

	// Token: 0x04001E03 RID: 7683
	public Transform MusicBoxDancer;

	// Token: 0x04001E04 RID: 7684
	public Transform MusicBoxLid;

	// Token: 0x04001E05 RID: 7685
	public Transform PedestalTransform;

	// Token: 0x04001E06 RID: 7686
	[Space]
	public AnimationCurve MusicBoxLidCurve;

	// Token: 0x04001E07 RID: 7687
	public AnimationCurve MusicBoxLidRattlerCurve;

	// Token: 0x04001E08 RID: 7688
	[Space]
	[Header("Sounds")]
	public Sound MusicBoxOpenSound;

	// Token: 0x04001E09 RID: 7689
	public Sound MusicBoxCloseSound;

	// Token: 0x04001E0A RID: 7690
	public Sound MusicBoxMusic;

	// Token: 0x04001E0B RID: 7691
	private float MusicBoxLidDuration = 0.5f;

	// Token: 0x04001E0C RID: 7692
	private float MusicBoxLidProgress;

	// Token: 0x04001E0D RID: 7693
	private bool MusicBoxOpenAnimationActive;

	// Token: 0x04001E0E RID: 7694
	private bool MusicBoxCloseAnimationActive;

	// Token: 0x04001E0F RID: 7695
	private bool MusicBoxPlaying;

	// Token: 0x04001E10 RID: 7696
	private bool openTheBox;

	// Token: 0x04001E11 RID: 7697
	private Rigidbody rb;
}
