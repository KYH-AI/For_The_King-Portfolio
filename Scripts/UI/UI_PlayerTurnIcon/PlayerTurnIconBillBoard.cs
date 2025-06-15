using System;
using TMPro;
using UnityEngine;

public class PlayerTurnIconBillBoard : MonoBehaviour
{
   private TextMeshProUGUI _currentTurnCountText;

   // Turn 아이콘 회전 값을 X축 45도 고정 (카메라와 같은 회전각을 이용)
   private static readonly Vector3 _FIXED_ROTATION = new Vector3(45, 0, 0);

   private void Awake()
   {
      _currentTurnCountText = GetComponentInChildren<TextMeshProUGUI>();
   }

   /// <summary>
   /// 플레이어 턴 UI 이벤트 등록 (1회 호출)
   /// </summary>
   /// <param name="player">플레이어</param>
   public void Init(WorldMapPlayerCharacter player)
   {
      // Turn Icon Text 업데이트 이벤트 등록
      player.UpdateTurnCountIcon += UpdateTurnCountText;
      // 플레이어 Turn인 경우 Turn Icon 활성화 이벤트 등록
      player.CheckPlayerTurn += TurnIconView;
      // 플레이어 Turn인 경우 Turn Button 활성화
      player.CheckPlayerTurn += Managers.UIManager.InteractionTurnOverButton;

      // 초기에는 Turn Icon 비활성화
      if (this.gameObject.activeSelf == true)
      {
         TurnIconView(false);
      }
   }
   
   /// <summary>
   /// Turn 아이콘 회전 값 고정
   /// </summary>
   private void FixedUpdate()
   {
      transform.rotation = Quaternion.Euler(_FIXED_ROTATION);
   }
   
   /// <summary>
   /// 플레이어 턴 아이콘 UI Text 업데이트
   /// </summary>
   /// <param name="turnCount">현재 남아있는 이동력</param>
   private void UpdateTurnCountText(int turnCount)
   {
      _currentTurnCountText.text = turnCount.ToString();
   }


   /// <summary>
   /// Turn 아이콘 활성화 또는 비활성화
   /// </summary>
   /// <param name="isEnable">활성화 또는 비활성화</param>
   private void TurnIconView(bool isEnable)
   {
      this.gameObject.SetActive(isEnable);
   }
}
