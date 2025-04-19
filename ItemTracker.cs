using System;
using Photon.Pun;
using TMPro;
using UnityEngine;

// Token: 0x02000176 RID: 374
public class ItemTracker : MonoBehaviour
{
	// Token: 0x06000C95 RID: 3221 RVA: 0x0006EAF4 File Offset: 0x0006CCF4
	private void Start()
	{
		this.rb = base.GetComponent<Rigidbody>();
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.itemToggle = base.GetComponent<ItemToggle>();
		this.itemBattery = base.GetComponent<ItemBattery>();
		this.photonView = base.GetComponent<PhotonView>();
		this.meshRenderer.material.SetColor("_EmissionColor", Color.black);
		this.nozzleLight.enabled = false;
		this.nozzleLight.intensity = 0f;
	}

	// Token: 0x06000C96 RID: 3222 RVA: 0x0006EB74 File Offset: 0x0006CD74
	private void ValuableTarget()
	{
		if (this.trackerType != ItemTracker.TrackerType.Valuable)
		{
			return;
		}
		Vector3 position = this.nozzleTransform.position;
		this.hasTarget = false;
		float radius = 15f;
		if (!this.currentTarget)
		{
			radius = 30f;
		}
		Collider[] array = Physics.OverlapSphere(base.transform.position, radius);
		float num = float.MaxValue;
		Collider[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			ValuableObject componentInParent = array2[i].gameObject.GetComponentInParent<ValuableObject>();
			if (componentInParent && !componentInParent.discovered)
			{
				PhysGrabObject component = componentInParent.GetComponent<PhysGrabObject>();
				PhysGrabObjectImpactDetector component2 = componentInParent.GetComponent<PhysGrabObjectImpactDetector>();
				float num2 = Vector3.Distance(position, component.midPoint);
				if (num2 < num && !component.grabbed && !component2.inCart)
				{
					num = num2;
					this.currentTarget = component.transform;
					this.currentTargetPhysGrabObject = component;
					this.hasTarget = true;
				}
			}
		}
		if (this.hasTarget)
		{
			this.SetTarget(this.currentTargetPhysGrabObject.photonView.ViewID);
		}
	}

	// Token: 0x06000C97 RID: 3223 RVA: 0x0006EC7C File Offset: 0x0006CE7C
	private void ExtractionTarget()
	{
		if (this.trackerType != ItemTracker.TrackerType.Extraction)
		{
			return;
		}
		this.hasTarget = false;
		ExtractionPoint extractionPoint = SemiFunc.ExtractionPointGetNearestNotActivated(this.nozzleTransform.position);
		if (extractionPoint)
		{
			this.currentTarget = extractionPoint.transform;
			this.hasTarget = true;
		}
		if (this.hasTarget)
		{
			this.SetTarget(this.currentTarget.GetComponent<PhotonView>().ViewID);
		}
	}

