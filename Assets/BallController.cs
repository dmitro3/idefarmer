using Project_Data.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    private ObjectPooler  pool;
    private Rigidbody2D rb;               // Thành phần Rigidbody2D để điều khiển vật lý        // Tham chiếu đến Object Pool để trả bóng về
    public float speed = 5f;              // Tốc độ di chuyển của bóng
    private Vector2 direction;            // Hướng di chuyển của bóng
    public bool isActive;
    private float spawnTime;  // Thời gian đối tượng được sinh ra
    private float collisionTime;  // Thời gian va chạm
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Lấy thành phần Rigidbody2D
    }
    public void Initialize(ObjectPooler objectPool)
    {
        pool = objectPool; 
       
    }
    private void Update()
    {
        if (isActive)
        {
            // Giới hạn vị trí x của quả bóng
            float clampedX = Mathf.Clamp(transform.localPosition.x, -28.0f, 28.0f);

            // Cập nhật vị trí quả bóng
            transform.localPosition = new Vector3(clampedX, transform.localPosition.y, transform.localPosition.z);

        }
    }
    private void OnEnable()
    {
        spawnTime = Time.time;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra nếu quả bóng va chạm với bất kỳ đối tượng nào khác (có thể tùy chỉnh điều kiện này)
        if (collision.CompareTag("destinationLift") || collision.CompareTag("EndPoint"))
        {
            collisionTime = Time.time;
            float timeFromSpawnToCollision = collisionTime - spawnTime;
            // Khi va chạm, đưa quả bóng về Object Pool
            GameManager.Instance.liftHandler.collectProduct(timeFromSpawnToCollision);
            ReturnToPool();
        }
    }
    private void ReturnToPool()
    {
        rb.velocity = Vector2.zero;  // Dừng di chuyển quả bóng
        isActive = false;
        pool.ReturnObjectToPool(gameObject); // Trả quả bóng về Object Pool
    }
}
