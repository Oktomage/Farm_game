using UnityEngine;

namespace Game.Objects.Entitys
{
    public class Butterflies_entity : MonoBehaviour
    {
        [Header("Agents")]
        public Transform[] butterflies;           // arraste suas borboletas aqui

        [Header("Área (opcional)")]
        public float areaRadius = 2f;

        [Header("Comportamento")]
        public Vector2 speedRange = new Vector2(1.2f, 2.2f);
        public float steerStrength = 4f;          // quão rápido vira para o alvo
        public float retargetIntervalMin = 1.5f;  // troca de alvo
        public float retargetIntervalMax = 3.0f;
        public float orbitRadius = 0.6f;          // orbita ao redor do alvo
        public float separationDistance = 0.8f;   // evita sobrepor
        public float separationStrength = 1.8f;

        [Header("Voo ‘vivo’")]
        public float flutterSpeed = 3f;           // velocidade do ruído
        public float flutterAmplitude = 0.08f;    // amplitude do “balanço”

        struct Agent
        {
            public Transform t;
            public Vector2 vel;
            public float speed;
            public float nextRetargetAt;
            public int targetIndex;   // -1 = alvo livre
            public Vector2 orbitOffset;
            public float noiseSeed;
        }

        Agent[] agents;

        void Start()
        {
            if (butterflies == null || butterflies.Length == 0) return;

            agents = new Agent[butterflies.Length];
            for (int i = 0; i < butterflies.Length; i++)
            {
                agents[i].t = butterflies[i];
                agents[i].vel = Vector2.zero;
                agents[i].speed = Random.Range(speedRange.x, speedRange.y);
                agents[i].nextRetargetAt = Time.time + Random.Range(retargetIntervalMin, retargetIntervalMax);
                agents[i].targetIndex = -1;
                agents[i].orbitOffset = Random.insideUnitCircle * orbitRadius;
                agents[i].noiseSeed = Random.value * 1000f;
            }
        }

        void Update()
        {
            if (agents == null) return;

            for (int i = 0; i < agents.Length; i++)
            {
                var a = agents[i];
                if (a.t == null) continue;

                // escolhe novo alvo de tempos em tempos
                if (Time.time >= a.nextRetargetAt)
                {
                    a.nextRetargetAt = Time.time + Random.Range(retargetIntervalMin, retargetIntervalMax);

                    // 50% das vezes segue outra borboleta, 50% um ponto aleatório na área
                    if (Random.value < 0.5f && agents.Length > 1)
                    {
                        int pick = i;
                        // evita escolher a si mesmo
                        while (pick == i) pick = Random.Range(0, agents.Length);
                        a.targetIndex = pick;
                    }
                    else
                    {
                        a.targetIndex = -1;
                    }

                    a.orbitOffset = Random.insideUnitCircle * orbitRadius;
                }

                // alvo desejado
                Vector2 RandomPointAroundManager()
                {
                    return (Vector2)transform.position + Random.insideUnitCircle * areaRadius;
                }

                Vector2 targetPos;
                targetPos = RandomPointAroundManager();

                // separação (evita colidir/empilhar)
                Vector2 separation = Vector2.zero;
                for (int j = 0; j < agents.Length; j++)
                {
                    if (j == i || agents[j].t == null) continue;
                    Vector2 toMe = (Vector2)a.t.position - (Vector2)agents[j].t.position;
                    float d = toMe.magnitude;
                    if (d > 0f && d < separationDistance)
                        separation += toMe.normalized * (1f - d / separationDistance);
                }
                separation *= separationStrength;

                // direção desejada + “flutter”
                Vector2 desired = (targetPos - (Vector2)a.t.position).normalized * a.speed + separation;
                a.vel = Vector2.Lerp(a.vel, desired, steerStrength * Time.deltaTime);

                // ruído para dar vida
                float wobbleX = (Mathf.PerlinNoise(a.noiseSeed, Time.time * flutterSpeed) - 0.5f) * flutterAmplitude;
                float wobbleY = (Mathf.PerlinNoise(a.noiseSeed + 77.7f, Time.time * flutterSpeed) - 0.5f) * flutterAmplitude;

                Vector2 newPos = (Vector2)a.t.position + a.vel * Time.deltaTime + new Vector2(wobbleX, wobbleY);

                a.t.position = newPos;

                // vira o sprite conforme a velocidade (flip X 2D)
                if (a.vel.x != 0f)
                {
                    var sr = a.t.GetComponent<SpriteRenderer>();
                    if (sr) sr.flipX = a.vel.x < 0f; // ou >0, conforme seu sprite
                }

                agents[i] = a;
            }
        }
    }
}
