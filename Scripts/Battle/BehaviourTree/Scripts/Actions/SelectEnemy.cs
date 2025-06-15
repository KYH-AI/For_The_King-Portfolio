using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using UnityEngine.UI;

[System.Serializable]
public class SelectEnemy : ActionNode
{
    Vector3 _nowpos;
    Vector3 _vectorAB;
    float _angle;
    bool _isTartgetChange =false;
    void HexFalse()
    {
        context.battleUnit.FXList[0].SetActive(false);
        context.battleUnit.FXList[1].SetActive(false);
        context.battleUnit.FXList[2].SetActive(false);
        context.battleUnit.Target.FXList[0].SetActive(false);
        context.battleUnit.Target.FXList[1].SetActive(false);
        context.battleUnit.Target.FXList[2].SetActive(false);
    }
    void SetHex()
    {
        if (context.uI_BattlePopUp.IsSkillChange|| _isTartgetChange)
        {
            if (context.uI_BattlePopUp.Clickskill.Target == 1)
            {
                context.battleUnit.FXList[1].SetActive(true);
                context.battleUnit.FXList[0].gameObject.SetActive(false);
                context.battleUnit.FXList[1].transform.localEulerAngles = new Vector3(-90, 180-_angle, 0);
            }
            else
            {
                context.battleUnit.FXList[0].gameObject.SetActive(true);
                context.battleUnit.FXList[1].SetActive(false);
                context.battleUnit.FXList[0].transform.localEulerAngles = new Vector3(-90, 180-_angle, 0);

            }
            context.uI_BattlePopUp.IsSkillChange=false;
        }
        if (_isTartgetChange)
        {
            context.battleUnit.Target.FXList[2].SetActive(true);
            context.battleUnit.Target.FXList[2].transform.localEulerAngles = new Vector3(-90, 180 - _angle, 0);
            _isTartgetChange = false;
        }
        
    }
    protected override void OnStart()
    {
        _nowpos = context.gameObject.GetComponent<BattleUnit>().nowpos;
        context.uI_BattlePopUp.IsSkillChange=true;
        _isTartgetChange = true;
        _vectorAB = context.battleUnit.Target.nowpos - _nowpos;
        _angle = Mathf.Atan2(_vectorAB.z, _vectorAB.x) * Mathf.Rad2Deg;
        context.gameObject.transform.rotation = Quaternion.Euler(0, 90 - _angle, 0f);
        if (Managers.Battle.IsCave)
        {
            _angle += 180;
        }
        Managers.Battle.SelectedEnemyIcon(context.battleUnit.Target);
        
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        if (!context.battleUnit.IsAttack)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.gameObject.CompareTag("Enemy"))
                    {
                        context.battleUnit.Target.FXList[2].SetActive(false);
                        context.battleUnit.Target = hit.transform.gameObject.GetComponent<BattleUnit>();
                        _vectorAB = context.battleUnit.Target.nowpos - _nowpos;
                        _angle = Mathf.Atan2(_vectorAB.z, _vectorAB.x) * Mathf.Rad2Deg;
                        context.gameObject.transform.rotation = Quaternion.Euler(0, 90 - _angle, 0f);
                        if (Managers.Battle.IsCave)
                        {
                            _angle += 180;
                        }
                        _isTartgetChange = true;
                        Managers.Battle.SelectedEnemyIcon(context.battleUnit.Target);
                        
                    }
                }
            }
            SetHex();

            return State.Running;
        }
        else
        {
            context.battleUnit.IsAttack = false;
            HexFalse();
            return State.Success;
        }

    }
}
