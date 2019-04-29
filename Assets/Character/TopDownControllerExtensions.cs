using UnityEngine;

public static class TopDownControllerExtensions
{
    public static void StopTopDownController(this GameObject collision)
    {
        collision.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        var playerController = collision.GetComponent<TopDownController>();
        playerController.xMove = 0;
        playerController.yMove = 0;
        playerController.UpdateAnimationOnly();
    }

}
