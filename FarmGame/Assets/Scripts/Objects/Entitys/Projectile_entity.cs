using Game.Characters;
using System.Collections;
using UnityEngine;

namespace Game.Entitys.Projectiles
{
    public class Projectile_entity : MonoBehaviour
    {
        [Header("Settings")]
        public float Min_damage = 0f;
        public float Max_damage = 0f;
        public float Speed = 3f;
        public float Max_duration = 3f;

        [Header("Components")]
        public Rigidbody2D Rb2d => this.gameObject.GetComponentInChildren<Rigidbody2D>();
        public SpriteRenderer Render => this.gameObject.GetComponentInChildren<SpriteRenderer>();
        public Collider2D Collider => this.gameObject.GetComponentInChildren<Collider2D>();

        //Internal variables
        internal GameObject Character_parent_obj;
        internal Vector2 current_direction;

        /// CORE METHODS
        internal void Configure(GameObject char_parent_obj, Sprite sprite, float Min_dmg, float Max_dmg, float speed, Vector2 dir)
        {
            // Set
            Character_parent_obj = char_parent_obj;

            Render.sprite = sprite;

            Min_damage = Min_dmg;
            Max_damage = Max_dmg;
            Speed = speed;

            current_direction = dir;

            StartCoroutine(Kill_obj_timer(Max_duration));
        }

        private void Update()
        {
            if (current_direction != Vector2.zero)
                Move();

            if(Character_parent_obj != null)
                Test_collisions();
        }

        /// MAIN METHODS
        private void Move()
        {
            // Set
            Rb2d.linearVelocity = current_direction * Speed;

            transform.right = current_direction;
        }

        private void Test_collisions()
        {
            CircleCollider2D circleCollider = Collider as CircleCollider2D;

            if (circleCollider != null)
            {
                Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, circleCollider.radius);
                
                foreach(Collider2D hit in hits)
                {
                    Character_behaviour char_bhv = null;

                    // Dont hit theses
                    if (hit.transform == transform)
                        continue;
                    if (Character_parent_obj && hit.transform.IsChildOf(Character_parent_obj.transform))
                        continue;
                    // Different one
                    else
                        char_bhv = hit.GetComponentInParent<Character_behaviour>();

                    if (char_bhv == null)
                        continue;

                    Do_damage(char_bhv);
                }
            }
        }

        private void Do_damage(Character_behaviour char_bhv)
        {
            float damage = Random.Range(Min_damage, Max_damage);

            // Set
            char_bhv.TakeDamage(damage, Character_parent_obj);

            Destroy_this_entity();
        }

        private void Destroy_this_entity()
        {
            Destroy(this.gameObject);
        }

        private IEnumerator Kill_obj_timer(float t)
        {
            yield return new WaitForSeconds(t);

            Destroy_this_entity();
        }
    }
}