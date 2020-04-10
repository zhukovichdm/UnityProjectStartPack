using System.Collections.Generic;
using Pixeye.Unity;
using Scripts.Behaviours.Other;
using Scripts.Component;
using Scripts.Component.Actions;
using Scripts.Data;
using Scripts.Modules.CameraPlayer;
using TMPro;
using UnityEditor.UIElements;
using UnityEngine;

namespace Scripts.Behaviours.Ui
{
    /// <summary>
    /// Обновление панели в соответствии с выбранным объектом.
    /// </summary>
    public class ControlObjectInformationPanel : MonoBehaviour
    {
        [Foldout("Для вывода в интерфейс", true)] [SerializeField]
        private ControlAnimator infoUnderCursorPanel;

        [SerializeField] private ControlAnimator shortDescriptionPanel;
        [SerializeField] private TMP_Text nameObjectUnderTheCursorText;
        [SerializeField] private TMP_Text nameObjectSelectedText;
        [SerializeField] private TMP_Text shortDescriptionText;
        [SerializeField] private ControlAnimator selectedGroup;

        [Foldout("Для вывода в панель информации", true)] [SerializeField]
        private TMP_Text headerText;

        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private GameObject disassembleButton;

        [Foldout("Для управления переключением камер", true)] [SerializeField]
        private ControlAnimator objectInformationPanel_ControlAnimator;

        [SerializeField] private GameObject viewedObjectCamera;
        [SerializeField] private GameObject playerCamera;
        [SerializeField] private ControlCamera playerControlCamera;
        [SerializeField] private int layerMask;

        private (bool, GameObject, DataObjectInformation) _arguments;
        private ControlAnimator _controlAnimator;
        private bool _flag;
        private int? _oldMask;

        private void Awake()
        {
            GameActions.UpdateObjectInformation.Subscribe(UpdateObjectInformation_Subscriber);
            GameActions.CursorOverObject.Subscribe(CursorOverObject_Subscriber);
            UserInput.InputEscapeAction.Subscribe(HideElements);
            viewedObjectCamera.SetActive(false);
        }

        private void OnDestroy()
        {
            GameActions.UpdateObjectInformation.Unsubscribe(UpdateObjectInformation_Subscriber);
            GameActions.CursorOverObject.Unsubscribe(CursorOverObject_Subscriber);
            UserInput.InputEscapeAction.Unsubscribe(HideElements);
        }

        // Обновление информации об объекте при его выборе.
        private void UpdateObjectInformation_Subscriber((bool, GameObject, DataObjectInformation) args)
        {
            _arguments = args;
            selectedGroup.SetSignAll(_arguments.Item1);
            _controlAnimator = args.Item2.GetComponent<ControlAnimator>();
            headerText.text = args.Item3.name;
            descriptionText.text = args.Item3.description;

            disassembleButton.SetActive(_controlAnimator);
        }

        // Обновление информации об объекте при наведении на него курсора.
        private void CursorOverObject_Subscriber((bool show, ControlObjectInformation data) valueTuple)
        {
            infoUnderCursorPanel.SetSignAll(!Testing.TestingMode && valueTuple.show);
            shortDescriptionPanel.SetSignAll(!Testing.TestingMode &&
                                          (valueTuple.data.dataObjectInformation.shortDescription != "" &&
                                           valueTuple.show));

            nameObjectUnderTheCursorText.text = valueTuple.data.dataObjectInformation.name;
            shortDescriptionText.text = valueTuple.data.dataObjectInformation.shortDescription;
        }

        public void AnimationButton() => _controlAnimator?.ReverseAll();

        private void HideElements()
        {
            if (playerControlCamera.enabled) // Только если управляем персонажем.
                selectedGroup.SetSignAll(false);
        }

        public void MakeSwitch()
        {
            if (!_arguments.Item1) return;

            _flag = !_flag;
            objectInformationPanel_ControlAnimator.SetSignAll(_flag);
            viewedObjectCamera.SetActive(_flag);
            playerCamera.SetActive(!_flag);
            playerControlCamera.enabled = !_flag;

            if (_flag)
            {
                _oldMask = _arguments.Item2.layer;
                _arguments.Item2.layer = layerMask;
            }
            else
            {
                if (_oldMask != null)
                    _arguments.Item2.layer = _oldMask.Value;
            }
        }
    }
}