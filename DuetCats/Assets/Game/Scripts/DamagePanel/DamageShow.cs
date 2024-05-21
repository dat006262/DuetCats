using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageShow
{
    #region DATA,REFERENT,INIT========================================
    [SerializeField] GameObject _textTrans;
    [SerializeField] TextMeshProUGUI _textMeshProUGUI;
    public Vector3 _startPos;
    Tween tween;
    #endregion
    public DamageShow(GameObject textTras, Vector2 startPos, TextMeshProUGUI textMeshProUGUI)
    {
        _textTrans = textTras;
        _startPos = startPos;
        _textMeshProUGUI = textMeshProUGUI;
    }


    public void RunAnimTxtShowDamage(string text)
    {

        _textMeshProUGUI.text = text;
        if (tween != null)
        {
            tween.Pause();
        }
        _textTrans.transform.position = _startPos;
        tween = _textTrans.transform.DOMove(_textTrans.transform.position + Vector3.up * 1, 0.5f)
            .OnKill(() => { _textTrans.SetActive(false); });
    }


}
