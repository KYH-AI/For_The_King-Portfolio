using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowHighlight : MonoBehaviour
{
    private Dictionary<Renderer, Material[]> _glowMaterialDictionary = new Dictionary<Renderer, Material[]>();
    
    private Dictionary<Renderer, Material[]> _originalMaterialDictionary = new Dictionary<Renderer, Material[]>();
    
    private Dictionary<Color, Material> _cachedGlowMaterials = new Dictionary<Color, Material>();

    [SerializeField] private Material _glowMaterial;
    
    private bool _isGlowing = false;

    public bool IsGlowing => _isGlowing;
    
    private Color _validSpaceColor = Color.green;

    private Color _selectSpaceColor = Color.yellow;
    
    private Color _originalGlowColor;

    public void Init()
    {
        PrePareMaterialDictionary();
        _originalGlowColor = _glowMaterial.GetColor("_GlowColor"); // Glow Shader 에셋에 접근함
    }
    
    private void PrePareMaterialDictionary()
    {
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            bool hasLegacyShader = false;
            
            foreach (var legacyShader in renderer.materials)
            {
                // Legacy 쉐이더 체크
                if (legacyShader.shader.name.StartsWith("Legacy Shaders/")) hasLegacyShader = true;
            }

            if (hasLegacyShader) return;
            
            
            Material[] originalMaterials = renderer.materials;

            _originalMaterialDictionary.Add(renderer, originalMaterials);
            
            Material[] newMaterials = new Material[renderer.materials.Length];
            
            for (int i = 0; i < originalMaterials.Length; i++)
            {
                
      
                
                Material mat = null;
                if (_cachedGlowMaterials.TryGetValue(originalMaterials[i].color, out mat) == false)
                {
                    mat = new Material(_glowMaterial);
                    mat.color = originalMaterials[i].color;
                    _cachedGlowMaterials[mat.color] = mat;
                }
                newMaterials[i] = mat;
            }
            _glowMaterialDictionary.Add(renderer, newMaterials);
        }
    }


    public void ToggleGlow()
    {
        if (_isGlowing == false)
        {
            ResetGlowHighlight();

            foreach (Renderer renderer in _originalMaterialDictionary.Keys)
            {
                renderer.materials = _glowMaterialDictionary[renderer];
            }
        }
        else
        {
            foreach (Renderer renderer in _originalMaterialDictionary.Keys)
            {
                renderer.materials = _originalMaterialDictionary[renderer];
            }
        }

        _isGlowing = !_isGlowing;
    }

    public void ToggleGlow(bool state)
    {
        if (_isGlowing == state) return;
        _isGlowing = !state;
        ToggleGlow();
    }

    public void ResetGlowHighlight()
    {
        // 23-11-11 추가됨 경로 Hex에 머티리얼 원복으로 복원
        foreach (Renderer renderer in _originalMaterialDictionary.Keys)
        {
            renderer.materials = _originalMaterialDictionary[renderer];
        }
        
        /* 23-11-11 최대 이동범위 Hex 머리티얼 대신 경로 Hex에만 머티리얼을 활성화는 걸로 변경
        foreach (Renderer renderer in _glowMaterialDictionary.Keys)
        {
            foreach (var item in _glowMaterialDictionary[renderer])
            {
                item.SetColor("_GlowColor", _originalGlowColor); // Hex Tile의 경로를 모두 원본 값으로 변경
            }
        }
        */
        
    }

    public void HighlightValidPath()
    {
        // 23-11-11 추가됨 경로 Hex에 하이라이트 머티리얼으로 변경
       foreach (Renderer renderer in _originalMaterialDictionary.Keys)
        {
             renderer.materials = _glowMaterialDictionary[renderer];
        }
        
       
        /*  23-11-11 최대 이동범위 Hex 머리티얼 대신 경로 Hex에만 머티리얼을 활성화는 걸로 변경
        if (_isGlowing == false) return;
         
          foreach (var renderer in _glowMaterialDictionary.Keys)
        {
            foreach (var item in _glowMaterialDictionary[renderer])
            {
                item.SetColor("_GlowColor", _validSpaceColor); // 선택된 Hex Tile 경로의 색상을 변경
            }
        }
        */
    }

}
