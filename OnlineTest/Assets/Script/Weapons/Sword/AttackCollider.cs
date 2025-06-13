using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/// <summary>
/// 攻撃用コライダー: 衝突処理を管理し、ダメージを適用するクラス。
/// 同じターゲットに短時間で複数回ダメージが入るのを防ぐ仕組みも含む。
/// </summary>
public class AttackCollider : MonoBehaviour
{
    // 攻撃音を再生するためのAudioSource
    public AudioSource m_audioSource;

    // チーム識別子（自分のチームか敵チームかを区別するため）
    public string m_team;

    public string m_targetTeam;

    // 最近ダメージを与えたターゲットを記録するためのHashSet
    private HashSet<Collider> m_recentlyHitTargets = new HashSet<Collider>();

    // ターゲットが無敵状態になる時間（秒単位）
    public float m_invincibilityDuration = 1.0f;

    // 武器を管理するインターフェース
    private IWeapon m_weapon;

    /// <summary>
    /// 初期化処理。親オブジェクトから武器情報を取得する。
    /// </summary>
    void Start()
    {
        m_targetTeam = null;

        //一番上の親についているIweaponコンポーネントを取得する
        m_weapon = transform.parent.GetComponentInParent<IWeapon>();
        if (m_weapon == null)
        {
            Debug.LogError("親オブジェクトに武器が見つかりません！"); // 武器が見つからない場合にエラーログを表示
        }

        // audioSource の null チェック
        if (m_audioSource == null)
        {
            Debug.LogError("AudioSource が設定されていません！", this);
        }
    }

    /// <summary>
    /// 攻撃コライダーが他のコライダーに衝突した際に実行される処理。
    /// </summary>
    /// <param name="other">衝突したコライダー</param>
    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {

        // 武器が未設定、もしくは対象がParametaコンポーネントを持たない場合は何もしない
        if (m_weapon == null || !other.TryGetComponent<Parameta>(out Parameta parameta))
            return;

        // すでに最近ヒットしたターゲットであれば、処理をスキップ
        if (m_recentlyHitTargets.Contains(other))
            return;

        m_targetTeam = parameta.m_team;

        if (m_team == m_targetTeam || m_targetTeam == null)
        {
            Debug.Log("ターゲットと持ち主が同じ");
            return;
        }

        // ダメージ計算: 武器の基本ダメージとチャージに応じた追加パワーを含める
        int DMG = CalculateDamage(m_weapon.Damage, m_weapon.AddPower, m_weapon.ChargeCount);
        parameta.Hitdamage(DMG, m_team);

        // 最近ヒットしたターゲットのリストに追加し、無敵時間を開始
        m_recentlyHitTargets.Add(other);
        StartCoroutine(RemoveFromRecentlyHitTargets(other));

        // エフェクトを生成し、攻撃音を再生
        if (m_weapon.AttackEffect != null)
        {
            // 攻撃エフェクトを生成し、一定時間後に破壊する
            GameObject dummy = Instantiate(m_weapon.AttackEffect, this.transform.position, this.transform.rotation);


            if (m_audioSource != null)
            {
                m_audioSource.Play(); // 音を再生
            }
            else
            {
                Debug.LogWarning("AudioSource が設定されていないため、攻撃音を再生できません。");
            }

            Destroy(dummy, m_weapon.AttackEffectDelTime);
            // 攻撃後、武器のチャージ数をリセット
            m_weapon.ChargeCount = 0;
        }
        else
        {
            Debug.LogWarning("AttackEffect が設定されていません。エフェクトを生成できません。");
        }

        if (other.CompareTag("EnemyAttack"))
        {
            Destroy(other.gameObject);
        }


        m_targetTeam = null;
    }

    private int CalculateDamage(float baseDamage, float addPower, int chargeCount)
    {

        float chargePower = m_weapon.AddPower * m_weapon.ChargeCount;
        float randomFactor = Random.Range((baseDamage * chargePower) / 2, baseDamage * chargePower);
        // 合計ダメージを四捨五入して整数に変換
        return Mathf.RoundToInt(baseDamage + randomFactor);
    }

    /// <summary>
    /// 指定時間後に対象を「最近ヒットしたターゲット」のリストから削除するコルーチン。
    /// </summary>
    /// <param name="target">リストから削除するコライダー</param>
    /// <returns>コルーチンの待機時間</returns>
    private IEnumerator RemoveFromRecentlyHitTargets(Collider target)
    {
        // 指定した無敵時間だけ待機
        yield return new WaitForSeconds(m_invincibilityDuration);

        // 無敵時間経過後に、対象をリストから削除
        m_recentlyHitTargets.Remove(target);
    }
}
