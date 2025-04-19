using System;
using UnityEngine;

// Token: 0x020001A4 RID: 420
public class PlayerAvatarMenu : MonoBehaviour
{
	// Token: 0x06000E08 RID: 3592 RVA: 0x0007E844 File Offset: 0x0007CA44
	private void Awake()
	{
		this.startPosition = new Vector3(0f, 0f, -2000f);
		if (PlayerAvatarMenu.instance && PlayerAvatarMenu.instance.startPosition == this.startPosition)
		{
			this.startPosition = new Vector3(0f, 4f, -2000f);
		}
		this.playerVisuals = base.GetComponentInChildren<PlayerAvatarVisuals>();
		PlayerAvatarMenu.instance = this;
	}

	// Token: 0x06000E09 RID: 3593 RVA: 0x0007E8BC File Offset: 0x0007CABC
	private void Start()
	{
		this.rb = base.GetComponent<Rigidbody>();
		this.parentPage = base.GetComponentInParent<MenuPage>();
		base.transform.SetParent(null);
		base.transform.localScale = Vector3.one;
		base.transform.position = this.startPosition;
		this.cameraAndStuff.SetParent(null);
		this.cameraAndStuff.localScale = Vector3.one;
	}

	// Token: 0x06000E0A RID: 3594 RVA: 0x0007E92C File Offset: 0x0007CB2C
	private void FixedUpdate()
	{
		this.rb.MovePosition(this.startPosition);
		if (this.rotationForce.magnitude > 0.1f)
		{
			this.rb.AddTorque(this.rotationForce * Time.fixedDeltaTime);
			this.rotationForce = Vector3.zero;
			this.rb.angularVelocity = Vector3.ClampMagnitude(this.rb.angularVelocity, 1f);
		}
	}

	// Token: 0x06000E0B RID: 3595 RVA: 0x0007E9A4 File Offset: 0x0007CBA4
	private void Update()
	{
		if (SemiFunc.InputMovementX() > 0.01f || SemiFunc.InputMovementX() < -0.01f)
		{
			float y = -SemiFunc.InputMovementX() * 3000f;
			this.Rotate(new Vector3(0f, y, 0f));
		}
		if (!this.parentPage)
		{
			Object.Destroy(this.cameraAndStuff.gameObject);
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06000E0C RID: 3596 RVA: 0x0007EA14 File Offset: 0x0007CC14
	public void Rotate(Vector3 _rotation)
	{
		this.rotationForce = _rotation;
	}

	// Token: 0x040016F6 RID: 5878
	public static PlayerAvatarMenu instance;

	// Token: 0x040016F7 RID: 5879
	public Transform cameraAndStuff;

	// Token: 0x040016F8 RID: 5880
	private MenuPage parentPage;

	// Token: 0x040016F9 RID: 5881
	private Vector3 startPosition;

	// Token: 0x040016FA RID: 5882
	internal Rigidbody rb;

	// Token: 0x040016FB RID: 5883
	private Vector3 rotationForce;

	// Token: 0x040016FC RID: 5884
	internal PlayerAvatarVisuals playerVisuals;
}
