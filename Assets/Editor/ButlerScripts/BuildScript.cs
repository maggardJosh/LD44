﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
namespace Editor.ButlerScripts
{
    public class BuildScript
    {
        
        public static void NewRelease(string releaseMessage)
        {
            
            AppVersionNum result = GetCurrentBuildVersions();
            List<string> buildMessages = new List<string>();
            if (ButlerSettings.Instance.BuildWeb)
                buildMessages.Add("web-v" + (result.webVersion + 1));
            if (ButlerSettings.Instance.BuildPC)
                buildMessages.Add("pc-v" + (result.pcVersion + 1));
            if (ButlerSettings.Instance.BuildAndroid)
                buildMessages.Add("android-v" + (result.androidVersion + 1));

            string mergeMessage = string.Join("_", buildMessages);
            ProcessStartInfo newProcInf = new ProcessStartInfo()
            {
                FileName = "cmd",
                Arguments = "/c \"git stash\"",
                UseShellExecute = false,
                CreateNoWindow = true
            };
            Process.Start(newProcInf).WaitForExit();
            newProcInf.Arguments = "/c \"git checkout master\"";
            Process.Start(newProcInf).WaitForExit();
            newProcInf.Arguments = "/c git merge develop --no-ff -m \"" + (!String.IsNullOrEmpty(releaseMessage) ? releaseMessage + " " : "") + "release_" + mergeMessage + "\"";
            Process.Start(newProcInf).WaitForExit();

            if (BuildAll())
            {
                newProcInf.Arguments = "/c \"git tag " + mergeMessage + "\"";
                Process.Start(newProcInf).WaitForExit();
                newProcInf.Arguments = "/c \"git push --all\"";
                Process.Start(newProcInf).WaitForExit();
                newProcInf.Arguments = "/c \"git push --tags\"";
                Process.Start(newProcInf).WaitForExit();
            }
            else
            {
                //failed build
                newProcInf.Arguments = "/c \"git reset --hard origin/master\"";
                Process.Start(newProcInf).WaitForExit();
            }
            newProcInf.Arguments = "/c \"git checkout develop\"";
            Process.Start(newProcInf).WaitForExit();
            newProcInf.Arguments = "/c \"git stash pop\"";
            Process.Start(newProcInf).WaitForExit();

        }

        [MenuItem("Build/New Release")]
        public static void NewRelease()
        {
            NewRelease("");
        }
        private struct AppVersionNum
        {
            public int webVersion;
            public int pcVersion;
            public int androidVersion;
        }
        private static AppVersionNum GetCurrentBuildVersions()
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd";
            p.StartInfo.Arguments = $"/c \"butler status {ButlerSettings.Instance.UserName}/{ButlerSettings.Instance.GameName}\"";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.CreateNoWindow = true;
            p.EnableRaisingEvents = true;
            int webVersion = 1;
            int pcVersion = 1;
            int androidVersion = 1;
            p.OutputDataReceived += (s, e) =>
            {
                string[] strings = e.Data.Split('|');
                if (strings.Length >= 5)
                    if (strings[1].Trim().Equals("web", StringComparison.CurrentCultureIgnoreCase))
                        int.TryParse(strings[4], out webVersion);
                    else if (strings[1].Trim().Equals("pc", StringComparison.CurrentCultureIgnoreCase))
                        int.TryParse(strings[4], out pcVersion);
                    else if (strings[1].Trim().Equals("android", StringComparison.CurrentCultureIgnoreCase))
                        int.TryParse(strings[4], out androidVersion);
            };
            p.Start();
            p.BeginOutputReadLine();
            p.WaitForExit();

            return new AppVersionNum()
            {
                webVersion = webVersion,
                pcVersion = pcVersion,
                androidVersion = androidVersion
            };

        }

        [MenuItem("Build/Itch.io Build 'n Push All", false, 0)]
        public static bool BuildAll()
        {
            if (ButlerSettings.Instance.BuildAndroid && !AndroidBuild())
                return false;
            if (ButlerSettings.Instance.BuildPC && !WindowsBuild())
                return false;
            if (ButlerSettings.Instance.BuildWeb && !WebBuild())
                return false;
            return true;
        }

        [MenuItem("Build/Itch.io Build Web", false, 10)]
        public static bool WebBuild()
        {
            var result = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = ButlerSettings.Instance.Scenes,
                locationPathName = "Builds/Web",
                target = BuildTarget.WebGL,
                options = BuildOptions.None

            });
            if (result.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
                return false;
            WebPush();
            return true;
        }

        [MenuItem("Build/Itch.io Push Web", false, 11)]
        public static void WebPush()
        {
            Process.Start(new ProcessStartInfo("cmd", $@"/c ""butler push .\Builds\Web {ButlerSettings.Instance.UserName}/{ButlerSettings.Instance.GameName}:Web && sleep 2"""));
        }

        [MenuItem("Build/Itch.io Build Windows", false, 12)]
        public static bool WindowsBuild()
        {
            var result = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = ButlerSettings.Instance.Scenes,
                locationPathName = $"Builds/PC/{ButlerSettings.Instance.GameName}.exe",
                target = BuildTarget.StandaloneWindows,
                options = BuildOptions.None
            });
            if (result.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
                return false;
            WindowsPush();
            return true;
        }

        [MenuItem("Build/Itch.io Push Windows", false, 13)]
        public static void WindowsPush()
        {
            Process.Start(new ProcessStartInfo("cmd", $@"/c ""butler push .\Builds\PC {ButlerSettings.Instance.UserName}/{ButlerSettings.Instance.GameName}:PC && sleep 2"""));
        }

        [MenuItem("Build/Itch.io Build Android", false, 14)]
        public static bool AndroidBuild()
        {
            var result = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = ButlerSettings.Instance.Scenes,
                locationPathName = $"Builds/Android/{ButlerSettings.Instance.GameName}.apk",
                target = BuildTarget.Android,
                options = BuildOptions.None
            });
            if (result.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
                return false;
            AndroidPush();
            return true;
        }

        [MenuItem("Build/Itch.io Push Android", false, 15)]
        public static void AndroidPush()
        {
            Process.Start(new ProcessStartInfo("cmd", $@"/c ""butler push .\Builds\Android\{ButlerSettings.Instance.GameName}.apk {ButlerSettings.Instance.UserName}/{ButlerSettings.Instance.GameName}:Android && sleep 2"""));
        }
    }
}