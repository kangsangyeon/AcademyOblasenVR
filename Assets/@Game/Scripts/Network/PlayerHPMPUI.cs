using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;
using TMPro;

public class PlayerHPMPUI : MonoBehaviour, IOnEventCallback
{

    #region public Variables

    public int MaxHP = 150;
    public int MaxMP = 100;

    #endregion

    #region private Variables
    [Header("Sprite")]
    [SerializeField] private Sprite MaleSprite;
    [SerializeField] private Sprite GirlSprite;
    [Space]

    [Header("Enemy UI")]
    [SerializeField] Image MyHPImage;
    [SerializeField] TMP_Text MyHPText;

    [SerializeField] Image MyMPImage;
    [SerializeField] TMP_Text MyMPText;

    [SerializeField] private Image MyPortrait;
    [SerializeField] Image MyElectric;
    [SerializeField] Image MyIgnore;
    [SerializeField] Image MyAtkDeburf;
    [SerializeField] Image MyDefDeburf;
    [Space]

    [Header("Enemy UI")]
    [SerializeField] Image EnemyHPImage;
    [SerializeField] TMP_Text EnemyHPText;

    [SerializeField] Image EnemyMPImage;
    [SerializeField] TMP_Text EnemyMPText;

    [SerializeField] private Image EnemyPortrait;
    [SerializeField] Image EnemyElectric;
    [SerializeField] Image EnemyIgnore;
    [SerializeField] Image EnemyAtkDeburf;
    [SerializeField] Image EnemyDefDeburf;
    [Space]


    private PhotonView photonView;

    private int My_nowHP;
    private int My_nowMP;
    private int Enemy_nowHP;
    private int Enemy_nowMP;

    private int isElectricCount;
    private bool isIgnore;
    private bool isAtkDeburf;
    private bool isDefDeburf;

    #endregion

    #region Unity Callback

    // Start is called before the first frame update
    void Start()
    {
        My_nowHP = MaxHP;
        My_nowMP = MaxMP;
        Enemy_nowHP = MaxHP;
        Enemy_nowMP = MaxMP;
    }

    #endregion

    #region Getrer

    public int GetMy_nowHP()
    {
        return My_nowHP;
    }

    public int GetMy_nowMP()
    {
        return My_nowMP;
    }

    public int GetEnemy_nowHP()
    {
        return Enemy_nowHP;
    }

    public int GetEnemy_nowMP()
    {
        return Enemy_nowMP;
    }

    #endregion

    public void SetCharacterPhotonView(PhotonView _photonView) => photonView = _photonView;

    public void SetIsGirl(bool _bIsGirl)
    {
        Sprite _me, _other;

        if (_bIsGirl)
        {
            _me = GirlSprite;
            _other = MaleSprite;
        }
        else
        {
            _me = MaleSprite;
            _other = GirlSprite;
        }

        MyPortrait.sprite = _me;
        EnemyPortrait.sprite = _other;
    }

    #region Network OnEvent

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        object[] data = (object[])photonEvent.CustomData;

        if (eventCode == (byte)NetworkCode.C2S_PLAYER_STATE)
        {
            int SenderviewID = (int)data[0];
            int SenderHP = (int)data[1];
            int SenderMP = (int)data[2];
            isElectricCount = (int)data[3];
            isIgnore = (bool)data[4];
            isAtkDeburf = (bool)data[5];
            isDefDeburf = (bool)data[6];


            if (photonView.ViewID == SenderviewID)
            {
                // 송신자가 내 뷰아이디랑 같다 == 나의 체력 정보이다.
                My_nowHP = SenderHP;
                My_nowMP = SenderMP;
                SetDeburfUI(true);
            }
            else
            {
                Enemy_nowHP = SenderHP;
                Enemy_nowMP = SenderMP;
                SetDeburfUI(false);
            }

            SetUIGauge();
        }

    }


    #endregion

    #region private Methods

    private void SetUIGauge()
    {
        MyHPImage.fillAmount = (float)My_nowHP / MaxHP;
        MyMPImage.fillAmount = (float)My_nowMP / MaxMP;
        MyHPText.text = Convert.ToString(My_nowHP);
        MyMPText.text = Convert.ToString(My_nowMP);

        EnemyHPImage.fillAmount = (float)Enemy_nowHP / MaxHP;
        EnemyMPImage.fillAmount = (float)Enemy_nowMP / MaxMP;
        EnemyHPText.text = Convert.ToString(Enemy_nowHP);
        EnemyMPText.text = Convert.ToString(Enemy_nowMP);
    }

    private void SetDeburfUI(bool _mine)
    {
        if (_mine)
        {
            if (isElectricCount > 0)
            {
                MyElectric.color = Color.white;
            }
            else
            {
                MyElectric.color = Color.black;
            }

            if (isIgnore)
            {
                MyIgnore.color = Color.white;
            }
            else
            {
                MyIgnore.color = Color.black;
            }

            if (isAtkDeburf)
            {
                MyAtkDeburf.color = Color.white;
            }
            else
            {
                MyAtkDeburf.color = Color.black;
            }

            if (isDefDeburf)
            {
                MyDefDeburf.color = Color.white;
            }
            else
            {
                MyDefDeburf.color = Color.black;
            }
        }

        else
        {
            if (isElectricCount > 0)
            {
                EnemyElectric.color = Color.white;
            }
            else
            {
                EnemyElectric.color = Color.black;
            }

            if (isIgnore)
            {
                EnemyIgnore.color = Color.white;
            }
            else
            {
                EnemyIgnore.color = Color.black;
            }

            if (isAtkDeburf)
            {
                EnemyAtkDeburf.color = Color.white;
            }
            else
            {
                EnemyAtkDeburf.color = Color.black;
            }

            if (isDefDeburf)
            {
                EnemyDefDeburf.color = Color.white;
            }
            else
            {
                EnemyDefDeburf.color = Color.black;
            }
        }
    }

    #endregion
}
