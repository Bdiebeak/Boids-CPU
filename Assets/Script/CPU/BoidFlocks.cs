using UnityEngine;

namespace Bdiebeak.BoidsCPU
{
	public class BoidFlocks : MonoBehaviour
	{
		[SerializeField] private BoidSettings _settings;

		private BoidMover[] _boids;

		private void Start()
		{
			_boids = FindObjectsOfType<BoidMover>();
			foreach (BoidMover boid in _boids)
			{
				boid.Initialize(_settings, null);
			}
		}

		private void Update()
		{
			HandleBoidsMovement();
		}

		private void HandleBoidsMovement()
		{
			if (_boids == null || _boids.Length == 0)
			{
				return;
			}

			int numBoids = _boids.Length;
			for (int indexA = 0; indexA < numBoids; indexA++)
			{
				BoidMover boidA = _boids[indexA];
				for (int indexB = indexA; indexB < numBoids; indexB++)
				{
					if (indexA == indexB)
					{
						continue;
					}

					BoidMover boidB = _boids[indexB];
					Vector3 offset = boidB.Position - boidA.Position;
					float sqrDst = offset.x * offset.x + offset.y * offset.y + offset.z * offset.z;

					if (sqrDst < _settings.perceptionRadius * _settings.perceptionRadius)
					{
						boidA.numPerceivedFlockmates += 1;
						boidA.avgFlockHeading += boidB.Forward;
						boidA.centreOfFlockmates += boidB.Position;

						if (sqrDst < _settings.avoidanceRadius * _settings.avoidanceRadius)
						{
							boidA.avgAvoidanceHeading -= offset / sqrDst;
						}
					}
				}
				boidA.UpdateBoid();
			}
		}
	}
}