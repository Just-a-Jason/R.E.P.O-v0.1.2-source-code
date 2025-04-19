using System;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

// Token: 0x02000196 RID: 406
public class PhysGrabCart : MonoBehaviour
{
	// Token: 0x06000DAD RID: 3501 RVA: 0x0007B984 File Offset: 0x00079B84
	private void Start()
	{
		this.itemEquippable = base.GetComponent<ItemEquippable>();
		this.originalHaulColor = this.displayText.color;
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.rb = base.GetComponent<Rigidbody>();
		this.rb.mass = 8f;
		this.inCart = base.transform.Find("In Cart");
		foreach (object obj in base.transform)
		{
			Transform transform = (Transform)obj;
			Transform transform2 = transform.Find("Semi Box Collider");
			if (transform.name.Contains("Inside"))
			{
				this.cartInside.Add(transform2.GetComponent<Collider>());
			}
			if (transform2 && (transform2.GetComponent<Collider>().gameObject.layer == LayerMask.NameToLayer("PhysGrabObject") || transform2.GetComponent<Collider>().gameObject.layer == LayerMask.NameToLayer("Default")))
			{
				transform2.GetComponent<Collider>().gameObject.layer = LayerMask.NameToLayer("PhysGrabObjectCart");
			}
			if (transform.name.Contains("Cart Mesh"))
			{
				this.cartMesh = transform.GetComponent<MeshRenderer>();
			}
			if (transform.name.Contains("Cart Wall Collider"))
			{
				transform2.GetComponent<Collider>().material = SemiFunc.PhysicMaterialPhysGrabObject();
			}
			if (transform.name.Contains("Capsule"))
			{
				this.capsuleColliders.Add(transform.GetComponent<Collider>());
			}
		}
		this.photonView = base.GetComponent<PhotonView>();
		this.physGrabObjectGrabArea = base.GetComponent<PhysGrabObjectGrabArea>();
		foreach (MeshRenderer meshRenderer in this.grabMesh)
		{
			this.grabMaterial.Add(meshRenderer.material);
		}
	}

	// Token: 0x06000DAE RID: 3502 RVA: 0x0007BB7C File Offset: 0x00079D7C
	private void ObjectsInCart()
	{
		if (SemiFunc.PlayerNearestDistance(base.transform.position) > 12f)
		{
			return;
		}
		if (this.objectInCartCheckTimer > 0f)
		{
			this.objectInCartCheckTimer -= Time.deltaTime;
		}
		else
		{
			Collider[] array = Physics.OverlapBox(this.inCart.position, this.inCart.localScale / 2f, this.inCart.rotation);
			this.itemsInCart.Clear();
			this.haulPrevious = this.haulCurrent;
			this.itemsInCartCount = 0;
			this.haulCurrent = 0;
			foreach (Collider collider in array)
			{
				if (collider.gameObject.layer == LayerMask.NameToLayer("PhysGrabObject"))
				{
					PhysGrabObject componentInParent = collider.GetComponentInParent<PhysGrabObject>();
					if (componentInParent && !this.itemsInCart.Contains(componentInParent))
					{
						this.itemsInCart.Add(componentInParent);
						ValuableObject componentInParent2 = collider.GetComponentInParent<ValuableObject>();
						if (componentInParent2)
						{
							this.haulCurrent += (int)componentInParent2.dollarValueCurrent;
						}
						this.itemsInCartCount++;
					}
				}
			}
			this.objectInCartCheckTimer = 0.5f;
		}
		if (this.haulPrevious != this.haulCurrent)
		{
			this.haulUpdateEffectTimer = 0.3f;
			if (this.haulCurrent > this.haulPrevious)
			{
				this.deductedFromHaul = false;
				this.soundHaulIncrease.Play(this.displayText.transform.position, 1f, 1f, 1f, 1f);
			}
			else
			{
				this.deductedFromHaul = true;
				this.soundHaulDecrease.Play(this.displayText.transform.position, 1f, 1f, 1f, 1f);
			}
			this.haulPrevious = this.haulCurrent;
		}
		if (this.haulUpdateEffectTimer > 0f)
		{
			this.haulUpdateEffectTimer -= Time.deltaTime;
			this.haulUpdateEffectTimer = Mathf.Max(0f, this.haulUpdateEffectTimer);
			Color color = Color.white;
			if (this.deductedFromHaul)
			{
				color = Color.red;
			}
			this.displayText.color = color;
			if (this.thirtyFPSUpdate)
			{
				this.displayText.text = this.GlitchyText();
			}
			this.resetHaulText = false;
			return;
		}
		if (!this.resetHaulText)
		{
			this.displayText.color = this.originalHaulColor;
			this.SetHaulText();
			this.resetHaulText = true;
		}
	}

