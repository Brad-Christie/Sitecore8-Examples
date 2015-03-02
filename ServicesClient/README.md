Sitecore8-ServicesClient
================

### Summary

This is an example of how to use the Sitecore.Services.Client in Sitecore8. At the time of this test, I was using Sitecore 8 rev 141212.

Also, this makes no assumptions about data present in Sitecore other than those found in a clean install. I tried to keep this demo as simple as possible, but if you find yourself having issues it may be variances from that of the default install.

### Initialization

To get the full benefit of this demo, be sure to publish the web project to your local instance. Once complete, please visit the following url to begin:

> http://sc8r141212/Sitecore8-ServicesClient/

_(Note that `sc8r141212` happens to be my instance name. Change this to match your installation's endpoint.)_

Also, for your own trials, I have included a console application as well that accesses these endpoints. This uses a library called [RestSharp](http://restsharp.org) to make calls easier, but you can use whatever means available/necessary. This is a REST API afterall, so use whatever you would for Twitter, Facebook, etc.

### Further Reading

* [Developer's Guide to Sitecore.Services.Client](https://sdn.sitecore.net/upload/sitecore7/75/developer%27s_guide_to_sitecore.services.client_sc75-a4.pdf)

### Disclaimer

> Use at own risk. This is meant to run on an instance you can tear down/rebuild, not a production environment. This is due to the relaxed security settings and my use of an MVC site (internally) to demo functionality (which may override views/custom config files you already have).
>
> If you feel this could be problematic, please do not install this on your instance.

I have Sitecore 8 binaries added as an internal NuGet package. As you know, I cannot distribute these binaries, so please make sure to add the appropriate DLLs to these projects before compiling. This should simply consist of:

* Remove Sitecore & Sitecore.Services.Client package references.
    * Add the following DLL references to the projects:
        * Sitecore.Kernel.dll,
        * Sitecore.Analytics.dll
        * Sitecore.Logging.dll
        * Sitecore.Services.Client.dll
        * Sitecore.Services.Core.dll
        * Sitecore.Services.Infrastructure.dll
        * Sitecore.Services.Infrastructue.Sitecore.dll
* Compile & build.
* Publish to your local Sitecore instance (mine happens to be named `sc8r141212` as that was the version I used for this test).