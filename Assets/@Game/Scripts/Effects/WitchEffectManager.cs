using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WitchEffectManager : MonoBehaviour
{
    #region Instance
    private static WitchEffectManager mInstance;
    public static WitchEffectManager instance
    {
        get
        {
            if (mInstance == null)
                mInstance = FindObjectOfType<WitchEffectManager>();
            return mInstance;
        }
    }
    #endregion

    #region Enum
    private enum EffectType
    {
        Meteor,
        FireStorm,

        HealingWave,
        ChainIceBolt,

        JudgementSword,
        LigteningStrike,

        CircleOfCurse,
        DarkMist
    }
    #endregion

    #region Inspector
    [Header("Floor")]
    [SerializeField] private GameObject floorObject;

    [Header("Meteor")]
    [SerializeField] private ParticleSystem fireCirclePrefab;
    [SerializeField] private GameObject meteorStonePrefab;

    [SerializeField] private ParticleSystem meteorExplosion_1Prefab;
    [SerializeField] private ParticleSystem meteorExplosion_2Prefab;
    [Space]

    [Header("Magma Impact")]
    [SerializeField] private ParticleSystem magmaImpactPrefab;
    [SerializeField] private ParticleSystem fireCirclePrefab2;
    [Space]

    [Header("Healing Wave")]
    [SerializeField] private ParticleSystem healingOraPrefab;
    [SerializeField] private ParticleSystem healingEffectPrefab;
    [Space]

    [Header("Chain Icebolt")]
    [SerializeField] private ParticleSystem iceCirclePrefab;
    [SerializeField] private ParticleSystem chainIceboltPrefab;

    [Header("Judgement Sword")]
    [SerializeField] private ParticleSystem swordAirPrefab;
    [SerializeField] private ParticleSystem swordCraterPrefab;
    [SerializeField] private ParticleSystem swordAoePrefab;
    [SerializeField] private ParticleSystem swordCraterULTPrefab;
    [SerializeField] private ParticleSystem swordULTPrefab;

    [Header("Lightening Strike")]
    [SerializeField] private ParticleSystem lightningStrikePrefab;
    [SerializeField] private ParticleSystem lightningStrike2Prefab;
    [SerializeField] private ParticleSystem lightCirclePrefab;
    [Space]

    [Header("Circle of Curse")]
    [SerializeField] private ParticleSystem curseStonePrefab;
    [SerializeField] private ParticleSystem curseCirclePrefab;
    [SerializeField] private ParticleSystem curseExplosionPrefab;
    [Space]

    [Header("Dark Mist")]
    [SerializeField] private ParticleSystem mistEffectPrefab;
    [SerializeField] private ParticleSystem mistExplosionPrefab;
    [Space]

    [Header("Meteor Options")]
    [SerializeField] private float fireCircleDelay;
    [SerializeField] private float meteorDropSpeed;
    [SerializeField] private float meteorSpawn_X;
    [SerializeField] private float meteorSpawn_Y;
    [SerializeField] private float meteorSpawn_Z;
    [SerializeField] private float meteorEffectPosition_Y;
    [SerializeField] private float meteorSkillDelayTime;
    [Space]

    [Header("Magma Options")]
    [SerializeField] private float magmaImpactDelay;
    [SerializeField] private int totalMagmaCount;
    [SerializeField] private float magmaImpactTotalSkillTime;
    [Space]

    [Header("Healing Options")]
    [SerializeField] private float healingTime;
    [SerializeField] private float effectPosition_Z;
    [SerializeField] private float effectPosition_Y;
    [SerializeField] private float oraPosition_z;


    [Header("Ice Bolt Options")]
    [SerializeField] private float icebolt_1_X;
    [SerializeField] private float icebolt_1_Y;
    [SerializeField] private float icebolt_1_Z;
    [Space]

    [SerializeField] private float icebolt_2_X;
    [SerializeField] private float icebolt_2_Y;
    [SerializeField] private float icebolt_2_Z;
    [Space]

    [SerializeField] private float icebolt_3_X;
    [SerializeField] private float icebolt_3_Y;
    [SerializeField] private float icebolt_3_Z;
    [Space]

    [SerializeField] private float icebolt_4_X;
    [SerializeField] private float icebolt_4_Y;
    [SerializeField] private float icebolt_4_Z;
    [Space]

    [SerializeField] private float icebolt_5_X;
    [SerializeField] private float icebolt_5_Y;
    [SerializeField] private float icebolt_5_Z;
    [Space]

    [SerializeField] private float enemyPos_y;
    [SerializeField] private float iceCircleDelay;
    [SerializeField] private float iceboltDelay;
    [SerializeField] private float iceboltSkillDelayTime;
    [Space]

    [Header("Lightning Strike Options")]
    [SerializeField] private float lightningPosition_Y;
    [SerializeField] private float lightningPosition_Z;
    [SerializeField] private float lightCircleDelay;
    [SerializeField] private float lightningSkillDelayTime;
    [Space]

    [Header("Judgement Sword Options")]
    [SerializeField] private float swordAoeDelay;
    [SerializeField] private float swordShootingDelay;
    [SerializeField] private float swordTime;
    [Space]

    [Header("Curse of Circle Options")]
    [SerializeField] private float curseDropDelay;
    [SerializeField] private float stoneDropPosition_Y;
    [SerializeField] private float curseExplosionDelay;
    [SerializeField] private float explosionPosition_Y;
    [SerializeField] private float curseTotalDelayTime;
    [Space]

    [Header("Dark Mist Options")]
    [SerializeField] private float mistTotalDelayTime;
    [SerializeField] private float mistPosition_Z;
    [SerializeField] private float mistPosition_Y;

    #endregion

    #region Private Fields
    //[SerializeField] private float iceboltTime;
    #endregion

    #region Private Methods
    private Vector3 GetNormal(Vector3 pos, Vector3 tar)
    {
        Vector3 normal = (pos - tar).normalized;
        return normal;
    }
    #endregion

    #region MonoBehaviour Callbacks
    private void Start()
    {
    }
    #endregion

    #region Effect
    #region Meteor

    public void MeteorEffect(Vector3 position, Vector3 target)
    {
        StartCoroutine(Meteor(position, target));
    }

    public float GetMeteorTime()
    {
        float skillTime = meteorSkillDelayTime;
        return skillTime;
    }

    IEnumerator Meteor(Vector3 position, Vector3 target)
    {
        Vector3 normalVector = -Vector3.up;
        Vector3 circleVector = new Vector3(0, 0.05f, 0);
        var circlePrefab = fireCirclePrefab;
        var circleInstance = Instantiate(circlePrefab, target + circleVector, Quaternion.LookRotation(normalVector));

        yield return new WaitForSeconds(fireCircleDelay);

        float _directionX = Mathf.Sign(position.x - target.x) * meteorSpawn_X;
        float _directionZ = meteorSpawn_Z;
        int _random = Random.Range(1, 3);
        if (_random == 1)
        {
            _directionZ *= -1;
        }

        Vector3 _meteorSpawnPosition = new Vector3(target.x + _directionX, meteorSpawn_Y, target.z + _directionZ);
        var targetPrefab = meteorStonePrefab;
        var meteorInstance = Instantiate(targetPrefab, _meteorSpawnPosition, Quaternion.LookRotation(normalVector));
        meteorInstance.transform.LookAt(target);

        while (meteorInstance.transform.position.y > target.y)
        {
            meteorInstance.transform.position = Vector3.MoveTowards(meteorInstance.transform.position, target, meteorDropSpeed);
            yield return null;
        }
        Destroy(meteorInstance);

        var effectPrefab_1 = meteorExplosion_1Prefab;
        var effectPrefab_2 = meteorExplosion_1Prefab;

        Vector3 effectVector = new Vector3(0f, meteorEffectPosition_Y, 0f);

        target.x = target.x - 1f;
        var effectInstance_1 = Instantiate(effectPrefab_1, target + effectVector, Quaternion.LookRotation(-normalVector));
        var effectInstance_2 = Instantiate(effectPrefab_2, target + effectVector, Quaternion.LookRotation(-normalVector));
    }

    #endregion

    #region Magma Impact

    public void MagmaImpactEffect(Vector3 position, Vector3 target)
    {
        StartCoroutine(MagmaImpact(position, target));
    }
    public float GetMagmaImpactTime(Vector3 position, Vector3 target)
    {
        float skilltime = magmaImpactTotalSkillTime;
        return skilltime;
    }

    IEnumerator MagmaImpact(Vector3 position, Vector3 target)
    {
        float _pos = position.x;
        float _tar = target.x;
        float _totalDis = _tar - _pos;
        float _dis = _totalDis / totalMagmaCount;
        int _counter = 1;

        Vector3 nowPos = new Vector3(position.x, -3.4f, position.z);
        Vector3 disVector = new Vector3(_dis, 0f, 0f);

        Vector3 normalVector = Vector3.up;

        var circlePrefab = fireCirclePrefab2;
        var magmaPrefab = magmaImpactPrefab;

        while (_counter <= totalMagmaCount)
        {
            var circleInstance = Instantiate(circlePrefab, nowPos + (disVector * _counter), Quaternion.LookRotation(normalVector));
            var magmaInstance = Instantiate(magmaPrefab, nowPos + (disVector * _counter), Quaternion.LookRotation(normalVector));
            Destroy(magmaInstance, 5f);
            ++_counter;
            yield return new WaitForSeconds(magmaImpactDelay);
        }
        yield return null;
    }
    #endregion

    #region Healing Wave

    public void HealingWaveEffect(Vector3 position)
    {
        StartCoroutine(HealingWave(position));
    }
    public float GetHealingWaveTime()
    {
        return healingTime;
    }

    IEnumerator HealingWave(Vector3 position)
    {
        position.z += effectPosition_Z;
        Vector3 effectPos = new Vector3(0, 0, effectPosition_Z);
        Vector3 oraPos = new Vector3(0, 0, oraPosition_z);
        var targetPrefab = healingOraPrefab;
        Vector3 normal = Vector3.up;
        Vector3 normalZero = Vector3.zero;

        var instanceOra = Instantiate(targetPrefab, position + effectPos, Quaternion.LookRotation(normalZero));

        yield return new WaitForSeconds(1f);

        targetPrefab = healingEffectPrefab;
        var instanceEffect = Instantiate(targetPrefab, position + oraPos, Quaternion.LookRotation(normal));

        yield return new WaitForSeconds(healingEffectPrefab.main.duration);
        instanceOra.Stop();
    }
    #endregion

    #region Chain Icebolt

    public void ChainIceboltEffect(Vector3 position, Vector3 target)
    {
        StartCoroutine(ChainIcebolt(position, target));
    }

    public float GetIceboltTime()
    {
        float skillTime = iceboltSkillDelayTime;
        return skillTime;
    }

    IEnumerator ChainIcebolt(Vector3 position, Vector3 target)
    {

        float _directionX = Mathf.Sign(target.x - position.x);
        Vector3 circle_1Pos = new Vector3(position.x + (_directionX * icebolt_1_X), position.y + icebolt_1_Y, position.z + icebolt_1_Z);
        Vector3 circle_2Pos = new Vector3(position.x + (_directionX * icebolt_2_X), position.y + icebolt_2_Y, position.z + icebolt_2_Z);
        Vector3 circle_3Pos = new Vector3(position.x + (_directionX * icebolt_3_X), position.y + icebolt_3_Y, position.z + icebolt_3_Z);
        Vector3 circle_4Pos = new Vector3(position.x + (_directionX * icebolt_4_X), position.y + icebolt_4_Y, position.z + icebolt_4_Z);
        Vector3 circle_5Pos = new Vector3(position.x + (_directionX * icebolt_5_X), position.y + icebolt_5_Y, position.z + icebolt_5_Z);

        Vector3 normalVector = Vector3.up;
        target.y += enemyPos_y;

        var circlePrefab = iceCirclePrefab;
        var circleInstance_3 = Instantiate(circlePrefab, circle_3Pos, Quaternion.LookRotation(normalVector));
        circleInstance_3.transform.LookAt(target);
        yield return new WaitForSeconds(0.2f);

        var circleInstance_2 = Instantiate(circlePrefab, circle_2Pos, Quaternion.LookRotation(normalVector));
        var circleInstance_4 = Instantiate(circlePrefab, circle_4Pos, Quaternion.LookRotation(normalVector));
        circleInstance_2.transform.LookAt(target);
        circleInstance_4.transform.LookAt(target);
        yield return new WaitForSeconds(0.2f);

        var circleInstance_1 = Instantiate(circlePrefab, circle_1Pos, Quaternion.LookRotation(normalVector));
        var circleInstance_5 = Instantiate(circlePrefab, circle_5Pos, Quaternion.LookRotation(normalVector));
        circleInstance_1.transform.LookAt(target);
        circleInstance_5.transform.LookAt(target);

        yield return new WaitForSeconds(iceCircleDelay);

        var iceboltPrefab = chainIceboltPrefab;

        var iceboltInstance_1 = Instantiate(iceboltPrefab, circle_1Pos, Quaternion.LookRotation(normalVector));
        iceboltInstance_1.transform.LookAt(target);
        yield return new WaitForSeconds(iceboltDelay);

        var iceboltInstance_2 = Instantiate(iceboltPrefab, circle_2Pos, Quaternion.LookRotation(normalVector));
        iceboltInstance_2.transform.LookAt(target);
        yield return new WaitForSeconds(iceboltDelay);

        var iceboltInstance_3 = Instantiate(iceboltPrefab, circle_3Pos, Quaternion.LookRotation(normalVector));
        iceboltInstance_3.transform.LookAt(target);
        yield return new WaitForSeconds(iceboltDelay);

        var iceboltInstance_4 = Instantiate(iceboltPrefab, circle_4Pos, Quaternion.LookRotation(normalVector));
        iceboltInstance_4.transform.LookAt(target);
        yield return new WaitForSeconds(iceboltDelay);

        var iceboltInstance_5 = Instantiate(iceboltPrefab, circle_5Pos, Quaternion.LookRotation(normalVector));
        iceboltInstance_5.transform.LookAt(target);
        yield return null;
    }

    #endregion

    #region Judgement Sword

    public void JudgementSwordEffect(Vector3 position, Vector3 target)
    {
        StartCoroutine(JudgementSword(position, target));
    }
    public float GetJudgementSwordTime()
    {
        return swordTime;
    }

    IEnumerator JudgementSword(Vector3 position, Vector3 target)
    {
        position.y = floorObject.transform.position.y + 0.5f; // ����
        target.y = position.y + 5f; // ����

        var targetPrefab = swordAirPrefab;
        Vector3 normal = Vector3.zero;
        var instanceAir = Instantiate(targetPrefab, position, Quaternion.LookRotation(normal));

        yield return new WaitForSeconds(swordShootingDelay);

        //targetPrefab = swordCraterPrefab;
        //var instanceCrater = Instantiate(targetPrefab, target, Quaternion.LookRotation(normal));
        targetPrefab = swordAoePrefab;
        var instanceAoe = Instantiate(targetPrefab, target, Quaternion.LookRotation(normal));

        yield return new WaitForSeconds(swordAoeDelay);


        targetPrefab = swordCraterULTPrefab;
        var instanceCULT = Instantiate(targetPrefab, target, Quaternion.LookRotation(normal));
        target.y = floorObject.transform.position.y + 5f; // ����
        targetPrefab = swordULTPrefab;
        var instanceULT = Instantiate(targetPrefab, target, Quaternion.LookRotation(normal));
    }

    #endregion

    #region Lightening Strike

    public void LighteningStrikeEffect(Vector3 target)
    {
        StartCoroutine(LightningStrike(target));
    }

    public void LigtningStrikeEffect2(Vector3 target)
    {
        StartCoroutine(LigtningStrikeAfter(target));
    }

    public float GetLightningStrikeTime()
    {
        return lightningSkillDelayTime;
    }

    IEnumerator LightningStrike(Vector3 target)
    {
        var targetPrefab = lightCirclePrefab;
        Vector3 normal = Vector3.up;
        target.z += lightningPosition_Z;
        var circleInstance = Instantiate(targetPrefab, target, Quaternion.LookRotation(normal));
        yield return new WaitForSeconds(lightCircleDelay);

        targetPrefab = lightningStrikePrefab;
        target.y += lightningPosition_Y;
        normal = Vector3.zero;
        var lightningInstance = Instantiate(targetPrefab, target, Quaternion.LookRotation(-normal));
        yield return null;
    }

    IEnumerator LigtningStrikeAfter(Vector3 target)
    {
        var targetPrefab = lightningStrike2Prefab;
        Vector3 normal = Vector3.zero;
        target.y += lightningPosition_Y;
        var lightningInstance = Instantiate(targetPrefab, target, Quaternion.LookRotation(-normal));
        yield return null;
    }
    #endregion

    #region Circle of Curse

    public void CircleOfCurseEffect(Vector3 target)
    {
        StartCoroutine(CircleOfCurse(target));
    }

    public float GetCircleOfCurseTime()
    {
        return curseTotalDelayTime;
    }

    IEnumerator CircleOfCurse(Vector3 target)
    {
        var objectPrefab = curseStonePrefab;
        Vector3 normal = Vector3.zero;
        Vector3 dropVector = new Vector3(0, stoneDropPosition_Y, 0);
        var stoneInstance = Instantiate(objectPrefab, target + dropVector, Quaternion.LookRotation(normal));
        yield return new WaitForSeconds(curseDropDelay);

        objectPrefab = curseCirclePrefab;
        Vector3 circleVector = new Vector3(0, 0.001f, 0);
        var effectInstance = Instantiate(objectPrefab, target + circleVector, Quaternion.LookRotation(normal));
        yield return new WaitForSeconds(curseExplosionDelay);

        target.y = floorObject.transform.position.y + explosionPosition_Y; // ����
        normal = Vector3.up;
        var targetPrefab = curseExplosionPrefab;
        var ringInstance = Instantiate(targetPrefab, target, Quaternion.LookRotation(normal));
    }
    #endregion

    #region Dark Mist

    public void DarkMistEffect(Vector3 target)
    {
        StartCoroutine(DarkMist(target));
    }

    public float GetDarkMistTime()
    {
        return mistTotalDelayTime;
    }

    IEnumerator DarkMist(Vector3 target)
    {
        Vector3 normalVector = Vector3.zero;
        var targetPrefab = mistExplosionPrefab;
        Vector3 explosionPos = new Vector3(0, 0, mistPosition_Z);
        var effectInstance = Instantiate(targetPrefab, target + explosionPos, Quaternion.LookRotation(normalVector));
        targetPrefab = mistEffectPrefab;
        var explosionInstance = Instantiate(targetPrefab, target, Quaternion.LookRotation(normalVector));
        yield return null;
    }

    #endregion

    #endregion
}