	// Token: 0x06000DAF RID: 3503 RVA: 0x0007BDF4 File Offset: 0x00079FF4
	private void SetHaulText()
	{
		string str = "<color=#bd4300>$</color>";
		this.displayText.text = str + SemiFunc.DollarGetString(Mathf.Max(0, this.haulCurrent));
	}

	// Token: 0x06000DB0 RID: 3504 RVA: 0x0007BE2C File Offset: 0x0007A02C
	private void ThirtyFPS()
	{
		if (this.thirtyFPSUpdateTimer > 0f)
		{
			this.thirtyFPSUpdateTimer -= Time.deltaTime;
			this.thirtyFPSUpdateTimer = Mathf.Max(0f, this.thirtyFPSUpdateTimer);
			return;
		}
		this.thirtyFPSUpdate = true;
		this.thirtyFPSUpdateTimer = 0.033333335f;
	}

	// Token: 0x06000DB1 RID: 3505 RVA: 0x0007BE84 File Offset: 0x0007A084
	private string GlitchyText()
	{
		string text = "";
		for (int i = 0; i < 9; i++)
		{
			bool flag = false;
			if (Random.Range(0, 4) == 0 && i <= 5)
			{
				text += "TAX";
				i += 2;
				flag = true;
			}
			if (Random.Range(0, 3) == 0 && !flag)
			{
				text += "$";
				flag = true;
			}
			if (!flag)
			{
				text += Random.Range(0, 10).ToString();
			}
		}
		return text;
	}

	// Token: 0x06000DB2 RID: 3506 RVA: 0x0007BEFC File Offset: 0x0007A0FC
	private void StateMessages()
	{
		if (SemiFunc.RunIsShop())
		{
			return;
		}
		if (this.physGrabObject.grabbedLocal)
		{
			if (this.currentState == PhysGrabCart.State.Handled)
			{
				Color color = new Color(0.2f, 0.8f, 0.1f);
				ItemInfoExtraUI.instance.ItemInfoText("Mode: STRONG", color);
			}
			if (this.currentState == PhysGrabCart.State.Dragged)
			{
				Color color2 = new Color(1f, 0.46f, 0f);
				ItemInfoExtraUI.instance.ItemInfoText("Mode: WEAK", color2);
			}
		}
	}

