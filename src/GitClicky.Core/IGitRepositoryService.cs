namespace GitClicky.Core
{
    public interface IGitRepositoryService
    {
        string GetRepositoryPath(string path);
        bool IsPathInRepository(string path);
        string GetRemote(string repositoryPath, params string[] preferredRemoteNames);
        GitRemoteProvider GetProviderForRemote(string remote);
        bool HasProviderForRemote(string remote);
        string GetFetchRemoteForPath(string path);
    }
}