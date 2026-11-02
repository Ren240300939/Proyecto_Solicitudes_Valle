$ErrorActionPreference = 'Stop'
$compiladorRpa = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
if (!(Test-Path -LiteralPath $compiladorRpa)) { $compiladorRpa = Join-Path $env:WINDIR 'Microsoft.NET\Framework\v4.0.30319\csc.exe' }
if (!(Test-Path -LiteralPath $compiladorRpa)) { throw 'No se encuentra el compilador de .NET Framework 4.x. Solicite al docente un ejecutable compilado.' }
$destinoRpa = Join-Path $PSScriptRoot 'bin'
New-Item -ItemType Directory -Force -Path $destinoRpa | Out-Null
& $compiladorRpa /nologo /target:winexe /utf8output /reference:System.Windows.Forms.dll /reference:System.Drawing.dll /reference:System.Xml.dll /reference:System.Core.dll "/out:$destinoRpa\SolicitudesRpa.exe" (Join-Path $PSScriptRoot 'app\Solicitudes.cs')
if ($LASTEXITCODE -ne 0) { throw 'La compilación falló.' }
Write-Output "Aplicación lista: $destinoRpa\SolicitudesRpa.exe"
