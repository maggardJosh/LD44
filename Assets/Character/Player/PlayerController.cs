using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TopDownController))]
public class PlayerController : MonoBehaviour
{
    private TopDownController controller;
    public bool hasLeftWarp = false;
    public string targetWarp = "";

    void Start()
    {
        controller = GetComponent<TopDownController>();
    }

    private void OnLevelWasLoaded(int level)
    {
        List<WarpPoint> points = new List<WarpPoint>(FindObjectsOfType<WarpPoint>());
        foreach (WarpPoint p in points)
        {
            if (p.gameObject.name == targetWarp)
            {
                transform.position = p.transform.position;
                break;
            }
        }
        FindObjectOfType<Cinemachine.CinemachineVirtualCamera>().Follow = transform;
    }

    void Update()
    {
        controller.xMove = Input.GetAxisRaw("Horizontal");
        controller.yMove = Input.GetAxisRaw("Vertical");
    }
}
