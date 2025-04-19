using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000159 RID: 345
public class ItemMine : MonoBehaviour
{
	// Token: 0x06000B8A RID: 2954 RVA: 0x000665F4 File Offset: 0x000647F4
	private void Start()
	{
		this.triggerSpringQuaternion = new SpringQuaternion();
		this.triggerSpringQuaternion.damping = 0.2f;
		this.triggerSpringQuaternion.speed = 10f;
		this.itemAttributes = base.GetComponent<ItemAttributes>();
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.photonView = base.GetComponent<PhotonView>();
		this.lightArmed.color = this.emissionColor;
		this.meshRenderer.material.SetColor("_EmissionColor", this.emissionColor);
		this.initialLightIntensity = this.lightArmed.intensity;
		this.impactDetector = base.GetComponent<PhysGrabObjectImpactDetector>();
		this.itemMineTrigger = base.GetComponentInChildren<ItemMineTrigger>();
		this.particleScriptExplosion = base.GetComponent<ParticleScriptExplosion>();
		this.startPosition = base.transform.position;
		this.itemEquippable = base.GetComponent<ItemEquippable>();
		this.startRotation = base.transform.rotation;
		this.itemToggle = base.GetComponent<ItemToggle>();
	}

	// Token: 0x06000B8B RID: 2955 RVA: 0x000666EC File Offset: 0x000648EC
	private void StateDisarmed()
	{
		if (this.stateStart)
		{
			this.soundDisarmedBeep.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.stateStart = false;
			this.lightArmed.intensity = this.initialLightIntensity * 3f;
			this.meshRenderer.material.SetColor("_EmissionColor", Color.green);
			this.lightArmed.color = Color.green;
			this.beepTimer = 1f;
		}
		if (this.firstLight)
		{
			this.meshRenderer.material.SetColor("_EmissionColor", this.emissionColor);
			this.lightArmed.color = this.emissionColor;
			this.firstLight = false;
		}
		else if (!this.firstLightDone)
		{
			this.meshRenderer.material.SetColor("_EmissionColor", Color.green);
			this.lightArmed.color = Color.green;
			this.firstLightDone = true;
		}
		if (this.lightArmed.intensity > 0f && this.beepTimer > 0f)
		{
			float t = 1f - this.beepTimer;
			this.lightArmed.intensity = Mathf.Lerp(this.lightArmed.intensity, 0f, t);
			Color value = Color.Lerp(this.meshRenderer.material.GetColor("_EmissionColor"), Color.black, t);
			this.meshRenderer.material.SetColor("_EmissionColor", value);
			this.beepTimer -= Time.deltaTime * 0.1f;
		}
		if (SemiFunc.IsMasterClientOrSingleplayer() && this.itemToggle.toggleState)
		{
			this.StateSet(ItemMine.States.Arming);
		}
	}

