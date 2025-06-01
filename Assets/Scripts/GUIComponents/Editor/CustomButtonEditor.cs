using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using CommonViewParts;

namespace CommonViewParts.Editor
{
    /// <summary>
    /// CustomButtonのエディタ拡張
    /// </summary>
    [CustomEditor(typeof(CustomButton))]
    public class CustomButtonEditor : UnityEditor.Editor
    {
        private SerializedProperty _raycastScaleProperty;

        private void OnEnable()
        {
            _raycastScaleProperty = serializedObject.FindProperty("_raycastScale");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var customButton = (CustomButton)target;

            // Validation確認
            ValidateComponents(customButton);

            EditorGUILayout.Space(5);
            
            // Raycast Scale設定
            EditorGUILayout.LabelField("Raycast Settings", EditorStyles.boldLabel);
            
            EditorGUI.BeginChangeCheck();
            
            // 現在の値を正しく取得
            var currentEnumValue = (RaycastScale)_raycastScaleProperty.enumValueFlag;
            
            var newRaycastScale = (RaycastScale)EditorGUILayout.EnumPopup(
                new GUIContent("Raycast Scale", "ボタンの当たり判定のスケールを設定します"),
                currentEnumValue
            );
            
            if (EditorGUI.EndChangeCheck())
            {
                _raycastScaleProperty.enumValueFlag = (int)newRaycastScale;
                
                // 実行時に即座に反映
                if (Application.isPlaying)
                {
                    customButton.SetRaycastScale(newRaycastScale);
                }
            }

            EditorGUILayout.Space(5);
            
            // 設定値の表示
            ShowCurrentSettings(customButton);
            
            EditorGUILayout.Space(5);
            
            // _raycastScaleを除外した他のフィールドを表示
            DrawPropertiesExcluding(serializedObject, "_raycastScale");

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 現在の設定値を表示
        /// </summary>
        private void ShowCurrentSettings(CustomButton customButton)
        {
            EditorGUILayout.LabelField("Current Settings", EditorStyles.boldLabel);
            
            using (new EditorGUI.DisabledScope(true))
            {
                var currentScale = (RaycastScale)_raycastScaleProperty.enumValueFlag;
                var scalePercentage = (int)currentScale;
                EditorGUILayout.LabelField("Scale Percentage", $"{scalePercentage}%");
                
                var scaleMultiplier = scalePercentage / 100f;
                EditorGUILayout.LabelField("Scale Multiplier", scaleMultiplier.ToString("F2"));
            }
        }

        /// <summary>
        /// 必要なコンポーネントのValidation
        /// </summary>
        private void ValidateComponents(CustomButton customButton)
        {
            var hasErrors = false;

            // Image コンポーネントの確認
            var image = customButton.GetComponent<Image>();
            if (image == null)
            {
                EditorGUILayout.HelpBox(
                    "Raycast Paddingを適用するためにImageコンポーネントが必要です。",
                    MessageType.Warning
                );
                hasErrors = true;

                if (GUILayout.Button("Imageコンポーネントを追加"))
                {
                    Undo.AddComponent<Image>(customButton.gameObject);
                }
            }

            // ObservableEventTrigger コンポーネントの確認（RequireComponentで自動追加されるが念のため）
            var eventTrigger = customButton.GetComponent<UniRx.Triggers.ObservableEventTrigger>();
            if (eventTrigger == null)
            {
                EditorGUILayout.HelpBox(
                    "ObservableEventTriggerコンポーネントが見つかりません。RequireComponentで自動追加されるはずです。",
                    MessageType.Error
                );
                hasErrors = true;
            }

            // RectTransformの確認
            var rectTransform = customButton.transform as RectTransform;
            if (rectTransform == null)
            {
                EditorGUILayout.HelpBox(
                    "CustomButtonはUI要素として使用する必要があります。RectTransformが必要です。",
                    MessageType.Error
                );
                hasErrors = true;
            }
            else
            {
                // サイズが0の場合の警告
                if (rectTransform.sizeDelta.x <= 0 || rectTransform.sizeDelta.y <= 0)
                {
                    EditorGUILayout.HelpBox(
                        "RectTransformのサイズが設定されていません。Raycast Paddingが正しく計算されない可能性があります。",
                        MessageType.Warning
                    );
                }
            }

            if (!hasErrors)
            {
                EditorGUILayout.HelpBox("全ての必要なコンポーネントが正しく設定されています。", MessageType.Info);
            }
        }
    }
} 