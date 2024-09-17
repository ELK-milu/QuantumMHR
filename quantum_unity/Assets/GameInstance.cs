using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class GameInstance : MonoBehaviour
{
	// 使用StringBuilder来优化字符串的重复构造
	StringBuilder m_logStr = new StringBuilder();
	// 日志文件存储位置
	string m_logFileSavePath;
    void Awake()
    {
	    // 当前时间
	    var t = System.DateTime.Now.ToString("yyyyMMddhhmmss");
	    m_logFileSavePath = string.Format("{0}/output_{1}.log", Application.persistentDataPath, t);
	    GameLogger.Log(m_logFileSavePath);
	    Application.logMessageReceived += OnLogCallBack;
	    GameLogger.Log("日志存储服务启动");
	}

    /// <summary>
    /// 打印日志回调
    /// </summary>
    /// <param name="condition">日志文本</param>
    /// <param name="stackTrace">调用堆栈</param>
    /// <param name="type">日志类型</param>
    private void OnLogCallBack(string condition, string stackTrace, LogType type)
    {
	    m_logStr.Append(condition);
	    m_logStr.Append("\n");
	    m_logStr.Append(stackTrace);
	    m_logStr.Append("\n");

	    if (m_logStr.Length <= 0) return;
	    if (!File.Exists(m_logFileSavePath))
	    {
		    var fs = File.Create(m_logFileSavePath);
		    fs.Close();
	    }
	    using (var sw = File.AppendText(m_logFileSavePath))
	    {
		    sw.WriteLine(m_logStr.ToString());
	    }
	    m_logStr.Remove(0, m_logStr.Length);
    }
}

