public class WorldMapHexMovementSlot
{
    /* <주 목표>
     *  1. 플레이어가 기본적으로 이동할 수 있는 최소 슬롯이 존재함 지형보정과 확정이동보정 2가지로 나뉨
     *  2. 지형의 보정은 플레이어가 얻어야 하는 슬롯을 의미함
     *      지형 보정  (육지 : 3 ,바다 : 보트종류에 따라)
     *  3. 기본 보정은 플레이어를 확정적으로 이동력을 부여함 (황금색 이동 아이콘임)
     *      기본 보정  (육지 : 2, 바다 : 보트종류에 따라)
     *
     *  4. 육지 기준으로 보면 플레이어가 이동할수 있는 이동판정은 5개, 그 중 2개는 확정으로 이동 나머지 3개는 플레이어의 이동능력치로 계산해서 얻음
     */     
    
    
    /// <summary>
    /// 지형에 대한 이동력 보정
    /// </summary>
    /// <param name="hexType">현재 위치한 지형</param>
    /// <returns>지형 보정 이동력</returns>
    public static int GetHexTerrainSlots(HexType hexType)
    {
        int slots = 0;
        
        switch (hexType)
        {
            //  case HexType.Water : slots = // TODO: 보트에 대한 정보를 받아와 보정 부여
            default: slots = 3; break;
        }

        return slots;
    }

    /// <summary>
    /// 기본 이동력 보정
    /// </summary>
    /// <param name="hexType">현재 위치한 지형</param>
    /// <returns>기본 보정 이동력</returns>
    public static int GetHexBaseSlots(HexType hexType)
    {
        int slots = 0;
        
        switch (hexType)
        {
            //  case HexType.Water : slots = // TODO: 보트에 대한 정보를 받아와 보정 부여
            default: slots = 2; break;
        }

        return slots;
    }
    
}