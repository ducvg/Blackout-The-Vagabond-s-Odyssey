﻿
using Assets.Script.Weapon.Throwable_Weapon;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.Tilemaps;
//using static UnityEngine.GraphicsBuffer;

public class PlayerPickController : MonoBehaviour
{
    [SerializeField]
    private InventorySO inventoryData;
    public bool isTouchItem = false;
    public GameObject Item;
    public bool isWeapon;
    BaseWeapon weapon1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<IPick>(out var weapon))
        {
            Item = weapon.GetPickGameOject();
            isTouchItem = true;
            // them UI hien thong tin vu khi la cai weapon detail so
            //weapon1=Item.GetComponent<BaseWeapon>();
            //WeaponDetailSO weaponDetailSO = weapon1.GetWeaponDetailSO();
            //ShowUIPanel(weaponDetailSO);
            if (weapon.IsPickingItemOrWeapon())
            {
                WeaponDetailSO currentWeaponDetail = GetComponentInChildren<BaseWeapon>().GetWeaponDetailSO();
                weapon1 = Item.GetComponent<BaseWeapon>();
                WeaponDetailSO weaponTouchingDetail = weapon1.GetWeaponDetailSO();
                ShowUIPanel(weaponTouchingDetail);
                string s1 = $"Damage {currentWeaponDetail.damageWeapon}, Time between attack {currentWeaponDetail.attackCooldown}, Force {currentWeaponDetail.force}";
                string s2 = $"Damage {weaponTouchingDetail.damageWeapon}, Time between attack {weaponTouchingDetail.attackCooldown}, Force {weaponTouchingDetail.force}";
                //RecommendAI.INSTANCE.CompareItems(s1, s2,currentWeaponDetail.weaponName, weaponDetailSO.weaponName);
                Talking.INSTANCE.Talk(weaponTouchingDetail.weaponName);
                isWeapon = true;
                StartCoroutine(ThinkUsingNomic(s1, s2, currentWeaponDetail.weaponName, weaponTouchingDetail.weaponName));
            }
            else
            {
                isWeapon = false;
                Talking.INSTANCE.Talk(Item.GetComponent<Item>().inventoryItem.name);
            }
        }
    }

    IEnumerator ThinkUsingNomic(string itemADescription, string itemBDescription, string weapon1Name, string weapon2Name)
    {
        yield return new WaitForSeconds(2f);
        RecommendAI.INSTANCE.CompareItems(itemADescription, itemBDescription, weapon1Name, weapon2Name);
        StartCoroutine(ThinkUsingGemini(itemADescription, itemBDescription, weapon1Name, weapon2Name));
    }
    IEnumerator ThinkUsingGemini(string itemADescription, string itemBDescription, string weapon1Name, string weapon2Name)
    {
        yield return new WaitForSeconds(3f);
        RecommendAI.INSTANCE.ThinkAboutTwoWeapon(itemADescription, itemBDescription, weapon1Name, weapon2Name);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.gameObject.TryGetComponent<IPick>(out var weapon))
        {
            Item = weapon.GetPickGameOject();
            isTouchItem = true;         
            if (weapon.IsPickingItemOrWeapon())
            {
                weapon1 = Item.GetComponent<BaseWeapon>();
                WeaponDetailSO weaponDetailSO = weapon1.GetWeaponDetailSO();
                ShowUIPanel(weaponDetailSO);
                isWeapon = true;
            }
            else
            {
                isWeapon = false;
            }
        }

    }
    private void OnTriggerExit2D()
    {
        if (Item != null)
        {
            StopAllCoroutines();
            isTouchItem = false;
            Item = null;
            HideUIPanel();
            Talking.INSTANCE.Talk("");
        }
        
    }
    void ShowUIPanel(WeaponDetailSO weaponDetailSO)
    {
        if (weaponDetailSO == null)
        {
            Debug.LogWarning("Thieu WeaponDetailSO, de nghi gan vao vu khi gap!!!!");
            return;
        }
        WeaponDetailUI.weaponDetailUI.UpdatePanel(weaponDetailSO);
        WeaponDetailUI.weaponDetailUI.showPanel();
    }
    void HideUIPanel()
    {
        WeaponDetailUI.weaponDetailUI.hidePanel();
    }
    public void PickItemWeapon()
    {
        if (isWeapon)
        {
            Item.transform.SetParent(GetComponentInChildren<PlayerWeaponController>().gameObject.transform, false);
            GetComponentInChildren<PlayerWeaponController>().ChangeAndAddWeaponIfHaveOne(Item);
            var weapon = Item.GetComponent<IPick>();
            weapon.Pick();
        }
        else
        {
            var item = Item.GetComponent<Item>();
            int reminder = inventoryData.AddItem(item.inventoryItem, item.quantity);
            if (reminder == 0)
            {
                item.DestroyItem();
            }
            else
            {
                item.quantity = reminder;
            }
        }

    }

}