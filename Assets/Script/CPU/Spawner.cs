using UnityEngine;

namespace Bdiebeak.BoidsCPU
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField]
        private BoidMover _boidPrefab;
        [SerializeField]
        private float _spawnRadius = 5;
        [SerializeField]
        private int _spawnCount = 10;

        private void Awake()
        {
            SpawnBoids();
        }

        private void SpawnBoids()
        {
            for (int i = 0; i < _spawnCount; i++)
            {
                Transform boidTransform = Instantiate(_boidPrefab).transform;
                boidTransform.position =  transform.position + Random.insideUnitSphere * _spawnRadius;
                boidTransform.forward = Random.insideUnitSphere;
            }
        }
    }
}