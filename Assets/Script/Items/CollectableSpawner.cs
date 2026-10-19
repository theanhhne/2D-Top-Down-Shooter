using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableSpawner : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _collectablePrefabs;

    // Trọng số cho từng prefab, cùng thứ tự với _collectablePrefabs.
    // Weight càng nhỏ thì càng hiếm. Phần tử thiếu sẽ tính là 1.
    [SerializeField]
    private List<float> _weights;

    public void SpawnCollectable(Vector2 position)
    {
        if (_collectablePrefabs == null || _collectablePrefabs.Count == 0)
        {
            return;
        }

        float totalWeight = 0f;
        for (int i = 0; i < _collectablePrefabs.Count; i++)
        {
            totalWeight += GetWeight(i);
        }

        float roll = Random.Range(0f, totalWeight);

        for (int i = 0; i < _collectablePrefabs.Count; i++)
        {
            float weight = GetWeight(i);

            if (roll < weight)
            {
                Instantiate(_collectablePrefabs[i], position, Quaternion.identity);
                return;
            }

            roll -= weight;
        }

        // Dự phòng khi sai số số thực
        Instantiate(_collectablePrefabs[_collectablePrefabs.Count - 1], position, Quaternion.identity);
    }

    private float GetWeight(int index)
    {
        if (_weights != null && index < _weights.Count)
        {
            return Mathf.Max(0f, _weights[index]);
        }

        return 1f;
    }
}