@echo off
echo tsc...
call tsc index.ts -d --outdir lib
call xcopy "lib" "..\..\~scripts\npm-clone" /Y

cd ..
echo webpack...
call webpack
echo copy dist to wwwroot
cd "package"
call xcopy "dist" "..\..\wwwroot\js" /Y
echo done.
