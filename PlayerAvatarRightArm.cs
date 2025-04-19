using System;
using UnityEngine;

// Token: 0x020001AA RID: 426
public class PlayerAvatarRightArm : MonoBehaviour
{
	// Token: 0x06000E68 RID: 3688 RVA: 0x00081608 File Offset: 0x0007F808
	private void Start()
	{
		this.playerAvatarVisuals = base.GetComponent<PlayerAvatarVisuals>();
		this.grabberLightIntensity = this.grabberLight.intensity;
		if (!GameManager.Multiplayer() || (this.playerAvatar && this.playerAvatar.photonView.IsMine))
		{
			this.grabberTransform.gameObject.SetActive(false);
			base.enabled = false;
		}
	}

	// Token: 0x06000E69 RID: 3689 RVA: 0x00081670 File Offset: 0x0007F870
	private void Update()
	{
		if (this.playerAvatarVisuals.isMenuAvatar)
		{
			return;
		}
		this.deltaTime = this.playerAvatarVisuals.deltaTime;
		if (this.playerAvatar.playerHealth.hurtFreeze)
		{
			return;
		}
		if (this.mapToolController.Active && !this.playerAvatar.playerAvatarVisuals.animInCrawl)
		{
			this.SetPose(this.mapPose);
			this.HeadAnimate(true);
			this.AnimatePose();
		}
		else if (this.physGrabBeam.gameObject.activeSelf && (!this.mapToolController.Active || !this.playerAvatar.playerAvatarVisuals.animInCrawl))
		{
			this.SetPose(this.grabberPose);
			this.HeadAnimate(false);
			this.AnimatePose();
		}
		else
		{
			this.SetPose(this.basePose);
			this.HeadAnimate(false);
			this.AnimatePose();
		}
		this.GrabberLogic();
	}

	// Token: 0x06000E6A RID: 3690 RVA: 0x00081754 File Offset: 0x0007F954
	private void HeadAnimate(bool _active)
	{
		if (_active)
		{
			float num = this.playerAvatar.localCameraRotation.eulerAngles.x;
			if (num > 90f)
			{
				num -= 360f;
			}
			this.headRotation = Mathf.Lerp(this.headRotation, num * 0.5f, 20f * this.deltaTime);
			return;
		}
		this.headRotation = Mathf.Lerp(this.headRotation, 0f, 20f * this.deltaTime);
	}

	// Token: 0x06000E6B RID: 3691 RVA: 0x000817D4 File Offset: 0x0007F9D4
	private void AnimatePose()
	{
		if (this.poseLerp < 1f)
		{
			this.poseLerp += this.poseSpeed * this.deltaTime;
			this.poseCurrent = Vector3.LerpUnclamped(this.poseOld, this.poseNew, this.poseCurve.Evaluate(this.poseLerp));
		}
		this.rightArmTransform.localEulerAngles = new Vector3(this.poseCurrent.x, this.poseCurrent.y + this.headRotation, this.poseCurrent.z);
	}

	// Token: 0x06000E6C RID: 3692 RVA: 0x00081868 File Offset: 0x0007FA68
	private void SetPose(Vector3 _poseNew)
	{
		if (this.poseNew != _poseNew)
		{
			this.poseOld = this.poseCurrent;
			this.poseNew = _poseNew;
			this.poseLerp = 0f;
		}
	}

