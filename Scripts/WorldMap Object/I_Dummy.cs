using UnityEngine;

public interface I_Dummy
{
    public enum CharacterPartsTransform
    {
        LeftHandWeaponRoot = 0,
        RightHandWeaponRoot = 1,
        HeadArmorRoot = 2,
        BodyArmorRoot = 3,
        LeftFootRoot = 4,
        RightFootRoot = 5
    }

    /// <summary>
    /// 캐릭터 아이템 위치 값 정보 초기화
    /// </summary>
    /// <param name="transform">캐릭터</param>
    public void SetDummyCharacterItemPartsTransform(UnityEngine.Transform transform);
    
    /// <summary>
    /// 캐릭터 아이템 착용 시 모델링 업데이트
    /// </summary>
    /// <param name="item">착용된 아이템</param>
    public void UpdateCharacterItemPart(Equipment item, bool isEquip);
    
}