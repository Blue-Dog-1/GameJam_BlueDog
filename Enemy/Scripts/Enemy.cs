using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PL
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] bool isIdel;

        [SerializeField] Transform Skin;
        [SerializeField] Animator animator;
        [SerializeField] Transform m_head;
        [SerializeField] Transform m_collisionCheckPoint;
        [SerializeField] public LayerMask m_layerMaskCheck;
        [Header("hit points and damage")]
        [SerializeField] int m_HP = 200;
        [SerializeField] float m_demageMadificator = 0.1f;


        [Header("Shoting settings")]
        [SerializeField] LayerMask m_layerMask;
        [Range(5, 40)]
        [SerializeField] float m_rayLenght = 30;
        [Range(0, 50)]
        [SerializeField] float m_force = 100;
        [Range(1, 15)]
        [SerializeField] float m_rechargeTime = 5;
        [Range(0, 50)]
        [SerializeField] int m_damageDone = 10;

        [Space]
        [Header("Drop Loot")]
        [SerializeField] Cartridge m_Cartridge;
        [SerializeField] int m_CartridgeCount = 5;
        [Space]


        public float diameterFieldView = 30;

        [SerializeField] float m_velocity = 10;
        [SerializeField]
        float m_attackDistance = 1,
                               m_stoppingDistance = 30f;
        [SerializeField] UnityEvent OnShootEvent;
        [SerializeField] UnityEvent OnCastEvent;
        [SerializeField] UnityEvent OnDischargeChargeEvent;


        State state;
        float deltaPosX;
        bool m_FacingRight;

        public Transform head => m_head;
        public Transform collisionCheckPoint => m_collisionCheckPoint;
        public float attackDistance => m_attackDistance;
        public float stoppingDistance => m_stoppingDistance;
        public float velocity => m_velocity;
        public int damageDone => m_damageDone;

        void Start()
        {
            deltaPosX = transform.position.x;
            state = new Idel(this);
            GameManager.Main.AddLisenerEndGame(() => { enabled = false; });
        }
        public void OnShoot() =>
            OnShootEvent.Invoke();
        public void OnCast() =>
            OnCastEvent.Invoke();
        public void SwitchState(States states)
        {
            OnDischargeChargeEvent.Invoke();
            switch (states)
            {
                case States.idel:
                    state = new Idel(this);
                    animator.SetBool("isIdel", true);
                    break;
                case States.run:
                    state = new Run(this, CharacterController2D.Main.transform);
                    animator.SetBool("isIdel", false);
                    break;
                case States.attack:
                    state = new Attack(this, CharacterController2D.Main.transform, m_rayLenght, m_layerMask, m_force, m_rechargeTime, OnShootEvent.Invoke);
                    animator.SetBool("isIdel", true);
                    break;
                default:
                    break;
            }
        }
        void Update()
        {
            deltaPosX = transform.position.x;
            state.Update();

            if (transform.position.y < -80)
                Destroy(gameObject);
        }
        public void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.rigidbody)
            {
                var damage = (int)((collision.rigidbody.mass * collision.rigidbody.velocity.magnitude) * m_demageMadificator * 0.01f);
                ToDamage(damage);
            }
        }
        public void ToDamage(int damage)
        {
            m_HP -= damage;
            if (m_HP <= 0)
            {
                m_HP = 0;
                Debug.Log("Kill enemy");
                Kill();
            }
        }
        [ContextMenu("Kill")]
        public void Kill()
        {
            OnDischargeChargeEvent.Invoke();
            state = new Perish(this);
            Destroy(GetComponent<Rigidbody2D>());
        }
        public void SpawnLoot()
        {
            var cartridge = Instantiate(m_Cartridge);
            Vector2 pos = transform.position;
            pos.y += 2f;
            cartridge.transform.position = pos;
            cartridge.m_count = m_CartridgeCount;
            WeaponFacade.Carent.Boost += 2;
        }
        public bool Flip(float x)
        {
            if (deltaPosX - x > 0 && m_FacingRight) /// x > 0 = is Right
            {
                Flip();
            }
            if (deltaPosX - x < 0 && !m_FacingRight) /// x < 0 = is Left
            {
                Flip();
            }
            return m_FacingRight;
        }
        void Flip()
        {
            m_FacingRight = !m_FacingRight;

            Vector3 theScale = Skin.localScale;
            theScale.x *= -1;
            Skin.localScale = theScale;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            //Gizmos.DrawWireSphere(transform.position, diameterFieldView / 2);
        }

    }
    public interface State
    {
        void Update();
    }

    public struct Idel : State
    {
        Enemy enemy;

        public Idel(Enemy enemy)
        {
            this.enemy = enemy;
            enemy.head.rotation = Quaternion.identity;
            Debug.Log("IdelState");

        }
        public void Update()
        {
            var colliders = Physics2D.OverlapCapsuleAll(enemy.transform.position, Vector3.one * enemy.diameterFieldView, CapsuleDirection2D.Horizontal, 100f);

            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject.name == GameManager.PLAYER_NAME)
                {
                    bool isGound = Physics2D.Raycast(enemy.collisionCheckPoint.position, enemy.collisionCheckPoint.up, 10f);
                    var right = enemy.Flip(enemy.transform.position.x);
                    var dir = right ? -enemy.collisionCheckPoint.right : enemy.collisionCheckPoint.right;

                    bool forwardCollison = Physics2D.Raycast(enemy.collisionCheckPoint.position, dir, 0.1f, enemy.m_layerMaskCheck);

                    if (isGound && !forwardCollison)
                        enemy.SwitchState(States.run);
                    return;
                }
            }
        }
    }
    public struct Attack : State
    {
        Enemy enemy;
        Transform target;
        float timer,
            rayLenght,
            force,
            rechargeTime;
        LayerMask layerMask;
        UnityAction OnShoot;

        bool chargeIsEmpty;
        public Attack(Enemy enemy, Transform target, float rayLenght, LayerMask layerMask, float force, float rechargeTime, UnityAction OnShoot)
        {
            this.enemy = enemy;
            this.target = target;
            timer = 0;
            this.rayLenght = rayLenght;
            this.layerMask = layerMask;
            this.force = force;
            this.rechargeTime = rechargeTime;
            this.OnShoot = OnShoot;
            chargeIsEmpty = false;
            Debug.Log("AttackState");


        }
        public void Update()
        {

            bool flip = enemy.Flip(target.transform.position.x);
            Vector2 vector = flip ? -Vector2.right : Vector2.right;
            enemy.head.rotation = Quaternion.FromToRotation(vector, enemy.head.position - target.position);

            timer += Time.deltaTime;
            if (timer > rechargeTime)
            {
                timer = 0;
                vector = flip ? enemy.head.right : -enemy.head.right;
                Shoot(vector);
            }
            else if (!chargeIsEmpty)
            {
                OnCast();
            }

            if (Vector2.Distance(enemy.transform.position, target.position) > enemy.attackDistance)
            {
                enemy.SwitchState(States.run);
            }
        }
        void OnCast()
        {
            enemy.OnCast();
            chargeIsEmpty = true;
        }
        public void Shoot(Vector2 vector)
        {
            RaycastHit2D hitInfo = Physics2D.Raycast(enemy.head.position, vector, rayLenght, layerMask);

            if (hitInfo.collider.attachedRigidbody == null) return;
            hitInfo.collider.attachedRigidbody?.AddForce((vector * Time.deltaTime * 100) * force, ForceMode2D.Impulse);

            if (hitInfo.collider.gameObject.name == GameManager.PLAYER_NAME)
                hitInfo.collider.gameObject.GetComponent<CharacterController2D>().ToDamage(enemy.damageDone);

            Debug.DrawRay(enemy.head.position, vector * rayLenght, Color.blue);

            OnShoot?.Invoke();
            chargeIsEmpty = false;
        }
    }
    public struct Run : State
    {
        Enemy enemy;
        Transform target;
        public Run(Enemy enemy, Transform target)
        {
            this.enemy = enemy;
            this.target = target;
            Debug.Log("RunState");
        }
        public void Update()
        {
            bool isGround = Physics2D.Raycast(enemy.collisionCheckPoint.position, enemy.collisionCheckPoint.up, 10f);

            var right = enemy.Flip(enemy.transform.position.x);
            var dir = right ? -enemy.collisionCheckPoint.right : enemy.collisionCheckPoint.right;
            bool forwardCollision = Physics2D.Raycast(enemy.collisionCheckPoint.position, dir, 0.1f, enemy.m_layerMaskCheck);

            if (!isGround || forwardCollision)
            {
                enemy.SwitchState(States.idel);
                return;
            }

            var vector = enemy.transform.position;
            vector.x = Mathf.Lerp(enemy.transform.position.x, target.position.x, enemy.velocity * Time.deltaTime);
            enemy.transform.position = vector;

            if (Vector2.Distance(enemy.transform.position, target.position) < enemy.attackDistance)
            {
                enemy.SwitchState(States.attack);
            }
            if (Vector2.Distance(enemy.transform.position, target.position) > enemy.stoppingDistance)
            {
                enemy.SwitchState(States.idel);
            }
        }
    }


    public struct Perish : State
    {

        Enemy enemy;
        float time;
        public Perish(Enemy enemy)
        {
            this.time = Time.time + 2f;
            this.enemy = enemy;
        }
        public void Update()
        {
            enemy.transform.rotation = Quaternion.Lerp(enemy.transform.rotation, Quaternion.FromToRotation(Vector2.up, Vector2.right), Time.deltaTime);

            enemy.transform.Translate(Vector2.down * Time.deltaTime);

            if (time < Time.time)
            {
                enemy.SpawnLoot();
                GameObject.Destroy(enemy.gameObject);
            }
        }
    }
    public enum States
    {
        idel,
        run,
        attack
    }
}