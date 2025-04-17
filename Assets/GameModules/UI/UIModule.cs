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

        private Transform root;
        private Camera worldCamera;
        private Camera uiCamera;
        
        private CanvasGroup blackMask;
        private CanvasGroup backgroundMask;
        private Tweener fadeTweener;

        private Dictionary<UIType, UIViewController> viewControllers;
        private Dictionary<UILayer, UILayerLogic> layers;
        private HashSet<UIType> openViews;
        private HashSet<UIType> residentViews;

        public EventSystem EventSystem { get; private set; }

        public void Init()
        {
            this.layers = new Dictionary<UILayer, UILayerLogic>();
            viewControllers = new Dictionary<UIType, UIViewController>();
            openViews = new HashSet<UIType>();
            residentViews = new HashSet<UIType>();

            worldCamera = Camera.main;
            worldCamera.cullingMask &= int.MaxValue ^ (1 << Layer.UI);

            var root = GameObject.Find("UIRoot");
            if (root == null)
            {
                root = new GameObject("UIRoot");
            }
            root.layer = Layer.UI;
            GameObject.DontDestroyOnLoad(root);
            this.root = root.transform;

            var camera = GameObject.Find("UICamera");
            if (camera == null)
            {
                camera = new GameObject("UICamera");
            }
            uiCamera = camera.GetOrAddComponent<Camera>();
            uiCamera.cullingMask = 1 << Layer.UI;
            uiCamera.transform.SetParent(this.root);
            uiCamera.orthographic = true;
            uiCamera.clearFlags = CameraClearFlags.Depth;

            EventSystem = EventSystem.current;

            var layers = Enum.GetValues(typeof(UILayer));
            foreach (UILayer layer in layers)
            {
                bool is3d = layer == UILayer.SceneLayer;
                Canvas layerCanvas = UIExtension.CreateLayerCanvas(layer, is3d, this.root, is3d ? worldCamera : uiCamera, Width, Height);
                UILayerLogic uILayerLogic = new UILayerLogic(layer, layerCanvas);
                this.layers.Add(layer, uILayerLogic);
            }
            blackMask = UIExtension.CreateBlackMask(this.layers[UILayer.BlackMaskLayer].canvas.transform);
            backgroundMask = UIExtension.CreateBlackMask(this.layers[UILayer.BackgroundLayer].canvas.transform);
            
            InitUIConfig();
        }

        public Rect GetSafeArea()
        {
            Rect rect = Screen.safeArea;
            if (UIFitterType == UIFitterType.Width)
            {
                var parent = layers[UILayer.BackgroundLayer].canvas.transform as RectTransform;
                float blackArea = Mathf.Abs(Height - parent.rect.height) / 2;
                rect.yMin = Mathf.Max(0, rect.yMin - blackArea);
                rect.yMax = Mathf.Min(rect.yMax + blackArea, Screen.height);
            }
            else if (UIFitterType == UIFitterType.Height)
            {
                var parent = layers[UILayer.BackgroundLayer].canvas.transform as RectTransform;
                float blackArea = Mathf.Abs(Width - parent.rect.width) / 2;
                rect.xMin = Mathf.Max(0, rect.xMin - blackArea);
                rect.xMax = Mathf.Min(rect.xMax + blackArea, Screen.width);
            }
            return rect;
        }

        public void EnableBackgroundMask(bool enable)
        {
            backgroundMask.alpha = enable ? 1 : 0;
        }

        public void InitUIConfig()
        {
            foreach (var cfg in UIConfig.ConfigList)
            {
                if (viewControllers.ContainsKey(cfg.UIType))
                {
                    Debug.LogErrorFormat("存在相同的uiType:{0}， 请检查UIConfig是否重复！", cfg.UIType.ToString());
                    continue;
                }

                viewControllers.Add(cfg.UIType, new UIViewController
                {
                    uiPath = cfg.Path,
                    uiType = cfg.UIType,
                    uiLayer = layers[cfg.UILayer],
                    isWindow = cfg.IsWindow,
                });
            }
        }

        /// <summary>
        /// 注册常驻UI
        /// </summary>
        public void AddResidentUI(UIType type)
        {
            residentViews.Add(type);
        }

        public void Open(UIType type, object userData = null, Action callback = null)
        {
            if (!viewControllers.ContainsKey(type))
            {
                Debug.LogErrorFormat("未配置uiType:{0}， 请检查UIConfig.cs！", type.ToString());
                return;
            }

            openViews.Add(type);
            viewControllers[type].Open(userData, callback);
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
            return openViews.Contains(type);
        }

        public void Close(UIType type, Action callback = null)
        {
            if (!viewControllers.ContainsKey(type))
            {
                Debug.LogErrorFormat("未配置uiType:{0}， 请检查UIConfig.cs！", type.ToString());
                return;
            }

            openViews.Remove(type);
            viewControllers[type].Close(callback);
        }

        public T GetView<T>(UIType type) where T : UIViewBase
        {
            if (!viewControllers.ContainsKey(type))
            {
                Debug.LogErrorFormat("未配置uiType:{0}， 请检查UIConfig.cs！", type.ToString());
                return null;
            }

            return viewControllers[type].uiView as T;
        }

        public void CloseAll(UIType ignoreType = UIType.Max, bool closeResidentView = false)
        {
            var list = ListPool<UIType>.Get();

            foreach (var uiType in openViews)
            {
                if (ignoreType == uiType) continue;

                if (closeResidentView || !residentViews.Contains(uiType))
                {
                    viewControllers[uiType].Close();
                    list.Add(uiType);
                }
            }
            foreach (var uiType in list)
            {
                openViews.Remove(uiType);
            }
            ListPool<UIType>.Release(list);
        }

        public void ReleaseAll()
        {
            foreach (var controller in viewControllers.Values)
            {
                if (!residentViews.Contains(controller.uiType))
                {
                    openViews.Remove(controller.uiType);
                    controller.Release();
                }
            }
        }

        public void FadeIn(float duration = 0.5f, TweenCallback callback = null)
        {
            if (fadeTweener != null && fadeTweener.IsPlaying())
                fadeTweener.Complete();
            fadeTweener = blackMask.DOFade(1.0f, duration);
            fadeTweener.onComplete = callback;
        }

        public void FadeOut(float duration = 0.5f, TweenCallback callback = null)
        {
            if (fadeTweener != null && fadeTweener.IsPlaying())
                fadeTweener.Complete();
            fadeTweener = blackMask.DOFade(0.0f, duration);
            fadeTweener.onComplete = callback;
        }

        public void FadeInOut(float duration = 1.0f, TweenCallback callback = null)
        {
            if (fadeTweener != null && fadeTweener.IsPlaying())
                fadeTweener.Complete();
            fadeTweener = blackMask.DOFade(1.0f, duration * 0.5f);
            fadeTweener.onComplete += () =>
            {
                fadeTweener = blackMask.DOFade(0.0f, duration * 0.5f);
                fadeTweener.onComplete = callback;
            };
        }

        public void Cancel()
        {
            if (layers.TryGetValue(UILayer.NormalLayer, out var layer) && layer.openedViews.Count > 0)
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
