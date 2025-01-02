namespace Qwaitumin.GameCore;

public abstract class DatabaseRepository
{
  protected Database database;

  protected DatabaseRepository(Database database)
  {
    this.database = database;
  }
}