	// Token: 0x06000DB3 RID: 3507 RVA: 0x0007BF7C File Offset: 0x0007A17C
	private void SmallCartLogic()
	{
		if (!this.isSmallCart)
		{
			return;
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (this.itemEquippable.isEquipping)
		{
			if (!this.smallCartHurtCollider.activeSelf)
			{
				this.smallCartHurtCollider.SetActive(true);
			}
		}
		else if (!this.smallCartHurtCollider.activeSelf)
		{
			this.smallCartHurtCollider.SetActive(false);
		}
		if (this.currentState == PhysGrabCart.State.Locked)
		{
			this.CartMassOverride(8f);
			this.physGrabObject.OverrideMaterial(this.physMaterialSticky, 0.1f);
		}
	}

	// Token: 0x06000DB4 RID: 3508 RVA: 0x0007C004 File Offset: 0x0007A204
	private void Update()
	{
		if (this.itemEquippable && this.itemEquippable.isUnequipping)
		{
			return;
		}
		this.ThirtyFPS();
		this.ObjectsInCart();
		this.StateMessages();
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		this.AutoTurnOff();
		this.StateLogic();
		if (this.playerInteractionTimer > 0f)
		{
			this.playerInteractionTimer -= Time.deltaTime;
		}
		this.thirtyFPSUpdate = false;
	}

	// Token: 0x06000DB5 RID: 3509 RVA: 0x0007C078 File Offset: 0x0007A278
	private void FixedUpdate()
	{
		if (this.physGrabObjectGrabArea && this.physGrabObjectGrabArea.listOfAllGrabbers.Count > 0)
		{
			this.CartSteer();
		}
		else
		{
			this.cartBeingPulled = false;
		}
		if (!LevelGenerator.Instance.Generated)
		{
			return;
		}
		if (GameManager.instance.gameMode == 1 && !PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (this.rb.IsSleeping())
		{
			if (Mathf.Abs(base.transform.rotation.eulerAngles.x) > 0.05f || Mathf.Abs(base.transform.rotation.eulerAngles.z) > 0.05f)
			{
				Vector3 eulerAngles = base.transform.rotation.eulerAngles;
				eulerAngles.x = 0f;
				eulerAngles.z = 0f;
				this.rb.MoveRotation(Quaternion.Euler(eulerAngles));
				this.rb.angularVelocity = new Vector3(0f, this.rb.angularVelocity.y, 0f);
			}
		}
		else if (!this.rb.isKinematic)
		{
			Vector3 eulerAngles2 = base.transform.rotation.eulerAngles;
			eulerAngles2.x = 0f;
			eulerAngles2.z = 0f;
			this.rb.MoveRotation(Quaternion.Euler(eulerAngles2));
			this.rb.angularVelocity = new Vector3(0f, this.rb.angularVelocity.y, 0f);
		}
		this.actualVelocity = (base.transform.position - this.actualVelocityLastPosition) / Time.fixedDeltaTime;
		this.actualVelocityLastPosition = base.transform.position;
	}

	// Token: 0x06000DB6 RID: 3510 RVA: 0x0007C245 File Offset: 0x0007A445
	private void AutoTurnOff()
	{
		if (this.physGrabObject.playerGrabbing.Count <= 0)
		{
			this.cartActive = false;
		}
	}

	// Token: 0x06000DB7 RID: 3511 RVA: 0x0007C261 File Offset: 0x0007A461
	private void CartMassOverride(float mass)
	{
		this.physGrabObject.OverrideMass(mass, 0.1f);
	}

	// Token: 0x06000DB8 RID: 3512 RVA: 0x0007C274 File Offset: 0x0007A474
	private void CartSteer()
	{
		List<PhysGrabber> listOfAllGrabbers = this.physGrabObjectGrabArea.listOfAllGrabbers;
		foreach (PhysGrabber physGrabber in listOfAllGrabbers)
		{
			if (physGrabber)
			{
				if (physGrabber.isLocal)
				{
					TutorialDirector.instance.playerUsedCart = true;
				}
				physGrabber.OverrideGrabPoint(this.cartGrabPoint);
			}
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			foreach (PhysGrabber physGrabber2 in listOfAllGrabbers)
			{
				if (physGrabber2 && !this.inCart.GetComponent<BoxCollider>().bounds.Contains(physGrabber2.transform.position))
				{
					float d = 1f;
					float d2 = 1f;
					Rigidbody component = base.GetComponent<Rigidbody>();
					this.CartMassOverride(4f);
					if (physGrabber2 == PhysGrabber.instance)
					{
						SemiFunc.PhysGrabberLocalChangeAlpha(0.1f);
					}
					if (!this.cartActive && physGrabber2.initialPressTimer > 0f)
					{
						this.cartActive = true;
					}
					if (!this.cartActive)
					{
						break;
					}
					if (physGrabber2 != listOfAllGrabbers[0])
					{
						break;
					}
					this.cartBeingPulled = true;
					physGrabber2.physGrabForcesDisabled = true;
					float a = 2f;
					float b = 2.5f;
					if (this.isSmallCart)
					{
						a = 1.5f;
						b = 2f;
					}
					float num = 5f;
					Vector3 lhs = PlayerController.instance.rb.velocity;
					if (!physGrabber2.isLocal)
					{
						lhs = physGrabber2.playerAvatar.rbVelocityRaw;
					}
					bool flag = Vector3.Dot(lhs, base.transform.forward) > 0f;
					if (physGrabber2.playerAvatar.isSprinting)
					{
						num = 7f;
					}
					if (physGrabber2.playerAvatar.isSprinting && flag)
					{
						a = 3f;
						b = 4f;
					}
					float t = Mathf.Clamp(Vector3.Dot(component.velocity, physGrabber2.transform.forward) / num, 0f, 1f);
					float d3 = Mathf.Lerp(a, b, t);
					Vector3 a2 = physGrabber2.transform.rotation * Vector3.back;
					Vector3 a3 = physGrabber2.playerAvatar.transform.position - a2 * d3;
					float num2 = Mathf.Clamp(Vector3.Distance(base.transform.position, a3 / 1f), 0f, 1f);
					Vector3 vector = (a3 - base.transform.position).normalized * 5f * num2;
					vector = Vector3.ClampMagnitude(vector, 5f);
					float y = component.velocity.y;
					component.velocity = Vector3.MoveTowards(component.velocity, vector, num2 * 2f);
					component.velocity = new Vector3(component.velocity.x, y, component.velocity.z) * d;
					component.velocity = Vector3.ClampMagnitude(component.velocity, 5f);
					Quaternion lhs2 = Quaternion.Euler(0f, Quaternion.LookRotation(physGrabber2.transform.position - base.transform.position, Vector3.up).eulerAngles.y + 180f, 0f);
					Quaternion rotation = Quaternion.Euler(0f, component.rotation.eulerAngles.y, 0f);
					float num3;
					Vector3 vector2;
					(lhs2 * Quaternion.Inverse(rotation)).ToAngleAxis(out num3, out vector2);
					if (num3 > 180f)
					{
						num3 -= 360f;
					}
					float num4 = Mathf.Clamp(Mathf.Abs(num3) / 180f, 0.2f, 1f) * 20f;
					num4 = Mathf.Clamp(num4, 0f, 4f);
					Vector3 vector3 = 0.017453292f * num3 * vector2.normalized * num4;
					vector3 = Vector3.ClampMagnitude(vector3, 4f);
					component.angularVelocity = Vector3.MoveTowards(component.angularVelocity, vector3, num4) * d2;
					component.angularVelocity = Vector3.ClampMagnitude(component.angularVelocity, 4f);
				}
			}
		}
	}

	// Token: 0x06000DB9 RID: 3513 RVA: 0x0007C720 File Offset: 0x0007A920
	private void StateLogic()
	{
		if (!LevelGenerator.Instance.Generated)
		{
			return;
		}
		if (this.cartActive != this.cartActivePrevious)
		{
			this.cartActivePrevious = this.cartActive;
		}
		if (this.physGrabObject.playerGrabbing.Count > 0)
		{
			this.draggedTimer += Time.deltaTime;
		}
		else
		{
			this.draggedTimer = 0f;
		}
		if (this.cartActive)
		{
			this.currentState = PhysGrabCart.State.Handled;
		}
		else if (this.draggedTimer > 0.25f)
		{
			this.currentState = PhysGrabCart.State.Dragged;
		}
		else
		{
			this.currentState = PhysGrabCart.State.Locked;
		}
		if (this.currentState != this.previousState)
		{
			this.previousState = this.currentState;
			this.StateSwitch(this.currentState);
		}
	}

	// Token: 0x06000DBA RID: 3514 RVA: 0x0007C7DA File Offset: 0x0007A9DA
	private void StateSwitch(PhysGrabCart.State _state)
	{
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("StateSwitchRPC", RpcTarget.All, new object[]
			{
				_state
			});
			return;
		}
		this.StateSwitchRPC(_state);
	}

	// Token: 0x06000DBB RID: 3515 RVA: 0x0007C80C File Offset: 0x0007AA0C
	[PunRPC]
	private void StateSwitchRPC(PhysGrabCart.State _state)
	{
		this.currentState = _state;
		if (this.currentState == PhysGrabCart.State.Locked)
		{
			this.soundLocked.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.cartMesh.material.SetColor("_EmissionColor", Color.red);
			foreach (Material material in this.grabMaterial)
			{
				Color red = Color.red;
				material.SetColor("_EmissionColor", red);
				material.mainTextureOffset = new Vector2(0f, 0f);
			}
			foreach (Collider collider in this.capsuleColliders)
			{
				collider.material = this.physMaterialNormal;
			}
			using (List<Collider>.Enumerator enumerator2 = this.cartInside.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					Collider collider2 = enumerator2.Current;
					collider2.material = this.physMaterialNormal;
				}
				return;
			}
		}
		if (this.currentState == PhysGrabCart.State.Dragged)
		{
			this.soundDragged.Play(base.transform.position, 1f, 1f, 1f, 1f);
			Material material2 = this.cartMesh.material;
			Color value = new Color(1f, 0.46f, 0f);
			material2.SetColor("_EmissionColor", value);
			foreach (Material material3 in this.grabMaterial)
			{
				material3.SetColor("_EmissionColor", value);
				material3.mainTextureOffset = new Vector2(0f, 0f);
			}
			foreach (Collider collider3 in this.capsuleColliders)
			{
				collider3.material = this.physMaterialALilSlippery;
			}
			using (List<Collider>.Enumerator enumerator2 = this.cartInside.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					Collider collider4 = enumerator2.Current;
					collider4.material = this.physMaterialALilSlippery;
				}
				return;
			}
		}
		this.soundHandled.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.cartMesh.material.SetColor("_EmissionColor", Color.green);
		int num = 0;
		foreach (Material material4 in this.grabMaterial)
		{
			material4.SetColor("_EmissionColor", Color.green);
			if (num == 1)
			{
				material4.mainTextureOffset = new Vector2(0.5f, 0f);
			}
			num++;
		}
		foreach (Collider collider5 in this.capsuleColliders)
		{
			collider5.material = this.physMaterialSlippery;
		}
		foreach (Collider collider6 in this.cartInside)
		{
			collider6.material = SemiFunc.PhysicMaterialPhysGrabObject();
		}
	}

	// Token: 0x04001674 RID: 5748
	public bool isSmallCart;

	// Token: 0x04001675 RID: 5749
	public GameObject smallCartHurtCollider;

	// Token: 0x04001676 RID: 5750
	internal PhysGrabCart.State currentState;

	// Token: 0x04001677 RID: 5751
	internal PhysGrabCart.State previousState = PhysGrabCart.State.Handled;

	// Token: 0x04001678 RID: 5752
	public TextMeshPro displayText;

	// Token: 0x04001679 RID: 5753
	public Transform handlePoint;

	// Token: 0x0400167A RID: 5754
	private PhysGrabObject physGrabObject;

	// Token: 0x0400167B RID: 5755
	internal Rigidbody rb;

	// Token: 0x0400167C RID: 5756
	public float stabilizationForce = 100f;

	// Token: 0x0400167D RID: 5757
	private Vector3 hitPoint;

	// Token: 0x0400167E RID: 5758
	private PhotonView photonView;

	// Token: 0x0400167F RID: 5759
	internal bool cartActive;

	// Token: 0x04001680 RID: 5760
	private bool cartActivePrevious;

	// Token: 0x04001681 RID: 5761
	public GameObject buttonObject;

	// Token: 0x04001682 RID: 5762
	private List<Collider> capsuleColliders = new List<Collider>();

	// Token: 0x04001683 RID: 5763
	private List<Collider> cartInside = new List<Collider>();

	// Token: 0x04001684 RID: 5764
	public PhysicMaterial physMaterialSlippery;

	// Token: 0x04001685 RID: 5765
	public PhysicMaterial physMaterialSticky;

	// Token: 0x04001686 RID: 5766
	public PhysicMaterial physMaterialALilSlippery;

	// Token: 0x04001687 RID: 5767
	public PhysicMaterial physMaterialNormal;

	// Token: 0x04001688 RID: 5768
	private Vector3 velocityRef;

	// Token: 0x04001689 RID: 5769
	internal bool cartBeingPulled;

	// Token: 0x0400168A RID: 5770
	private float playerInteractionTimer;

	// Token: 0x0400168B RID: 5771
	private PhysGrabObjectGrabArea physGrabObjectGrabArea;

	// Token: 0x0400168C RID: 5772
	private MeshRenderer cartMesh;

	// Token: 0x0400168D RID: 5773
	public MeshRenderer[] grabMesh;

	// Token: 0x0400168E RID: 5774
	private List<Material> grabMaterial = new List<Material>();

	// Token: 0x0400168F RID: 5775
	[Space]
	public PhysGrabInCart physGrabInCart;

	// Token: 0x04001690 RID: 5776
	internal Transform inCart;

	// Token: 0x04001691 RID: 5777
	internal Vector3 actualVelocity;

	// Token: 0x04001692 RID: 5778
	internal Vector3 actualVelocityLastPosition;

	// Token: 0x04001693 RID: 5779
	private Vector3 lastPosition;

	// Token: 0x04001694 RID: 5780
	internal List<PhysGrabObject> itemsInCart = new List<PhysGrabObject>();

	// Token: 0x04001695 RID: 5781
	internal int itemsInCartCount;

	// Token: 0x04001696 RID: 5782
	internal int haulCurrent;

	// Token: 0x04001697 RID: 5783
	private float objectInCartCheckTimer = 0.5f;

	// Token: 0x04001698 RID: 5784
	private int haulPrevious;

	// Token: 0x04001699 RID: 5785
	private float haulUpdateEffectTimer;

	// Token: 0x0400169A RID: 5786
	private bool deductedFromHaul;

	// Token: 0x0400169B RID: 5787
	private bool resetHaulText;

	// Token: 0x0400169C RID: 5788
	private Color originalHaulColor;

	// Token: 0x0400169D RID: 5789
	public Sound soundHaulIncrease;

	// Token: 0x0400169E RID: 5790
	public Sound soundHaulDecrease;

	// Token: 0x0400169F RID: 5791
	[Space]
	public Sound soundLocked;

	// Token: 0x040016A0 RID: 5792
	public Sound soundDragged;

	// Token: 0x040016A1 RID: 5793
	public Sound soundHandled;

	// Token: 0x040016A2 RID: 5794
	private bool thirtyFPSUpdate;

	// Token: 0x040016A3 RID: 5795
	private float thirtyFPSUpdateTimer;

	// Token: 0x040016A4 RID: 5796
	private float autoTurnOffTimer;

	// Token: 0x040016A5 RID: 5797
	private float draggedTimer;

	// Token: 0x040016A6 RID: 5798
	public Transform cartGrabPoint;

	// Token: 0x040016A7 RID: 5799
	private ItemEquippable itemEquippable;

	// Token: 0x02000369 RID: 873
	public enum State
	{
		// Token: 0x04002773 RID: 10099
		Locked,
		// Token: 0x04002774 RID: 10100
		Dragged,
		// Token: 0x04002775 RID: 10101
		Handled
	}
}
