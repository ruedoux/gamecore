using System.Collections;
using System.Runtime.CompilerServices;
using System.Text;

namespace Qwaitumin.GameCore;

public record LogSettings(
  bool SupressError = false,
  bool SupressWarning = false,
  bool LogThread = false,
  bool Debug = false,
  int MemoryCapacity = 50
);


public class Logger
{
  private const string NULL_STRING = "<null>";
  private static readonly object printLock = new();

  private readonly ObserverNotifier<Message> observerNotifier;
  private readonly LogSettings logSettings;
  private static readonly char[] separator = new[] { '\r', '\n' };

  public Logger(IEnumerable<Action<Message>> observers, LogSettings logSettings)
  {
    this.logSettings = logSettings;
    observerNotifier = new(observers);
  }

  public static string ParseAsString(params object?[]? msgs)
  {
    List<object?> objects = new();
    if (msgs is null)
      objects.Add(null);
    else
      foreach (var msg in msgs)
        objects.Add(msg);

    var builder = new StringBuilder();
    builder.Append(string.Join("", objects.Select(ParseAsString)));
    return builder.ToString();
  }

  public void Log(string message, [CallerFilePath] string callerFilePath = "")
  {
    PushMessage(Message.GetInfo(
      GetSourceClassName(callerFilePath), logSettings.LogThread, message));
  }

  public void LogError(string message, [CallerFilePath] string callerFilePath = "")
  {
    if (logSettings.SupressError) return;

    PushMessage(Message.GetError(
      GetSourceClassName(callerFilePath), logSettings.LogThread, message));
  }

  public void LogWarning(string message, [CallerFilePath] string callerFilePath = "")
  {
    if (logSettings.SupressWarning) return;

    PushMessage(Message.GetWarning(
      GetSourceClassName(callerFilePath), logSettings.LogThread, message));
  }

  public void LogDebug(string message, [CallerFilePath] string callerFilePath = "")
  {
    if (!logSettings.Debug) return;

    PushMessage(Message.GetDebug(
      GetSourceClassName(callerFilePath), logSettings.LogThread, message));
  }

  public void LogException(string message, Exception ex, [CallerFilePath] string callerFilePath = "")
  {
    if (logSettings.SupressError) return;
    if (ex is null) return;

    string output = ex.Message;
    if (ex.InnerException is not null)
    {
      List<string> parsedExceptions = new() { ex.InnerException.Message };
      string[] lines = ex.InnerException.ToString()
        .Split(separator, StringSplitOptions.RemoveEmptyEntries);
      foreach (string line in lines) parsedExceptions.Add(line);
      output = message + "\n" + string.Join('\n', parsedExceptions.ToArray());
    }

    PushMessage(Message.GetError(
      GetSourceClassName(callerFilePath), logSettings.LogThread, output));
  }

  // NOTE: This could probably delegate the notification to
  //       a different thread so it wont block printing. 
  private static void ForwardMessageToObservers(
    Message message, ObserverNotifier<Message> messageObservers)
  {
    lock (printLock)
    {
      messageObservers.NotifyObservers(message);
    }
  }

  private static string GetSourceClassName(string filePath)
    => Path.GetFileNameWithoutExtension(filePath) ?? "<Unknown>";

  private static string ParseAsString(object? msg)
  {
    if (msg is null)
      return NULL_STRING;

    if (msg is IDictionary dictionary)
    {
      var pairs = new List<string>();
      foreach (DictionaryEntry entry in dictionary)
        pairs.Add($"\"{ParseAsString(entry.Key)}\": {ParseAsString(entry.Value)}");
      return $"{{{string.Join(", ", pairs)}}}";
    }

    if (msg is ICollection collection)
    {
      var elements = new List<string>();
      foreach (var item in collection)
        elements.Add(ParseAsString(item));
      return $"[{string.Join(", ", elements)}]";
    }

    if (msg is IEnumerable enumerable && !(msg is string))
    {
      var elements = new List<string>();
      foreach (var item in enumerable)
        elements.Add(ParseAsString(item));
      return $"[{string.Join(", ", elements)}]";
    }

    return msg.ToString() ?? NULL_STRING;
  }

  private void PushMessage(Message message)
   => ForwardMessageToObservers(message, observerNotifier);
}