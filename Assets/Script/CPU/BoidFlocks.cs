using UnityEngine;

namespace Bdiebeak.BoidsCPU
{
	public class BoidFlocks : MonoBehaviour
	{
		[SerializeField] private BoidSettings _settings;

		private BoidMover[] _boids;

		private void Start()
		{
			if (_settings == null)
			{
				Debug.LogError("Settings is null.");
				return;
			}

			_boids = FindObjectsOfType<BoidMover>();
			foreach (BoidMover boid in _boids)
			{
				boid.Initialize(_settings, null);
			}
		}

		private void Update()
		{
			UpdateBoidsCPU();
		}

		private void UpdateBoidsCPU()
		{
			if (_boids == null || _boids.Length == 0)
			{
				return;
			}

			for (int i = 0; i < _boids.Length; i++)
			{
				// Reset cached values.
				_boids[i].numPerceivedFlockmates = 0;
				_boids[i].avgFlockHeading = Vector3.zero;
				_boids[i].centreOfFlockmates = Vector3.zero;
				_boids[i].avgAvoidanceHeading = Vector3.zero;

				// Recalculate values.
				for (int j = 0; j < _boids.Length; j++)
				{
					if (i == j)
					{
						continue;
					}

					Vector3 offset = _boids[j].Position - _boids[i].Position;
					float sqrDst = offset.sqrMagnitude;
					if (sqrDst > _settings.perceptionRadius * _settings.perceptionRadius)
					{
						continue;
					}

					_boids[i].numPerceivedFlockmates++;
					_boids[i].avgFlockHeading += _boids[j].Forward;
					_boids[i].centreOfFlockmates += _boids[j].Position;
					if (sqrDst < _settings.avoidanceRadius * _settings.avoidanceRadius)
					{
						_boids[i].avgAvoidanceHeading -= offset / sqrDst;
					}
				}
				_boids[i].UpdateBoid();
			}
		}
	}
}