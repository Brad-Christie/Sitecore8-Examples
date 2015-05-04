ReadOnlyDataProvider
====================

### Summary

Shows how to create a data provider in sitecore that only brings in content from an external source.

This example was tested on Sitecore 8 rev 150223.

### Initial Setup

This project relies on two templates (`Products` & `Product`), as well as an item in the content section based on `Products` that the DataProvider will populate.

I've provided a package for this demo in the `\sitecore` folder of this solution. I've also included a TDS project which has the same items. Please feel free to use whichever is more comfortable.

### Release Notes

* Initial Release (05/05/2015)

### Dislaimer

As you know I cannot distribute Sitecore binaries, so I have excluded the `Sitecore.Kernel.dll` file from the project. Before building, please place that library it in the `\sitecore` directory before building. 