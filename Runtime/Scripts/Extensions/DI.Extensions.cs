using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace GameFramework
{
    public static partial class Extensions
    {
        /// <summary>
        /// 한 씬의 [DIGameObject]/[DIMonoBehaviour] 마킹 객체를 주어진 컨테이너로 주입한다.
        /// </summary>
        public static void InjectSceneObjects(this IObjectResolver resolver, Scene scene)
        {
            foreach (var DIGameObject in scene.FindGameObjectsWithAttribute<DIGameObjectAttribute>().OrEmpty())
            {
                resolver.InjectGameObject(DIGameObject);
            }

            foreach (var DIMonoBehaviour in scene.FindComponentsWithAttribute<DIMonoBehaviourAttribute>().OrEmpty())
            {
                resolver.Inject(DIMonoBehaviour);
            }
        }
    }
}
