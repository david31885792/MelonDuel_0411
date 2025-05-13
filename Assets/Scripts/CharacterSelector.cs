using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelector : MonoBehaviour
{
    [SerializeField] private string characterId;

    public void OnSelect()
    {
        PlayerPrefs.SetString("SelectedCharacterID", characterId);
        SceneManager.LoadScene("MainScene");
    }
}
