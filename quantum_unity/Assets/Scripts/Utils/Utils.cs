using Quantum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static string ColorToHex(this Color color)
    {
        if (color == Color.red)
        {
            return "FF0000";
        }
        if (color == Color.green)
        {
            return "00FF00";
        }
        if (color == Color.blue)
        {
            return "0000FF";
        }
        int r = Mathf.RoundToInt(color.r * 255.0f);
        int g = Mathf.RoundToInt(color.g * 255.0f);
        int b = Mathf.RoundToInt(color.b * 255.0f);
        int a = Mathf.RoundToInt(color.a * 255.0f);
        string hex = string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", r, g, b, a);
        return hex;
    }
    public static Animator AnimatorSetBool (this Animator animator,out bool target, string name, bool value)
    {
        target = value;
        //animator.SetBool(name,value);
        return animator;
    }
	public static bool TryGetQuantumFrame (out Frame frame)
	{
		frame = null;
		if (QuantumRunner.Default == null)
		{
			return false;
		}
		if (QuantumRunner.Default.Game == null)
		{
			return false;
		}
		frame = QuantumRunner.Default.Game.Frames.Predicted;
		if (frame == default)
		{
			return false;
		}
		return true;
	}
}

public class GameLogger
{
    // 普通调试日志开关
    public static bool s_debugLogEnable = true;
    // 警告日志开关
    public static bool s_warningLogEnable = true;
    // 错误日志开关
    public static bool s_errorLogEnable = true;

    public static void Log(object message, Object context = null)
    {
        if (!s_debugLogEnable) return;
        Debug.Log(message, context);
    }

    public static void LogWarning(object message, Object context = null)
    {
        if (!s_warningLogEnable) return;
        Debug.LogWarning(message, context);
    }

    public static void LogError(object message, Object context = null)
    {
        if (!s_warningLogEnable) return;
        Debug.LogError(message, context);
    }



#region 解决日志双击溯源问题
#if UNITY_EDITOR
    [UnityEditor.Callbacks.OnOpenAssetAttribute(0)]
    static bool OnOpenAsset(int instanceID, int line)
    {
        string stackTrace = GetStackTrace();
        if (!string.IsNullOrEmpty(stackTrace) && stackTrace.Contains("GameLogger:Log"))
        {
            // 使用正则表达式匹配at的哪个脚本的哪一行
            var matches = System.Text.RegularExpressions.Regex.Match(stackTrace, @"\(at (.+)\)",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            string pathLine = "";
            while (matches.Success)
            {
                pathLine = matches.Groups[1].Value;

                if (!pathLine.Contains("GameLogger.cs"))
                {
                    int splitIndex = pathLine.LastIndexOf(":");
                    // 脚本路径
                    string path = pathLine.Substring(0, splitIndex);
                    // 行号
                    line = System.Convert.ToInt32(pathLine.Substring(splitIndex + 1));
                    string fullPath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("Assets"));
                    fullPath = fullPath + path;
                    // 跳转到目标代码的特定行
                    UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(fullPath.Replace('/', '\\'), line);
                    break;
                }
                matches = matches.NextMatch();
            }
            return true;
        }
        return false;
    }

    /// <summary>
    /// 获取当前日志窗口选中的日志的堆栈信息
    /// </summary>
    /// <returns></returns>
    static string GetStackTrace()
    {
        // 通过反射获取ConsoleWindow类
        var ConsoleWindowType = typeof(UnityEditor.EditorWindow).Assembly.GetType("UnityEditor.ConsoleWindow");
        // 获取窗口实例
        var fieldInfo = ConsoleWindowType.GetField("ms_ConsoleWindow",
            System.Reflection.BindingFlags.Static |
            System.Reflection.BindingFlags.NonPublic);
        var consoleInstance = fieldInfo.GetValue(null);
        if (consoleInstance != null)
        {
            if ((object)UnityEditor.EditorWindow.focusedWindow == consoleInstance)
            {
                // 获取m_ActiveText成员
                fieldInfo = ConsoleWindowType.GetField("m_ActiveText",
                    System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.NonPublic);
                // 获取m_ActiveText的值
                string activeText = fieldInfo.GetValue(consoleInstance).ToString();
                return activeText;
            }
        }
        return null;
    }
#endif
    #endregion

}
