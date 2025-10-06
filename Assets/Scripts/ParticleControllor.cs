using UnityEngine;

public class ParticleControllor : MonoBehaviour
{
    public int particleCount = 500;        // 每种粒子数量
    public Vector3 cubeSize = new Vector3(10, 10, 10);  // 立方体范围
    public GameObject Sphere;       // 第一种粒子Prefab
    public GameObject Sphere2;      // 第二种粒子Prefab
    
    [Header("Brownian Motion Settings")]
    public float maxSpeed = 2f;              // 最大移动速度
    public float directionChangeInterval = 0.5f;  // 方向改变间隔
    public float turbulence = 1.5f;          // 湍流强度
    public bool keepInBounds = true;         // 是否保持在边界内
    
    private ParticleData[] particles;
    private ParticleData[] particles2;

    void Start()
    {
        particles = new ParticleData[particleCount];
        particles2 = new ParticleData[particleCount];
        
        // 创建第一组粒子 (Sphere)
        for (int i = 0; i < particleCount; i++)
        {
            Vector3 pos = new Vector3(
                Random.Range(-cubeSize.x / 2, cubeSize.x / 2),
                Random.Range(-cubeSize.y / 2, cubeSize.y / 2),
                Random.Range(-cubeSize.z / 2, cubeSize.z / 2)
            );

            GameObject particle = Instantiate(Sphere, transform.position + pos, Quaternion.identity, transform);
            
            particles[i] = new ParticleData
            {
                transform = particle.transform,
                velocity = Random.insideUnitSphere * maxSpeed,
                nextDirectionChange = Time.time + Random.Range(0.1f, directionChangeInterval)
            };
        }
        
        // 创建第二组粒子 (Sphere2)
        for (int i = 0; i < particleCount; i++)
        {
            Vector3 pos = new Vector3(
                Random.Range(-cubeSize.x / 2, cubeSize.x / 2),
                Random.Range(-cubeSize.y / 2, cubeSize.y / 2),
                Random.Range(-cubeSize.z / 2, cubeSize.z / 2)
            );

            GameObject particle2 = Instantiate(Sphere2, transform.position + pos, Quaternion.identity, transform);
            
            particles2[i] = new ParticleData
            {
                transform = particle2.transform,
                velocity = Random.insideUnitSphere * maxSpeed,
                nextDirectionChange = Time.time + Random.Range(0.1f, directionChangeInterval)
            };
        }
    }

    void Update()
    {
        if (particles == null || particles2 == null) return;

        Vector3 center = transform.position;
        
        // 更新第一组粒子
        UpdateParticles(particles, center);
        
        // 更新第二组粒子
        UpdateParticles(particles2, center);
    }
    
    void UpdateParticles(ParticleData[] particleArray, Vector3 center)
    {
        for (int i = 0; i < particleArray.Length; i++)
        {
            ParticleData particle = particleArray[i];
            
            // 检查是否需要改变方向
            if (Time.time >= particle.nextDirectionChange)
            {
                // 添加布朗运动 - 随机改变速度方向
                Vector3 randomDirection = Random.insideUnitSphere;
                particle.velocity = Vector3.Lerp(particle.velocity, randomDirection * maxSpeed, turbulence * Time.deltaTime);
                
                // 设置下次改变方向的时间
                particle.nextDirectionChange = Time.time + Random.Range(0.2f, directionChangeInterval);
            }
            
            // 添加轻微的随机扰动
            Vector3 noise = new Vector3(
                Random.Range(-0.1f, 0.1f),
                Random.Range(-0.1f, 0.1f),
                Random.Range(-0.1f, 0.1f)
            ) * turbulence;
            
            particle.velocity += noise * Time.deltaTime;
            
            // 限制速度
            if (particle.velocity.magnitude > maxSpeed)
            {
                particle.velocity = particle.velocity.normalized * maxSpeed;
            }
            
            // 移动粒子
            particle.transform.position += particle.velocity * Time.deltaTime;
            
            // 边界检查（可选）
            if (keepInBounds)
            {
                Vector3 localPos = particle.transform.position - center;
                
                if (Mathf.Abs(localPos.x) > cubeSize.x / 2)
                {
                    particle.velocity.x *= -0.8f; // 反弹但损失一些能量
                    particle.transform.position = new Vector3(
                        center.x + Mathf.Sign(localPos.x) * cubeSize.x / 2,
                        particle.transform.position.y,
                        particle.transform.position.z
                    );
                }
                
                if (Mathf.Abs(localPos.y) > cubeSize.y / 2)
                {
                    particle.velocity.y *= -0.8f;
                    particle.transform.position = new Vector3(
                        particle.transform.position.x,
                        center.y + Mathf.Sign(localPos.y) * cubeSize.y / 2,
                        particle.transform.position.z
                    );
                }
                
                if (Mathf.Abs(localPos.z) > cubeSize.z / 2)
                {
                    particle.velocity.z *= -0.8f;
                    particle.transform.position = new Vector3(
                        particle.transform.position.x,
                        particle.transform.position.y,
                        center.z + Mathf.Sign(localPos.z) * cubeSize.z / 2
                    );
                }
            }
            
            particleArray[i] = particle;
        }
    }
    
    // 粒子数据结构
    [System.Serializable]
    public struct ParticleData
    {
        public Transform transform;
        public Vector3 velocity;
        public float nextDirectionChange;
    }
    
    // 在Scene视图中绘制边界
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, cubeSize);
    }
}
