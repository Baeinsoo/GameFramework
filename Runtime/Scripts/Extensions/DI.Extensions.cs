using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace GameFramework
{
    public static partial class Extensions
    {
        /// <summary>
        /// 한 씬의 [SceneInjectGameObject]/[SceneInjectMonoBehaviour] 마킹 객체를 주어진 컨테이너로 주입한다.
        /// </summary>
        public static void InjectSceneObjects(this IObjectResolver resolver, Scene scene)
        {
            foreach (var go in scene.FindGameObjectsWithAttribute<SceneInjectGameObjectAttribute>().OrEmpty())
            {
                resolver.InjectGameObject(go);
            }

            foreach (var mb in scene.FindComponentsWithAttribute<SceneInjectMonoBehaviourAttribute>().OrEmpty())
            {
                resolver.Inject(mb);
            }
        }
    }
}
