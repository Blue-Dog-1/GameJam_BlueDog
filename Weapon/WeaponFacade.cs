using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WeaponFacade : MonoBehaviour
{
    static public WeaponFacade Carent { get; private set;}
    [SerializeField]
    CharacterController2D characterController2D;
    [SerializeField] int m_viewingAngle;

    [Header("RayCast")]
    [SerializeField] LayerMask layerMask = 1;

    [SerializeField] Weapon PushGan = null;
    [SerializeField] Weapon PullGan = null;
    [SerializeField] CharacterController2D m_characterController2D;


    Vector3 mouse_pos, object_pos;
    float angle;


    void Start()
    {
        Carent = this;

        GameManager.UIController.CoutTextPush = WeaponFacade.Carent.PushGan.m_cartridge.ToString();

        GameManager.UIController.CoutTextPull = WeaponFacade.Carent.PullGan.m_cartridge.ToString();
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
        else {
            angle = Mathf.Clamp(angle, -35f, 50f);
        }

        transform.localRotation = Quaternion.Euler(new Vector3(0, y,  angle));
        #endregion

        PushGan.Update(Input.GetKey(KeyCode.Mouse0));
        PullGan.Update(Input.GetKey(KeyCode.Mouse1));

        if (Input.GetKeyUp(KeyCode.Mouse0))
            PushGan.Shoot(transform, layerMask);

        if (Input.GetKeyUp(KeyCode.Mouse1))
            PullGan.Shoot(transform, layerMask);

        
    }

    public void OnPickUp(bool push)
    {
        if (push)
        {
            WeaponFacade.Carent.PushGan.m_cartridge++;
            GameManager.UIController.CoutTextPush = WeaponFacade.Carent.PushGan.m_cartridge.ToString();
            return;
        }
        WeaponFacade.Carent.PullGan.m_cartridge++;
        GameManager.UIController.CoutTextPull = WeaponFacade.Carent.PullGan.m_cartridge.ToString();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.right * 8);
    }
}

[System.Serializable]
public class Weapon
{
    [SerializeField] bool isPushGan = false;
    [Range(1, 10)]
    [SerializeField] protected int m_rayLenght = 1;

    [SerializeField] protected float m_forse = 1;
    public int m_cartridge = 1;
    
    [SerializeField] int m_shotTimeOut = 1;
    [Range(0, 10)]
    [SerializeField] int m_maxIntensity = 1;


    [SerializeField]
    UnityEvent OnShot = new UnityEvent();


    protected float timeLastShot = 0;
    protected float intens = 0f;

    public void Update(bool mouseDown)
    {
        if (mouseDown && m_cartridge > 0)
            intens += Time.deltaTime;

        intens = Mathf.Clamp(intens, 0, m_maxIntensity);
        if (isPushGan)
             GameManager.UIController.chargePuhs = 1 - ((1f / m_maxIntensity) * intens);
        else GameManager.UIController.chargePull = 1 - ((1f / m_maxIntensity) * intens);

        if (!mouseDown)
            intens -= Time.deltaTime;
    }

    public void Shoot(Transform transform, LayerMask layerMask)
    {
        if (Time.time < timeLastShot + m_shotTimeOut)
        {
            return;
        }
        if (m_cartridge <= 0)
        {
            return;
        }
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, transform.right, m_rayLenght, layerMask);
        var force = m_forse; // / (1 + Mathf.Pow(hitInfo.distance, 2));
        if (!isPushGan) force *= -1; 
        hitInfo.collider?.attachedRigidbody?.AddForce(transform.right * force * intens, ForceMode2D.Impulse);
        timeLastShot = Time.time;

        m_cartridge--;

        if (isPushGan)
        {
            GameManager.UIController.CoutTextPush = m_cartridge.ToString();
        }
        else
        {
            GameManager.UIController.CoutTextPull = m_cartridge.ToString();
        }

        OnShot.Invoke();
    }
}


