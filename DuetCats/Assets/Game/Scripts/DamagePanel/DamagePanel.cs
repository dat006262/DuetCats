using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamagePanel : SingletonMonoBehaviour<DamagePanel>
{
    #region DATA,REFERENT,INIT========================================
    public GameObject damageShowPrefab;
    public GameObject holder;
    #endregion

    #region UNITY FUNC================================================

    #endregion

    #region PUBLIC====================================================
    public void SpawnDamageShow(Vector2 startPos, string text)
    {
        GameObject gameObject = ObjectPool_Advanced.Instance.GetObject(damageShowPrefab);
        gameObject.transform.SetParent(holder.GetComponent<RectTransform>(), false);
        //  GameObject gameObject = Instantiate(damageShowPrefab, holder.transform);
        DamageShow x = new DamageShow(gameObject, startPos, gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>());
        x.RunAnimTxtShowDamage(text);

    }
    #endregion

    #region PRIVATE===================================================
    #endregion

    #region INPUT====================================================
    #endregion

    #region OUTPUT====================================================
    #endregion
}
