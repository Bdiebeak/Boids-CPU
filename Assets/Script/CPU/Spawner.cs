using System.Collections.Generic;
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
        [SerializeField]
        private List<Transform> _spawnPoints = new();

        private void Awake()
        {
            if (_spawnPoints.Count == 0)
            {
                _spawnPoints.Add(transform);
            }
            SpawnBoids();
        }

        private void SpawnBoids()
        {
            int objectsPerSpawnPoint = _spawnCount / _spawnPoints.Count;
            int remainder = _spawnCount % _spawnPoints.Count;

            for (int i = 0; i < _spawnPoints.Count; i++)
            {
                Transform spawnPoint = _spawnPoints[i];
                int spawnCount = objectsPerSpawnPoint + (i == _spawnPoints.Count - 1 ? remainder : 0);
                for (int j = 0; j < spawnCount; j++)
                {
                    SpawnBoid(spawnPoint.position + Random.insideUnitSphere * _spawnRadius);
                }
            }
        }

        public float spawnCount;
        private void SpawnBoid(Vector3 spawnPoint)
        {
            Transform boidTransform = Instantiate(_boidPrefab).transform;
            boidTransform.position =  spawnPoint;
            boidTransform.forward = Random.insideUnitSphere;
            spawnCount++;
        }
    }
}