using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MageSimulator.BrowserUI
{
    public abstract class BrowserComponent : MonoBehaviour
    {
        private bool _initializeComponentFlag = false;
        private bool _closePageFlag = false;
        public Browser Browser { get; private set; }

        /// <summary>
        ///     コンポーネントが初期化されるときに実行される
        ///     <para>親オブジェクトの OnInitializeComponent() よりも後に呼ばれることが保証される</para>
        /// </summary>
        protected virtual UniTask OnInitializeComponent(CancellationToken token = new CancellationToken())
        {
            return UniTask.CompletedTask;
        }

        /// <summary>
        ///     ブラウザでページが閉じられるシグナルを受け取ったときに実行される
        ///     <para>子オブジェクトの OnClosePage() よりも後に呼ばれることが保証される</para>
        /// </summary>
        /// <param name="source">シグナルを発行したコンポーネント</param>
        protected virtual UniTask OnClosePage(Link source, CancellationToken token = new CancellationToken())
        {
            return UniTask.CompletedTask;
        }

        public async UniTask InitializeComponent(Browser browser, CancellationToken token)
        {
            if (_initializeComponentFlag) return;
            _initializeComponentFlag = true;
            Browser = browser;

            await OnInitializeComponent(token);
            if (token.IsCancellationRequested) return;

            var childrenComponents = SearchChildrenComponent(transform);
            var initializerTasks = childrenComponents.Select(x => x.InitializeComponent(browser, token));
            await UniTask.WhenAll(initializerTasks);
        }

        public static IEnumerable<BrowserComponent> SearchChildrenComponent(Transform current)
        {
            return Enumerable.Range(0, current.childCount).Select(current.GetChild).SelectMany(child =>
            {
                var component = child.GetComponent<BrowserComponent>();
                return component != null ? new[] {component} : SearchChildrenComponent(child);
            });
        }

        protected async UniTask RequestClosePage(Link source, Action<Browser> onClose, CancellationToken token)
        {
            if (_closePageFlag) return;
            _closePageFlag = true;

            await OnClosePage(source, token);
            if (token.IsCancellationRequested) return;

            var parent = transform.parent;
            while (true)
            {
                var browser = parent.GetComponent<Browser>();
                if (browser != null)
                {
                    await browser.ClosePage();
                    onClose(browser);
                    return;
                }

                var components = parent.GetComponents<BrowserComponent>();
                if (components != null && components.Length > 0)
                {
                    foreach (var comp in components)
                        comp.RequestClosePage(source, onClose, token).Forget();
                    return;
                }

                parent = parent.parent;
                if (parent == null) return;
            }
        }
    }
}