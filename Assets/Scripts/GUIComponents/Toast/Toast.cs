using System;
using Cysharp.Threading.Tasks;
using Demo.Subsystem.GUIComponents;
using UnityEngine;

namespace Demo.Subsystem
{

    /// <summary>
    /// 適当なView（あるいはデバッグならPresenter）からToastっぽく数秒で消えるメッセージを表示する
    /// </summary>
    public class Toast : MonoBehaviour
    {
        static readonly ChannelExecutor _executor = new ChannelExecutor();
        [SerializeField] private RectTransform toastParent = default;

        static Transform _ParentTransform;
        void Awake()
        {
            if (toastParent == null)
            {
                Debug.LogError("toastParent is null please set it in the inspector");
                return;
            }
            _ParentTransform = toastParent;
            destroyCancellationToken.Register(() => _executor.Dispose());
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                QueueToast(DateTime.Now.ToString("HH:mm:ss"));

            }
        }

        /// <summary>
        /// 外部から気軽に叩くためにstaticで定義
        /// </summary>
        /// <param name="text"></param>
        /// <param name="seconds"></param>
        public static void QueueToast(string text, float seconds=3.0f)
        {
            _executor.Register(async ct =>
            {
                //note: 適切な名前のprefabをResources以下に配置しておいてください
                var prefab = Resources.Load<GameObject>("ToastElement");
                var go = Instantiate (prefab, _ParentTransform.position, Quaternion.identity,_ParentTransform);
                go.GetComponent<ToastElement>().Show(text, seconds);
                await UniTask.Delay(TimeSpan.FromSeconds(seconds), cancellationToken: ct);
            });
        }
    }
}