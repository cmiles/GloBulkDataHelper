# GLO Bulk Data Helper

THIS IS AN EARLY WORK IN PROGRESS ONLY!!! ATM DON'T EXPECT ANY WORKING CODE IN THIS REPO!!!

The goal of this project is to provide some tools to make working with bulk General Land Office a bit easier...

## Background

In the Western United States the General Land Office was responsible for surveying, platting, and recording the removal of Public lands to other entities. While these records do not record sales between private individuals - and omit some interesting details such a homestead claims that were filed but never completed - they are an interesting source of information about early land ownership in many Western States.

The GLO Records can be searched on the [BLM's Official Federal Land Records Site](https://glorecords.blm.gov/default.aspx) which provides an excellent interface for looking up and viewing records - for most uses (including looking for records related to individuals/family members) this is probably the best way to search and view the records.

However the [BLM's Official Federal Land Records Site](https://glorecords.blm.gov/default.aspx) does not provide a good interface for a question/workflow such as 'show me all the land patents for the San Pedro River, exclude transfers to the state of Arizona and return the results as GIS data to view/query/visualize in [QGIS](https://qgis.org/)'.

Thankfully the Bureau of Land Management, Eastern States Office, [does offer the data in bulk on https://glorecords.blm.gov/](https://glorecords.blm.gov/BulkData/) via CSV files that 'are suitable for importing into your own database' - it is great that this data is easily available and it is a good start towards addressing the question/workflow above...

Unfortunately the GLO Bulk data only provides [Public Land Survery System](https://nationalmap.gov/small_scale/a_plss.html) descriptions - not latitude/longitude or other data needed to easily draw boundaries onto a map. The best source I have found to get the Lat/Long data from the PLSS descriptions is the FindLd service offered thru the [BLM's Navigator Site](https://navigator.blm.gov/home) [Web Services](https://navigator.blm.gov/services). This service can not translate all PLSS legal descriptions - but translates most and is quite useful because it can provide information on some of the 'lot' style descriptions that can not be 'calculated'.