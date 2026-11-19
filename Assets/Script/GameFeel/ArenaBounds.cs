using UnityEngine;

/// Biên của đấu trường theo tọa độ world. Dùng chung cho người chơi, enemy và camera.
/// Mặc định khớp với sprite Background (21.6 x 32.4 đơn vị, tâm ở 0,0).
public class ArenaBounds : MonoBehaviour
{
    public static ArenaBounds Instance { get; private set; }

    [SerializeField] private Vector2 _min = new Vector2(-10.8f, -16.2f);
    [SerializeField] private Vector2 _max = new Vector2(10.8f, 16.2f);

    public Vector2 Min => _min;
    public Vector2 Max => _max;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Có hơn một ArenaBounds trong scene, bỏ qua cái này.", this);
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 centre = (_min + _max) * 0.5f;
        Vector3 size = _max - _min;
        Gizmos.DrawWireCube(centre, size);
    }
}
