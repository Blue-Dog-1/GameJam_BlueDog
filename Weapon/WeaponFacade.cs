using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WeaponFacade : MonoBehaviour
{
    static public WeaponFacade Carent { get; private set;}
    [SerializeField] CharacterController2D m_characterController2D;
    [SerializeField] int m_viewingAngle;

    [Header("RayCast")]
    [SerializeField] LayerMask layerMask = 1;

    [SerializeField] Weapon PushGan = null;
    [SerializeField] Weapon PullGan = null;

    [SerializeField] UnityEvent OnMouseDownEvent;

    Vector3 mouse_pos, object_pos;
    float angle;


    void Start()
    {
        Carent = this;

        GameManager.UIController.CoutTextPush = WeaponFacade.Carent.PushGan.m_cartridge.ToString();

        GameManager.UIController.CoutTextPull = WeaponFacade.Carent.PullGan.m_cartridge.ToString();

        PushGan.OnChargingGun += OnMouseDownEvent.Invoke;
        PullGan.OnChargingGun += OnMouseDownEvent.Invoke;
    }

    void Update()
    {
        #region look at
        mouse_pos = Input.mousePosition;
        mouse_pos.z = transform.position.z; //The distance between the camera and object
        object_pos = Camera.main.WorldToScreenPoint(transform.position);
        mouse_pos.x = mouse_pos.x - object_pos.x;
        mouse_pos.y = mouse_pos.y - object_pos.y;
        angle = Mathf.Atan2(mouse_pos.y, mouse_pos.x) * Mathf.Rad2Deg;

        var y = 0; // =(m_characterController2D.FacingRight) ? 180 : 0;

        if(m_characterController2D.FacingRight){
            angle += 180;
            y = 180;
        }

        transform.localRotation = Quaternion.Euler(new Vector3(0, y,  angle));
        #endregion

        PushGan.Update(Input.GetKey(KeyCode.Mouse0));
        PullGan.Update(Input.GetKey(KeyCode.Mouse1));

        if (Input.GetKeyUp(KeyCode.Mouse0))
            StartCoroutine(PushGan.Shoot(transform, transform.right, layerMask));

        if (Input.GetKeyUp(KeyCode.Mouse1))
            StartCoroutine(PullGan.Shoot(transform, -transform.right, layerMask));

    }


    public void OnPickUp(bool push, int count )
    {
        if (push)
        {
            WeaponFacade.Carent.PushGan.m_cartridge += count;
            GameManager.UIController.CoutTextPush = WeaponFacade.Carent.PushGan.m_cartridge.ToString();
            return;
        }
        WeaponFacade.Carent.PullGan.m_cartridge += count;
        GameManager.UIController.CoutTextPull = WeaponFacade.Carent.PullGan.m_cartridge.ToString();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.right * PullGan.rayLenght);
    }
}

[System.Serializable]
public class Weapon
{
    [SerializeField] bool isPushGan = false;
    [Range(1, 50)]
    [SerializeField] protected int m_rayLenght = 1;

    [SerializeField] protected float m_forse = 1;
    public int m_cartridge = 1;
    
    [SerializeField] int m_shotTimeOut = 1;
    [Range(0, 10)]
    [SerializeField] int m_maxIntensity = 1;

    [Header("shoot VFX ")]
    [SerializeField] ParticleSystem m_laser;
    [SerializeField] ParticleSystem m_light;
    [SerializeField] ParticleSystem m_lightningSpark;

    [SerializeField]
    UnityEvent OnShot = new UnityEvent();

    public UnityAction OnChargingGun;


    protected float timeLastShot = 0;
    protected float intens = 0f;
    public float rayLenght => m_rayLenght;

    public void Update(bool mouseDown)
    {
        if (mouseDown && m_cartridge > 0)
        {
            intens += Time.deltaTime * 5;
            OnCast();
        }

        intens = Mathf.Clamp(intens, 0, m_maxIntensity);
        if (isPushGan)
             GameManager.UIController.chargePuhs = 1 - ((1f / m_maxIntensity) * intens);
        else GameManager.UIController.chargePull = 1 - ((1f / m_maxIntensity) * intens);

        if (!mouseDown)
            intens -= Time.deltaTime * 5;
    }
    void OnCast()
    {
        if (!m_lightningSpark.isPlaying)
        {
            m_lightningSpark.Play();
            OnChargingGun?.Invoke();
        }
    }
    public IEnumerator Shoot(Transform transform, Vector2 vector,LayerMask layerMask)
    {
        
        if (Time.time < timeLastShot + m_shotTimeOut)
        {
            yield break;
        }
        if (m_cartridge <= 0)
        {
            yield break;
        }
        m_light.Play();
        m_laser.Play();
        m_lightningSpark.Stop();

        OnShot.Invoke();

        while (m_laser.isPlaying)
        {
            RaycastHit2D[] hitInfo = Physics2D.RaycastAll(transform.position, transform.right, m_rayLenght, layerMask);
            for (int i = 0; i < hitInfo.Length; i++)
            {
                if (hitInfo[i].collider.gameObject.layer == 8) break;
                hitInfo[i].collider?.attachedRigidbody?.AddForce((vector * intens * Time.deltaTime) * m_forse, ForceMode2D.Impulse);
            }

            yield return new WaitForFixedUpdate();
        }
        timeLastShot = Time.time;
        m_cartridge--;

        if (isPushGan)
            GameManager.UIController.CoutTextPush = m_cartridge.ToString();
        else
            GameManager.UIController.CoutTextPull = m_cartridge.ToString();
       
    }
    
}


