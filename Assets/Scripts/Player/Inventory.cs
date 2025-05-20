using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Inventory
{
    [SerializeField] private int _limiteInventory;
    [SerializeField] private List<GameObject> _inventory = new List<GameObject>();
    [SerializeField] private GameObject _element0;
    [SerializeField] public int _weaponSelected; public int WeaponSelected { get { return _weaponSelected; } }
    [SerializeField] public int _lastWeaponSelected; public int LastWeaponSelected { get { return _lastWeaponSelected; } }

    public Inventory(int limiteInventory, GameObject element0)
    {
        _limiteInventory = limiteInventory;
        _element0 = element0;
        AddWeapon(element0);
        _weaponSelected = 0;
    }

    public void AddWeapon(GameObject weapon)
    {
        if (_inventory.Count >= _limiteInventory)
        {
            Debug.Log("Inventario lleno");
        }
        if (!_inventory.Contains(weapon))
        {
            _inventory.Add(weapon);
            SelectWeapon(_inventory.Count - 1);
            _weaponSelected = _inventory.Count - 1;
        } else
        {
            Debug.Log("La arma ya esta en el inventario");
        }
    }

    public void RemoveWeapon(GameObject weapon)
    {
        if (weapon = _element0)
        {
            Debug.Log("Elemento no eliminable");
            return;
        }
        if (_inventory.Contains(weapon)) _inventory.Remove(weapon);
    }

    public GameObject SelectWeapon(int index)
    {
        if (_weaponSelected != 0) 
        {
            _inventory[_weaponSelected].SetActive(false); //Oculto arma anterior
        }

        _inventory[_weaponSelected].GetComponent<Weapon>().MyBodyFBX.SetActive(false); //Oculto el cuerpo anterior
        
        _weaponSelected = index;
        _inventory[_weaponSelected].SetActive(true); //Muestro arma nueva
        _inventory[_weaponSelected].GetComponent<Weapon>().MyBodyFBX.SetActive(true); //Oculto el cuerpo anterior
        return _inventory[_weaponSelected];
    }

    public Animator MyCurrentAnimator()
    {
        return _inventory[_weaponSelected].GetComponent<Weapon>().MyAnimator;
    }
}
