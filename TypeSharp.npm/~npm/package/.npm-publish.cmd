for /f %%x in ('npm config get registry') do set registry=%%x
call npm config set registry "https://registry.npmjs.org/"
call npm publish --access=public
@echo on
call npm config set registry %registry%
