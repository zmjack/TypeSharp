call npm install --save bootstrap type-sharp dotnet-linq axios
call npm install --save react react-dom @types/react @types/react-dom react-router
call npm install --save antd@4.17.0-alpha.7 @ant-design/icons
call npm install --save-dev typescript@3.9.3 awesome-typescript-loader source-map-loader

echo.
echo Maybe you should disable TypeScript compilation when the project is compiled.
echo To use the part of configuration:
echo.
echo   ^<PropertyGroup^>
echo     ^<TypeScriptCompileBlocked^>true^</TypeScriptCompileBlocked^>
echo   ^</PropertyGroup^>
echo.
