using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class WeaponFX : MonoBehaviour {

    public Transform muzzleFlash;
    public Transform muzzleSmoke;
    public Light muzzleLight;

    public AudioClip shootClip;
    public AudioClip reloadClip;

    private Animation wpanim;
    private AudioSource audioSource;
    private IShooter weapon;

    void Awake () {
        weapon = GetComponent<Weapon>();
        audioSource = GetComponent<AudioSource>();
        wpanim = GetComponentInChildren<Animation>();
	}
	
	// Update is called once per frame
	void Update () {
        if (weapon != null)
        {
            // play shooting effects
            // emit fire flash if weapon is Rifle/Sniper/shotgun/pistol
            if(!(weapon.Type == WeaponType.ROCKET))
                EmitMuzzleFlash();

            // play animations
            if (weapon.IsShooting)
            {
                if(wpanim) // play animation
                    wpanim.Play("shooting");
                // emit smoke if weapon is RPG
                if(muzzleSmoke && weapon.Type == WeaponType.ROCKET)
                    EmitSmoke();
            }
        }
	}

    private void EmitSmoke()
    {
        foreach (ParticleEmitter pe in muzzleSmoke.GetComponentsInChildren<ParticleEmitter>())
            pe.Emit(200);
    }

    private void EmitMuzzleFlash()
    {
        foreach (ParticleEmitter pe in muzzleFlash.GetComponentsInChildren<ParticleEmitter>())
            pe.emit = muzzleLight.enabled = weapon.IsShooting;
    }

    public void PlayShootingSound()
    {
        if (!audioSource.isPlaying && shootClip)
            audioSource.PlayOneShot(shootClip);
    }
}
