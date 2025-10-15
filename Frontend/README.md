# Frontend PetCloud

Ubicación: `PetCloud-App/Frontend`

## Estructura
- `index.html`: portada
- `login.html` / `register.html`: autenticación, integradas con la API
- `dashboard.html`: panel principal
- `gastos.html`: CRUD de gastos por mascota
- `js/config.js`: base de la API (cámbiala si tu backend corre en otro puerto)
- `js/api.js`: `authFetch` con envío de token
- `js/auth.js`: login/registro y manejo de token
- `js/ui.js`: header con selector de mascota y toggle de tema

## Configuración de API
Por defecto, apunta a `https://localhost:5001`. Puedes sobrescribir temporalmente en el navegador con:

```js
localStorage.setItem('apiBase', 'https://localhost:5001')
```

## Cómo usar
1. Levanta el backend: `dotnet run` en `Backend/PetCloud.Api` (Development muestra Swagger).
2. Abre `PetCloud-App/Frontend/index.html` en tu navegador (o usa Live Server).
3. Regístrate y luego inicia sesión para obtener token.
4. En el header selecciona una mascota (debe existir). En Gastos podrás crear/editar/eliminar.

## Notas
- Dark mode por clase (`dark`).
- Usa Tailwind CDN para simplicidad en dev.
- Si abres archivos directamente con `file://`, las rutas relativas funcionan. Si usas live server, las rutas también deben funcionar.
