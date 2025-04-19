using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000B3 RID: 179
public class CanvasHandler : MonoBehaviour
{
	// Token: 0x060006CE RID: 1742 RVA: 0x00040CDC File Offset: 0x0003EEDC
	private void Start()
	{
		this.dirt1Renderer = this.Dirt1.GetComponent<MeshRenderer>();
		this.dirt2Renderer = this.Dirt2.GetComponent<MeshRenderer>();
		this.dirtHangRenderer = this.DirtHang.GetComponent<MeshRenderer>();
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x060006CF RID: 1743 RVA: 0x00040D28 File Offset: 0x0003EF28
	[PunRPC]
	private void StartCleaningRPC()
	{
		this.multiplayerCleaning = true;
	}

	// Token: 0x060006D0 RID: 1744 RVA: 0x00040D31 File Offset: 0x0003EF31
	[PunRPC]
	private void StopCleaningRPC()
	{
		this.multiplayerCleaning = false;
	}

	// Token: 0x060006D1 RID: 1745 RVA: 0x00040D3C File Offset: 0x0003EF3C
	[PunRPC]
	private void CleaningDoneRPC()
	{
		this.currentState = CanvasHandler.State.Clean;
		this.cleanEffect.Clean();
		this.InteractableLight.StartFading();
		this.InteractionArea.SetActive(false);
		this.SetMaterialAlpha(this.dirt1Renderer, 0f);
		this.SetMaterialAlpha(this.dirt2Renderer, 0f);
		this.SetMaterialAlpha(this.dirtHangRenderer, 0f);
	}

	// Token: 0x060006D2 RID: 1746 RVA: 0x00040DA8 File Offset: 0x0003EFA8
	private void Update()
	{
		if (this.isCleaningTimer > 0f)
		{
			this.isCleaning = true;
			this.isCleaningTimer -= 1f * Time.deltaTime;
		}
		else
		{
			this.isCleaning = false;
		}
		if (this.currentState == CanvasHandler.State.Clean)
		{
			this.isCleaning = false;
		}
		if ((double)this.fadeMultiplier < 0.5 && this.fadeMultiplier != 0f)
		{
			this.isCleaning = true;
		}
		if (this.isCleaning)
		{
			this.cleanStateTimer = 0f;
			if (this.currentState == CanvasHandler.State.Dirty || this.currentState == CanvasHandler.State.Cleaning)
			{
				if (this.currentState != CanvasHandler.State.Cleaning)
				{
					this.StartWiggle();
					this.currentState = CanvasHandler.State.Cleaning;
				}
			}
			else
			{
				this.isCleaning = false;
			}
		}
		if (this.currentState == CanvasHandler.State.Cleaning)
		{
			if (!this.DebugNoClean)
			{
				if ((double)this.fadeMultiplier > 0.5)
				{
					this.fadeMultiplier -= this.cleaningSpeed * Time.deltaTime;
				}
				else
				{
					this.fadeMultiplier -= this.cleaningSpeed * 2f * Time.deltaTime;
				}
			}
			this.SetMaterialAlpha(this.dirt1Renderer, this.fadeMultiplier);
			this.SetMaterialAlpha(this.dirt2Renderer, this.fadeMultiplier);
			this.SetMaterialAlpha(this.dirtHangRenderer, this.fadeMultiplier);
			this.cleanStateTimer += 1f * Time.deltaTime;
			if ((double)this.cleanStateTimer > 0.2)
			{
				this.PaintingSwingEnd.Play(base.transform.position, 1f, 1f, 1f, 1f);
				this.currentState = CanvasHandler.State.Dirty;
			}
			if (this.fadeMultiplier < 0f)
			{
				this.PaintingSwingEnd.Play(base.transform.position, 1f, 1f, 1f, 1f);
				this.currentState = CanvasHandler.State.Clean;
				this.cleanEffect.Clean();
				this.InteractableLight.StartFading();
				this.fadeMultiplier = 0f;
				this.InteractionArea.SetActive(false);
			}
		}
		if (this.currentState != CanvasHandler.State.Cleaning)
		{
			this.StopWiggle();
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
			if (this.currentState == CanvasHandler.State.Clean && this.currentState != this.previousState)
			{
				this.photonView.RPC("CleaningDoneRPC", RpcTarget.AllBuffered, Array.Empty<object>());
				this.previousState = this.currentState;
			}
			this.cleanInputPrevious = this.cleanInput;
			this.cleanInput = this.multiplayerCleaning;
		}
		this.PaintingSwingLoop.PlayLoop(this.isCleaning, 1f, 2f, 1f);
		this.isCleaning = false;
		if (this.cleanInput)
		{
			this.isCleaningTimer = 0.1f;
			this.cleanInput = false;
		}
	}

	// Token: 0x060006D3 RID: 1747 RVA: 0x000410CC File Offset: 0x0003F2CC
	private void SetMaterialAlpha(MeshRenderer renderer, float alpha)
	{
		Color color = renderer.material.color;
		color.a = Mathf.Clamp(alpha, 0f, 1f);
		renderer.material.color = color;
	}

	// Token: 0x060006D4 RID: 1748 RVA: 0x00041108 File Offset: 0x0003F308
	private void StartWiggle()
	{
		this.isWiggling = true;
		this.DustParticles.Play();
		this.currentWiggleAmount = this.wiggleAmount;
		base.StopAllCoroutines();
		base.StartCoroutine(this.WiggleCoroutine());
	}

	// Token: 0x060006D5 RID: 1749 RVA: 0x0004113B File Offset: 0x0003F33B
	private void StopWiggle()
	{
		this.DustParticles.Stop();
		this.isWiggling = false;
	}

	// Token: 0x060006D6 RID: 1750 RVA: 0x0004114F File Offset: 0x0003F34F
	private IEnumerator WiggleCoroutine()
	{
		float time = 0f;
		float currentZRotation = this.Painting.transform.localRotation.eulerAngles.z;
		if (currentZRotation > 180f)
		{
			currentZRotation -= 360f;
		}
		float phaseOffset = Mathf.Asin(currentZRotation / this.wiggleAmount) - this.wiggleSpeed * time;
		float lerpFactor = 0.15f;
		while (this.isWiggling || Mathf.Abs(this.currentWiggleAmount) > 0.1f)
		{
			float b = Mathf.Sin(time * this.wiggleSpeed + phaseOffset) * this.currentWiggleAmount;
			float num = Mathf.Lerp(currentZRotation, b, lerpFactor);
			this.Painting.transform.localRotation = Quaternion.Euler(0f, 0f, num);
			currentZRotation = num;
			if (!this.isWiggling)
			{
				this.currentWiggleAmount *= this.dampening;
			}
			time += Time.deltaTime;
			yield return null;
		}
		yield break;
	}

	// Token: 0x04000B78 RID: 2936
	public bool DebugNoClean;

	// Token: 0x04000B79 RID: 2937
	private PhotonView photonView;

	// Token: 0x04000B7A RID: 2938
	[Header("Connected Objects")]
	public GameObject Dirt1;

	// Token: 0x04000B7B RID: 2939
	public GameObject Dirt2;

	// Token: 0x04000B7C RID: 2940
	public GameObject DirtHang;

	// Token: 0x04000B7D RID: 2941
	public GameObject Painting;

	// Token: 0x04000B7E RID: 2942
	public ParticleSystem DustParticles;

	// Token: 0x04000B7F RID: 2943
	public CleanEffect cleanEffect;

	// Token: 0x04000B80 RID: 2944
	public GameObject InteractionArea;

	// Token: 0x04000B81 RID: 2945
	public LightInteractableFadeRemove InteractableLight;

	// Token: 0x04000B82 RID: 2946
	[Space]
	[Header("Sounds")]
	public Sound PaintingSwingLoop;

	// Token: 0x04000B83 RID: 2947
	public Sound PaintingSwingEnd;

	// Token: 0x04000B84 RID: 2948
	[Space]
	[Header("Painting Swing Settings")]
	public float wiggleSpeed = 20f;

	// Token: 0x04000B85 RID: 2949
	public float wiggleAmount = 5f;

	// Token: 0x04000B86 RID: 2950
	public float dampening = 0.95f;

	// Token: 0x04000B87 RID: 2951
	private bool isWiggling;

	// Token: 0x04000B88 RID: 2952
	private float currentWiggleAmount;

	// Token: 0x04000B89 RID: 2953
	[Space]
	[Header("Cleaning Settings")]
	private float cleanStateTimer;

	// Token: 0x04000B8A RID: 2954
	public bool isCleaning;

	// Token: 0x04000B8B RID: 2955
	public bool isCleaningPrevious;

	// Token: 0x04000B8C RID: 2956
	[HideInInspector]
	public float isCleaningTimer;

	// Token: 0x04000B8D RID: 2957
	private bool cleanInputPrevious;

	// Token: 0x04000B8E RID: 2958
	public bool cleanInput;

	// Token: 0x04000B8F RID: 2959
	[HideInInspector]
	public bool CleanDone;

	// Token: 0x04000B90 RID: 2960
	private bool multiplayerCleaning;

	// Token: 0x04000B91 RID: 2961
	public CanvasHandler.State currentState;

	// Token: 0x04000B92 RID: 2962
	public CanvasHandler.State previousState;

	// Token: 0x04000B93 RID: 2963
	[HideInInspector]
	public float fadeMultiplier = 1f;

	// Token: 0x04000B94 RID: 2964
	public float cleaningSpeed = 0.1f;

	// Token: 0x04000B95 RID: 2965
	private MeshRenderer dirt1Renderer;

	// Token: 0x04000B96 RID: 2966
	private MeshRenderer dirt2Renderer;

	// Token: 0x04000B97 RID: 2967
	private MeshRenderer dirtHangRenderer;

	// Token: 0x020002F3 RID: 755
	public enum State
	{
		// Token: 0x04002528 RID: 9512
		Dirty,
		// Token: 0x04002529 RID: 9513
		Cleaning,
		// Token: 0x0400252A RID: 9514
		Clean
	}
}
