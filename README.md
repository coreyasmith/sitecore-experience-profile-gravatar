# 🎭 Gravatars in Sitecore Experience Profile

This repository contains code for displaying [Gravatars][1] in the Sitecore
Experience Profile that [Rick Bauer][2] and I demonstrated in our _JavaScript
Services & Marketing Automation_ and _Crank Up Your Sitecore Authoring and
Marketing Experience_ presentations.

## 🏗️ Setup

### 🐳 Docker

1. Build the Sitecore 9.3 docker images using the steps in the
   [Sitecore Docker images repository][3].
2. Build the solution with the `Docker` build configuration.
3. Publish the projects in the solution with the `Docker` publish profile.
4. On the command line:
   1. `cd C:\[path-to]\sitecore-improved-page-attribute-controls`
   2. `docker-compose up`

### 💽 Locally

1. Install a new instance of [Sitecore 9.3][4].
2. Update the `publishUrl` in [`PublishSettings.Sitecore.targets`][5] to your
   Sitecore installation's web root (e.g., `C:\inetpub\wwwroot\sc93.sc`).
3. Build the solution with the `Debug` build configuration.
4. Publish the projects in the solution with the `Local` publish profile.

[1]: https://www.gravatar.com/
[2]: https://twitter.com/Sitecordial
[3]: https://github.com/sitecore/docker-images
[4]: https://dev.sitecore.net/Downloads/Sitecore_Experience_Platform/93/Sitecore_Experience_Platform_93_Initial_Release.aspx
[5]: PublishSettings.Sitecore.targets
