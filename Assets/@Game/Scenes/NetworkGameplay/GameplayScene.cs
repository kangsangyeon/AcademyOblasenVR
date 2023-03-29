using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class GameplayScene : MonoBehaviour
{
    [SerializeField] private AudioClip m_Sfx_Ready;
    [SerializeField] private AudioClip m_Sfx_Fight;
    [SerializeField] private AudioClip m_Sfx_Turn;
    [SerializeField] private AudioClip m_Sfx_Ele_Select;
    [SerializeField] private AudioClip m_Sfx_Ele_Cancel;
    [SerializeField] private AudioClip m_Sfx_Skill_Select;
    [SerializeField] private AudioClip m_Sfx_Draw_Popup;
    [SerializeField] private AudioClip m_Sfx_Draw_ing;
    [SerializeField] private AudioClip m_Sfx_Draw_Fail;
    [SerializeField] private AudioClip m_Sfx_Draw_Success;
    [SerializeField] private AudioClip m_Sfx_GameSet;
    [SerializeField] private AudioClip m_Sfx_Victory;

    [SerializeField] private InputActionReference m_CancelElementSelectionActionRef;

    [SerializeField] private GameplayTurn m_Gameplay;
    [SerializeField] private SkillInfoFinder m_SkillInfoFinder;
    [SerializeField] private PlayerHPMPUI m_PlayerHPMPUI;

    [SerializeField] private Transform m_UIHolder;
    [SerializeField] private Transform m_2PHPMPUI;
    [SerializeField] private Transform m_ImagesHolder;
    [SerializeField] private Transform m_ElementUIHolder;

    [SerializeField] private Image m_Image_Ready;
    [SerializeField] private Image m_Image_Fight;
    [SerializeField] private Image m_Image_MyTurn;
    [SerializeField] private Image m_Image_EnemyTurn;


    [SerializeField] private Transform m_PatternRecognitionTimer;
    [SerializeField] private Image m_Image_PatternRecognitionPhaseLeftoverTimeSlider;

    [SerializeField] private Transform m_SpellUIHolder;
    [SerializeField] private TextMeshProUGUI m_Text_Spell;
    [SerializeField] private TextMeshProUGUI m_Text_CurrentLevel;
    [SerializeField] private TextMeshProUGUI m_Text_HighestLevel;
    [SerializeField] private Image m_Image_SpellPhaseLeftoverTimeSlider;

    [SerializeField] private GameObject m_TotalDamageUI;
    [SerializeField] private TMP_Text m_Damage_Text;
    [SerializeField] private TMP_Text m_Total_Damage_Text;

    [SerializeField] private Transform m_Player1Point;
    [SerializeField] private Transform m_Player1ForwardPoint;
    [SerializeField] private Transform m_Player1UIPoint;
    [SerializeField] private Transform m_Player2Point;
    [SerializeField] private Transform m_Player2ForwardPoint;
    [SerializeField] private Transform m_Player2UIPoint;

    [SerializeField] private float m_NodeShakeDuration = 0.5f;
    [SerializeField] private float m_NodeShakeStrength = 2.0f;
    [SerializeField] private float m_HapticAmplitude = 0.7f;
    [SerializeField] private float m_HapticDuration = 0.5f;

    private NetworkCharacter m_MyCharacter;

    public GameplayTurn GetGameplayTurn() => m_Gameplay;
    public SkillInfoFinder GetSkillInfoFinder() => m_SkillInfoFinder;
    public PlayerHPMPUI GetPlayerHPMPUI() => m_PlayerHPMPUI;

    public Image GetReadyImage() => m_Image_Ready;
    public Image GetFightImage() => m_Image_Fight;
    public Image GetMyTurnImage() => m_Image_MyTurn;
    public Image GetEnemyTurnImage() => m_Image_EnemyTurn;
    public Transform GetUIHolder() => m_UIHolder;
    public GameObject GetTotalDamageUI() => m_TotalDamageUI;
    public TMP_Text GetDamageText() => m_Damage_Text;
    public TMP_Text GetTotalDamageText() => m_Total_Damage_Text;

    public Transform GetPlayerPoint(int index) => index == 0 ? m_Player1Point : m_Player2Point;
    public Transform GetPlayerForwardPoint(int index) => index == 0 ? m_Player1ForwardPoint : m_Player2ForwardPoint;
    public Transform GetPlayerUIPoint(int index) => index == 0 ? m_Player1UIPoint : m_Player2UIPoint;

    public NetworkCharacter GetMyCharacter() => m_MyCharacter;

    public void SetMyCharacter(NetworkCharacter _character)
    {
        if (_character == m_MyCharacter)
            return;

        if (m_MyCharacter != null)
        {
            m_MyCharacter.OnReady.RemoveListener(OnReady);
            m_MyCharacter.OnFight.RemoveListener(OnFight);
            m_MyCharacter.OnStartTurn.RemoveListener(OnStartTurn);
            m_MyCharacter.OnGameSet.RemoveListener(OnGameSet);

            m_MyCharacter.OnChangedLockedElement.RemoveListener(OnChangedLockedElement);

            m_PlayerHPMPUI.SetCharacterPhotonView(null);
        }

        m_MyCharacter = _character;

        if (m_MyCharacter != null)
        {
            m_MyCharacter.OnReady.AddListener(OnReady);
            m_MyCharacter.OnFight.AddListener(OnFight);
            m_MyCharacter.OnStartTurn.AddListener(OnStartTurn);
            m_MyCharacter.OnGameSet.AddListener(OnGameSet);

            m_MyCharacter.OnChangedLockedElement.AddListener(OnChangedLockedElement);

            m_PlayerHPMPUI.SetCharacterPhotonView(m_MyCharacter.GetPhotonView());
        }
    }

    private void Start()
    {
        m_2PHPMPUI.gameObject.SetActive(true);
        m_ImagesHolder.gameObject.SetActive(true);
        m_ElementUIHolder.gameObject.SetActive(true);
        
        m_Image_Ready.gameObject.SetActive(false);
        m_Image_Fight.gameObject.SetActive(false);
        m_Image_MyTurn.gameObject.SetActive(false);
        m_Image_EnemyTurn.gameObject.SetActive(false);

        m_PatternRecognitionTimer.gameObject.SetActive(false);

        m_SpellUIHolder.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        m_Gameplay.GetSelectSkillPhase().OnSelectElementType.AddListener(OnSelectElementType);
        m_Gameplay.GetSelectSkillPhase().OnSelectSkillType.AddListener(OnSelectSkillType);
        m_Gameplay.GetSelectSkillPhase().OnCancelElementSelection.AddListener(OnCancelElementSelection);
        m_Gameplay.GetSelectSkillPhase().OnEndPhase.AddListener(OnEndSelectSkillPhase);

        m_Gameplay.GetPatternRecognitionPhase().OnStartTryDrawStep.AddListener(OnStartTryDrawStep);
        m_Gameplay.GetPatternRecognitionPhase().OnPopupNode.AddListener(OnPopupNode);
        m_Gameplay.GetPatternRecognitionPhase().OnStartDrawing.AddListener(OnStartDrawing);
        m_Gameplay.GetPatternRecognitionPhase().OnEndDrawing.AddListener(OnEndDrawing);
        m_Gameplay.GetPatternRecognitionPhase().OnUserFault.AddListener(OnUserFault);
        m_Gameplay.GetPatternRecognitionPhase().OnUserSuccess.AddListener(OnUserSuccess);
        m_Gameplay.GetPatternRecognitionPhase().OnUserProgressChanged.AddListener(OnUserProgressChanged);

        m_Gameplay.GetSpellPhase().OnStartPhase.AddListener(OnStartSpellPhase);
    }

    private void OnDisable()
    {
        m_Gameplay.GetSelectSkillPhase().OnSelectElementType.RemoveListener(OnSelectElementType);
        m_Gameplay.GetSelectSkillPhase().OnSelectSkillType.RemoveListener(OnSelectSkillType);
        m_Gameplay.GetSelectSkillPhase().OnCancelElementSelection.RemoveListener(OnCancelElementSelection);
        m_Gameplay.GetSelectSkillPhase().OnEndPhase.RemoveListener(OnEndSelectSkillPhase);

        m_Gameplay.GetPatternRecognitionPhase().OnPopupNode.RemoveListener(OnPopupNode);
        m_Gameplay.GetPatternRecognitionPhase().OnStartDrawing.RemoveListener(OnStartDrawing);
        m_Gameplay.GetPatternRecognitionPhase().OnEndDrawing.RemoveListener(OnEndDrawing);
        m_Gameplay.GetPatternRecognitionPhase().OnUserFault.RemoveListener(OnUserFault);
        m_Gameplay.GetPatternRecognitionPhase().OnUserSuccess.RemoveListener(OnUserSuccess);
        m_Gameplay.GetPatternRecognitionPhase().OnUserProgressChanged.RemoveListener(OnUserProgressChanged);

        m_Gameplay.GetSpellPhase().OnStartPhase.RemoveListener(OnStartSpellPhase);

        SetMyCharacter(null);
    }

    private void OnReady()
    {
        SoundManager.instance.SFXPlay("Sfx_Ready", m_Sfx_Ready);
    }

    private void OnFight()
    {
        SoundManager.instance.SFXPlay("Sfx_Fight", m_Sfx_Fight);
    }

    private void OnStartTurn()
    {
        SoundManager.instance.SFXPlay("Sfx_Turn", m_Sfx_Turn);
    }

    private void OnSelectElementType(EElementType _type)
    {
        SoundManager.instance.SFXPlay("Sfx_Ele_Select", m_Sfx_Ele_Select);
        GetGameplayTurn().GetSelectSkillPhase().SetCurrentHPMP(m_MyCharacter.GetHP(), m_MyCharacter.GetMP());

        m_CancelElementSelectionActionRef.action.performed += BackToElementSelection;
    }

    private void OnSelectSkillType(ESkillType _type)
    {
        SoundManager.instance.SFXPlay("Sfx_Skill_Select", m_Sfx_Skill_Select);
    }

    private void OnCancelElementSelection()
    {
        SoundManager.instance.SFXPlay("Sfx_Ele_Cancel", m_Sfx_Ele_Cancel);

        m_CancelElementSelectionActionRef.action.performed -= BackToElementSelection;
    }

    private void OnEndSelectSkillPhase()
    {
        m_CancelElementSelectionActionRef.action.performed -= BackToElementSelection;
    }

    private void BackToElementSelection(InputAction.CallbackContext _context)
    {
        m_Gameplay.GetSelectSkillPhase().OnClickBack();
    }

    private void OnStartTryDrawStep()
    {
        StartCoroutine(UpdatePatternRecognitionPhaseUICoroutine());
    }
    
    private IEnumerator UpdatePatternRecognitionPhaseUICoroutine()
    {
        m_Image_PatternRecognitionPhaseLeftoverTimeSlider.fillAmount = 1.0f;
        m_PatternRecognitionTimer.gameObject.SetActive(true);

        // spell phase가 끝날 때 자동으로 코루틴을 종료시키도록,
        // bool변수의 변경을 리스너로 등록합니다.
        bool _bEndPhase = false;
        UnityAction _callback = () => _bEndPhase = true;
        m_Gameplay.GetPatternRecognitionPhase().OnEndPhase.AddListener(_callback);

        while (true)
        {
            if (_bEndPhase)
                break;

            m_Image_PatternRecognitionPhaseLeftoverTimeSlider.fillAmount =
                m_Gameplay.GetPatternRecognitionPhase().GetPhaseLeftoverTime() / m_Gameplay.GetPatternRecognitionPhase().GetTryDrawDuration();

            yield return null;
        }

        m_Gameplay.GetSpellPhase().OnEndPhase.RemoveListener(_callback);

        m_PatternRecognitionTimer.gameObject.SetActive(false);
    }
    
    private void OnPopupNode(Node _node)
    {
        SoundManager.instance.SFXPlay("Sfx_Draw_Popup", m_Sfx_Draw_Popup);
    }

    private void OnStartDrawing()
    {
        // TODO: 반복되는 효과음을 재생해야 합니다.
        SoundManager.instance.SFXPlay("Sfx_Draw_ing", m_Sfx_Draw_ing);
    }

    private void OnEndDrawing()
    {
        // TODO: 반복되는 효과음 재생을 종료합니다.
    }

    private void OnGameSet()
    {
        SoundManager.instance.SFXPlay("Sfx_GameSet", m_Sfx_GameSet);
    }

    private void OnUserProgressChanged(Node _node, float _p)
    {
        _node.PlayUserProgressChangedSfx(_p);
    }

    private void OnUserFault(Node _node, ActionBasedController _controller)
    {
        SoundManager.instance.SFXPlay("Sfx_Draw_Fail", m_Sfx_Draw_Fail);

        _node.GetComponent<ShakeTransform>()?.DoShake(m_NodeShakeDuration, m_NodeShakeStrength);
        _controller.SendHapticImpulse(m_HapticAmplitude, m_HapticDuration);
    }

    private void OnUserSuccess()
    {
        SoundManager.instance.SFXPlay("Sfx_Draw_Success", m_Sfx_Draw_Success);
    }

    private void OnChangedLockedElement(int _index)
    {
        for (int i = 0; i < 4; ++i)
            m_Gameplay.GetSelectSkillPhase().GetElement(i).gameObject.SetActive(true);

        if (_index == 0)
            return;

        m_Gameplay.GetSelectSkillPhase().GetElement(_index - 1).gameObject.SetActive(false);
    }

    private void OnStartSpellPhase()
    {
        StartCoroutine(UpdateSkillPhaseUICoroutine());
    }

    private IEnumerator UpdateSkillPhaseUICoroutine()
    {
        var _skill = m_Gameplay.GetSelectSkillPhase().GetSelectedSkillInfo();
        
        m_Text_Spell.text =_skill.GetRandomSkillSpell();
        if (_skill.SpellTextColorGradient != null)
        {
            m_Text_Spell.enableVertexGradient = true;
            m_Text_Spell.colorGradientPreset = _skill.SpellTextColorGradient;
        }
        
        m_Image_SpellPhaseLeftoverTimeSlider.fillAmount = 1.0f;
        m_SpellUIHolder.gameObject.SetActive(true);

        // spell phase가 끝날 때 자동으로 코루틴을 종료시키도록,
        // bool변수의 변경을 리스너로 등록합니다.
        bool _bEndPhase = false;
        UnityAction _callback = () => _bEndPhase = true;
        m_Gameplay.GetSpellPhase().OnEndPhase.AddListener(_callback);

        while (true)
        {
            if (_bEndPhase)
                break;

            m_Text_CurrentLevel.text = m_Gameplay.GetSpellPhase().GetCurrentLevel().ToString();
            m_Text_HighestLevel.text = m_Gameplay.GetSpellPhase().GetHighestLevel().ToString();
            m_Image_SpellPhaseLeftoverTimeSlider.fillAmount = m_Gameplay.GetSpellPhase().GetPhaseLeftoverTime() / m_Gameplay.GetSpellPhase().GetDuration();

            yield return null;
        }

        m_Gameplay.GetSpellPhase().OnEndPhase.RemoveListener(_callback);

        m_SpellUIHolder.gameObject.SetActive(false);
    }
}