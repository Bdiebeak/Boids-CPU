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
			if (_boids == null || _boids.Length == 0)
			{
				return;
			}

			UpdateBoids();
		}

		private void UpdateBoids()
		{
			int numBoids = _boids.Length;
			var boids = new Boid[numBoids];
			for (int i = 0; i < numBoids; i++)
			{
				boids[i] = new()
				{
					position = _boids[i].Position,
					forward = _boids[i].Forward
				};
			}

			for (int i = 0; i < numBoids; i++)
			{
				for (int j = 0; j < numBoids; j++)
				{
					if (i != j)
					{
						Vector3 offset = boids[j].position - boids[i].position;
						float sqrDst = offset.sqrMagnitude;

						if (sqrDst < _settings.perceptionRadius * _settings.perceptionRadius)
						{
							boids[i].numPerceivedFlockmates++;
							boids[i].avgFlockHeading += boids[j].forward;
							boids[i].centreOfFlockmates += boids[j].position;

							if (sqrDst < _settings.avoidanceRadius * _settings.avoidanceRadius)
							{
								boids[i].avgAvoidanceHeading -= offset / sqrDst;
							}
						}
					}
				}
			}

			for (int i = 0; i < boids.Length; i++)
			{
				_boids[i].avgFlockHeading = boids[i].avgFlockHeading;
				_boids[i].centreOfFlockmates = boids[i].centreOfFlockmates;
				_boids[i].avgAvoidanceHeading = boids[i].avgAvoidanceHeading;
				_boids[i].numPerceivedFlockmates = boids[i].numPerceivedFlockmates;
				_boids[i].UpdateBoid();
			}
		}

		public class Boid
		{
			public Vector3 position;
			public Vector3 forward;

			public Vector3 avgFlockHeading;
			public Vector3 centreOfFlockmates;
			public Vector3 avgAvoidanceHeading;
			public int numPerceivedFlockmates;
		}
	}
}