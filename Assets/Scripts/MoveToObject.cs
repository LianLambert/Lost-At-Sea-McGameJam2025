using UnityEngine;

public class MoveToObject : MonoBehaviour
{
    public Vector3 target;
    public float speed = 0.4f;
    public float stopDistance = 0.1f;

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            // direction to target
            Vector3 direction = (target - transform.position).normalized;

            // move
            transform.position += direction * speed * Time.deltaTime;

            // disappear once there
            if (Vector3.Distance(transform.position, target) <= stopDistance)
            {
                //Destroy(gameObject);
            }
        }
    }
}
