using System;
using UnityEngine;

// Token: 0x0200018E RID: 398
[RequireComponent(typeof(LineRenderer))]
public class PhysGrabBeam : MonoBehaviour
{
	// Token: 0x06000D01 RID: 3329 RVA: 0x00071DDC File Offset: 0x0006FFDC
	private void Start()
	{
		if (!this.playerAvatar.isLocal)
		{
			this.PhysGrabPointOrigin = this.PhysGrabPointOriginClient;
		}
		this.originalScrollSpeed = this.scrollSpeed;
		this.lineRenderer = base.GetComponent<LineRenderer>();
		this.originalMaterial = this.lineRenderer.material;
		this.lineMaterial = this.lineRenderer.material;
	}

	// Token: 0x06000D02 RID: 3330 RVA: 0x00071E3C File Offset: 0x0007003C
	private void LateUpdate()
	{
		this.DrawCurve();
		this.ScrollTexture();
	}

	// Token: 0x06000D03 RID: 3331 RVA: 0x00071E4A File Offset: 0x0007004A
	private void OnEnable()
	{
		this.physGrabPointPullerSmoothPosition = this.PhysGrabPointPuller.position;
		if (VideoGreenScreen.instance)
		{
			this.lineMaterial = this.greenScreenMaterial;
			base.GetComponent<LineRenderer>().material = this.greenScreenMaterial;
		}
	}

	// Token: 0x06000D04 RID: 3332 RVA: 0x00071E86 File Offset: 0x00070086
	private void OnDisable()
	{
		this.lineMaterial = this.originalMaterial;
		if (this.lineRenderer)
		{
			this.lineRenderer.material = this.originalMaterial;
		}
	}

	// Token: 0x06000D05 RID: 3333 RVA: 0x00071EB4 File Offset: 0x000700B4
	private void DrawCurve()
	{
		if (!this.PhysGrabPointPuller)
		{
			return;
		}
		Vector3[] array = new Vector3[this.CurveResolution];
		Vector3 position = this.PhysGrabPointPuller.position;
		Vector3 zero = Vector3.zero;
		this.physGrabPointPullerSmoothPosition = Vector3.Lerp(this.physGrabPointPullerSmoothPosition, position, Time.deltaTime * 10f);
		Vector3 p = this.physGrabPointPullerSmoothPosition * this.CurveStrength;
		for (int i = 0; i < this.CurveResolution; i++)
		{
			float t = (float)i / ((float)this.CurveResolution - 1f);
			array[i] = this.CalculateBezierPoint(t, this.PhysGrabPointOrigin.position, p, this.PhysGrabPoint.position);
		}
		this.lineRenderer.positionCount = this.CurveResolution;
		this.lineRenderer.SetPositions(array);
	}

	// Token: 0x06000D06 RID: 3334 RVA: 0x00071F84 File Offset: 0x00070184
	private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
	{
		return Mathf.Pow(1f - t, 2f) * p0 + 2f * (1f - t) * t * p1 + Mathf.Pow(t, 2f) * p2;
	}

	// Token: 0x06000D07 RID: 3335 RVA: 0x00071FDC File Offset: 0x000701DC
	private void ScrollTexture()
	{
		if (this.lineMaterial)
		{
			if (this.playerAvatar.physGrabber.colorState == 1)
			{
				this.lineMaterial.mainTextureScale = new Vector2(-1f, 1f);
			}
			else
			{
				this.lineMaterial.mainTextureScale = new Vector2(1f, 1f);
			}
			Vector2 mainTextureOffset = Time.time * this.scrollSpeed;
			this.lineMaterial.mainTextureOffset = mainTextureOffset;
		}
	}

	// Token: 0x040014E8 RID: 5352
	public PlayerAvatar playerAvatar;

	// Token: 0x040014E9 RID: 5353
	public Transform PhysGrabPointOrigin;

	// Token: 0x040014EA RID: 5354
	public Transform PhysGrabPointOriginClient;

	// Token: 0x040014EB RID: 5355
	public Transform PhysGrabPoint;

	// Token: 0x040014EC RID: 5356
	public Transform PhysGrabPointPuller;

	// Token: 0x040014ED RID: 5357
	public Material greenScreenMaterial;

	// Token: 0x040014EE RID: 5358
	private Material originalMaterial;

	// Token: 0x040014EF RID: 5359
	[HideInInspector]
	public Vector3 physGrabPointPullerSmoothPosition;

	// Token: 0x040014F0 RID: 5360
	public float CurveStrength = 1f;

	// Token: 0x040014F1 RID: 5361
	public int CurveResolution = 20;

	// Token: 0x040014F2 RID: 5362
	[Header("Texture Scrolling")]
	public Vector2 scrollSpeed = new Vector2(5f, 0f);

	// Token: 0x040014F3 RID: 5363
	[HideInInspector]
	public Vector2 originalScrollSpeed;

	// Token: 0x040014F4 RID: 5364
	private LineRenderer lineRenderer;

	// Token: 0x040014F5 RID: 5365
	[HideInInspector]
	public Material lineMaterial;
}
