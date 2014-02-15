namespace GitClicky.Core
{
    public interface IGitRepositoryService
    {
        string GetFetchRemoteForPath(string path);
    }
}