using System.Collections.Generic;
using UnityEngine;

public class DamagePopupSpawner : MonoBehaviour
{
    public static DamagePopupSpawner Instance { get; private set; }

    [Header("Prefab & Settings")]
    [SerializeField] private DamagePopup _popupPrefab;
    [SerializeField] private int _maxActive = 30;
    [SerializeField] private Transform _poolParent;
    [SerializeField] private Vector3 _extraOffset = new Vector3(0f, 0.2f, 0f);

    private readonly Queue<DamagePopup> _pool = new Queue<DamagePopup>();
    private readonly List<DamagePopup> _activePopups = new List<DamagePopup>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (_poolParent == null)
        {
            _poolParent = transform;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void Spawn(Vector3 worldPos, float damage)
    {
        if (_popupPrefab == null) return;

        Vector3 targetPos = worldPos + _extraOffset;

        // Nếu vượt quá số lượng tối đa, tái sử dụng popup cũ nhất
        if (_activePopups.Count >= _maxActive && _activePopups.Count > 0)
        {
            DamagePopup oldest = _activePopups[0];
            oldest.CancelAndReturn();
        }

        DamagePopup popup;
        if (_pool.Count > 0)
        {
            popup = _pool.Dequeue();
        }
        else
        {
            popup = Instantiate(_popupPrefab, _poolParent);
        }

        if (_poolParent != null && popup.transform.parent != _poolParent)
        {
            popup.transform.SetParent(_poolParent, true);
        }

        popup.gameObject.SetActive(true);
        popup.transform.position = targetPos;

        Debug.Log($"[DamagePopupSpawner] worldPos={worldPos}, targetPos={targetPos}, popup.position={popup.transform.position}");
        _activePopups.Add(popup);

        popup.Setup(damage, OnPopupFinished);
    }

    private void OnPopupFinished(DamagePopup popup)
    {
        if (popup == null) return;

        _activePopups.Remove(popup);
        popup.gameObject.SetActive(false);
        _pool.Enqueue(popup);
    }
}