	// Token: 0x06000E6D RID: 3693 RVA: 0x00081898 File Offset: 0x0007FA98
	private void GrabberClawLogic()
	{
		if (this.playerAvatar.physGrabber.physGrabBeam.activeSelf)
		{
			if (this.grabberClawHidden)
			{
				this.grabberClawHidden = false;
				this.grabberClawParent.gameObject.SetActive(true);
			}
			this.grabberClawLerp = Mathf.Clamp01(this.grabberClawLerp + 3f * this.deltaTime);
		}
		else if (!this.grabberClawHidden)
		{
			if (this.grabberClawLerp <= 0f)
			{
				this.grabberClawHidden = true;
				this.grabberClawParent.gameObject.SetActive(false);
				this.grabberClawRotation = 0f;
			}
			this.grabberClawLerp = Mathf.Clamp01(this.grabberClawLerp - 3f * this.deltaTime);
		}
		if (!this.grabberClawHidden)
		{
			this.grabberClawChildLerp = Mathf.Clamp01(this.grabberClawChildLerp + 1f * this.deltaTime);
			if (this.grabberClawChildLerp >= 1f)
			{
				this.grabberClawChildLerp = 0f;
			}
			Vector3 euler = Vector3.LerpUnclamped(new Vector3(60f, 0f, 0f), new Vector3(80f, 0f, 0f), this.grabberClawChildCurve.Evaluate(this.grabberClawChildLerp));
			for (int i = 0; i < this.grabberClawChildren.Length; i++)
			{
				this.grabberClawChildren[i].localRotation = Quaternion.Euler(euler);
			}
			float num = Mathf.LerpUnclamped(500f, 200f, this.grabberClawChildCurve.Evaluate(this.grabberClawChildLerp));
			this.grabberClawRotation += num * this.deltaTime;
			if (this.grabberClawRotation > 360f)
			{
				this.grabberClawRotation -= 360f;
			}
			this.grabberClawParent.localScale = Vector3.one * this.grabberClawHideCurve.Evaluate(this.grabberClawLerp);
			this.grabberClawParent.localRotation = Quaternion.Euler(0f, 0f, this.grabberClawRotation);
			this.grabberOrb.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 1.2f, this.grabberClawChildCurve.Evaluate(this.grabberClawChildLerp));
			this.grabberLight.intensity = Mathf.Lerp(0f, this.grabberLightIntensity, this.grabberClawHideCurve.Evaluate(this.grabberClawLerp));
		}
	}

	// Token: 0x06000E6E RID: 3694 RVA: 0x00081AF4 File Offset: 0x0007FCF4
	private void GrabberLogic()
	{
		this.GrabberClawLogic();
		this.grabberTransform.position = this.grabberTransformTarget.position;
		this.grabberTransform.localScale = this.grabberTransformTarget.localScale;
		this.grabberTransform.rotation = SemiFunc.SpringQuaternionGet(this.grabberClawSpring, this.grabberTransformTarget.rotation, this.deltaTime);
		Quaternion targetRotation = Quaternion.identity;
		Quaternion localRotation = this.rightArmParentTransform.localRotation;
		if (this.physGrabBeam.gameObject.activeSelf && !this.mapToolController.Active)
		{
			Vector3 position = this.grabberAimTarget.position;
			this.grabberAimTarget.position = this.physGrabBeam.PhysGrabPointPuller.position;
			float b = 0f;
			if (this.grabberAimTarget.localPosition.x < -1f)
			{
				b = 1f;
			}
			this.grabberAimTarget.localPosition = new Vector3(Mathf.Max(this.grabberAimTarget.localPosition.x, 0.2f), this.grabberAimTarget.localPosition.y, Mathf.Max(this.grabberAimTarget.localPosition.z, b));
			this.grabberAimTarget.position = Vector3.Lerp(position, this.grabberAimTarget.position, 30f * this.deltaTime);
			this.rightArmParentTransform.LookAt(this.grabberAimTarget);
			targetRotation = this.rightArmParentTransform.localRotation;
		}
		this.rightArmParentTransform.localRotation = localRotation;
		this.rightArmParentTransform.localRotation = SemiFunc.SpringQuaternionGet(this.grabberSteerSpring, targetRotation, this.deltaTime);
		this.grabberReachDifferenceTimer += this.deltaTime;
		if (this.grabberReachDifferenceTimer > 1f)
		{
			this.grabberReachDifference = 0f;
			this.grabberReachDifferenceTimer = 0f;
		}
		this.grabberReachDifference += this.grabberReachPrevious - this.playerAvatar.physGrabber.pullerDistance;
		this.grabberReachPrevious = this.playerAvatar.physGrabber.pullerDistance;
		if (Mathf.Abs(this.grabberReachDifference) > 1f)
		{
			if (this.grabberReachDifference < 0f)
			{
				this.grabberReachTarget = 0.2f;
			}
			else
			{
				this.grabberReachTarget = -0.2f;
			}
			this.grabberReachTimer = 0.25f;
			this.grabberReachDifference = 0f;
		}
		else
		{
			this.grabberReachTimer -= this.deltaTime;
			if (this.grabberReachTimer <= 0f)
			{
				this.grabberReachTarget = 0f;
			}
		}
		float num = SemiFunc.SpringFloatGet(this.grabberReachSpring, this.grabberReachTarget, this.deltaTime);
		this.rightArmParentTransform.localScale = new Vector3(1f, 1f, 1f + num);
		if (this.playerAvatar.physGrabber.healing)
		{
			if (!this.grabberHealing)
			{
				this.grabberLight.color = this.grabberLightColorHeal;
				GameObject[] array = this.grabberOrbSpheres;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].GetComponent<Renderer>().material = this.grabberMaterialHeal;
				}
				this.grabberHealing = true;
				return;
			}
		}
		else if (this.grabberHealing)
		{
			this.grabberLight.color = this.grabberLightColor;
			GameObject[] array = this.grabberOrbSpheres;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].GetComponent<Renderer>().material = this.grabberMaterial;
			}
			this.grabberHealing = false;
		}
	}

	// Token: 0x04001789 RID: 6025
	public PlayerAvatar playerAvatar;

	// Token: 0x0400178A RID: 6026
	public Transform rightArmTransform;

	// Token: 0x0400178B RID: 6027
	public Transform rightArmParentTransform;

	// Token: 0x0400178C RID: 6028
	public MapToolController mapToolController;

	// Token: 0x0400178D RID: 6029
	public PhysGrabBeam physGrabBeam;

	// Token: 0x0400178E RID: 6030
	public AnimationCurve poseCurve;

	// Token: 0x0400178F RID: 6031
	public float poseSpeed;

	// Token: 0x04001790 RID: 6032
	private float poseLerp;

	// Token: 0x04001791 RID: 6033
	private Vector3 poseNew;

	// Token: 0x04001792 RID: 6034
	private Vector3 poseOld;

	// Token: 0x04001793 RID: 6035
	private Vector3 poseCurrent;

	// Token: 0x04001794 RID: 6036
	[Space]
	public Vector3 basePose;

	// Token: 0x04001795 RID: 6037
	public Vector3 mapPose;

	// Token: 0x04001796 RID: 6038
	public Vector3 grabberPose;

	// Token: 0x04001797 RID: 6039
	private float headRotation;

	// Token: 0x04001798 RID: 6040
	public Transform grabberTransform;

	// Token: 0x04001799 RID: 6041
	public Transform grabberTransformTarget;

	// Token: 0x0400179A RID: 6042
	[Space]
	public Material grabberMaterial;

	// Token: 0x0400179B RID: 6043
	public Material grabberMaterialHeal;

	// Token: 0x0400179C RID: 6044
	[Space]
	public Transform grabberOrb;

	// Token: 0x0400179D RID: 6045
	public GameObject[] grabberOrbSpheres;

	// Token: 0x0400179E RID: 6046
	[Space]
	public Light grabberLight;

	// Token: 0x0400179F RID: 6047
	public Color grabberLightColor;

	// Token: 0x040017A0 RID: 6048
	public Color grabberLightColorHeal;

	// Token: 0x040017A1 RID: 6049
	private float grabberLightIntensity;

	// Token: 0x040017A2 RID: 6050
	private bool grabberHealing;

	// Token: 0x040017A3 RID: 6051
	[Space]
	public Transform grabberAimTarget;

	// Token: 0x040017A4 RID: 6052
	[Space]
	public SpringQuaternion grabberSteerSpring;

	// Token: 0x040017A5 RID: 6053
	public SpringQuaternion grabberClawSpring;

	// Token: 0x040017A6 RID: 6054
	public SpringFloat grabberReachSpring;

	// Token: 0x040017A7 RID: 6055
	private float grabberReachTimer;

	// Token: 0x040017A8 RID: 6056
	private float grabberReachTarget;

	// Token: 0x040017A9 RID: 6057
	private float grabberReachPrevious;

	// Token: 0x040017AA RID: 6058
	private float grabberReachDifference;

	// Token: 0x040017AB RID: 6059
	private float grabberReachDifferenceTimer;

	// Token: 0x040017AC RID: 6060
	[Space]
	public Transform grabberClawParent;

	// Token: 0x040017AD RID: 6061
	public Transform[] grabberClawChildren;

	// Token: 0x040017AE RID: 6062
	public AnimationCurve grabberClawHideCurve;

	// Token: 0x040017AF RID: 6063
	public AnimationCurve grabberClawChildCurve;

	// Token: 0x040017B0 RID: 6064
	private float grabberClawLerp;

	// Token: 0x040017B1 RID: 6065
	private float grabberClawChildLerp;

	// Token: 0x040017B2 RID: 6066
	private bool grabberClawHidden;

	// Token: 0x040017B3 RID: 6067
	private float grabberClawRotation;

	// Token: 0x040017B4 RID: 6068
	private float deltaTime;

	// Token: 0x040017B5 RID: 6069
	private PlayerAvatarVisuals playerAvatarVisuals;
}
