using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using GameModules;

namespace GameModules
{
    public class UIModule
    {
        public static int Width = 1080;
        public static int Height = 1920;
        public static UIFitterType UIFitterType = UIFitterType.Width;

        private Transform _root;
        private Camera _worldCamera;
        private Camera _uiCamera;
        
        private CanvasGroup _blackMask;
        private CanvasGroup _backgroundMask;
        private Tweener _fadeTweener;

        private Dictionary<UIType, UIViewController> _viewControllers;
        private Dictionary<UILayer, UILayerLogic> _layers;
        private HashSet<UIType> _openViews;
        private HashSet<UIType> _residentViews;

        public EventSystem EventSystem { get; private set; }

        public void Init()
        {
            this._layers = new Dictionary<UILayer, UILayerLogic>();
            _viewControllers = new Dictionary<UIType, UIViewController>();
            _openViews = new HashSet<UIType>();
            _residentViews = new HashSet<UIType>();

            _worldCamera = Camera.main;
            _worldCamera.cullingMask &= int.MaxValue ^ (1 << Layer.UI);

            var root = GameObject.Find("UIRoot");
            if (root == null)
            {
                root = new GameObject("UIRoot");
            }
            root.layer = Layer.UI;
            GameObject.DontDestroyOnLoad(root);
            this._root = root.transform;

            var camera = GameObject.Find("UICamera");
            if (camera == null)
            {
                camera = new GameObject("UICamera");
            }
            _uiCamera = camera.GetOrAddComponent<Camera>();
            _uiCamera.cullingMask = 1 << Layer.UI;
            _uiCamera.transform.SetParent(this._root);
            _uiCamera.orthographic = true;
            _uiCamera.clearFlags = CameraClearFlags.Depth;

            EventSystem = EventSystem.current;

            var layers = Enum.GetValues(typeof(UILayer));
            foreach (UILayer layer in layers)
            {
                bool is3d = layer == UILayer.SceneLayer;
                Canvas layerCanvas = UIExtension.CreateLayerCanvas(layer, is3d, this._root, is3d ? _worldCamera : _uiCamera, Width, Height);
                UILayerLogic uILayerLogic = new UILayerLogic(layer, layerCanvas);
                this._layers.Add(layer, uILayerLogic);
            }
            _blackMask = UIExtension.CreateBlackMask(this._layers[UILayer.BlackMaskLayer].canvas.transform);
            _backgroundMask = UIExtension.CreateBlackMask(this._layers[UILayer.BackgroundLayer].canvas.transform);
            
            InitUIConfig();
        }

        public Rect GetSafeArea()
        {
            Rect rect = Screen.safeArea;
            if (UIFitterType == UIFitterType.Width)
            {
                var parent = _layers[UILayer.BackgroundLayer].canvas.transform as RectTransform;
                float blackArea = Mathf.Abs(Height - parent.rect.height) / 2;
                rect.yMin = Mathf.Max(0, rect.yMin - blackArea);
                rect.yMax = Mathf.Min(rect.yMax + blackArea, Screen.height);
            }
            else if (UIFitterType == UIFitterType.Height)
            {
                var parent = _layers[UILayer.BackgroundLayer].canvas.transform as RectTransform;
                float blackArea = Mathf.Abs(Width - parent.rect.width) / 2;
                rect.xMin = Mathf.Max(0, rect.xMin - blackArea);
                rect.xMax = Mathf.Min(rect.xMax + blackArea, Screen.width);
            }
            return rect;
        }

        public void EnableBackgroundMask(bool enable)
        {
            _backgroundMask.alpha = enable ? 1 : 0;
        }

        public void InitUIConfig()
        {
            foreach (var cfg in UIConfig.ConfigList)
            {
                if (_viewControllers.ContainsKey(cfg.UIType))
                {
                    Debug.LogErrorFormat("存在相同的uiType:{0}， 请检查UIConfig是否重复！", cfg.UIType.ToString());
                    continue;
                }

                _viewControllers.Add(cfg.UIType, new UIViewController
                {
                    uiPath = cfg.Path,
                    uiType = cfg.UIType,
                    uiLayer = _layers[cfg.UILayer],
                    isWindow = cfg.IsWindow,
                });
            }
        }

