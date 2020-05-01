# 🎭 Gravatars in Sitecore Experience Profile

This repository contains code for displaying [Gravatars][1] in the Sitecore
Experience Profile that [Rick Bauer][2] and I demonstrated in our _JavaScript
Services & Marketing Automation_ and _Crank Up Your Sitecore Authoring and
Marketing Experience_ presentations.

![Contact in Experience Profile with robohash default image](docs/robohash.png)

## ⚙️ Implementation

There are two ways to integrate [Gravatar][1] with the Experience Profile:

1. Override the `IContactService` that the Experience Profile uses to retrieve
   the contact's [Gravatar][1] on page load; and/or
2. Write to the contact's `Avatar` facet at some point in their journey with
   your website (see the [warning below](#-avatar-facet-warning)!).

The first option will add [Gravatar][1] to _all_ contacts in the Experience
Profile as it does not require you to write to the `Avatar` facet of your
contacts. The second option makes the contact's [Gravatar][1] available
anywhere you use the `Avatar` facet in your site.

At a minimum I recommend implementing the first option in your Sitecore
solutions. The second option is nice for new Sitecore installs, but probably not
practical for existing solutions with lots of existing contacts. Both options
are compatible with each other.

See the code in the [Experience Profile feature module][3] to see how you can
integrate a [`GravatarService`][4] into the `IContactService` to load the
[Gravatar][1] for _all_ contacts in the Experience Profile that don't have the
`Avatar` facet set.

See the code in the [`ContactRepository`][5] of the [Website project module][6]
to see how to set the `Avatar` facet on contacts using [Gravatar][1].

### 🚨 `Avatar` Facet Warning

There is a bug in the xConnect Index Worker in Sitecore 9.1 and above that
causes it to fail and _stop indexing any contacts_ if images above a certain
size are written to the `Avatar` facet. New contacts will not show up in the
Experience Profile until the offending `Avatar` facet is removed from the data
store (e.g., SQL Server).

To fix this issue, set the `StringFieldMaximumLength` to a value that is
divisible by `4` in the xConnect Indexer Worker's config as I've done in
[`sc.Xdb.Collection.Search.IndexTruncation.xml`][7] and deploy it to your
xConnect Index Worker in all environments.

No matter what precautions you take (e.g., downsizing images, compressing
images), if you write to the `Avatar` facet in xConnect you **are** going to be
affected by this issue. **DO NOT** write to the `Avatar` facet (using
[Gravatar][1] or otherwise) in Sitecore 9.1 or above without applying this fix
first.

## 🏗️ Setup

### 🐳 Docker

1. Build the Sitecore 9.3 docker images using the steps in the
   [Sitecore Docker images repository][8].
2. Build the solution with the `Docker` build configuration.
3. All projects in the solution will be automatically published to Docker on
   build courtesy of [Helix Publishing Pipeline][9].
4. On the command line:
   1. `cd C:\[path-to]\sitecore-experience-profile-gravatar`
   2. `docker-compose up`

### 💽 Locally

1. Install a new instance of [Sitecore 9.3][10].
2. Update the `publishUrl` in [`PublishSettings.Sitecore.targets`][11] to your
   Sitecore installation's web root (e.g., `C:\inetpub\wwwroot\sc93.sc`).
3. Update the `publishUrl` in [`PublishSettings.XConnectIndexer.targets`][12] to
   your Sitecore installation's xConnect Indexer Worker root
   (e.g., `C:\inetpub\wwwroot\sc93.xconnect\App_Data\jobs\continuous\IndexWorker`).
4. Update the `sourceFolder` in [`CoreyAndRick.Project.Common.Dev.config`][13] to
   point to the root of this repository on your disk.
5. Build the solution with the `Debug` build configuration.
6. All projects in the solution will be automatically published to Sitecore on
   build courtesy of [Helix Publishing Pipeline][8].

[1]: https://www.gravatar.com/
[2]: https://twitter.com/Sitecordial
[3]: src/Feature/ExperienceProfile/sitecore
[4]: src/Feature/ExperienceProfile/sitecore/Services/GravatarService.cs
[5]: src/Project/Website/sitecore/Repositories/ContactRepository.cs
[6]: src/Project/Website/sitecore
[7]: src/Project/Common/xconnect-indexer/App_Data/config/sitecore/CoreyAndRick/sc.Xdb.Collection.Search.IndexTruncation.xml#L12
[8]: https://github.com/sitecore/docker-images
[9]: https://github.com/richardszalay/helix-publishing-pipeline
[10]: https://dev.sitecore.net/Downloads/Sitecore_Experience_Platform/93/Sitecore_Experience_Platform_93_Initial_Release.aspx
[11]: PublishSettings.Sitecore.targets
[12]: PublishSettings.XConnectIndexer.targets
[13]: src/Project/Common/sitecore/App_Config/Environment/CoreyAndRick.Project.Common.Dev.config
