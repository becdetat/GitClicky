using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GitClicky.Core;
using GitClicky.Core.Extensions;
using SharpShell.Attributes;
using SharpShell.SharpContextMenu;

namespace GitClicky.Extension
{
    [ComVisible(true)]
    [COMServerAssociation(AssociationType.AllFiles)]
    public class GitClickyExtension : SharpContextMenu
    {
        private readonly IGitRepositoryService _gitRepositoryService;

        public GitClickyExtension()
        {
            _gitRepositoryService = new GitRepositoryService();
        }

        protected override bool CanShowMenu()
        {
            return
                SelectedItemPaths.Any()
                && SelectedItemPaths.All(x => _gitRepositoryService.IsPathInRepository(x));
        }

        protected override ContextMenuStrip CreateMenu()
        {
            var provider = !SelectedItemPaths.Any()
                ? "Git provider"
                : _gitRepositoryService.GetProviderForRemote(SelectedItemPaths.First()).ToString();

            var getRawGitUrlMenuItem = new ToolStripMenuItem()
            {
                Text = "Get {0} URL".FormatWith(provider)
            };

            getRawGitUrlMenuItem.Click += (sender, args) =>
            {
                var urls = SelectedItemPaths
                    .Select(x => _gitRepositoryService.GetFetchRemoteForPath(x))
                    .Join(Environment.NewLine);
                Clipboard.SetText(urls);
            };

            var menu = new ContextMenuStrip();
            menu.Items.Add(getRawGitUrlMenuItem);

            return menu;
        }
    }
}
