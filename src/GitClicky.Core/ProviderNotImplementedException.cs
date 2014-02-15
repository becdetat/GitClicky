using System;

namespace GitClicky.Core
{
    public class ProviderNotImplementedException : Exception
    {
        public ProviderNotImplementedException(GitRemoteProvider provider)
            : base(provider.ToString())
        {
        }
    }
}