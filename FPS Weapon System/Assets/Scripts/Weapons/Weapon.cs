using UnityEngine;
using System.Collections;

[RequireComponent(typeof(WeaponFX))]
public class Weapon : MonoBehaviour , IShooter {

    public GameWeapon weapon;

    public Transform rocket;

    public int ammo;
    public int clip;

    private CameraRaycaster _raycaster;
    private bool _canShoot = true;
    private bool _shooting;
    private bool _reloading;
    private bool _hasAmmo;
    private bool _canLaunch;
    private bool _launched;
    private bool rocketTriggered = false;

    private float rocketSpeed = 9f;
    private float rocketLifeTime = 60f;
    private float rocketLife = 0;
    private float fireTimer = 0;
    private WeaponFX wFx = null;
    private Vector3 rocketPosition;

    void Awake()
    {
        _raycaster = GameObject.FindGameObjectWithTag("WeaponCamera").GetComponent<CameraRaycaster>();
        wFx = GetComponent<WeaponFX>();
        rocket = GameObject.FindGameObjectWithTag("Rocket").transform;
        if (rocket)
            rocketPosition = rocket.localPosition;

    }

	void Start () {

    }
	
	void Update () {
        // shoot
        if (_canShoot && !_reloading)
        {
            if (Shoot(weapon.type))
            {
                RaycastHit hit;
                if (_raycaster.RayCast(Vector3.forward, weapon.fireRange, out hit))
                {
                    // TODO: check hit object
                }
            }
        }

        // reload
        if(Input.GetButton("Reload") && clip < weapon.clipSize)
            Reload();
        // update state variables
        _hasAmmo = (ammo != 0);
        //
        if (weapon.type == WeaponType.ROCKET && !_launched)
            _canLaunch = true;
        else
            _canLaunch = false;
        //
        if (_launched)
        {

        }
	}

    public void FixedUpdate()
    {
        // launch rocket if triggered
        if (rocketTriggered)
        {
            _launched = true;
            rocket.localPosition += Vector3.forward * rocketSpeed * Time.deltaTime;
            Reload();
        }
        if (_launched)
        {
            if(++rocketLife >= rocketLifeTime)
            {
                // Explode
                ResetRocket();
            }
        }
    }

    public bool Shoot(WeaponType _type)
    {
        bool _hasShoot = false;
        switch (_type)
        {
            case WeaponType.MELEE:
                // TODO:
                break;
            case WeaponType.PISTOL:
                _hasShoot = ShootSemiAuto();
                break;
            case WeaponType.RIFLE:
                _hasShoot = ShootAutomatic();
                break;
            case WeaponType.SNIPER:
                _hasShoot = ShootSemiAuto();
                break;
            case WeaponType.ROCKET:
                _hasShoot = LaunchRocket();
                break;
            default:
                print("Invalid weapon type.");
                break;
        }
        return _hasShoot;
    }

    private bool ShootAutomatic()
    {
        bool _hasShoot = false;
        if ((_shooting = Input.GetButton("Fire1")) && clip != 0)
        {
            if (clip == 0 && HasAmmo)
                Reload();
            if (Time.time - fireTimer > (1/weapon.fireRate))
            {
                fireTimer = Time.time;
                --clip;
                // play sound
                wFx.PlayShootingSound(); _hasShoot = true;
            }
        }
        else
        {
            if (clip == 0 && HasAmmo)
                Reload();
        }
        return _hasShoot;
    }

    private bool ShootSemiAuto()
    {
        bool _hasShoot = false;
        
        if ( (_shooting = Input.GetButtonDown("Fire1")) && clip != 0)
        {
            if (Time.time - fireTimer > (1 / weapon.fireRate))
            {
                --clip;
                fireTimer = Time.time;
                // TODO:
                // play sound
                wFx.PlayShootingSound();
                _hasShoot = true;
            }
        }
        else
        {
            if (clip == 0 && HasAmmo)
               Reload();
        }
        return _hasShoot;
    }

    private bool LaunchRocket()
    {
        bool _hasShoot = false;
        if ((_shooting = Input.GetButtonDown("Fire1")) && clip != 0)
        {
            if (_canShoot)
            {
                _canShoot = false;
                _launched = true;
                --clip;
            }
        }
        return _hasShoot;
    }

    public void Reload()
    {
        if (!(weapon.type == WeaponType.ROCKET))
        {
            _shooting = false;
            if (HasAmmo)
                StartCoroutine(ReloadWeapon());
            else
                print("You don\'t have ammo!"); 
        }
        else
        {
            if (!_launched)
                return;

            if (HasAmmo)
                StartCoroutine(ReloadRPG());
            else
                print("You don\'t have rockets!");
        }
    }

    private IEnumerator ReloadWeapon()
    {
        /* *********************************************** */
        _reloading = true;
        print("Reloading...");
        yield return new WaitForSeconds(weapon.reloadTime);
        if (clip != 0)
        {
            ammo += clip;
            clip = 0;
        }
        /* *********************************************** */
        if(ammo <= weapon.clipSize)
        {
            clip = ammo;
            ammo = 0;
        }
        else
        {
            clip = weapon.clipSize;
            ammo -= weapon.clipSize;
        }
        _reloading = false;
        print("Ready.");
    }

    private IEnumerator ReloadRPG()
    {
        _canLaunch = false;
        _reloading = false;
        print("Reloading...");
        yield return new WaitForSeconds(weapon.reloadTime);
        ++clip;
        --ammo;
        CreateRocket();
        _reloading = false;
    }

    private void ResetRocket(){
        _launched = false;
        rocket.gameObject.SetActive(false);
        rocket.localPosition = rocketPosition;
    }

    private void CreateRocket()
    {
        rocket.gameObject.SetActive(true);
        _canLaunch = true;
    }

    // properities
    public bool HasAmmo
    {
        get { return _hasAmmo; }
    }

    public bool IsReloading
    {
        get { return _reloading; }
    }

    public bool IsShooting
    {
        get { return _shooting; }
    }

    public WeaponName Name
    {
        get { return weapon.weapon; }
    }

    public WeaponType Type
    {
        get { return weapon.type; }
    }

}
