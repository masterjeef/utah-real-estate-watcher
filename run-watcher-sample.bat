::
:: This script will not work if the project has not been built
:: It's easy to verify, simply go to the Debug directory
::
:: UtahRealEstateWatcher/UtahRealEstateWatcher/bin/Debug
::

@echo off

SET exePath=UtahRealEstateWatcher/UtahRealEstateWatcher/bin/Debug

SET minPrice=200000

SET maxPrice=280000

SET cities="Herriman;Lehi;Draper;Saratoga Springs;Riverton;South Jordan"

cd %exePath%

UtahRealEstateWatcher Cities=%cities% MinPrice=%minPrice% MaxPrice=%maxPrice%

::
:: This is another example without variables 
::
:: UtahRealEstateWatcher Cities=Herriman;Lehi MinPrice=200000 MaxPrice=300000
:: 
:: The order of the arguments do not matter
:: Add the -d flag to delete the last run file, like the following :
::
:: UtahRealEstateWatcher Cities=Herriman;Lehi MinPrice=200000 MaxPrice=300000 -d
::