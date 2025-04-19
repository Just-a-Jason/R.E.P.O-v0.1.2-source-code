using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000288 RID: 648
public class ValuableWizardTimeGlass : MonoBehaviour
{
	// Token: 0x06001409 RID: 5129 RVA: 0x000B0140 File Offset: 0x000AE340
	private void StateActive()
	{
		if (this.stateStart)
		{
			this.particleSystemGlitter.Play();
			this.particleSystemSwirl.Play();
			this.stateStart = false;
			this.timeGlassLight.gameObject.SetActive(true);
		}
		if (!this.timeGlassLight.gameObject.activeSelf)
		{
			this.timeGlassLight.gameObject.SetActive(true);
		}
		if (this.particleSystemTransform.gameObject.activeSelf)
		{
			List<PhysGrabber> playerGrabbing = this.physGrabObject.playerGrabbing;
			if (playerGrabbing.Count > this.particleFocus)
			{
				PhysGrabber physGrabber = playerGrabbing[this.particleFocus];
				if (physGrabber)
				{
					Transform headLookAtTransform = physGrabber.playerAvatar.playerAvatarVisuals.headLookAtTransform;
					if (headLookAtTransform)
					{
						this.particleSystemTransform.LookAt(headLookAtTransform);
					}
					this.particleFocus++;
				}
				else
				{
					this.particleFocus = 0;
				}
			}
			else
			{
				this.particleFocus = 0;
			}
		}
		this.soundPitchLerp = Mathf.Lerp(this.soundPitchLerp, 1f, Time.deltaTime * 2f);
		this.timeGlassLight.intensity = Mathf.Lerp(this.timeGlassLight.intensity, 4f, Time.deltaTime * 2f);
		Color a = new Color(0.5f, 0f, 1f);
		this.timeGlassMaterial.material.SetColor("_EmissionColor", a * this.timeGlassLight.intensity);
		foreach (PhysGrabber physGrabber2 in this.physGrabObject.playerGrabbing)
		{
			if (physGrabber2 && !physGrabber2.isLocal)
			{
				physGrabber2.playerAvatar.voiceChat.OverridePitch(0.65f, 1f, 2f, 0.1f, true);
			}
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.physGrabObject.OverrideDrag(20f, 0.1f);
			this.physGrabObject.OverrideAngularDrag(40f, 0.1f);
			if (!this.physGrabObject.grabbed)
			{
				this.SetState(ValuableWizardTimeGlass.States.Idle);
			}
		}
		if (this.physGrabObject.grabbedLocal)
		{
			PlayerAvatar instance = PlayerAvatar.instance;
			if (instance.voiceChat)
			{
				instance.voiceChat.OverridePitch(0.65f, 1f, 2f, 0.1f, true);
			}
			instance.OverridePupilSize(3f, 4, 1f, 1f, 5f, 0.5f, 0.1f);
			PlayerController.instance.OverrideSpeed(0.5f, 0.1f);
			PlayerController.instance.OverrideLookSpeed(0.5f, 2f, 1f, 0.1f);
			PlayerController.instance.OverrideAnimationSpeed(0.2f, 1f, 2f, 0.1f);
			PlayerController.instance.OverrideTimeScale(0.1f, 0.1f);
			this.physGrabObject.OverrideTorqueStrength(0.6f, 0.1f);
			CameraZoom.Instance.OverrideZoomSet(50f, 0.1f, 0.5f, 1f, base.gameObject, 0);
			PostProcessing.Instance.SaturationOverride(50f, 0.1f, 0.5f, 0.1f, base.gameObject);
		}
	}

	// Token: 0x0600140A RID: 5130 RVA: 0x000B04A8 File Offset: 0x000AE6A8
	private void StateIdle()
	{
		if (this.stateStart)
		{
			this.particleSystemGlitter.Stop();
			this.particleSystemSwirl.Stop();
			this.stateStart = false;
		}
		if (SemiFunc.IsMasterClientOrSingleplayer() && this.physGrabObject.grabbed)
		{
			this.SetState(ValuableWizardTimeGlass.States.Active);
		}
		this.timeGlassLight.intensity = Mathf.Lerp(this.timeGlassLight.intensity, 0f, Time.deltaTime * 10f);
		this.soundPitchLerp = Mathf.Lerp(this.soundPitchLerp, 0f, Time.deltaTime * 10f);
		Color a = new Color(0.5f, 0f, 1f);
		this.timeGlassMaterial.material.SetColor("_EmissionColor", a * this.timeGlassLight.intensity);
		if (this.timeGlassLight.intensity < 0.01f)
		{
			this.timeGlassLight.gameObject.SetActive(false);
		}
	}

	// Token: 0x0600140B RID: 5131 RVA: 0x000B05A0 File Offset: 0x000AE7A0
	[PunRPC]
	public void SetStateRPC(ValuableWizardTimeGlass.States state)
	{
		this.currentState = state;
		this.stateStart = true;
	}

	// Token: 0x0600140C RID: 5132 RVA: 0x000B05B0 File Offset: 0x000AE7B0
	private void SetState(ValuableWizardTimeGlass.States state)
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (!SemiFunc.IsMultiplayer())
		{
			this.SetStateRPC(state);
			return;
		}
		this.photonView.RPC("SetStateRPC", RpcTarget.All, new object[]
		{
			state
		});
	}

	// Token: 0x0600140D RID: 5133 RVA: 0x000B05E9 File Offset: 0x000AE7E9
	private void Start()
	{
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x0600140E RID: 5134 RVA: 0x000B0604 File Offset: 0x000AE804
	private void Update()
	{
		float pitchMultiplier = Mathf.Lerp(2f, 0.5f, this.soundPitchLerp);
		this.soundTimeGlassLoop.PlayLoop(this.currentState == ValuableWizardTimeGlass.States.Active, 0.8f, 0.8f, pitchMultiplier);
		ValuableWizardTimeGlass.States states = this.currentState;
		if (states != ValuableWizardTimeGlass.States.Idle)
		{
			if (states == ValuableWizardTimeGlass.States.Active)
			{
				this.StateActive();
				return;
			}
		}
		else
		{
			this.StateIdle();
		}
	}

	// Token: 0x0400222B RID: 8747
	private PhysGrabObject physGrabObject;

	// Token: 0x0400222C RID: 8748
	private PhotonView photonView;

	// Token: 0x0400222D RID: 8749
	internal ValuableWizardTimeGlass.States currentState;

	// Token: 0x0400222E RID: 8750
	private bool stateStart;

	// Token: 0x0400222F RID: 8751
	public Transform particleSystemTransform;

	// Token: 0x04002230 RID: 8752
	public ParticleSystem particleSystemSwirl;

	// Token: 0x04002231 RID: 8753
	public ParticleSystem particleSystemGlitter;

	// Token: 0x04002232 RID: 8754
	public MeshRenderer timeGlassMaterial;

	// Token: 0x04002233 RID: 8755
	[FormerlySerializedAs("light")]
	public Light timeGlassLight;

	// Token: 0x04002234 RID: 8756
	public Sound soundTimeGlassLoop;

	// Token: 0x04002235 RID: 8757
	private float soundPitchLerp;

	// Token: 0x04002236 RID: 8758
	private int particleFocus;

	// Token: 0x020003C4 RID: 964
	public enum States
	{
		// Token: 0x040028E6 RID: 10470
		Idle,
		// Token: 0x040028E7 RID: 10471
		Active
	}
}
