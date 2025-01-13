call npm install --save bootstrap
call npm install --save react react-dom @types/react @types/react-dom
call npm install --save-dev typescript@3.9.3 awesome-typescript-loader source-map-loader

echo.
echo Maybe you should disable TypeScript compilation when the project is compiled.
echo To use the part of configuration:
echo.
echo   ^<PropertyGroup^>
echo     ^<TypeScriptCompileBlocked^>true^</TypeScriptCompileBlocked^>
echo   ^</PropertyGroup^>
echo.
