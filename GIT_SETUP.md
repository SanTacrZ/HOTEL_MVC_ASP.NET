# 📤 Instrucciones para Subir el Proyecto a GitHub

## Pasos para Subir el Proyecto

### 1. Inicializar Git (si no está inicializado)

```bash
cd C:\Users\santa\source\repos\hotel_web_final
git init
```

### 2. Agregar el Remote de GitHub

```bash
git remote add origin https://github.com/SanTacrZ/hotel_MVC_ASP.NET.git
```

### 3. Agregar todos los archivos

```bash
git add .
```

### 4. Hacer el primer commit

```bash
git commit -m "Initial commit: Sistema de Gestión Hotelera completo"
```

### 5. Cambiar a la rama main (si es necesario)

```bash
git branch -M main
```

### 6. Subir al repositorio

```bash
git push -u origin main
```

## Si el repositorio ya tiene contenido

Si el repositorio remoto ya tiene commits, necesitarás hacer un pull primero:

```bash
git pull origin main --allow-unrelated-histories
```

Luego resuelve cualquier conflicto y haz push:

```bash
git push -u origin main
```

## Comandos Útiles

### Ver el estado de los archivos
```bash
git status
```

### Ver qué archivos se van a subir
```bash
git diff --cached
```

### Si necesitas autenticarte
GitHub ahora requiere tokens de acceso personal. Puedes:
1. Ir a GitHub Settings > Developer settings > Personal access tokens
2. Generar un nuevo token
3. Usarlo como contraseña cuando Git te lo pida

## Estructura que se Subirá

✅ **Se subirán:**
- Todo el código fuente (.cs, .cshtml, .css, .js)
- Archivos de configuración (.csproj, .json)
- README.md
- .gitignore
- LICENSE

❌ **NO se subirán (gracias a .gitignore):**
- Carpetas bin/ y obj/
- Archivos .user
- Archivos temporales de Visual Studio
- node_modules (si los hay)

## Notas Importantes

1. **Archivos de Datos**: Los archivos en `Arhivos/` (Clientes.txt, Huespedes.txt) NO están en el .gitignore por defecto. Si contienen datos sensibles, agrégalos al .gitignore.

2. **Biblioteca DLL**: La referencia a `biblioteca_hotel.dll` está en el .csproj, pero la DLL misma no se subirá. Asegúrate de documentar cómo obtenerla.

3. **Secrets**: Nunca subas archivos con contraseñas, API keys o información sensible.

