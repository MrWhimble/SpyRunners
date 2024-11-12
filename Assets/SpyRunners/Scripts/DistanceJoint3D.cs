using System;
using UnityEngine;

namespace SpyRunners
{
    [RequireComponent(typeof(Rigidbody))]
	public class DistanceJoint3D : MonoBehaviour
	{
		public bool enabled;
		[Min(0.001f)]
		public float distance;

		public bool maxDistanceOnly = false;

		public float springStrength;
		public float springDampening;

		/// <summary>
		/// The local position of the connection on the object the joint is part of
		/// </summary>
		//public Vector3 anchor;

		//public bool setAnchorToCenterOfMassOnAwake = true;
		/// <summary>
		/// The other Rigidbody this joint connects to<br/>
		/// Can be null, in which case the joint will connect to the ConnectedAnchor in world space<br/>
		/// Preferably set this using Connect/Disconnect
		/// </summary>
		public Rigidbody? connectedBody;
		/// <summary>
		/// The local position of the connection on the object being connected to<br/>
		/// or the world position of the connection if ConnectedRigidbody is null
		/// </summary>
		//public Vector3 connectedAnchor;
		
		private Rigidbody rigidbody;

		private Vector3 impulse;

		private void Awake()
		{
			rigidbody = GetComponent<Rigidbody>();
			//if (setAnchorToCenterOfMassOnAwake)
			//	anchor = rigidbody.centerOfMass;
		}
		
		private void FixedUpdate()
		{
			if (!enabled)
				return;

			Vector3 direction = connectedBody.worldCenterOfMass - rigidbody.worldCenterOfMass;
			float currentDistance = direction.magnitude;
			if (maxDistanceOnly && currentDistance < distance)
				return;
			direction.Normalize();

			float directionVelocity = Vector3.Dot(direction, rigidbody.velocity);
			float otherDirectionVelocity = Vector3.Dot(direction, connectedBody.velocity);

			float relativeVelocity = directionVelocity - otherDirectionVelocity;

			float distanceDelta = currentDistance - distance;

			float springForce = (distanceDelta * springStrength) - (relativeVelocity * springDampening);
			float halfSpringForce = springForce / 2f;
			
			rigidbody.AddForce(direction * halfSpringForce, ForceMode.Force);
			connectedBody.AddForce(-direction * halfSpringForce, ForceMode.Force);
		}

		public void Connect(Rigidbody? otherRigidbody, bool recalculateDistance = true)
		{
			if (otherRigidbody == null)
			{
				Disconnect();
				return;
			}

			connectedBody = otherRigidbody;
			//connectedAnchor = otherRigidbody.centerOfMass;
			if (recalculateDistance)
				distance = GetDistance();
			enabled = true;
		}

		//public void Connect(Rigidbody? otherRigidbody, Vector3 otherAnchor, bool recalculateDistance)
		//{
		//	connectedBody = otherRigidbody;
		//	//connectedAnchor = otherAnchor;
		//	if (recalculateDistance)
		//		distance = GetDistance();
		//	enabled = true;
		//}

		private float GetDistance()
		{
			//Vector3 worldAnchor = transform.TransformPoint(anchor);
			//Vector3 connectedWorldAnchor = connectedBody == null 
			//	? connectedAnchor 
			//	: connectedBody.transform.TransformPoint(connectedAnchor);
			if (connectedBody == null)
				throw new NullReferenceException();
			return Vector3.Distance(rigidbody.worldCenterOfMass, connectedBody.worldCenterOfMass);
		}

		private Vector3 GetDirection()
		{
			if (connectedBody == null)
				throw new NullReferenceException();
			return (connectedBody.worldCenterOfMass - rigidbody.worldCenterOfMass).normalized;
		}

		public void Disconnect()
		{
			connectedBody = null;
			enabled = false;
		}
	}
}