using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200009E RID: 158
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyNavMeshAgent : MonoBehaviour
{
	// Token: 0x06000608 RID: 1544 RVA: 0x0003AB68 File Offset: 0x00038D68
	private void Awake()
	{
		this.Agent = base.GetComponent<NavMeshAgent>();
		if (!this.updateRotation)
		{
			this.Agent.updateRotation = false;
		}
		if (GameManager.instance.gameMode == 0 || PhotonNetwork.IsMasterClient)
		{
			this.Agent.enabled = true;
		}
		else
		{
			this.Agent.enabled = false;
		}
		this.DefaultSpeed = this.Agent.speed;
		this.DefaultAcceleration = this.Agent.acceleration;
	}

	// Token: 0x06000609 RID: 1545 RVA: 0x0003ABE4 File Offset: 0x00038DE4
	private void Update()
	{
		this.AgentVelocity = this.Agent.velocity;
		if (this.SetPathTimer > 0f)
		{
			this.SetPathTimer -= Time.deltaTime;
		}
		if (this.DisableTimer > 0f)
		{
			this.Agent.enabled = false;
			this.DisableTimer -= Time.deltaTime;
			return;
		}
		if (!this.Agent.enabled)
		{
			this.Agent.enabled = true;
		}
		if (this.StopTimer > 0f)
		{
			this.Agent.isStopped = true;
			this.StopTimer -= Time.deltaTime;
		}
		else if (this.Agent.enabled && this.Agent.isStopped)
		{
			this.Agent.isStopped = false;
		}
		if (this.OverrideTimer > 0f)
		{
			this.OverrideTimer -= Time.deltaTime;
			if (this.OverrideTimer <= 0f)
			{
				this.Agent.speed = this.DefaultSpeed;
				this.Agent.acceleration = this.DefaultAcceleration;
			}
		}
	}

	// Token: 0x0600060A RID: 1546 RVA: 0x0003AD07 File Offset: 0x00038F07
	public void OverrideAgent(float speed, float acceleration, float time)
	{
		this.Agent.speed = speed;
		this.Agent.acceleration = acceleration;
		this.OverrideTimer = time;
	}

	// Token: 0x0600060B RID: 1547 RVA: 0x0003AD28 File Offset: 0x00038F28
	public void UpdateAgent(float speed, float acceleration)
	{
		this.Agent.speed = speed;
		this.Agent.acceleration = acceleration;
	}

	// Token: 0x0600060C RID: 1548 RVA: 0x0003AD44 File Offset: 0x00038F44
	public void AgentMove(Vector3 position)
	{
		Vector3 velocity = this.Agent.velocity;
		Vector3 destination = this.Agent.destination;
		if (!this.OnNavmesh(position))
		{
			return;
		}
		this.Warp(position);
		this.SetDestination(destination);
		this.Agent.velocity = velocity;
	}

	// Token: 0x0600060D RID: 1549 RVA: 0x0003AD90 File Offset: 0x00038F90
	private bool OnNavmesh(Vector3 position)
	{
		NavMeshHit navMeshHit;
		return NavMesh.SamplePosition(position, out navMeshHit, 5f, -1);
	}

	// Token: 0x0600060E RID: 1550 RVA: 0x0003ADAC File Offset: 0x00038FAC
	public void Warp(Vector3 position)
	{
		if (Vector3.Distance(base.transform.position, position) < 1f)
		{
			return;
		}
		if (this.DisableTimer > 0f)
		{
			this.Agent.enabled = true;
		}
		if (!this.OnNavmesh(position))
		{
			return;
		}
		this.Agent.Warp(position);
		if (this.DisableTimer > 0f)
		{
			this.Agent.enabled = false;
		}
	}

	// Token: 0x0600060F RID: 1551 RVA: 0x0003AE1B File Offset: 0x0003901B
	public void ResetPath()
	{
		if (!this.Agent.enabled)
		{
			return;
		}
		if (!this.HasPath())
		{
			return;
		}
		this.Agent.ResetPath();
	}

	// Token: 0x06000610 RID: 1552 RVA: 0x0003AE3F File Offset: 0x0003903F
	public bool CanReach(Vector3 _target, float _range)
	{
		return !this.Agent.enabled || !this.Agent.hasPath || Vector3.Distance(this.GetPoint(), _target) <= _range;
	}

	// Token: 0x06000611 RID: 1553 RVA: 0x0003AE71 File Offset: 0x00039071
	public void SetDestination(Vector3 position)
	{
		if (!this.Agent.enabled)
		{
			return;
		}
		if (!this.Agent.hasPath)
		{
			this.SetPathTimer = 0.1f;
		}
		this.Agent.SetDestination(position);
	}

	// Token: 0x06000612 RID: 1554 RVA: 0x0003AEA6 File Offset: 0x000390A6
	public void Stop(float time)
	{
		if (!this.Agent.enabled)
		{
			return;
		}
		this.StopTimer = time;
		if (this.StopTimer == 0f)
		{
			this.Agent.isStopped = false;
			return;
		}
		this.Agent.isStopped = true;
	}

	// Token: 0x06000613 RID: 1555 RVA: 0x0003AEE3 File Offset: 0x000390E3
	public bool IsStopped()
	{
		return this.StopTimer > 0f;
	}

	// Token: 0x06000614 RID: 1556 RVA: 0x0003AEF5 File Offset: 0x000390F5
	public void Disable(float time)
	{
		this.Agent.enabled = false;
		this.DisableTimer = time;
	}

	// Token: 0x06000615 RID: 1557 RVA: 0x0003AF0A File Offset: 0x0003910A
	public void Enable()
	{
		if (this.DisableTimer > 0f)
		{
			this.Agent.enabled = true;
			this.DisableTimer = 0f;
		}
	}

	// Token: 0x06000616 RID: 1558 RVA: 0x0003AF30 File Offset: 0x00039130
	public bool IsDisabled()
	{
		return this.DisableTimer > 0f;
	}

	// Token: 0x06000617 RID: 1559 RVA: 0x0003AF44 File Offset: 0x00039144
	public Vector3 GetPoint()
	{
		if (this.Agent.hasPath)
		{
			return this.Agent.path.corners[this.Agent.path.corners.Length - 1];
		}
		return new Vector3(-1000f, 1000f, 1000f);
	}

	// Token: 0x06000618 RID: 1560 RVA: 0x0003AF9C File Offset: 0x0003919C
	public Vector3 GetDestination()
	{
		if (this.Agent.hasPath)
		{
			return this.Agent.destination;
		}
		return base.transform.position;
	}

	// Token: 0x06000619 RID: 1561 RVA: 0x0003AFC2 File Offset: 0x000391C2
	public bool HasPath()
	{
		return this.SetPathTimer > 0f || this.Agent.hasPath;
	}

	// Token: 0x0600061A RID: 1562 RVA: 0x0003AFE4 File Offset: 0x000391E4
	public NavMeshPath CalculatePath(Vector3 position)
	{
		NavMeshPath navMeshPath = new NavMeshPath();
		if (!this.Agent.enabled)
		{
			return navMeshPath;
		}
		this.Agent.CalculatePath(position, navMeshPath);
		return navMeshPath;
	}

	// Token: 0x04000A01 RID: 2561
	internal NavMeshAgent Agent;

	// Token: 0x04000A02 RID: 2562
	internal Vector3 AgentVelocity;

	// Token: 0x04000A03 RID: 2563
	public bool updateRotation;

	// Token: 0x04000A04 RID: 2564
	private float StopTimer;

	// Token: 0x04000A05 RID: 2565
	private float DisableTimer;

	// Token: 0x04000A06 RID: 2566
	internal float DefaultSpeed;

	// Token: 0x04000A07 RID: 2567
	internal float DefaultAcceleration;

	// Token: 0x04000A08 RID: 2568
	private float OverrideTimer;

	// Token: 0x04000A09 RID: 2569
	private float SetPathTimer;
}
