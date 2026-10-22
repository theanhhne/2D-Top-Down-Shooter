using UnityEngine;

public class Collectable : MonoBehaviour
{
    private ICollectableBehaviour[] _collectableBehaviours;

    private void Awake()
    {
        _collectableBehaviours =
            GetComponents<ICollectableBehaviour>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.GetComponent<PlayerMovement>();

        if (player != null)
        {
            foreach (ICollectableBehaviour behaviour
                     in _collectableBehaviours)
            {
                behaviour.OnCollected(player.gameObject);
            }

            // Đóng tutorial sau khi nhặt vật phẩm.
            TutorialManager.Instance?.NotifyCollectablePickedUp();

            Destroy(gameObject);
        }
    }
}