namespace GitClicky.Core
{
    public interface IGitRepositoryService
    {
        string GetFetchRemoteForPath(string path);
        bool IsPathInRepository(string path);
        GitRemoteProvider GetProviderForRemote(string remote);
    }
}