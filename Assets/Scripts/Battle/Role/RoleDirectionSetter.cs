using System;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Role))]
public class RoleDirectionSetter : MonoBehaviour
{
    [SerializeField] private Role role;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(role == null) role = GetComponent<Role>();
    }

    void OnEnable()
    {
        if(role!=null)
        {
            role.directionChangeAction += CaculateRoleDirection;
        }
    }

    void OnDisable()
    {
        if(role!=null)
        {
            role.directionChangeAction -= CaculateRoleDirection;
        }
    }

    // Update is called once per frame
    void Update()
    {
        LerpRoleDirection();   
    }

    [SerializeField] private Vector3 target = Vector3.forward;
    [SerializeField] private float lerpSpeed = 3;
    public float GetLerpSpeed() => lerpSpeed;
    public void SetLerpSpeed(float newLerpSpeed) => lerpSpeed = newLerpSpeed;
    
    private void CaculateRoleDirection()
    {
        if(role == null) return;
        BattleDirection direction = role.GetDirection();
        target = Vector3.zero;
        switch(direction)
        {
            case BattleDirection.UP:
                target = Vector3.forward;
                break;
            case BattleDirection.DOWN:
                target = Vector3.back;
                break;
            case BattleDirection.LEFT:
                target = Vector3.left;
                break;
            case BattleDirection.RIGHT:
            default:
                target = Vector3.right;
                break;
        }
    }

    private void LerpRoleDirection()
    {
        if(role == null) return;
        role.transform.rotation = Quaternion.Lerp(
            role.transform.rotation,//当前方向
            Quaternion.LookRotation(target),//目标方向
            lerpSpeed*Time.deltaTime//插值速度
        );
    }
}
