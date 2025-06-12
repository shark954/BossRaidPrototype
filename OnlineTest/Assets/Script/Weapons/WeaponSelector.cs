using UnityEngine;
using TMPro;

public class WeaponSelector : MonoBehaviour
{
    public TMP_Dropdown dropdown;

    void Start()
    {
        // 初期化時に選択状態を保存
        OnDropdownChanged(dropdown.value);
        dropdown.onValueChanged.AddListener(OnDropdownChanged);
    }

    public void OnDropdownChanged(int index)
    {
        string weapon = dropdown.options[index].text;
        WeaponSelection.selectedWeapon = weapon;
        Debug.Log("選んだ武器: " + weapon);
    }

    public void OnConfirmButton()
    {
        // 武器が決定されたら次のシーンへ（例: ロビーシーン）
        UnityEngine.SceneManagement.SceneManager.LoadScene("LobbyScene");
    }
}