	// Token: 0x06000B8C RID: 2956 RVA: 0x000668B0 File Offset: 0x00064AB0
	private void StateArming()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
			this.beepTimer = 1f;
			this.lightArmed.color = this.emissionColor;
			Color color = new Color(1f, 0.5f, 0f);
			this.soundArmingBeep.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.ColorSet(color);
		}
		this.beepTimer -= Time.deltaTime * 4f;
		if (this.beepTimer <= 0f)
		{
			this.soundArmingBeep.Play(base.transform.position, 1f, 1f, 1f, 1f);
			Color color2 = new Color(1f, 0.5f, 0f);
			this.ColorSet(color2);
			this.beepTimer = 1f;
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (!this.physGrabObject.grabbed)
			{
				this.stateTimer += Time.deltaTime;
			}
			else
			{
				this.stateTimer += Time.deltaTime * 0.25f;
			}
			if (this.physGrabObject.grabbed || this.physGrabObject.rb.velocity.magnitude > 1f)
			{
				this.stateTimer = 0f;
			}
			if (this.stateTimer >= this.armingTime)
			{
				this.StateSet(ItemMine.States.Armed);
			}
		}
		if (SemiFunc.IsMasterClientOrSingleplayer() && !this.itemToggle.toggleState)
		{
			this.StateSet(ItemMine.States.Disarming);
		}
	}

	// Token: 0x06000B8D RID: 2957 RVA: 0x00066A54 File Offset: 0x00064C54
	private void StateArmed()
	{
		if (this.stateStart)
		{
			this.soundArmedBeep.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.ColorSet(this.emissionColor);
			this.lightArmed.intensity = this.initialLightIntensity * 8f;
			this.stateStart = false;
			this.secondArmedTimer = 2f;
		}
		this.lightArmed.intensity = Mathf.Lerp(this.lightArmed.intensity, this.initialLightIntensity, Time.deltaTime * 4f);
		if (this.secondArmedTimer > 0f)
		{
			this.secondArmedTimer -= Time.deltaTime;
		}
		if (SemiFunc.IsMasterClientOrSingleplayer() && this.secondArmedTimer <= 0f && (this.triggeredByForces && this.physGrabObject.rb.velocity.magnitude > 0.5f))
		{
			this.StateSet(ItemMine.States.Triggering);
		}
		if (SemiFunc.IsMasterClientOrSingleplayer() && !this.itemToggle.toggleState)
		{
			this.StateSet(ItemMine.States.Disarming);
		}
	}

	// Token: 0x06000B8E RID: 2958 RVA: 0x00066B76 File Offset: 0x00064D76
	private void ColorSet(Color _color)
	{
		this.lightArmed.intensity = this.initialLightIntensity;
		this.lightArmed.color = _color;
		this.meshRenderer.material.SetColor("_EmissionColor", _color);
	}

	// Token: 0x06000B8F RID: 2959 RVA: 0x00066BAC File Offset: 0x00064DAC
	private void StateDisarming()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
			this.beepTimer = 1f;
			this.soundDisarmingBeep.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.ColorSet(this.emissionColor);
			this.beepTimer = 1f;
		}
		this.beepTimer -= Time.deltaTime * 4f;
		if (this.beepTimer <= 0f)
		{
			this.soundDisarmingBeep.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.ColorSet(Color.green);
			this.beepTimer = 1f;
		}
		this.stateTimer += Time.deltaTime;
		if (this.stateTimer > 0.1f)
		{
			this.StateSet(ItemMine.States.Disarmed);
		}
	}

	// Token: 0x06000B90 RID: 2960 RVA: 0x00066CA4 File Offset: 0x00064EA4
	private void StateTriggering()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
			this.beepTimer = 1f;
		}
		this.beepTimer -= Time.deltaTime * 4f;
		if (this.beepTimer < 0f)
		{
			this.soundTriggereringBeep.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.ColorSet(this.emissionColor);
			this.beepTimer = 1f;
		}
		this.stateTimer += Time.deltaTime;
		if (this.stateTimer > this.triggeringTime)
		{
			this.StateSet(ItemMine.States.Triggered);
		}
		if (SemiFunc.IsMasterClientOrSingleplayer() && !this.itemToggle.toggleState)
		{
			this.StateSet(ItemMine.States.Disarming);
		}
	}

	// Token: 0x06000B91 RID: 2961 RVA: 0x00066D74 File Offset: 0x00064F74
	private void StateTriggered()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
			this.beepTimer = 1f;
			if (!this.destroyAfterTimer)
			{
				this.DestroyMine();
			}
			Color color = new Color(0.5f, 0.9f, 1f);
			this.ColorSet(color);
		}
		this.stateTimer += Time.deltaTime;
		if (this.destroyAfterTimer && this.stateTimer > this.destroyTimer)
		{
			this.DestroyMine();
		}
	}

	// Token: 0x06000B92 RID: 2962 RVA: 0x00066DF4 File Offset: 0x00064FF4
	public void DestroyMine()
	{
		if (!SemiFunc.RunIsShop())
		{
			if (!this.mineDestroyed)
			{
				StatsManager.instance.ItemRemove(this.itemAttributes.instanceName);
				this.impactDetector.DestroyObject(true);
				this.mineDestroyed = true;
				return;
			}
		}
		else
		{
			this.ResetMine();
			this.physGrabObject.Teleport(this.startPosition, this.startRotation);
		}
	}

	// Token: 0x06000B93 RID: 2963 RVA: 0x00066E58 File Offset: 0x00065058
	private void ResetMine()
	{
		this.hasBeenGrabbed = false;
		this.StateSet(ItemMine.States.Disarmed);
		this.itemToggle.ToggleItem(false, -1);
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.stateTimer = 0f;
			Rigidbody component = base.GetComponent<Rigidbody>();
			if (!component.isKinematic)
			{
				component.velocity = Vector3.zero;
				component.angularVelocity = Vector3.zero;
			}
		}
	}

	// Token: 0x06000B94 RID: 2964 RVA: 0x00066EB8 File Offset: 0x000650B8
	private void AnimateLight()
	{
		if (this.lightArmed.intensity > 0f && this.beepTimer > 0f)
		{
			float t = 1f - this.beepTimer;
			this.lightArmed.intensity = Mathf.Lerp(this.lightArmed.intensity, 0f, t);
			Color value = Color.Lerp(this.meshRenderer.material.GetColor("_EmissionColor"), Color.black, t);
			this.meshRenderer.material.SetColor("_EmissionColor", value);
		}
	}

	// Token: 0x06000B95 RID: 2965 RVA: 0x00066F4C File Offset: 0x0006514C
	private void Update()
	{
		this.TriggerRotation();
		this.TriggerLineVisuals();
		this.TriggerScaleFixer();
		this.AnimateLight();
		if (this.physGrabObject.grabbedLocal && !SemiFunc.RunIsShop())
		{
			PhysGrabber.instance.OverrideGrabDistance(1f);
		}
		if (this.physGrabObject.grabbed)
		{
			this.hasBeenGrabbed = true;
		}
		if (this.itemEquippable.isEquipped && SemiFunc.IsMasterClientOrSingleplayer() && this.hasBeenGrabbed)
		{
			this.StateSet(ItemMine.States.Disarmed);
		}
		if (!SemiFunc.RunIsShop())
		{
			if (SemiFunc.IsMasterClientOrSingleplayer() && this.wasGrabbed && !this.physGrabObject.grabbed)
			{
				Rigidbody component = base.GetComponent<Rigidbody>();
				if (!component.isKinematic)
				{
					component.velocity *= 0.15f;
				}
			}
			this.wasGrabbed = this.physGrabObject.grabbed;
		}
		switch (this.state)
		{
		case ItemMine.States.Disarmed:
			this.StateDisarmed();
			return;
		case ItemMine.States.Arming:
			this.StateArming();
			return;
		case ItemMine.States.Armed:
			this.StateArmed();
			return;
		case ItemMine.States.Disarming:
			this.StateDisarming();
			return;
		case ItemMine.States.Triggering:
			this.StateTriggering();
			return;
		case ItemMine.States.Triggered:
			this.StateTriggered();
			return;
		default:
			return;
		}
	}

	// Token: 0x06000B96 RID: 2966 RVA: 0x00067073 File Offset: 0x00065273
	[PunRPC]
	public void TriggeredRPC()
	{
		this.onTriggered.Invoke();
	}

	// Token: 0x06000B97 RID: 2967 RVA: 0x00067080 File Offset: 0x00065280
	private void StateSet(ItemMine.States newState)
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (newState == this.state)
		{
			return;
		}
		if (newState == ItemMine.States.Triggered)
		{
			if (SemiFunc.IsMultiplayer())
			{
				this.photonView.RPC("TriggeredRPC", RpcTarget.All, Array.Empty<object>());
			}
			else
			{
				this.TriggeredRPC();
			}
		}
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("StateSetRPC", RpcTarget.All, new object[]
			{
				(int)newState
			});
			return;
		}
		this.StateSetRPC((int)newState);
	}

	// Token: 0x06000B98 RID: 2968 RVA: 0x000670F7 File Offset: 0x000652F7
	[PunRPC]
	public void StateSetRPC(int newState)
	{
		this.state = (ItemMine.States)newState;
		this.stateStart = true;
		this.stateTimer = 0f;
		this.beepTimer = 0f;
	}

	// Token: 0x06000B99 RID: 2969 RVA: 0x00067120 File Offset: 0x00065320
	private void TriggerScaleFixer()
	{
		if (this.state != ItemMine.States.Armed)
		{
			return;
		}
		bool flag = false;
		if (SemiFunc.FPSImpulse30())
		{
			if (Vector3.Distance(this.prevPos, base.transform.position) > 0.01f)
			{
				flag = true;
				this.prevPos = base.transform.position;
			}
			if (Quaternion.Angle(this.prevRot, base.transform.rotation) > 0.01f)
			{
				flag = true;
				this.prevRot = base.transform.rotation;
			}
		}
		if ((!flag && SemiFunc.FPSImpulse1()) || (flag && SemiFunc.FPSImpulse30()))
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(this.triggerTransform.position, this.triggerTransform.forward, out raycastHit, 1f, LayerMask.GetMask(new string[]
			{
				"Default"
			})))
			{
				this.targetLineLength = raycastHit.distance * 0.8f;
			}
			else
			{
				this.targetLineLength = 1f;
			}
		}
		this.triggerTransform.localScale = Mathf.Lerp(this.triggerTransform.localScale.z, this.targetLineLength, Time.deltaTime * 8f) * Vector3.one;
	}

	// Token: 0x06000B9A RID: 2970 RVA: 0x00067248 File Offset: 0x00065448
	private void TriggerRotation()
	{
		this.upsideDown = true;
		if (Vector3.Dot(base.transform.up, Vector3.up) < 0f)
		{
			this.upsideDown = false;
		}
		if (this.upsideDown)
		{
			this.triggerTargetRotation = Quaternion.Euler(-90f, 0f, 0f);
		}
		else
		{
			this.triggerTargetRotation = Quaternion.Euler(90f, 0f, 0f);
		}
		this.triggerTransform.localRotation = SemiFunc.SpringQuaternionGet(this.triggerSpringQuaternion, this.triggerTargetRotation, -1f);
	}

	// Token: 0x06000B9B RID: 2971 RVA: 0x000672E0 File Offset: 0x000654E0
	private void TriggerLineVisuals()
	{
		if (this.state == ItemMine.States.Armed)
		{
			this.triggerLine.material.SetTextureOffset("_MainTex", new Vector2(-Time.time * 2f, 0f));
			if (!this.triggerLine.enabled)
			{
				this.triggerLine.enabled = true;
				this.lineParticles.Play();
			}
			this.triggerLine.widthMultiplier = Mathf.Lerp(this.triggerLine.widthMultiplier, 1f, Time.deltaTime * 4f);
			return;
		}
		if (this.triggerLine.enabled)
		{
			this.triggerLine.widthMultiplier = Mathf.Lerp(this.triggerLine.widthMultiplier, 0f, Time.deltaTime * 8f);
			if (this.triggerLine.widthMultiplier < 0.01f)
			{
				this.triggerLine.enabled = false;
				this.lineParticles.Stop();
			}
		}
	}

	// Token: 0x06000B9C RID: 2972 RVA: 0x000673D2 File Offset: 0x000655D2
	public void SetTriggered()
	{
		if (this.state == ItemMine.States.Armed)
		{
			this.StateSet(ItemMine.States.Triggering);
		}
	}

	// Token: 0x040012B0 RID: 4784
	public ItemMine.MineType mineType;

	// Token: 0x040012B1 RID: 4785
	public Color emissionColor;

	// Token: 0x040012B2 RID: 4786
	public UnityEvent onTriggered;

	// Token: 0x040012B3 RID: 4787
	public float armingTime;

	// Token: 0x040012B4 RID: 4788
	public float triggeringTime;

	// Token: 0x040012B5 RID: 4789
	private SpringQuaternion triggerSpringQuaternion;

	// Token: 0x040012B6 RID: 4790
	private Quaternion triggerTargetRotation;

	// Token: 0x040012B7 RID: 4791
	private bool upsideDown;

	// Token: 0x040012B8 RID: 4792
	public Transform triggerTransform;

	// Token: 0x040012B9 RID: 4793
	public LineRenderer triggerLine;

	// Token: 0x040012BA RID: 4794
	public ParticleSystem lineParticles;

	// Token: 0x040012BB RID: 4795
	private float beepTimer;

	// Token: 0x040012BC RID: 4796
	private float checkTimer;

	// Token: 0x040012BD RID: 4797
	private ItemMineTrigger itemMineTrigger;

	// Token: 0x040012BE RID: 4798
	private ItemEquippable itemEquippable;

	// Token: 0x040012BF RID: 4799
	private ItemAttributes itemAttributes;

	// Token: 0x040012C0 RID: 4800
	private ItemToggle itemToggle;

	// Token: 0x040012C1 RID: 4801
	[Space(20f)]
	private PhotonView photonView;

	// Token: 0x040012C2 RID: 4802
	private PhysGrabObject physGrabObject;

	// Token: 0x040012C3 RID: 4803
	public MeshRenderer meshRenderer;

	// Token: 0x040012C4 RID: 4804
	public Light lightArmed;

	// Token: 0x040012C5 RID: 4805
	[Space(20f)]
	public Sound soundArmingBeep;

	// Token: 0x040012C6 RID: 4806
	public Sound soundArmedBeep;

	// Token: 0x040012C7 RID: 4807
	public Sound soundDisarmingBeep;

	// Token: 0x040012C8 RID: 4808
	public Sound soundDisarmedBeep;

	// Token: 0x040012C9 RID: 4809
	public Sound soundTriggereringBeep;

	// Token: 0x040012CA RID: 4810
	private float initialLightIntensity;

	// Token: 0x040012CB RID: 4811
	private ParticleScriptExplosion particleScriptExplosion;

	// Token: 0x040012CC RID: 4812
	private bool hasBeenGrabbed;

	// Token: 0x040012CD RID: 4813
	private Vector3 startPosition;

	// Token: 0x040012CE RID: 4814
	private Quaternion startRotation;

	// Token: 0x040012CF RID: 4815
	internal Vector3 triggeredPosition;

	// Token: 0x040012D0 RID: 4816
	internal Transform triggeredTransform;

	// Token: 0x040012D1 RID: 4817
	internal PlayerAvatar triggeredPlayerAvatar;

	// Token: 0x040012D2 RID: 4818
	internal PlayerTumble triggeredPlayerTumble;

	// Token: 0x040012D3 RID: 4819
	internal PhysGrabObject triggeredPhysGrabObject;

	// Token: 0x040012D4 RID: 4820
	public bool triggeredByRigidBodies = true;

	// Token: 0x040012D5 RID: 4821
	public bool triggeredByEnemies = true;

	// Token: 0x040012D6 RID: 4822
	public bool triggeredByPlayers = true;

	// Token: 0x040012D7 RID: 4823
	public bool triggeredByForces = true;

	// Token: 0x040012D8 RID: 4824
	public bool destroyAfterTimer;

	// Token: 0x040012D9 RID: 4825
	public float destroyTimer = 10f;

	// Token: 0x040012DA RID: 4826
	internal bool wasTriggeredByEnemy;

	// Token: 0x040012DB RID: 4827
	internal bool wasTriggeredByPlayer;

	// Token: 0x040012DC RID: 4828
	internal bool wasTriggeredByForce;

	// Token: 0x040012DD RID: 4829
	internal bool wasTriggeredByRigidBody;

	// Token: 0x040012DE RID: 4830
	internal bool firstLight = true;

	// Token: 0x040012DF RID: 4831
	private bool firstLightDone;

	// Token: 0x040012E0 RID: 4832
	private float secondArmedTimer;

	// Token: 0x040012E1 RID: 4833
	private bool wasGrabbed;

	// Token: 0x040012E2 RID: 4834
	private float targetLineLength = 1f;

	// Token: 0x040012E3 RID: 4835
	private Vector3 prevPos = Vector3.zero;

	// Token: 0x040012E4 RID: 4836
	private Quaternion prevRot = Quaternion.identity;

	// Token: 0x040012E5 RID: 4837
	internal ItemMine.States state;

	// Token: 0x040012E6 RID: 4838
	private bool stateStart = true;

	// Token: 0x040012E7 RID: 4839
	private float stateTimer;

	// Token: 0x040012E8 RID: 4840
	private PhysGrabObjectImpactDetector impactDetector;

	// Token: 0x040012E9 RID: 4841
	private bool mineDestroyed;

	// Token: 0x02000347 RID: 839
	public enum MineType
	{
		// Token: 0x040026E6 RID: 9958
		None,
		// Token: 0x040026E7 RID: 9959
		Explosive,
		// Token: 0x040026E8 RID: 9960
		Shockwave,
		// Token: 0x040026E9 RID: 9961
		Stun
	}

	// Token: 0x02000348 RID: 840
	public enum States
	{
		// Token: 0x040026EB RID: 9963
		Disarmed,
		// Token: 0x040026EC RID: 9964
		Arming,
		// Token: 0x040026ED RID: 9965
		Armed,
		// Token: 0x040026EE RID: 9966
		Disarming,
		// Token: 0x040026EF RID: 9967
		Triggering,
		// Token: 0x040026F0 RID: 9968
		Triggered
	}
}
