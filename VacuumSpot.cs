using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000D0 RID: 208
public class VacuumSpot : MonoBehaviour
{
	// Token: 0x06000748 RID: 1864 RVA: 0x00045498 File Offset: 0x00043698
	private void Start()
	{
		this.LightIntensity = this.Light.intensity;
		this.photonView = base.GetComponent<PhotonView>();
		this.PileRendererAlpha = this.PileRenderer.material.color.a;
		this.DecalRendererAlpha = this.DecalRenderer.material.color.a;
	}

	// Token: 0x06000749 RID: 1865 RVA: 0x000454F8 File Offset: 0x000436F8
	[PunRPC]
	private void StartCleaningRPC()
	{
		this.multiplayerCleaning = true;
	}

	// Token: 0x0600074A RID: 1866 RVA: 0x00045501 File Offset: 0x00043701
	[PunRPC]
	private void StopCleaningRPC()
	{
		this.multiplayerCleaning = false;
	}

	// Token: 0x0600074B RID: 1867 RVA: 0x0004550C File Offset: 0x0004370C
	private void Update()
	{
		if (this.DecreaseTimer > 0f)
		{
			this.Amount -= this.DecreaseSpeed * Time.deltaTime;
			if (this.Amount > 0.2f)
			{
				this.DecreaseTimer -= 1f * Time.deltaTime;
			}
			float num = Mathf.Lerp(0f, 1f, this.AlphaCurve.Evaluate(this.Amount));
			Color color = this.PileRenderer.material.color;
			color.a = this.PileRendererAlpha * num;
			this.PileRenderer.material.color = color;
			Color color2 = this.DecalRenderer.material.color;
			color2.a = this.DecalRendererAlpha * num;
			this.DecalRenderer.material.color = color2;
			float num2 = Mathf.Lerp(0f, 1f, this.ScaleCurve.Evaluate(this.Amount));
			this.PileMesh.localScale = new Vector3(1f - (1f - num2) * 0.4f, 0.5f + num2 * 0.5f, 1f - (1f - num2) * 0.4f);
			this.DecalMesh.localScale = new Vector3(1f - (1f - num2) * 0.2f, 1f, 1f - (1f - num2) * 0.2f);
			this.Light.intensity = this.LightIntensity * num2;
			if (this.Amount <= 0f)
			{
				this.CleanEffect.SetActive(true);
				this.CleanEffect.GetComponent<CleanEffect>().Clean();
				this.CleanEffect.transform.parent = null;
				if (GameManager.instance.gameMode == 1)
				{
					if (PhotonNetwork.IsMasterClient && !this.syncDestroy)
					{
						PhotonNetwork.Destroy(base.gameObject);
						this.syncDestroy = true;
					}
				}
				else
				{
					Object.Destroy(base.gameObject);
				}
			}
		}
		if (GameManager.instance.gameMode == 1)
		{
			if (this.cleanInput && this.cleanInput != this.cleanInputPrevious)
			{
				this.photonView.RPC("StartCleaningRPC", RpcTarget.All, Array.Empty<object>());
			}
			if (!this.cleanInput && this.cleanInput != this.cleanInputPrevious)
			{
				this.photonView.RPC("StopCleaningRPC", RpcTarget.All, Array.Empty<object>());
			}
			this.cleanInputPrevious = this.cleanInput;
			this.cleanInput = this.multiplayerCleaning;
		}
		if (this.cleanInput)
		{
			this.DecreaseTimer = 0.1f;
			this.cleanInput = false;
		}
	}

	// Token: 0x04000CCD RID: 3277
	public GameObject VacuumSpotVisual;

	// Token: 0x04000CCE RID: 3278
	[HideInInspector]
	public float Amount = 1f;

	// Token: 0x04000CCF RID: 3279
	public float DecreaseSpeed;

	// Token: 0x04000CD0 RID: 3280
	[HideInInspector]
	public bool CleanDone;

	// Token: 0x04000CD1 RID: 3281
	[Space]
	public Transform PileMesh;

	// Token: 0x04000CD2 RID: 3282
	public MeshRenderer PileRenderer;

	// Token: 0x04000CD3 RID: 3283
	private float PileRendererAlpha;

	// Token: 0x04000CD4 RID: 3284
	[Space]
	public Transform DecalMesh;

	// Token: 0x04000CD5 RID: 3285
	public MeshRenderer DecalRenderer;

	// Token: 0x04000CD6 RID: 3286
	private float DecalRendererAlpha;

	// Token: 0x04000CD7 RID: 3287
	[Space]
	public AnimationCurve ScaleCurve;

	// Token: 0x04000CD8 RID: 3288
	public AnimationCurve AlphaCurve;

	// Token: 0x04000CD9 RID: 3289
	public Light Light;

	// Token: 0x04000CDA RID: 3290
	private float LightIntensity;

	// Token: 0x04000CDB RID: 3291
	[HideInInspector]
	public bool Decreasing;

	// Token: 0x04000CDC RID: 3292
	[HideInInspector]
	public float DecreaseTimer;

	// Token: 0x04000CDD RID: 3293
	public GameObject CleanEffect;

	// Token: 0x04000CDE RID: 3294
	private bool multiplayerCleaning;

	// Token: 0x04000CDF RID: 3295
	private PhotonView photonView;

	// Token: 0x04000CE0 RID: 3296
	private bool cleanInputPrevious;

	// Token: 0x04000CE1 RID: 3297
	public bool cleanInput;

	// Token: 0x04000CE2 RID: 3298
	private bool syncDestroy;
}
