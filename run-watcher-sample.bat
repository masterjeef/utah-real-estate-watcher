:: This script will not work if the project has not been built
:: It's easy to verify, simply go to the Debug directory below

@echo off

SET exePath=UtahRealEstateWatcher/UtahRealEstateWatcher/bin/Debug

SET minPrice=200000

SET maxPrice=280000

SET cities="Herriman;Lehi;Draper;Saratoga Springs;Riverton;South Jordan"

cd %exePath%

UtahRealEstateWatcher %cities% %minPrice% %maxPrice%