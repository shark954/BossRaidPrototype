using UnityEngine;
using TMPro;

public class WeaponSelector : MonoBehaviour
{
    public TMP_Dropdown dropdown;

    void Start()
    {
        // ���������ɑI����Ԃ�ۑ�
        OnDropdownChanged(dropdown.value);
        dropdown.onValueChanged.AddListener(OnDropdownChanged);
    }

    public void OnDropdownChanged(int index)
    {
        string weapon = dropdown.options[index].text;
        WeaponSelection.selectedWeapon = weapon;
        Debug.Log("�I�񂾕���: " + weapon);
    }

    public void OnConfirmButton()
    {
        // ���킪���肳�ꂽ�玟�̃V�[���ցi��: ���r�[�V�[���j
        UnityEngine.SceneManagement.SceneManager.LoadScene("LobbyScene");
    }
}
