using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move
{
    [SerializeField] private Transform _trans;
    [SerializeField] private float _speed;
    private Vector2 _velocity { get => _trans.GetComponent<Rigidbody2D>().velocity; set => _trans.GetComponent<Rigidbody2D>().velocity = value; }

    public Move(Transform playerTrans, float speed)
    {
        _trans = playerTrans;
        _speed = speed;
    }
    public void DoMove(Vector2 direc, float tick)
    {
        if (direc == Vector2.zero)
        {
            _velocity = Vector2.zero;
        }
        _velocity += direc.normalized * _speed * tick * 10;
        _velocity = Vector2.ClampMagnitude(_velocity, _speed);

    }

    #region PRIVATE===================================================
    #endregion

    #region INPUT====================================================
    #endregion

    #region OUTPUT====================================================
    #endregion
}