        /// <summary>
        /// 注册常驻UI
        /// </summary>
        public void AddResidentUI(UIType type)
        {
            _residentViews.Add(type);
        }

        public void Open(UIType type, object userData = null, Action callback = null)
        {
            if (!_viewControllers.ContainsKey(type))
            {
                Debug.LogErrorFormat("未配置uiType:{0}， 请检查UIConfig.cs！", type.ToString());
                return;
            }

            _openViews.Add(type);
            _viewControllers[type].Open(userData, callback);
        }

        /*
        public AsyncOperationHandle Preload(UIType type)
        {
            if (!viewControllers.TryGetValue(type, out var controller))
            {
                Debug.LogErrorFormat("未配置uiType:{0}， 请检查UIConfig.cs！", type.ToString());
                return default;
            }
            return controller.Load();
        }

        public void PreloadAll()
        {
            foreach (var controller in viewControllers.Values)
            {
                ResourceManager.Instance.LoadAssetAsync<GameObject>(controller.uiPath, null);
            }
        }*/

        public bool IsOpen(UIType type)
        {
            return _openViews.Contains(type);
        }

        public void Close(UIType type, Action callback = null)
        {
            if (!_viewControllers.ContainsKey(type))
            {
                Debug.LogErrorFormat("未配置uiType:{0}， 请检查UIConfig.cs！", type.ToString());
                return;
            }

            _openViews.Remove(type);
            _viewControllers[type].Close(callback);
        }

        public T GetView<T>(UIType type) where T : UIViewBase
        {
            if (!_viewControllers.ContainsKey(type))
            {
                Debug.LogErrorFormat("未配置uiType:{0}， 请检查UIConfig.cs！", type.ToString());
                return null;
            }

            return _viewControllers[type].uiView as T;
        }

        public void CloseAll(UIType ignoreType = UIType.Max, bool closeResidentView = false)
        {
            var list = ListPool<UIType>.Get();

            foreach (var uiType in _openViews)
            {
                if (ignoreType == uiType) continue;

                if (closeResidentView || !_residentViews.Contains(uiType))
                {
                    _viewControllers[uiType].Close();
                    list.Add(uiType);
                }
            }
            foreach (var uiType in list)
            {
                _openViews.Remove(uiType);
            }
            ListPool<UIType>.Release(list);
        }

        public void ReleaseAll()
        {
            foreach (var controller in _viewControllers.Values)
            {
                if (!_residentViews.Contains(controller.uiType))
                {
                    _openViews.Remove(controller.uiType);
                    controller.Release();
                }
            }
        }

        public void FadeIn(float duration = 0.5f, TweenCallback callback = null)
        {
            if (_fadeTweener != null && _fadeTweener.IsPlaying())
                _fadeTweener.Complete();
            _fadeTweener = _blackMask.DOFade(1.0f, duration);
            _fadeTweener.onComplete = callback;
        }

        public void FadeOut(float duration = 0.5f, TweenCallback callback = null)
        {
            if (_fadeTweener != null && _fadeTweener.IsPlaying())
                _fadeTweener.Complete();
            _fadeTweener = _blackMask.DOFade(0.0f, duration);
            _fadeTweener.onComplete = callback;
        }

        public void FadeInOut(float duration = 1.0f, TweenCallback callback = null)
        {
            if (_fadeTweener != null && _fadeTweener.IsPlaying())
                _fadeTweener.Complete();
            _fadeTweener = _blackMask.DOFade(1.0f, duration * 0.5f);
            _fadeTweener.onComplete += () =>
            {
                _fadeTweener = _blackMask.DOFade(0.0f, duration * 0.5f);
                _fadeTweener.onComplete = callback;
            };
        }

        public void Cancel()
        {
            if (_layers.TryGetValue(UILayer.NormalLayer, out var layer) && layer.openedViews.Count > 0)
            {
                var viewController = layer.openedViews.Peek();
                if (viewController.uiView != null)
                {
                    viewController.uiView.OnCancel();
                }
            }
        }
    }
    
    
    public enum UIFitterType
    {
        Width,      // 宽适配
        Height,     // 高适配
    }
}
