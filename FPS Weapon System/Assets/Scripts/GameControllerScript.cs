
using UnityEngine;
using System.Collections;
using System;

public class GameControllerScript : MonoBehaviour {


}

public enum WeaponName {
	None,
    AXE,
	SF45APC,
	M9,
	UZI,
	AK47,
    M4,
	M4A1,
	RPG,
	L11A3,
    MK16,
	SPAS12,
    FNSCARMK16
}

public enum WeaponType {
    MELEE, PISTOL, RIFLE, SNIPER, ROCKET
}

public enum Ammo {
	M9,
	SR45APC,
	UZI,
	AK47,
	M4A1,
	RIFLE,
	RPGROCKET,
	SPAS12
}

public struct WeaponData
{
    public Weapon weapon;
    public Vector3 Hip;
    public Vector3 Aim;
    public Quaternion rotation;
}

public interface IShooter {

    bool Shoot(WeaponType _type);

    void Reload();

	bool HasAmmo { 
		get;
	}

	bool IsReloading {
		get;
	}

	bool IsShooting {
		get;
	}

    WeaponName Name {
        get;
    }

    WeaponType Type
    {
        get;
    }

}

public interface IWeapon 
{
    void AddAmmo(uint amount);
    bool HasAmmo { get; }
}

public interface IAmmunition {
	Ammo AmmoType { get; }
	int Amount { get; }
}

public interface IPhysiscalObject
{
    bool canGrab { get; }
    bool Grabbed { get; set; }
}

[Serializable]
public class GameWeapon
{
    public WeaponName weapon;
    public WeaponType type;
    public int clipSize = 0;
    public float fireRate = 0; // number of bullets per second.
    public float fireRange = 0;
    public float reloadTime = 0;
}







