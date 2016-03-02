:: This script will not work if the project has not been built
:: It's easy to verify, simply go to the Debug directory below

@echo off

SET exePath=UtahRealEstateWatcher/UtahRealEstateWatcher/bin/Debug

cd %exePath%

UtahRealEstateWatcher "Herriman;Lehi;Draper;Saratoga Springs;Riverton;South Jordan" 200000 280000