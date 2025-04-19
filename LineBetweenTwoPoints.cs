using System;
using UnityEngine;

// Token: 0x02000137 RID: 311
public class LineBetweenTwoPoints : MonoBehaviour
{
	// Token: 0x06000A8B RID: 2699 RVA: 0x0005CE54 File Offset: 0x0005B054
	private void Start()
	{
		this.line = this.lineBetweenTwoPoints.transform.Find("Line").gameObject;
		this.sphere1 = this.lineBetweenTwoPoints.transform.Find("Sphere1").gameObject;
		this.sphere2 = this.lineBetweenTwoPoints.transform.Find("Sphere2").gameObject;
		ItemDrone component = base.GetComponent<ItemDrone>();
		if (component)
		{
			this.lineColor = component.beamColor;
		}
		this.lineRenderer = this.line.GetComponent<LineRenderer>();
		this.lineRenderer.positionCount = 2;
		this.lineRenderer.enabled = false;
		if (this.sphere1)
		{
			this.sphere1.GetComponent<MeshRenderer>().enabled = false;
		}
		if (this.sphere2)
		{
			this.sphere2.GetComponent<MeshRenderer>().enabled = false;
		}
		this.lineRenderer.material.SetColor("_EmissionColor", this.lineColor);
		this.lineRenderer.material.SetColor("_Color", this.lineColor);
		this.lineRenderer.material.SetColor("_AlbedoColor", this.lineColor);
		if (this.sphere1)
		{
			this.sphere1.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", this.lineColor);
			this.sphere1.GetComponent<MeshRenderer>().material.SetColor("_Color", this.lineColor);
			this.sphere1.GetComponent<MeshRenderer>().material.SetColor("_AlbedoColor", this.lineColor);
		}
		if (this.sphere2)
		{
			this.sphere2.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", this.lineColor);
			this.sphere2.GetComponent<MeshRenderer>().material.SetColor("_Color", this.lineColor);
			this.sphere2.GetComponent<MeshRenderer>().material.SetColor("_AlbedoColor", this.lineColor);
		}
		if (!this.hasSpheres)
		{
			Object.Destroy(this.sphere1);
			Object.Destroy(this.sphere2);
		}
	}

	// Token: 0x06000A8C RID: 2700 RVA: 0x0005D08C File Offset: 0x0005B28C
	private void Update()
	{
		float num = 2f;
		this.lineRenderer.material.mainTextureOffset = new Vector2(Time.time * num, 0f);
		if (this.sphere1)
		{
			this.sphere1.GetComponent<MeshRenderer>().material.mainTextureOffset = new Vector2(Time.time * num, 0f);
		}
		if (this.sphere2)
		{
			this.sphere2.GetComponent<MeshRenderer>().material.mainTextureOffset = new Vector2(Time.time * num, 0f);
		}
		float num2 = 0.05f;
		float num3 = 0.025f;
		this.lineRenderer.startWidth = num2 + Mathf.Sin(Time.time * 5f) * num3;
		this.lineRenderer.endWidth = num2 + Mathf.Cos(Time.time * 5f) * num3;
		if (this.lineRenderer.enabled)
		{
			if (this.lineRenderLifetime <= 0f)
			{
				this.lineRenderer.enabled = false;
				this.lineRenderer.gameObject.SetActive(false);
				if (this.sphere1)
				{
					this.sphere1.GetComponent<MeshRenderer>().enabled = false;
				}
				if (this.sphere2)
				{
					this.sphere2.GetComponent<MeshRenderer>().enabled = false;
					return;
				}
			}
			else
			{
				this.lineRenderLifetime -= Time.deltaTime;
			}
		}
	}

	// Token: 0x06000A8D RID: 2701 RVA: 0x0005D1F8 File Offset: 0x0005B3F8
	public void DrawLine(Vector3 point1, Vector3 point2)
	{
		this.lineRenderer.gameObject.SetActive(true);
		this.lineRenderer.enabled = true;
		if (this.sphere1)
		{
			this.sphere1.GetComponent<MeshRenderer>().enabled = true;
			this.sphere1.transform.position = point1;
		}
		if (this.sphere2)
		{
			this.sphere2.GetComponent<MeshRenderer>().enabled = true;
			this.sphere2.transform.position = point2;
		}
		float num = 2f;
		if (this.sphere1)
		{
			this.sphere1.transform.localScale = new Vector3(this.lineRenderer.startWidth * num, this.lineRenderer.startWidth * num, this.lineRenderer.startWidth * num);
		}
		if (this.sphere2)
		{
			this.sphere2.transform.localScale = new Vector3(this.lineRenderer.endWidth * num, this.lineRenderer.endWidth * num, this.lineRenderer.endWidth * num);
		}
		this.lineRenderer.SetPosition(0, point1);
		this.lineRenderer.SetPosition(1, point2);
		this.lineRenderLifetime = 0.01f;
	}

	// Token: 0x04001118 RID: 4376
	public GameObject lineBetweenTwoPoints;

	// Token: 0x04001119 RID: 4377
	public bool hasSpheres = true;

	// Token: 0x0400111A RID: 4378
	private GameObject line;

	// Token: 0x0400111B RID: 4379
	private GameObject sphere1;

	// Token: 0x0400111C RID: 4380
	private GameObject sphere2;

	// Token: 0x0400111D RID: 4381
	public Color lineColor = Color.white;

	// Token: 0x0400111E RID: 4382
	private LineRenderer lineRenderer;

	// Token: 0x0400111F RID: 4383
	private Vector3 pointA;

	// Token: 0x04001120 RID: 4384
	private Vector3 pointB;

	// Token: 0x04001121 RID: 4385
	private float lineRenderLifetime;
}
