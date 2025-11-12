# Script para configurar y subir el proyecto a GitHub
# Ejecutar desde PowerShell: .\setup-git.ps1

Write-Host "🚀 Configurando Git para Hotel MVC ASP.NET" -ForegroundColor Cyan
Write-Host ""

# Verificar si Git está instalado
try {
    $gitVersion = git --version
    Write-Host "✅ Git encontrado: $gitVersion" -ForegroundColor Green
} catch {
    Write-Host "❌ Git no está instalado. Por favor instálalo primero." -ForegroundColor Red
    exit 1
}

# Inicializar Git si no está inicializado
if (-not (Test-Path .git)) {
    Write-Host "📦 Inicializando repositorio Git..." -ForegroundColor Yellow
    git init
    Write-Host "✅ Repositorio inicializado" -ForegroundColor Green
} else {
    Write-Host "✅ Repositorio Git ya está inicializado" -ForegroundColor Green
}

# Agregar remote si no existe
$remoteExists = git remote get-url origin 2>$null
if ($LASTEXITCODE -ne 0) {
    Write-Host "🔗 Agregando remote de GitHub..." -ForegroundColor Yellow
    git remote add origin https://github.com/SanTacrZ/hotel_MVC_ASP.NET.git
    Write-Host "✅ Remote agregado" -ForegroundColor Green
} else {
    Write-Host "✅ Remote ya configurado: $remoteExists" -ForegroundColor Green
}

# Agregar todos los archivos
Write-Host "📝 Agregando archivos al staging..." -ForegroundColor Yellow
git add .
Write-Host "✅ Archivos agregados" -ForegroundColor Green

# Verificar si hay cambios para commitear
$status = git status --porcelain
if ($status) {
    Write-Host "💾 Creando commit inicial..." -ForegroundColor Yellow
    git commit -m "Initial commit: Sistema de Gestión Hotelera completo con validaciones y diseño profesional"
    Write-Host "✅ Commit creado" -ForegroundColor Green
} else {
    Write-Host "ℹ️  No hay cambios para commitear" -ForegroundColor Yellow
}

# Cambiar a rama main
Write-Host "🌿 Configurando rama main..." -ForegroundColor Yellow
git branch -M main
Write-Host "✅ Rama configurada" -ForegroundColor Green

Write-Host ""
Write-Host "📤 Para subir al repositorio, ejecuta:" -ForegroundColor Cyan
Write-Host "   git push -u origin main" -ForegroundColor White
Write-Host ""
Write-Host "⚠️  Si es la primera vez, GitHub puede pedirte autenticación." -ForegroundColor Yellow
Write-Host "   Usa un Personal Access Token como contraseña." -ForegroundColor Yellow
Write-Host ""

