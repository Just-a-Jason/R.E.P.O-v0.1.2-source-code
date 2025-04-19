using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000285 RID: 645
public class ValuableWizardStaff : MonoBehaviour
{
	// Token: 0x060013F5 RID: 5109 RVA: 0x000ADCE0 File Offset: 0x000ABEE0
	private void Start()
	{
		this.photonView = base.GetComponent<PhotonView>();
		this.rb = base.GetComponent<Rigidbody>();
	}

	// Token: 0x060013F6 RID: 5110 RVA: 0x000ADCFC File Offset: 0x000ABEFC
	private void Update()
	{
		if (this.laserTimer > 0f)
		{
			this.laserTimer -= Time.deltaTime;
			Vector3 endPosition = this.laserTransform.position + this.laserTransform.forward * 15f;
			bool isHitting = false;
			RaycastHit raycastHit;
			if (Physics.Raycast(this.laserTransform.position, this.laserTransform.forward, out raycastHit, 15f, SemiFunc.LayerMaskGetVisionObstruct()))
			{
				endPosition = raycastHit.point;
				isHitting = true;
			}
			this.semiLaser.LaserActive(this.laserTransform.position, endPosition, isHitting);
		}
	}

	// Token: 0x060013F7 RID: 5111 RVA: 0x000ADDA4 File Offset: 0x000ABFA4
	private void FixedUpdate()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && this.laserTimer > 0f)
		{
			Vector3 force = -this.laserTransform.forward * 1000f * Time.fixedDeltaTime;
			this.rb.AddForce(force, ForceMode.Force);
		}
	}

	// Token: 0x060013F8 RID: 5112 RVA: 0x000ADDF8 File Offset: 0x000ABFF8
	public void StaffLaser()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			float num = Random.Range(1f, 4f);
			if (SemiFunc.IsMultiplayer())
			{
				this.photonView.RPC("StaffLaserRPC", RpcTarget.All, new object[]
				{
					num
				});
				return;
			}
			this.StaffLaserRPC(num);
		}
	}

	// Token: 0x060013F9 RID: 5113 RVA: 0x000ADE4B File Offset: 0x000AC04B
	[PunRPC]
	public void StaffLaserRPC(float _time)
	{
		this.laserTimer = _time;
	}

	// Token: 0x04002211 RID: 8721
	private PhotonView photonView;

	// Token: 0x04002212 RID: 8722
	private float laserTimer;

	// Token: 0x04002213 RID: 8723
	public SemiLaser semiLaser;

	// Token: 0x04002214 RID: 8724
	public Transform laserTransform;

	// Token: 0x04002215 RID: 8725
	private Rigidbody rb;
}
