using System;

namespace GameFramework.Auth
{
    /// <summary>한 기기에서 여러 계정을 쓰기 위한 프로필. Multiplayer Play Mode가 인스턴스마다
    /// 넘겨주는 -name 인자를 그대로 쓴다(MPPM에는 "몇 번 인스턴스인가"를 주는 API가 없다).</summary>
    public static class AuthProfile
    {
        public const string DefaultProfile = "default";

        private const string InstanceNameArgument = "-name";
        private const string FirstInstanceName = "Player1";

        private static string cached;

        public static string Current => cached ??= Resolve(Environment.GetCommandLineArgs());

        public static string Resolve(string[] commandLineArgs)
        {
            if (commandLineArgs == null)
            {
                return DefaultProfile;
            }

            for (int i = 0; i < commandLineArgs.Length - 1; i++)
            {
                if (commandLineArgs[i] != InstanceNameArgument)
                {
                    continue;
                }

                string name = commandLineArgs[i + 1];
                //  첫 인스턴스를 기본과 같게 둬야 평소 에디터 실행과 계정이 갈리지 않는다.
                return string.IsNullOrEmpty(name) || name == FirstInstanceName ? DefaultProfile : name;
            }

            return DefaultProfile;
        }
    }
}
