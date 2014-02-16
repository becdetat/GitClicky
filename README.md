GitClicky
========

Windows Explorer context menu extension to get the raw URL for a file contained in a Git repository on GitHub or BitBucket.


## Usage


<img align="right" src="http://snag.gy/y9VrV.jpg" alt="Screenshot of GitClicky's addition to the context menu"/>

- Download the [latest version](https://github.com/bendetat/GitClicky/releases/latest) or find [older releases here](https://github.com/bendetat/GitClicky/releases)
- Installation requires elevated privileges and you may need to [manually restart Windows Explorer](https://www.google.com.au/search?q=restart+windows+explorer)
	- The installation script attempts to restart Explorer but it isn't perfect
- Inside a repository with a remote hosted by BitBucket or GitHub, right click a file and select "Copy {GitHub/Bitbucket} repository raw URL"
- The URL to the file in the remote repository will be generated and placed on the clipboard

Multiple files can be selected and the URLs will be copied to the clipboard as a list

The remote is identified by finding the first remote hosted on GitHub or Bitbucket, in order of `origin`, `upstream`, or libgit2's default ordering.


## Known issues

- Windows Explorer doesn't correctly restart on installation
- The extension may not correctly uninstall, workaround is (hopefully):
	- open a console as administrator
	- navigate to `%userprofile%\AppData\Local\bendetat\GitClicky\{version}`
	- execute `srm.exe uninstall GitClicky.Extension.dll -codebase`
	- restart Explorer manually
	- remove the GitClicky files from `%userprofile%\AppData\Local\bendetat\`
- The URL generation assumes that the branch is `master`
- The URL generation relies on the username in the Git remote. If your GitHub or Bitbucket username has changed the link may not work. GitHub seems to be ok with redirecting after a changed username but Bitbucket isn't happy.
- The URL generation relies on some basic manipulation to create the URL, it doesn't depend on any of GitHub's or BitBucket's APIs. For this reason the URL generation should be considered fairly fragile and non-canonical.
	- GitHub's API doesn't actually provide a raw data URL by itself anyway from what I could find. There is an API for getting [the BASE64 encoded content](http://developer.github.com/v3/repos/contents/#contents) of a file but not a canonical URL to the file.


## Roadmap

- Pull out the current branch and use that in the url
- Only show context menu for files that are checked in
- fix explorer restart issue
- fix uninstaller
- unit tests


## Contributing

If you would like to contribute to this project then feel free to send a pull request or submit an issue. Communicate with me via Twitter at [@bendetat](http://twitter.com/bendetat).


## Version history

### [0.1.0](https://github.com/bendetat/GitClicky/releases/tag/0.1.0)
- Initial version


## Third party libraries and tools

- [libgit2sharp](https://github.com/libgit2/libgit2sharp) is used for getting data from the Git repository. It is included as a submodule and compiled as part of GitClicky to add strong naming (required for the shell extension)
- [SharpShell](https://github.com/dwmkerr/sharpshell) is used for the shell extension heavy lifting, including registering the extension on installation
- [NSIS](http://nsis.sourceforge.net/Main_Page) is used to create the installation script. I know, how old fashioned of me, but it works.
- [nsRestartExplorer](https://github.com/sherpya/nsRestartExplorer) is used in the installation script to (attempt to) restart Windows Explorer


