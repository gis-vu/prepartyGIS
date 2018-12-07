from pathlib import Path


def proccess(index):
    
    print(index)

    mxd = arcpy.mapping.MapDocument("CURRENT")
    df = arcpy.mapping.ListDataFrames(mxd)[0]

    addLayer = arcpy.mapping.Layer(inputDir + str(index) + ".shp")

    arcpy.mapping.AddLayer(df, addLayer, "BOTTOM")

    arcpy.Project_management(str(index), wipDir + str(index) + "_lt.shp" , "PROJCS['LKS_1994_Lithuania_TM',GEOGCS['GCS_LKS_1994',DATUM['D_Lithuania_1994',SPHEROID['GRS_1980',6378137.0,298.257222101]],PRIMEM['Greenwich',0.0],UNIT['Degree',0.0174532925199433]],PROJECTION['Transverse_Mercator'],PARAMETER['False_Easting',500000.0],PARAMETER['False_Northing',0.0],PARAMETER['Central_Meridian',24.0],PARAMETER['Scale_Factor',0.9998],PARAMETER['Latitude_Of_Origin',0.0],UNIT['Meter',1.0]]", 'LKS_1994_To_WGS_1984', "GEOGCS['GCS_WGS_1984',DATUM['D_WGS_1984',SPHEROID['WGS_1984',6378137.0,298.257223563]],PRIMEM['Greenwich',0.0],UNIT['Degree',0.0174532925199433]]", 'NO_PRESERVE_SHAPE', '#', 'NO_VERTICAL')

    arcpy.FeatureToLine_management(str(index) + "_lt", wipDir + str(index) + "_lt_plan.shp", '0 DecimalDegrees', 'ATTRIBUTES')

    arcpy.AddGeometryAttributes_management(str(index) + "_lt_plan", 'LENGTH', 'METERS', '#', '#')

    arcpy.Project_management(str(index) + "_lt_plan", outputDir + str(index) + ".shp", "GEOGCS['GCS_WGS_1984',DATUM['D_WGS_1984',SPHEROID['WGS_1984',6378137.0,298.257223563]],PRIMEM['Greenwich',0.0],UNIT['Degree',0.0174532925199433]]", 'LKS_1994_To_WGS_1984', "PROJCS['LKS_1994_Lithuania_TM',GEOGCS['GCS_LKS_1994',DATUM['D_Lithuania_1994',SPHEROID['GRS_1980',6378137.0,298.257222101]],PRIMEM['Greenwich',0.0],UNIT['Degree',0.0174532925199433]],PROJECTION['Transverse_Mercator'],PARAMETER['False_Easting',500000.0],PARAMETER['False_Northing',0.0],PARAMETER['Central_Meridian',24.0],PARAMETER['Scale_Factor',0.9998],PARAMETER['Latitude_Of_Origin',0.0],UNIT['Meter',1.0]]", 'NO_PRESERVE_SHAPE', '#', 'NO_VERTICAL')

    arcpy.mapping.RemoveLayer(df, arcpy.mapping.ListLayers(mxd)[0])

    for l in arcpy.mapping.ListLayers(mxd):
        arcpy.mapping.RemoveLayer(df,l)

        
inputDir = "C:\\Users\\daini\\Documents\\ArcGIS\\Data\\python\\input\\"
outputDir = "C:\\Users\\daini\\Documents\\ArcGIS\\Data\\python\\output\\"
wipDir = "C:\\Users\\daini\\Documents\\ArcGIS\\Data\\python\\WIP\\"       
        

for index in range(205, 1500):
  file = inputDir + str(index) + ".shp"
  my_file = Path(file)
  if not my_file.is_file():
    continue
  proccess(index)