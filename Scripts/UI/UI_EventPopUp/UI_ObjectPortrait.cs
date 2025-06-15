using UnityEngine;
using UnityEngine.UI;

public class UI_ObjectPortrait : UI_Base
{
    private enum PortraitImage
    {
        PortraitImage
    }
    
    public override void Init()
    {
        Bind<Image>(typeof(PortraitImage));
    }
    
    public void SetPortraitTexture(Sprite portrait)
    {
        Get<Image>((int)PortraitImage.PortraitImage).sprite = portrait;
    }
}
