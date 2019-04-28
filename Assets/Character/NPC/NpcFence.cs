using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class NpcFence : MonoBehaviour
{
    public NpcMovingController npc;

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<NpcMovingController>() == npc)
        {
            Vector2 collisionNormal = GetComponent<BoxCollider2D>().ClosestPoint(collision.transform.position) - new Vector2(collision.transform.position.x, collision.transform.position.y);
            npc.Bonk(collisionNormal.normalized);
        }
    }
}
