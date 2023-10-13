using UnityEngine;

namespace Bdiebeak.BoidsCPU
{
	public class BoidMover : MonoBehaviour
	{
		public Transform target;
		public Vector3 Position { get; private set; }
		public Vector3 Forward { get; private set; }
		public bool CanMove { get; set; } = true;

		[HideInInspector] public Vector3 avgFlockHeading;
		[HideInInspector] public Vector3 avgAvoidanceHeading;
		[HideInInspector] public Vector3 centreOfFlockmates;
		[HideInInspector] public int numPerceivedFlockmates;

		private Transform _cachedTransform;
		private BoidSettings _settings;
		private Vector3 _velocity;
		private Vector3 _spawnPosition;

		private void Awake()
		{
			_cachedTransform = transform;
		}

		public void Initialize(BoidSettings settings, Vector3 spawnPosition, Transform target)
		{
			this.target = target;
			_spawnPosition = spawnPosition;
			_settings = settings;

			Position = _cachedTransform.position;
			Forward = _cachedTransform.forward;
			_velocity = transform.forward * (settings.minSpeed + settings.maxSpeed) / 2;
		}

		public void UpdateBoid()
		{
			if (_settings == null || CanMove == false)
			{
				return;
			}

			Vector3 acceleration = Vector3.zero;

			// Calculate steering towards to target if the boid has one.
			if (target != null)
			{
				acceleration = SteerTowards(target.position - Position) * _settings.targetWeight;
			}

			// Calculate steering to spawn position.
			acceleration += SteerTowards(_spawnPosition - Position) * _settings.spawnPointStayWeight;

			// Calculate steering towards to flockmates of another boids.
			if (numPerceivedFlockmates != 0)
			{
				centreOfFlockmates /= numPerceivedFlockmates;
				acceleration += SteerTowards(avgFlockHeading) * _settings.alignWeight;
				acceleration += SteerTowards(centreOfFlockmates - Position) * _settings.cohesionWeight;
				acceleration += SteerTowards(avgAvoidanceHeading) * _settings.seperateWeight;
			}

			// Calculate collision avoiding.
			if (IsHeadingForCollision())
			{
				Vector3 collisionAvoidDir = CalculateObstacleAvoidDirection();
				Vector3 collisionAvoidForce = SteerTowards(collisionAvoidDir) * _settings.avoidCollisionWeight;
				acceleration += collisionAvoidForce;
			}

			// Calculate and change boid movement.
			_velocity += acceleration * Time.deltaTime;
			_velocity = _velocity.normalized * Mathf.Clamp(_velocity.magnitude, _settings.minSpeed, _settings.maxSpeed);
			_cachedTransform.position += _velocity * Time.deltaTime;
			_cachedTransform.forward = _velocity.normalized;
			Position = _cachedTransform.position;
			Forward = _cachedTransform.forward;
		}

		private Vector3 SteerTowards(Vector3 vector)
		{
			Vector3 steer = vector.normalized * _settings.maxSpeed - _velocity;
			return Vector3.ClampMagnitude(steer, _settings.maxSteerForce);
		}

		private bool IsHeadingForCollision()
		{
			return Physics.SphereCast(Position, _settings.boundsRadius, Forward, out _,
				                      _settings.collisionAvoidDst, _settings.obstacleMask);
		}

		private Vector3 CalculateObstacleAvoidDirection()
		{
			foreach (Vector3 direction in BoidHelper.directions)
			{
				Vector3 relativeDirection = _cachedTransform.TransformDirection(direction);
				if (false == Physics.SphereCast(Position, _settings.boundsRadius, relativeDirection, out _,
												_settings.collisionAvoidDst, _settings.obstacleMask))
				{
					return relativeDirection;
				}
			}
			return Forward;
		}

		private void OnDrawGizmosSelected()
		{
			// Check forward collision.
			if (Physics.SphereCast(Position, _settings.boundsRadius, Forward, out _,
								   _settings.collisionAvoidDst, _settings.obstacleMask))
			{
				Gizmos.color = Color.red;
			}
			else
			{
				Gizmos.color = Color.green;
			}
			Gizmos.DrawSphere(Position, _settings.boundsRadius);

			// Check possible way.
			foreach (Vector3 direction in BoidHelper.directions)
			{
				if (Physics.SphereCast(Position, _settings.boundsRadius, direction, out _,
									   _settings.collisionAvoidDst, _settings.obstacleMask))
				{
					Gizmos.color = Color.yellow;
				}
				else
				{
					Gizmos.color = Color.green;
				}
				Gizmos.DrawRay(Position, direction * _settings.collisionAvoidDst);
			}
		}
	}
}