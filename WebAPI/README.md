Sitecore8-WebAPI
================

### Summary

This is an example of how to use the WebAPI in Sitecore8. At the time of this test, I was using Sitecore 8 rev 141212.

### Example Data

http://sitecore8/api/products

    [
      {
        "Id": 1,
        "Name": "Sitecore 7.1",
        "Category": "CMS"
      },
      {
        "Id": 2,
        "Name": "Sitecore 7.2",
        "Category": "CMS"
      },
      {
        "Id": 3,
        "Name": "Sitecore 7.5",
        "Category": "CMS"
      },
      {
        "Id": 4,
        "Name": "Sitecore 8.0",
        "Category": "CMS"
      },
      {
        "Id": 5,
        "Name": "WFFM 2.3",
        "Category": "Module"
      },
      {
        "Id": 6,
        "Name": "WFFM 2.4",
        "Category": "Module"
      },
      {
        "Id": 7,
        "Name": "WFFM 2.5",
        "Category": "Module"
      }
    ]

http://sitecore8/api/products/5

    {
      "Id": 5,
      "Name": "WFFM 2.3",
      "Category": "Module"
    }

**Disclaimer**

I have Sitecore 8 binaries added as an internal NuGet package. As you know, I cannot distribute these binaries, so please make sure to add the appropriate DLLs to these projects before compiling. This should simply consist of:

* Remove Sitecore8 & Sitecore8.Mvc package references.
* * Add the following DLL references to the projects:
    * Sitecore.Kernel.dll,
    * Sitecore.Analytics.dll,
    * Sitecore.Mvc.dll
    * Sitecore.Mvc.Analytics.dll
* Compile & build.
* Publish to your local Sitecore instance (mine happens to be named `sc8r141212` as that was the version I used for this test).