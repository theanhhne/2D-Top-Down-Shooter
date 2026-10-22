
using System.Collections.Generic;
using UnityEngine;

public class CollectableSpawner : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _collectablePrefabs;

    // Trọng số cùng thứ tự với danh sách prefab.
    // Weight càng nhỏ thì vật phẩm càng hiếm.
    [SerializeField]
    private List<float> _weights;

    public void SpawnCollectable(Vector2 position)
    {
        if (_collectablePrefabs == null ||
            _collectablePrefabs.Count == 0)
        {
            return;
        }

        float totalWeight = 0f;

        for (int i = 0; i < _collectablePrefabs.Count; i++)
        {
            if (_collectablePrefabs[i] != null)
                totalWeight += GetWeight(i);
        }

        if (totalWeight <= 0f)
            return;

        float roll = Random.Range(0f, totalWeight);

        for (int i = 0; i < _collectablePrefabs.Count; i++)
        {
            GameObject prefab = _collectablePrefabs[i];

            if (prefab == null)
                continue;

            float weight = GetWeight(i);

            if (roll < weight)
            {
                SpawnAndNotify(prefab, position);
                return;
            }

            roll -= weight;
        }

        // Dự phòng sai số số thực.
        for (int i = _collectablePrefabs.Count - 1; i >= 0; i--)
        {
            if (_collectablePrefabs[i] != null)
            {
                SpawnAndNotify(_collectablePrefabs[i], position);
                return;
            }
        }
    }

    private void SpawnAndNotify(GameObject prefab, Vector2 position)
    {
        GameObject spawnedObject = Instantiate(
            prefab,
            position,
            Quaternion.identity
        );

        if (spawnedObject != null)
        {
            TutorialManager.Instance?.NotifyCollectableSpawned();
        }
    }

    private float GetWeight(int index)
    {
        if (_weights != null && index < _weights.Count)
            return Mathf.Max(0f, _weights[index]);

        return 1f;
    }
}
