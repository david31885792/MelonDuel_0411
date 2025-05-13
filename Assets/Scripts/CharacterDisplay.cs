using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterDisplay : MonoBehaviour
{
    [System.Serializable]
    public class CharacterData
    {
        public string characterId;
        public Sprite characterSprite; // ✅ 캐릭터 이미지 (UI용)
        public Sprite skillIcon;       // ✅ 스킬 아이콘 이미지
    }

    [Header("캐릭터/아이콘 UI 이미지")]
    public Image characterImageSlot;  // 캐릭터를 보여줄 Image
    public Image skillIconFrame;      // 스킬 아이콘을 보여줄 Image

    [Header("캐릭터 리스트")]
    public List<CharacterData> characterList;

    private void Start()
    {
        string selectedId = PlayerPrefs.GetString("SelectedCharacterID", "player1");

        CharacterData selectedCharacter = characterList.Find(c => c.characterId == selectedId);

        if (selectedCharacter != null)
        {
            if (characterImageSlot != null)
            {
                characterImageSlot.sprite = selectedCharacter.characterSprite;
                characterImageSlot.SetNativeSize(); // ← 여기에서 원본 크기로 조정
            }

            if (skillIconFrame != null)
            {
                skillIconFrame.sprite = selectedCharacter.skillIcon;
                skillIconFrame.SetNativeSize(); // ← 아이콘도 원본 크기로 조정
            }
        }
        else
        {
            Debug.LogWarning($"⚠️ Character ID '{selectedId}'에 해당하는 데이터를 찾을 수 없습니다.");
        }
    }

}
