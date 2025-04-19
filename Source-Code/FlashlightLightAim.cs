using System;
using UnityEngine;

// Token: 0x020000AA RID: 170
public class FlashlightLightAim : MonoBehaviour
{
	// Token: 0x060006B4 RID: 1716 RVA: 0x000407DD File Offset: 0x0003E9DD
	private void Start()
	{
		this.lightComponent = base.GetComponent<Light>();
		if (!this.playerAvatar.isLocal)
		{
			this.lightComponent.shadowBias = 0.1f;
		}
	}

	// Token: 0x060006B5 RID: 1717 RVA: 0x00040808 File Offset: 0x0003EA08
	private void Update()
	{
		this.clientAimPointCurrent = Vector3.Lerp(this.clientAimPointCurrent, this.clientAimPoint, Time.deltaTime * 20f);
		if (!this.playerAvatar.isLocal)
		{
			Vector3 vector = this.clientAimPointCurrent - base.transform.position;
			vector = SemiFunc.ClampDirection(vector, base.transform.parent.forward, 45f);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation(vector), Time.deltaTime * 10f);
			return;
		}
		RaycastHit raycastHit;
		if (Physics.Raycast(base.transform.position, base.transform.forward, out raycastHit, 100f, SemiFunc.LayerMaskGetVisionObstruct()) && !raycastHit.transform.GetComponentInParent<PlayerController>())
		{
			this.clientAimPoint = raycastHit.point;
		}
	}

	// Token: 0x060006B6 RID: 1718 RVA: 0x000408F2 File Offset: 0x0003EAF2
	private void OnDrawGizmos()
	{
		Gizmos.color = new Color(1f, 0f, 0f, 0.25f);
		Gizmos.DrawSphere(this.clientAimPointCurrent, 0.1f);
	}

	// Token: 0x04000B61 RID: 2913
	public PlayerAvatar playerAvatar;

	// Token: 0x04000B62 RID: 2914
	public Vector3 clientAimPoint;

	// Token: 0x04000B63 RID: 2915
	private Vector3 clientAimPointCurrent;

	// Token: 0x04000B64 RID: 2916
	private Light lightComponent;

	// Token: 0x04000B65 RID: 2917
	private bool setBias;
}
