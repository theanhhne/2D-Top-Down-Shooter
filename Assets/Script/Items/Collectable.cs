using UnityEngine;
 
public class Collectable : MonoBehaviour
{
    private ICollectableBehaviour[] _collectableBehaviours;
 
    private void Awake()
    {
        // Lấy all behaviour trên prefab, để một vật phẩm có thể có nhiều hiệu ứng
        _collectableBehaviours = GetComponents<ICollectableBehaviour>();
    }
 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.GetComponent<PlayerMovement>();
 
        if (player != null)
        {
            foreach (ICollectableBehaviour behaviour in _collectableBehaviours)
            {
                behaviour.OnCollected(player.gameObject);
            }
 
            Destroy(gameObject);
        }
    }
}
 