	// Token: 0x06000C98 RID: 3224 RVA: 0x0006ECE4 File Offset: 0x0006CEE4
	private void FindATarget()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (this.itemBattery.batteryLife <= 0f)
		{
			return;
		}
		this.timer += Time.deltaTime;
		if (this.timer > 2f)
		{
			this.ValuableTarget();
			this.ExtractionTarget();
			this.timer = 0f;
		}
	}

	// Token: 0x06000C99 RID: 3225 RVA: 0x0006ED44 File Offset: 0x0006CF44
	private void AnimateEmissionToBlack()
	{
		if (this.itemToggle.toggleState)
		{
			return;
		}
		Color color = this.meshRenderer.material.GetColor("_EmissionColor");
		if (color != Color.black)
		{
			this.meshRenderer.material.SetColor("_EmissionColor", Color.Lerp(color, Color.black, Time.deltaTime * 20f));
		}
		if (this.nozzleLight.intensity > 0f)
		{
			this.nozzleLight.intensity = Mathf.Lerp(this.nozzleLight.intensity, 0f, Time.deltaTime * 10f);
			return;
		}
		this.nozzleLight.enabled = false;
	}

	// Token: 0x06000C9A RID: 3226 RVA: 0x0006EDF8 File Offset: 0x0006CFF8
	private void PhysGrabOverrides()
	{
		if (this.physGrabObject.grabbed && this.physGrabObject.grabbedLocal)
		{
			PhysGrabber.instance.OverrideGrabDistance(0.8f);
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (this.physGrabObject.grabbed)
		{
			Quaternion turnX = Quaternion.Euler(0f, 0f, 0f);
			Quaternion turnY = Quaternion.Euler(0f, 0f, 0f);
			Quaternion identity = Quaternion.identity;
			this.physGrabObject.TurnXYZ(turnX, turnY, identity);
			this.physGrabObject.OverrideTorqueStrengthX(2f, 0.1f);
			if (this.currentTarget && this.itemToggle.toggleState)
			{
				this.physGrabObject.OverrideTorqueStrengthY(0.1f, 0.1f);
			}
			this.physGrabObject.OverrideGrabVerticalPosition(-0.2f);
			return;
		}
		if (this.itemToggle.toggleState)
		{
			this.itemToggle.ToggleItem(false, -1);
		}
	}

	// Token: 0x06000C9B RID: 3227 RVA: 0x0006EEF4 File Offset: 0x0006D0F4
	private void DisplayLogic()
	{
		if (this.display.gameObject.activeSelf)
		{
			Vector2 textureOffset = this.display.material.GetTextureOffset("_MainTex");
			textureOffset.y += Time.deltaTime * 2f;
			this.display.material.SetTextureOffset("_MainTex", textureOffset);
			if (SemiFunc.IsMasterClientOrSingleplayer())
			{
				this.itemBattery.batteryLife -= Time.deltaTime * 0.5f;
			}
		}
		else if (this.displayText.text != "--")
		{
			this.displayText.text = "--";
		}
		if (this.displayOverrideTimer >= 0f)
		{
			this.displayOverrideTimer -= Time.deltaTime;
			if (this.displayOverrideTimer <= 0f)
			{
				this.displayText.text = "--";
				Color color = this.colorScreenNeutral;
				color.a = 0.2f;
				this.displayText.color = this.colorScreenNeutral;
				this.display.material.color = color;
				this.displayLight.color = this.colorScreenNeutral;
			}
		}
		if (this.trackerType == ItemTracker.TrackerType.Valuable && this.currentTargetPhysGrabObject)
		{
			this.targetPosition = this.currentTargetPhysGrabObject.midPoint;
		}
		if (this.trackerType == ItemTracker.TrackerType.Extraction && this.currentTarget)
		{
			this.targetPosition = this.currentTarget.position;
		}
		if (this.changeDigitTimer <= 0f && this.displayOverrideTimer <= 0f)
		{
			if (this.hasTarget && this.display.gameObject.activeSelf)
			{
				int num = Mathf.RoundToInt(Vector3.Distance(this.nozzleTransform.position, this.targetPosition));
				if (num != this.prevDigit)
				{
					this.changeDigitTimer = 1f;
					this.digitSwap.Play(this.display.transform.position, 1f, 1f, 1f, 1f);
					this.prevDigit = num;
				}
				this.displayText.text = num.ToString();
			}
			else
			{
				this.displayText.text = "--";
			}
		}
		else
		{
			this.changeDigitTimer -= Time.deltaTime;
		}
		if (!SemiFunc.FPSImpulse15())
		{
			return;
		}
		if (this.itemToggle.toggleState)
		{
			if (!this.display.gameObject.activeSelf)
			{
				this.display.gameObject.SetActive(true);
				return;
			}
		}
		else if (this.display.gameObject.activeSelf)
		{
			this.display.gameObject.SetActive(false);
		}
	}

	// Token: 0x06000C9C RID: 3228 RVA: 0x0006F1AC File Offset: 0x0006D3AC
	private void TargetLogic()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (!this.itemToggle.toggleState)
		{
			this.hasTarget = false;
			this.currentTarget = null;
			this.displayOverrideTimer = 0f;
			return;
		}
		if (this.trackerType == ItemTracker.TrackerType.Valuable)
		{
			if (this.currentTarget && this.currentTarget.GetComponent<ValuableObject>().discovered && this.hasTarget)
			{
				this.CurrentTargetUpdate(true);
				this.currentTarget = null;
				this.hasTarget = false;
			}
			if (!this.currentTarget && this.hasTarget && this.physGrabObject.grabbed)
			{
				this.CurrentTargetUpdate(false);
				this.hasTarget = false;
			}
		}
		if (this.trackerType == ItemTracker.TrackerType.Extraction)
		{
			if (this.currentTarget)
			{
				this.hasTarget = true;
				return;
			}
			this.hasTarget = false;
		}
	}

	// Token: 0x06000C9D RID: 3229 RVA: 0x0006F284 File Offset: 0x0006D484
	private void Update()
	{
		this.PhysGrabOverrides();
		if (this.itemBattery.batteryLifeInt == 0 && this.itemToggle.toggleState)
		{
			if (!this.display.gameObject.activeSelf && this.itemToggle.toggleState)
			{
				this.display.gameObject.SetActive(true);
				this.batteryOutTimer = 0f;
			}
			if (this.batteryOutTimer == 0f)
			{
				this.soundTargetLost.Play(this.display.transform.position, 1f, 1f, 1f, 1f);
			}
			if (this.batteryOutTimer > 2f && this.itemToggle.toggleState)
			{
				this.itemToggle.ToggleItem(false, -1);
				this.display.gameObject.SetActive(false);
				this.batteryOutTimer = 0f;
				return;
			}
			this.DisplayColorOverride("X", Color.red, 2f);
			this.batteryOutTimer += Time.deltaTime;
			return;
		}
		else
		{
			this.batteryOutTimer = 0f;
			this.DisplayLogic();
			this.TargetLogic();
			if (this.displayOverrideTimer > 0f)
			{
				return;
			}
			this.AnimateEmissionToBlack();
			if (!this.itemToggle.toggleState)
			{
				return;
			}
			this.FindATarget();
			this.Blinking();
			return;
		}
	}

	// Token: 0x06000C9E RID: 3230 RVA: 0x0006F3E0 File Offset: 0x0006D5E0
	private void Blinking()
	{
		Color color = this.meshRenderer.material.GetColor("_EmissionColor");
		Color color2 = this.colorBleepOff;
		if (color != color2)
		{
			Color color3 = Color.Lerp(color, color2, Time.deltaTime * 4f);
			this.meshRenderer.material.SetColor("_EmissionColor", color3);
			this.nozzleLight.color = color3;
		}
		if (this.nozzleLight.intensity < 1f)
		{
			if (!this.nozzleLight.enabled)
			{
				this.nozzleLight.enabled = true;
			}
			this.nozzleLight.intensity = Mathf.Lerp(this.nozzleLight.intensity, 2f, Time.deltaTime * 10f);
		}
		if (this.hasTarget)
		{
			Vector3 position = this.nozzleTransform.position;
			float b = 1.5f;
			float a = 0.2f;
			this.blipTimer += Time.deltaTime;
			float num = 5f;
			float num2 = 0f;
			float time = (Mathf.Clamp(Vector3.Distance(position, this.targetPosition), num2, num) - num2) / (num - num2);
			float num3 = this.animationCurve.Evaluate(time);
			float num4 = Mathf.Lerp(a, b, num3);
			if (this.blipTimer > num4)
			{
				this.blipTimer = 0f;
				this.soundBleep.Pitch = Mathf.Lerp(1f, 2f, 1f - num3);
				this.soundBleep.Play(this.nozzleTransform.position, 1f, 1f, 1f, 1f);
				this.meshRenderer.material.SetColor("_EmissionColor", this.colorBleep);
				this.nozzleLight.color = this.colorBleep;
				this.nozzleLight.enabled = true;
			}
		}
	}

	// Token: 0x06000C9F RID: 3231 RVA: 0x0006F5B8 File Offset: 0x0006D7B8
	private void FixedUpdate()
	{
		if (this.itemBattery.batteryLife <= 0f)
		{
			return;
		}
		if (!this.itemToggle.toggleState)
		{
			this.currentTarget = null;
			this.hasTarget = false;
			return;
		}
		if (this.displayOverrideTimer > 0f)
		{
			return;
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (this.hasTarget && this.physGrabObject.grabbed)
		{
			SemiFunc.PhysLookAtPositionWithForce(this.rb, base.transform, this.targetPosition, 10f);
			this.rb.AddForceAtPosition(base.transform.forward * 1f, this.nozzleTransform.position, ForceMode.Force);
		}
	}

	// Token: 0x06000CA0 RID: 3232 RVA: 0x0006F667 File Offset: 0x0006D867
	private void SetTarget(int photonViewID)
	{
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("SetTargetRPC", RpcTarget.All, new object[]
			{
				photonViewID
			});
		}
	}

	// Token: 0x06000CA1 RID: 3233 RVA: 0x0006F690 File Offset: 0x0006D890
	[PunRPC]
	private void SetTargetRPC(int targetViewID)
	{
		PhysGrabObject component = PhotonView.Find(targetViewID).GetComponent<PhysGrabObject>();
		Transform transform = PhotonView.Find(targetViewID).transform;
		this.currentTarget = transform;
		if (component)
		{
			this.currentTargetPhysGrabObject = component;
		}
		this.hasTarget = true;
	}

	// Token: 0x06000CA2 RID: 3234 RVA: 0x0006F6D4 File Offset: 0x0006D8D4
	private void DisplayColorOverride(string _text, Color _color, float _time)
	{
		this.displayText.text = _text;
		this.displayText.color = _color;
		this.displayOverrideTimer = _time;
		this.displayLight.color = _color;
		_color.a = 0.2f;
		this.display.material.color = _color;
	}

	// Token: 0x06000CA3 RID: 3235 RVA: 0x0006F729 File Offset: 0x0006D929
	private void CurrentTargetUpdate(bool _found)
	{
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("CurrentTargetUpdateRPC", RpcTarget.All, new object[]
			{
				_found
			});
			return;
		}
		this.CurrentTargetUpdateRPC(_found);
	}

	// Token: 0x06000CA4 RID: 3236 RVA: 0x0006F75C File Offset: 0x0006D95C
	[PunRPC]
	public void CurrentTargetUpdateRPC(bool _found)
	{
		if (_found)
		{
			this.soundTargetFound.Play(this.display.transform.position, 1f, 1f, 1f, 1f);
			this.DisplayColorOverride("FOUND", this.colorTargetFound, 2f);
		}
		else
		{
			this.soundTargetLost.Play(this.display.transform.position, 1f, 1f, 1f, 1f);
			this.DisplayColorOverride("NOT FOUND", Color.red, 2f);
		}
		this.currentTarget = null;
		this.currentTargetPhysGrabObject = null;
		this.hasTarget = false;
	}

	// Token: 0x040013EB RID: 5099
	public ItemTracker.TrackerType trackerType;

	// Token: 0x040013EC RID: 5100
	private float timer;

	// Token: 0x040013ED RID: 5101
	private Transform currentTarget;

	// Token: 0x040013EE RID: 5102
	private PhysGrabObject currentTargetPhysGrabObject;

	// Token: 0x040013EF RID: 5103
	private Rigidbody rb;

	// Token: 0x040013F0 RID: 5104
	public Transform nozzleTransform;

	// Token: 0x040013F1 RID: 5105
	private PhysGrabObject physGrabObject;

	// Token: 0x040013F2 RID: 5106
	public MeshRenderer meshRenderer;

	// Token: 0x040013F3 RID: 5107
	public AnimationCurve animationCurve;

	// Token: 0x040013F4 RID: 5108
	private float blipTimer;

	// Token: 0x040013F5 RID: 5109
	public Sound soundBleep;

	// Token: 0x040013F6 RID: 5110
	public Sound digitSwap;

	// Token: 0x040013F7 RID: 5111
	public Sound soundTargetFound;

	// Token: 0x040013F8 RID: 5112
	public Sound soundTargetLost;

	// Token: 0x040013F9 RID: 5113
	private ItemToggle itemToggle;

	// Token: 0x040013FA RID: 5114
	private ItemBattery itemBattery;

	// Token: 0x040013FB RID: 5115
	private PhotonView photonView;

	// Token: 0x040013FC RID: 5116
	private bool currentToggleState;

	// Token: 0x040013FD RID: 5117
	public Light nozzleLight;

	// Token: 0x040013FE RID: 5118
	public MeshRenderer display;

	// Token: 0x040013FF RID: 5119
	public TextMeshPro displayText;

	// Token: 0x04001400 RID: 5120
	private int prevDigit;

	// Token: 0x04001401 RID: 5121
	private float changeDigitTimer;

	// Token: 0x04001402 RID: 5122
	private float displayOverrideTimer;

	// Token: 0x04001403 RID: 5123
	public Light displayLight;

	// Token: 0x04001404 RID: 5124
	private bool hasTarget;

	// Token: 0x04001405 RID: 5125
	public Color colorBleep;

	// Token: 0x04001406 RID: 5126
	public Color colorBleepOff;

	// Token: 0x04001407 RID: 5127
	public Color colorTargetFound;

	// Token: 0x04001408 RID: 5128
	public Color colorScreenNeutral;

	// Token: 0x04001409 RID: 5129
	private Vector3 targetPosition;

	// Token: 0x0400140A RID: 5130
	private float batteryOutTimer;

	// Token: 0x02000350 RID: 848
	public enum TrackerType
	{
		// Token: 0x04002703 RID: 9987
		Valuable,
		// Token: 0x04002704 RID: 9988
		Extraction
	}
}
