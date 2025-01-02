namespace Qwaitumin.GameCore;

public static class Random
{
  public static string GetRandomString(
    int length, string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789")
  {
    var stringChars = new char[length];
    var random = new System.Random();

    for (int i = 0; i < stringChars.Length; i++)
      stringChars[i] = chars[random.Next(chars.Length)];

    return new string(stringChars);
  }
}