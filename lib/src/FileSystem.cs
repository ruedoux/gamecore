namespace Qwaitumin.GameCore;

public static class FileSystem
{
  public static string[] GetFilesFromDirectory(string searchFolder, string[] filters, bool isRecursive)
  {
    List<string> filesFound = new();
    var searchOption = isRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
    foreach (var filter in filters)
      filesFound.AddRange(Directory.GetFiles(searchFolder, string.Format("*.{0}", filter), searchOption));

    return filesFound.ToArray();
  }
}