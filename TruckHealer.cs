using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000F5 RID: 245
public class TruckHealer : MonoBehaviour
{
	// Token: 0x060008A6 RID: 2214 RVA: 0x00052FF4 File Offset: 0x000511F4
	private void Start()
	{
		this.healSphereRenderer = this.healSphere.GetComponent<MeshRenderer>();
		this.zRotationHatch1Open = this.hatch1Transform.localEulerAngles.z;
		this.zRotationHatch2Open = this.hatch2Transform.localEulerAngles.z;
		this.hatch1Transform.localEulerAngles = new Vector3(0f, 0f, 0f);
		this.hatch2Transform.localEulerAngles = new Vector3(0f, 180f, 0f);
		this.lightIntensityOriginal = this.healerLight.intensity;
		this.healerLight.intensity = 0f;
		this.healerLight.enabled = false;
		this.healSphereSizeOriginal = this.healSphere.localScale.x;
		this.healSphere.localScale = new Vector3(0f, 0f, 0f);
		this.healSphere.gameObject.SetActive(false);
		this.healParticlesList.AddRange(this.healParticles.GetComponentsInChildren<ParticleSystem>());
		TruckHealer.instance = this;
	}

	// Token: 0x060008A7 RID: 2215 RVA: 0x0005310B File Offset: 0x0005130B
	private void StateClosed()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		if (!this.allHealingDone && RoundDirector.instance.allExtractionPointsCompleted)
		{
			this.StateUpdate(TruckHealer.State.Opening);
		}
	}

	// Token: 0x060008A8 RID: 2216 RVA: 0x00053138 File Offset: 0x00051338
	private void PlayHealParticles()
	{
		foreach (ParticleSystem particleSystem in this.healParticlesList)
		{
			particleSystem.Play();
		}
	}

	// Token: 0x060008A9 RID: 2217 RVA: 0x00053188 File Offset: 0x00051388
	private void StateOpening()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
			this.hatchAnimEval = 0f;
			this.healerLight.enabled = true;
			this.swirlParticles.Play();
			this.hatchOpenedEffect = false;
			this.healSphere.gameObject.SetActive(true);
			this.soundOpen.Play(this.healerBeamOrigin.position, 1f, 1f, 1f, 1f);
		}
		if (this.hatchAnimEval < 1f)
		{
			this.hatchAnimEval += Time.deltaTime * 2f;
			if (this.hatchAnimEval > 0.8f && !this.hatchOpenedEffect)
			{
				this.hatchOpenedEffect = true;
				SemiFunc.CameraShakeImpactDistance(base.transform.position, 4f, 0.1f, 6f, 15f);
				this.partSmokeCeiling.Play();
				this.PartSmokeCeilingPoof.Play();
				this.soundSlam.Play(this.healerBeamOrigin.position, 1f, 1f, 1f, 1f);
			}
			if (this.healerLight.intensity < this.lightIntensityOriginal - 0.01f)
			{
				this.healerLight.intensity = Mathf.Lerp(this.healerLight.intensity, this.lightIntensityOriginal, this.hatchCurve.Evaluate(this.hatchAnimEval));
			}
			if (this.healSphere.localScale.x < this.healSphereSizeOriginal - 0.01f)
			{
				this.healSphere.localScale = new Vector3(Mathf.Lerp(0f, this.healSphereSizeOriginal, this.hatchCurve.Evaluate(this.hatchAnimEval)), Mathf.Lerp(0f, this.healSphereSizeOriginal, this.hatchCurve.Evaluate(this.hatchAnimEval)), Mathf.Lerp(0f, this.healSphereSizeOriginal, this.hatchCurve.Evaluate(this.hatchAnimEval)));
			}
			else
			{
				this.healSphere.localScale = new Vector3(this.healSphereSizeOriginal, this.healSphereSizeOriginal, this.healSphereSizeOriginal);
			}
			this.hatch1Transform.localEulerAngles = new Vector3(0f, 0f, Mathf.Lerp(0f, this.zRotationHatch1Open, this.hatchCurve.Evaluate(this.hatchAnimEval)));
			this.hatch2Transform.localEulerAngles = new Vector3(0f, 180f, Mathf.Lerp(0f, this.zRotationHatch2Open, this.hatchCurve.Evaluate(this.hatchAnimEval)));
			return;
		}
		this.StateUpdate(TruckHealer.State.Open);
	}

	// Token: 0x060008AA RID: 2218 RVA: 0x00053430 File Offset: 0x00051630
	private void StateOpen()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		if (SemiFunc.FPSImpulse5())
		{
			List<PlayerAvatar> list = SemiFunc.PlayerGetAll();
			int count = list.Count;
			int num = 0;
			using (List<PlayerAvatar>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.finalHeal)
					{
						num++;
					}
				}
			}
			if (num >= count)
			{
				this.allHealingDone = true;
				this.StateUpdate(TruckHealer.State.Closing);
			}
		}
	}

	// Token: 0x060008AB RID: 2219 RVA: 0x000534B8 File Offset: 0x000516B8
	private void StateClosing()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
			this.swirlParticles.Stop();
			this.hatchClosedEffect = false;
			this.hatchAnimEval = 0f;
			this.soundClose.Play(this.healerBeamOrigin.position, 1f, 1f, 1f, 1f);
		}
		if (this.hatchAnimEval < 1f)
		{
			this.hatchAnimEval += Time.deltaTime * 2f;
			if (this.healerLight.intensity > 0.01f)
			{
				this.healerLight.intensity = Mathf.Lerp(this.healerLight.intensity, 0f, this.hatchCurve.Evaluate(this.hatchAnimEval));
			}
			else
			{
				this.healerLight.enabled = false;
			}
			if (this.hatchAnimEval > 0.8f && !this.hatchClosedEffect)
			{
				this.hatchClosedEffect = true;
				SemiFunc.CameraShakeImpactDistance(base.transform.position, 4f, 0.1f, 6f, 15f);
				this.partSmokeCeiling.Play();
				this.PartSmokeCeilingPoof.Play();
				this.soundSlam.Play(this.healerBeamOrigin.position, 1f, 1f, 1f, 1f);
			}
			if (this.healSphere.localScale.x > 0.01f)
			{
				this.healSphere.localScale = new Vector3(Mathf.Lerp(this.healSphereSizeOriginal, 0f, this.hatchCurve.Evaluate(this.hatchAnimEval)), Mathf.Lerp(this.healSphereSizeOriginal, 0f, this.hatchCurve.Evaluate(this.hatchAnimEval)), Mathf.Lerp(this.healSphereSizeOriginal, 0f, this.hatchCurve.Evaluate(this.hatchAnimEval)));
			}
			else
			{
				this.healSphere.localScale = new Vector3(0f, 0f, 0f);
				this.healSphere.gameObject.SetActive(false);
			}
			this.hatch1Transform.localEulerAngles = new Vector3(0f, 0f, Mathf.Lerp(this.zRotationHatch1Open, 0f, this.hatchCurve.Evaluate(this.hatchAnimEval)));
			this.hatch2Transform.localEulerAngles = new Vector3(0f, 180f, Mathf.Lerp(this.zRotationHatch2Open, 0f, this.hatchCurve.Evaluate(this.hatchAnimEval)));
			return;
		}
		this.StateUpdate(TruckHealer.State.Closed);
	}

	// Token: 0x060008AC RID: 2220 RVA: 0x00053750 File Offset: 0x00051950
	private void StateMachine()
	{
		switch (this.currentState)
		{
		case TruckHealer.State.Closed:
			this.StateClosed();
			return;
		case TruckHealer.State.Opening:
			this.StateOpening();
			return;
		case TruckHealer.State.Open:
			this.StateOpen();
			return;
		case TruckHealer.State.Closing:
			this.StateClosing();
			return;
		default:
			return;
		}
	}

	// Token: 0x060008AD RID: 2221 RVA: 0x00053798 File Offset: 0x00051998
	private void Update()
	{
		bool playing = this.currentState == TruckHealer.State.Open;
		this.soundLoop.PlayLoop(playing, 2f, 2f, 1f);
		this.StateMachine();
		this.ScrollSphereTexture();
	}

	// Token: 0x060008AE RID: 2222 RVA: 0x000537D6 File Offset: 0x000519D6
	private void StateUpdate(TruckHealer.State _newState)
	{
		if (this.currentState != _newState)
		{
			this.currentState = _newState;
			this.stateStart = true;
		}
	}

	// Token: 0x060008AF RID: 2223 RVA: 0x000537F0 File Offset: 0x000519F0
	private void ScrollSphereTexture()
	{
		if (!this.healSphereRenderer.gameObject.activeSelf)
		{
			return;
		}
		this.healSphereRenderer.material.mainTextureOffset = new Vector2(this.healSphereRenderer.material.mainTextureOffset.x, this.healSphereRenderer.material.mainTextureOffset.y + Time.deltaTime * 0.1f);
		this.healSpherePulseParent.localScale = new Vector3(1f + Mathf.Sin(Time.time * 5f) * 0.1f, 1f + Mathf.Sin(Time.time * 5f) * 0.1f, 1f + Mathf.Sin(Time.time * 5f) * 0.1f);
		this.healSpherePulseParent.localEulerAngles = new Vector3(0f, Time.time * 200f, 0f);
	}

	// Token: 0x060008B0 RID: 2224 RVA: 0x000538E4 File Offset: 0x00051AE4
	public void Heal(PlayerAvatar _playerAvatar)
	{
		if (this.currentState != TruckHealer.State.Open)
		{
			return;
		}
		TruckHealerLine component = Object.Instantiate<GameObject>(this.healerBeamPrefab, this.healerBeamOrigin.position, Quaternion.identity).GetComponent<TruckHealerLine>();
		if (!_playerAvatar.isLocal)
		{
			component.lineTarget = _playerAvatar.playerAvatarVisuals.attachPointTopHeadMiddle;
		}
		else
		{
			component.lineTarget = _playerAvatar.localCameraTransform;
		}
		this.PlayHealParticles();
	}

	// Token: 0x04000FB4 RID: 4020
	public Transform hatch1Transform;

	// Token: 0x04000FB5 RID: 4021
	public Transform hatch2Transform;

	// Token: 0x04000FB6 RID: 4022
	public Light healerLight;

	// Token: 0x04000FB7 RID: 4023
	public AnimationCurve hatchCurve;

	// Token: 0x04000FB8 RID: 4024
	public Transform healSphere;

	// Token: 0x04000FB9 RID: 4025
	public Transform healSpherePulseParent;

	// Token: 0x04000FBA RID: 4026
	public ParticleSystem swirlParticles;

	// Token: 0x04000FBB RID: 4027
	private MeshRenderer healSphereRenderer;

	// Token: 0x04000FBC RID: 4028
	private float hatchAnimEval;

	// Token: 0x04000FBD RID: 4029
	private float zRotationHatch1Open;

	// Token: 0x04000FBE RID: 4030
	private float zRotationHatch2Open;

	// Token: 0x04000FBF RID: 4031
	private float lightIntensityOriginal;

	// Token: 0x04000FC0 RID: 4032
	private float healSphereSizeOriginal;

	// Token: 0x04000FC1 RID: 4033
	private bool hatchClosedEffect;

	// Token: 0x04000FC2 RID: 4034
	private bool hatchOpenedEffect;

	// Token: 0x04000FC3 RID: 4035
	public ParticleSystem partSmokeCeiling;

	// Token: 0x04000FC4 RID: 4036
	public ParticleSystem PartSmokeCeilingPoof;

	// Token: 0x04000FC5 RID: 4037
	public Transform healParticles;

	// Token: 0x04000FC6 RID: 4038
	private List<ParticleSystem> healParticlesList = new List<ParticleSystem>();

	// Token: 0x04000FC7 RID: 4039
	public GameObject healerBeamPrefab;

	// Token: 0x04000FC8 RID: 4040
	public Transform healerBeamOrigin;

	// Token: 0x04000FC9 RID: 4041
	public Sound soundOpen;

	// Token: 0x04000FCA RID: 4042
	public Sound soundClose;

	// Token: 0x04000FCB RID: 4043
	public Sound soundSlam;

	// Token: 0x04000FCC RID: 4044
	public Sound soundLoop;

	// Token: 0x04000FCD RID: 4045
	private bool allHealingDone;

	// Token: 0x04000FCE RID: 4046
	public static TruckHealer instance;

	// Token: 0x04000FCF RID: 4047
	internal TruckHealer.State currentState;

	// Token: 0x04000FD0 RID: 4048
	private bool stateStart = true;

	// Token: 0x02000326 RID: 806
	public enum State
	{
		// Token: 0x0400261E RID: 9758
		Closed,
		// Token: 0x0400261F RID: 9759
		Opening,
		// Token: 0x04002620 RID: 9760
		Open,
		// Token: 0x04002621 RID: 9761
		Closing
	}
}
