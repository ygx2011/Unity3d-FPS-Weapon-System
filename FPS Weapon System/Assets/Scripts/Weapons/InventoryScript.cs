using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoryScript : MonoBehaviour {

    public List<Weapon> weapons;

    public List<GameObject> gameWeapons;

    public int weaponIndex = 0;

    public Weapon weapon;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (InventorySize > 0)
        {

            if (Input.GetAxisRaw("Switch Weapon") > 0)
                weaponIndex++;
            else if (Input.GetAxisRaw("Switch Weapon") < 0)
                weaponIndex--;

            weaponIndex = Mathf.Clamp(weaponIndex, 0, InventorySize);

            weapon = weapons[weaponIndex];
        }

	}

    public void SwitchWeapon(int _index)
    {
        weaponIndex = _index;
    }

    /// <summary>
    /// Add weapon to inventory.
    /// </summary>
    /// <param name="_weapon">weapon to add.</param>
    /// <returns>Index of add weapon.</returns>
    public int AddWeapon(Weapon _weapon)
    {
        weapons.Add(_weapon);
        return weapons.IndexOf(_weapon);
    }

    /// <summary>
    /// this will instantiate weapon GameObject as child of WeaponManager.
    /// </summary>
    /// <param name="_weaponInfo">Information to instantiate GameObject correctly.</param>
    public void InstantiateWeapon(WeaponData _weaponInfo)
    {
        
    }

    public int InventorySize 
    { 
        get 
        { 
            return weapons.Count - 1;
        }
    }

}
