using System.Text;

namespace Qwaitumin.GameCore;

public class Message
{
  public enum Level { INFO, WARN, ERROR, DEBUG }
  private static readonly Dictionary<Level, string> typeColorDict = new()
  { {Level.INFO, "Deepskyblue"}, {Level.WARN, "Orange"}, {Level.ERROR, "Red"}, {Level.DEBUG, "Purple"}};

  public readonly Level Type;
  public readonly string TypeStr;
  public readonly string Time;
  public readonly string ClassName;
  public readonly string ThreadId;
  public readonly string Text;
  public readonly bool LogThread;

  private Message(Level type, string className, bool logThread, string text)
  {
    Time = DateTime.Now.ToString("HH:mm:ss.fff");
    ThreadId = Environment.CurrentManagedThreadId.ToString("X").PadLeft(8)[..8];
    TypeStr = type.ToString().PadLeft(5)[..5];
    Type = type;
    ClassName = className.PadLeft(16)[..16];
    LogThread = logThread;
    Text = text;
  }

  public string GetAsString(bool withBBCode, bool withContext = true)
  {
    var builder = new StringBuilder();
    if (withContext)
      builder.Append(GetContext(withBBCode)).Append(" : ");
    builder.Append(Text);
    return builder.ToString();
  }

  public override string ToString()
    => GetAsString(false);

  private string GetContext(bool withBBCode)
  {
    string finalTypeStr = TypeStr;
    if (withBBCode)
      finalTypeStr = AddColorToString(finalTypeStr, typeColorDict[Type]);

    var contextBuilder = new StringBuilder();
    contextBuilder.Append($"{Time} {finalTypeStr} [{ClassName}]");
    if (LogThread)
      contextBuilder.Append($" [{ThreadId}]");

    return contextBuilder.ToString();
  }

  private static string AddColorToString(string msg, string color)
    => $"[color={color}]{msg}[/color]";

  public static Message GetInfo(string className, bool logThread, string text)
    => new(Level.INFO, className, logThread, text);
  public static Message GetWarning(string className, bool logThread, string text)
    => new(Level.WARN, className, logThread, text);
  public static Message GetError(string className, bool logThread, string text)
    => new(Level.ERROR, className, logThread, text);
  public static Message GetDebug(string className, bool logThread, string text)
    => new(Level.DEBUG, className, logThread, text);
}