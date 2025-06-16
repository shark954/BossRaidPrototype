using UnityEngine;
using Mirror;

/// <summary>
/// �v���C���[�܂��͕��킩�甭�˂����e�ۂ̋������Ǘ�����N���X�B
/// �e�̈ړ��A�F�̓����A�����Ǘ����܂ށB
/// </summary>
public class Bullet : NetworkBehaviour
{
    [Header("�e�̐ݒ�")]
    [SerializeField] private float m_bulletSpeed = 10f;             // �e�̈ړ����x
    [SerializeField] private float m_lifeTime = 2f;                 // �e�̎����i�b�j

    [Header("�r�W���A��")]
    [SerializeField] private Renderer m_visualRenderer;             // �e�̌����ځi�}�e���A���̐F��ύX�j

    [SyncVar(hook = nameof(OnColorChanged))]
    private Color m_bulletColor;                                    // �T�[�o�[����N���C�A���g�ɐF�𓯊�

    /// <summary>
    /// ���t���[���A�O���Ɉړ��B
    /// </summary>
    void Update()
    {
        transform.Translate(Vector3.forward * m_bulletSpeed * Time.deltaTime);
    }

    /// <summary>
    /// �T�[�o�[���Œe�̐F��ݒ�B���������B
    /// </summary>
    /// <param name="color">�e�ɓK�p����F</param>
    [Server]
    public void SetColor(Color color)
    {
        m_bulletColor = color;
    }

    /// <summary>
    /// �N���C�A���g���ŐF���ς�����Ƃ��Ƀ}�e���A���̐F��ύX�B
    /// </summary>
    void OnColorChanged(Color oldColor, Color newColor)
    {
        if (m_visualRenderer != null)
        {
            m_visualRenderer.material.color = newColor;
        }
    }

    /// <summary>
    /// �T�[�o�[�ŊJ�n���ꂽ�Ƃ��Ɏ����Ŏ����^�C�}�[���Z�b�g�B
    /// </summary>
    [ServerCallback]
    void Start()
    {
        Invoke(nameof(DestroySelf), m_lifeTime);
    }

    /// <summary>
    /// �l�b�g���[�N�z���ɒe���폜����B
    /// </summary>
    [Server]
    void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }
}
